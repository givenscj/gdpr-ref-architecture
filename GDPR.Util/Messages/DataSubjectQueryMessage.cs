using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class DataSubjectQueryMessage : DataSubjectMessage
    {
        public DataSubjectQueryMessage() : base()
        {
        }

        public override bool Process()
        {
            //has to go first to get the subject request id!
            base.Process();

            //get all applications                
            List<Application> apps = MasterGDPRHelper.GDPRDatabase.Applications.ToList();

            //send the message to all applications
            foreach (Application app in apps)
            {
                if (app.IsActive)
                {
                    DataRequestMessage dm = new DataRequestMessage();
                    dm.Subject = this.Subject;
                    dm.ApplicationId = app.ApplicationId.ToString();
                    dm.SubjectRequestId = this.SubjectRequestId;
                    GDPR.Util.MasterGDPRHelper.SendMessage((GDPRMessage)dm);

                    //Create a Subject Request Application entry...
                    string sql = string.Format("select * from SubjectRequestApplication where Applicationid = '{0}' and SubjectRequestId = '{1}'", app.ApplicationId.ToString(), this.SubjectRequestId);
                    SubjectRequestApplication sra = MasterGDPRHelper.GDPRDatabase.SubjectRequestApplications.SqlQuery(sql).FirstOrDefault();

                    if (sra == null)
                    {
                        sra = new SubjectRequestApplication();
                        sra.SubjectRequestApplicationResultId = Guid.NewGuid();
                        sra.ApplicationId = app.ApplicationId;
                        sra.SubjectRequestId = Guid.Parse(this.SubjectRequestId);
                        sra.Status = "Pending";
                        sra.ModifyDate = DateTime.Now;
                        sra.CreateDate = DateTime.Now;
                        MasterGDPRHelper.GDPRDatabase.Entry(sra).State = System.Data.Entity.EntityState.Added;
                    }
                    else
                    {
                        sra.Status = "Pending";
                        sra.ModifyDate = DateTime.Now;
                        MasterGDPRHelper.GDPRDatabase.Entry(sra).State = System.Data.Entity.EntityState.Modified;
                    }

                    MasterGDPRHelper.GDPRDatabase.SaveChanges();
                }
            }

            return true;
        }
    }
}
