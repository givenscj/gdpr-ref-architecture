using GDPR.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.DB;
using GDPR.Util.Messages;
using Newtonsoft.Json;

namespace GDPR.Applications
{
    public class CRMApplication : GDPRApplication
    {
        CRM.DB.CRMEntities e = new CRM.DB.CRMEntities();

        public CRMApplication()
        {
            this.ApplicationId = Guid.Parse("9B9BCC39-1170-48F2-A3F8-62E81316E5B1");
            e = Util.Util.GetCRMDBContext(Util.Util.CRMSQLConnectionString);
        }

        public override List<GDPRMessage> GetChanges(DateTime changeDate)
        {
            List<GDPRMessage> messages = new List<GDPRMessage>();
            string sql = string.Format("select * from customer where createdate >= '{0}'", changeDate);
            //find all customers that are new...
            List <Customer> newcustomers = e.Customers.SqlQuery(sql).ToList();

            foreach(Customer c in newcustomers)
            {
                CreateMessage cm = new CreateMessage();
                cm.ApplicationSubjectId = c.CustomerId.ToString();
                cm.Direction = "in";
                cm.ApplicationId = this.ApplicationId.ToString();
                GDPRSubject s = new GDPRSubject();
                s.Email = c.Email;
                cm.Subject = s;
                messages.Add(cm);
            }

            sql = string.Format("select * from customer where modifydate >= '{0}' and createdate < modifydate", changeDate);

            //find all customers that have been modified...
            List<Customer> modifiedcustomers = e.Customers.SqlQuery(sql).ToList();

            foreach(Customer c in modifiedcustomers)
            {
                UpdateMessage cm = new UpdateMessage();
                cm.ApplicationSubjectId = c.CustomerId.ToString();
                cm.Direction = "in";
                cm.ApplicationId = this.ApplicationId.ToString();
                GDPRSubject s = new GDPRSubject();
                s.Email = c.Email;
                cm.Subject = s;
                messages.Add(cm);
            }            

            return messages;
        }
        
        void GetAllRecords()
        {
            List<Customer> customers = e.Customers.ToList();

            foreach(Customer c in customers)
            {
                CreateMessage cm = new CreateMessage();
                cm.ApplicationSubjectId = c.CustomerId.ToString();
                cm.ApplicationId = this.ApplicationId.ToString();
                MasterGDPRHelper.SendMessage(cm);
            }
        }

        
        void RecordCreateIn(GDPRSubject subject)
        {
            /*
            CreateMessage cm = new CreateMessage();
            cm.ApplicationSubjectId = c.CustomerId.ToString();
            cm.ApplicationId = this.ApplicationId.ToString();
            MasterGDPRHelper.SendMessage(cm);
            */
        }

        void RecordCreateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        
        public override void RecordDeleteIn(GDPRSubject subject)
        {
            string sql = string.Format("Delete from Customer where email = '{0}'", subject.Email);
            e.Database.ExecuteSqlCommand(sql);
        }

        void RecordDeleteOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordHold(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordNotify(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }        
        
        void RecordUpdateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordUpdateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void ValidateSubject(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public override List<GDPRSubject> RecordSearch(GDPRSubject search)
        {
            List<GDPRSubject> subjects = new List<GDPRSubject>();

            List<GDPRMessage> messages = new List<GDPRMessage>();
            string sql = string.Format("select * from customer where email ='{0}'", search.Email);
            
            //find all customers that are new...
            List<Customer> searchResults = e.Customers.SqlQuery(sql).ToList();

            foreach(Customer c in searchResults)
            {
                GDPRSubject s = new GDPRSubject();
                s.ApplicationSubjectId = c.CustomerId.ToString();
                subjects.Add(s);
            }

            return subjects;
        }

        public override string ExportData(string applicationSubjectId)
        {            
            //package the customer record as a json file...
            string fileName = string.Format("CRM_{0}.json", applicationSubjectId);
            string sql = string.Format("select * from customer where customerid = '{0}'", applicationSubjectId);

            //find all customers that have been modified...
            Customer subject = e.Customers.Find(Guid.Parse(applicationSubjectId));
            string json = JsonConvert.SerializeObject(subject);

            string filePath = string.Format("c:\\temp\\{0}", fileName);
            System.IO.File.AppendAllText(filePath,json);

            //Copy the file to the storage account
            string blobUrl = Util.Util.UploadBlob(this.ApplicationId, filePath);            
            return blobUrl;
        }
    }
}
