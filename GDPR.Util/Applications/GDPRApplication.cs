using GDPR.Util;
using GDPR.Util.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Applications
{
    public class GDPRApplication : IGDPRDataSubjectActions
    {
        public GDPRMessage Request{ get; set; }
        public GDPRMessage Response { get; set; }
        public Guid ApplicationId { get; set; }

        virtual public string ExportData(string applicationSubjectId)
        {
            throw new NotImplementedException();
        }

        virtual public List<GDPRMessage> GetAllRecords()
        {
            throw new NotImplementedException();
        }

        virtual public List<GDPRMessage> GetChanges(DateTime changeDate)
        {
            throw new NotImplementedException();
        }

        public void ProcessRequest(GDPRMessage message)
        {
            this.Request = message;

            GDPRSubject s = new GDPRSubject(message.Subject);

            switch(message.GetType().Name)
            {
                case "DeleteMessage":
                    this.RecordDeleteIn(message.Subject);
                    break;
                case "DataRequestMessage":
                   List<GDPRSubject> customers = this.RecordSearch(s);

                    foreach(GDPRSubject c in customers)
                    {
                        string storageLocation = this.ExportData(c.ApplicationSubjectId);

                        //send a export message...
                        ExportMessage em = new ExportMessage();
                        em.ApplicationId = Request.ApplicationId;
                        em.ApplicationSubjectId = c.ApplicationSubjectId;
                        em.SubjectRequestId = Request.SubjectRequestId;
                        em.Subject = Request.Subject;
                        em.BlobUrl = storageLocation;
                        this.Response = em;

                        MasterGDPRHelper.SendMessage(em);
                    }
                    break;
            }
        }

        virtual public void RecordCreateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void RecordCreateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        virtual public void RecordDeleteIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void RecordDeleteOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void RecordHold(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void RecordNotify(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        virtual public List<GDPRSubject> RecordSearch(GDPRSubject search)
        {
            List<GDPRSubject> results = new List<GDPRSubject>();
            return results;           
        }        

        public void RecordUpdateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }        

        public void RecordUpdateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        public void ValidateSubject(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }        
    }
}
