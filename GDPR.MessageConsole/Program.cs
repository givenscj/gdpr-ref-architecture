using GDPR.Util;
using GDPR.Util.Messages;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Azure.KeyVault;
using System.Web.Configuration;
using GDPR.Applications;
using CRM.DB;

namespace GDPRMessageConsole
{
    class Program
    {        
        static void Main(string[] args)
        {
            //create the user in the system...
            Setup();

            //Check for new additions (non http outgoing trigger based system like Azure SQL)
            CheckForChanges();

            //send a notify message...
            NotifyMessage nm = new NotifyMessage();
            nm.Direction = "out";
            GDPRSubject s = new GDPRSubject();
            s.Email = "chris@solliance.net";
            nm.Subject = s;
            nm.Title = "CRM Compromised";
            nm.ShortMessage = "CRM Compromised";
            nm.LongMessage = "As of this morning we have noticed abnormal activity in our system that looks hacker related.  We will notify you of future updates.";           
            MasterGDPRHelper.SendMessage(nm);

            //send me my data request...
            DataSubjectQueryMessage dsqm = new DataSubjectQueryMessage();
            s = new GDPRSubject();
            s.Email = "chris@solliance.net";
            dsqm.Subject = s;            
            MasterGDPRHelper.SendMessage(dsqm);

            //delete request...
            DataSubjectDeleteMessage dsdm = new DataSubjectDeleteMessage();
            s = new GDPRSubject();
            s.Email = "chris@solliance.net";
            dsdm.Subject = s;            
            MasterGDPRHelper.SendMessage(dsdm);            
        }

        static void CheckForChanges()
        {            
            foreach(Application a in MasterGDPRHelper.GDPRDatabase.Applications)
            {
                if (a.IsActive)
                {                    
                    Type t = Type.GetType(a.ProcessorClass + ",GDPR.Util");
                    GDPRApplication app = (GDPRApplication)Activator.CreateInstance(t);

                    List<GDPRMessage> messages = app.GetChanges(a.ChangeDate);

                    foreach (GDPRMessage msg in messages)
                    {
                        MasterGDPRHelper.SendMessage(msg);
                    }

                    MasterGDPRHelper.UpdateApplicationChangeDate(a.ApplicationId, DateTime.Now);
                }
            }                        
        }

        static private void Setup()
        {
            AddCRMCustomer("Chris", "Givens", "chris@solliance.net");            
        }

        static private void AddCRMCustomer(string firstName, string lastName, string emailAddress)
        {
            try
            {
                //create the user manually in CRM
                CRMEntities e = Util.GetCRMDBContext(Util.CRMSQLConnectionString);
                CRM.DB.Customer c = new Customer();
                c.CustomerId = Guid.NewGuid();
                c.FirstName = firstName;
                c.LastName = lastName;
                c.Email = emailAddress;
                c.OnHold = false;
                c.IsDeleted = false;
                c.ModifyDate = DateTime.Now;
                c.CreateDate = DateTime.Now;
                e.Entry(c).State = System.Data.Entity.EntityState.Added;
                e.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }        
    }
}
