using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class ExportMessage : ApplicationMessage
    {
        public string BlobUrl { get; set; }
        public override bool Process()
        {
            string sql = string.Format("select * from SubjectRequestApplication where Applicationid = '{0}' and SubjectRequestId = '{1}'", this.ApplicationId, this.SubjectRequestId);
            SubjectRequestApplication sra = MasterGDPRHelper.GDPRDatabase.SubjectRequestApplications.SqlQuery(sql).FirstOrDefault();

            if (sra == null)
            {
                sra = new SubjectRequestApplication();
                sra.SubjectRequestApplicationResultId = Guid.NewGuid();
                sra.ApplicationId = Guid.Parse(this.ApplicationId);
                sra.SubjectRequestId = Guid.Parse(this.SubjectRequestId);
                sra.Status = "Completed";
                sra.IsComplete = true;
                sra.CompletedDate = DateTime.Now;
                sra.ModifyDate = DateTime.Now;
                sra.CreateDate = DateTime.Now;
                MasterGDPRHelper.GDPRDatabase.Entry(sra).State = System.Data.Entity.EntityState.Added;
            }
            else
            {
                sra.IsComplete = true;
                sra.CompletedDate = DateTime.Now;
                sra.ModifyDate = DateTime.Now;
                sra.Status = "Completed";
                MasterGDPRHelper.GDPRDatabase.Entry(sra).State = System.Data.Entity.EntityState.Modified;
            }

            sra.DataExportLink = BlobUrl;
            MasterGDPRHelper.GDPRDatabase.SaveChanges();

            return true;
        }
    }
}
