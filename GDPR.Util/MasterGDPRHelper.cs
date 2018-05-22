using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using GDPR.Applications;
using GDPR.Util.Messages;
using Microsoft.ServiceBus.Messaging;

namespace GDPR.Util
{
    public class MasterGDPRHelper
    {
        static GDPRDatabaseEntities gdprDB;
        static EventHubClient eventHubClient;

        static MasterGDPRHelper()
        {
            const string metaData = "res://*/GDPRModel.csdl|res://*/GDPRModel.ssdl|res://*/GDPRModel.msl";
            const string appName = "EntityFramework";
            const string providerName = "System.Data.SqlClient";

            EntityConnectionStringBuilder efBuilder = new EntityConnectionStringBuilder();
            efBuilder.Metadata = metaData;
            efBuilder.Provider = providerName;
            efBuilder.ProviderConnectionString = Util.GDPRSQLConnectionString;

            gdprDB = new GDPRDatabaseEntities(efBuilder.ConnectionString);
        }

        static public GDPRDatabaseEntities GDPRDatabase
        {
            get { return gdprDB; }
        }


        public static List<Subject> SearchGDPRSubject(GDPRSubject subject)
        {
            //search the database for matching email...
            List<Subject> results = gdprDB.Database.SqlQuery<Subject>("exec SearchSubject @p0", subject.Email).ToList();
            return results;
        }

        static public Subject FindSubject(GDPRSubject searchS)
        {            
            List<Subject> subjects = SearchGDPRSubject(searchS);

            if (subjects.Count > 0)
            {
                return subjects[0];
            }

            return null;
        }

        static public void SendMessage(GDPRMessage message)
        {            
            GDPRMessageWrapper w = new GDPRMessageWrapper();
            w.ApplicationId = message.ApplicationId;
            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            w.Type = message.GetType().Name;
            w.Object = msg;
            w.MessageDate = DateTime.Now;
            SendMessage(w);
        }

        public static string CreateGDPRRequest(string emailAddress, string type)
        {
            try
            {
                GDPRMessage msg = new GDPRMessage();

                switch (type)
                {
                    case "Consent":
                        DataSubjectConsentMessage dscm = new DataSubjectConsentMessage();
                        msg = dscm;
                        break;
                    case "Delete":
                        DataSubjectDeleteMessage dm = new DataSubjectDeleteMessage();
                        msg = dm;
                        break;
                    case "Query":
                        DataSubjectQueryMessage nm = new DataSubjectQueryMessage();
                        msg = nm;
                        break;
                    case "Update":
                        DataSubjectUpdateMessage um = new DataSubjectUpdateMessage();
                        msg = um;
                        break;
                }

                GDPRSubject s = new GDPRSubject();
                s.Email = emailAddress;
                msg.Subject = s;

                MasterGDPRHelper.SendMessage(msg);
            }
            catch
            {
                return "Failure";
            }

            return "Success";
        }

        static private void SendMessage(GDPRMessageWrapper message)
        {
            //send a message...
            //string connectionStringBuilder =  "Endpoint=sb://gdprevents.servicebus.windows.net/;SharedAccessKeyName=GDPRPolicy;SharedAccessKey=ecCNa0UQXlCFiAIbAOMCBJLbzlWqOaCI7YQX3DCmSgo=;EntityPath=gdprmessagehub";
            string connectionStringBuilder = Util.EventHubConnectionStringWithPath;
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            if (!string.IsNullOrEmpty(message.ApplicationId))
            {
                EventHubSender sender = eventHubClient.CreateSender("gdprgroup");
                //EventHubSender sender = eventHubClient.CreatePartitionedSender("gdprgroup");
                string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                sender.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
            }
            else
            {
                string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
            }
            //eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(msg)));
            //eventHubClient.CloseAsync();
        }

        public static void UpdateApplicationChangeDate(Guid applicationId, DateTime now)
        {
            GDPRDatabaseEntities g = Util.GetGDPRDBContext(Util.GDPRSQLConnectionString);
            string sql = string.Format("update application set ChangeDate='{1}' where applicationid = '{0}'", applicationId, now);
            g.Database.ExecuteSqlCommand(sql);
        }

        public static Guid SaveConsumerRequest(GDPRMessage msg)
        {
            Subject s = FindSubject(msg.Subject);

            if (s != null)
            {
                //check for a current pending request of same type...
                string sql = string.Format("select * from subjectrequest where subjectid = '{0}' and type = '{1}'", s.SubjectId, msg.GetType().Name); 
                SubjectRequest sr = MasterGDPRHelper.GDPRDatabase.SubjectRequests.SqlQuery(sql).FirstOrDefault();

                try
                {
                    if (sr == null)
                    {
                        sr = new SubjectRequest();
                        sr.SubjectRequestId = Guid.NewGuid();
                        sr.Type = msg.GetType().Name;
                        sr.SubjectId = s.SubjectId;
                        sr.ModifyDate = DateTime.Now;
                        sr.CreateDate = DateTime.Now;
                        GDPRDatabase.Entry(sr).State = System.Data.Entity.EntityState.Added;
                        GDPRDatabase.SaveChanges();
                    }
                }
                catch(Exception ex)
                {

                }

                return sr.SubjectRequestId;
            }

            return Guid.Empty;
        }

        public static void SaveApplicationRequest(GDPRMessage msg)
        {
            SubjectRequestApplication sr = new SubjectRequestApplication();

            try
            {                
                sr.SubjectRequestApplicationResultId = Guid.NewGuid();
                sr.ApplicationId = Guid.Parse(msg.ApplicationId);
                sr.SubjectRequestId = Guid.Parse(msg.SubjectRequestId);
                sr.ModifyDate = DateTime.Now;
                sr.CreateDate = DateTime.Now;
                GDPRDatabase.Entry(sr).State = System.Data.Entity.EntityState.Added;
                GDPRDatabase.SaveChanges();
            }
            catch (Exception ex)
            {
                GDPRDatabase.Entry(sr).State = System.Data.Entity.EntityState.Detached;
            }
        }

        public static void StartMessageProcessing(string consumerGroup, out EventProcessorHost eventProcessorHost)
        {
            string applicationId = WebConfigurationManager.AppSettings["applicationId"];
            string eventHubConnectionString = Util.EventHubConnectionString;
            string storageAccountKey = Util.StorageAccountKey;
            string eventHubName = WebConfigurationManager.AppSettings["eventHubName"];
            string storageAccountName = WebConfigurationManager.AppSettings["storageAccountName"];
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, consumerGroup, eventHubConnectionString, storageConnectionString);
            var options = new EventProcessorOptions();
            //options.InitialOffsetProvider = (PartitionId) => DateTime.UtcNow.AddHours(-1);
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<GDPREventProcessor>(options).Wait();            
        }

        public static DateTime GetOffset(string hubName, string partitionId)
        {
            EventHub eh = GDPRDatabase.EventHubs.Find(hubName, partitionId);

            if (eh == null)
            {
                SetOffSet(hubName, partitionId, DateTime.MinValue);
                return DateTime.MinValue;
            }

            return eh.CheckPoint;
        }

        public static bool SetOffSet(string hubName, string partitionId, DateTime offset)
        {
            try
            {                
                EventHub eh = GDPRDatabase.EventHubs.Find(hubName, partitionId);

                if (eh == null)
                {
                    eh = new EventHub();
                    eh.EventHubName = hubName;
                    eh.PartitionId = partitionId;
                    eh.ModifyDate = DateTime.Now;
                    eh.CreateDate = DateTime.Now;
                    GDPRDatabase.Entry(eh).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    GDPRDatabase.Entry(eh).State = System.Data.Entity.EntityState.Modified;
                }

                eh.CheckPoint = offset;                
                GDPRDatabase.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }        

        public static void ProcessRequest(GDPRMessageWrapper msg)
        {                                    
            Type t = Type.GetType("GDPR.Util.Messages." + msg.Type);
            GDPRMessage actionMessage = (GDPRMessage)Newtonsoft.Json.JsonConvert.DeserializeObject(msg.Object, t);
            actionMessage.Process();                        
        }
    }
}
