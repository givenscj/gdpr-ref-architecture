using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class CreateMessage : ApplicationMessage
    {
        public override bool Process()
        {
            //we should add the information to the master GDPR database...
            Subject s = new Subject();
            s.SubjectId = Guid.NewGuid();

            if (this.Subject.FirstName == null)
                s.FirstName = "EMPTY";
            else
                s.FirstName = this.Subject.FirstName;

            s.LastName = this.Subject.LastName;
            s.EmailAddress = this.Subject.Email;
            s.HomePhone = this.Subject.HomePhone;
            s.WorkPhone = this.Subject.WorkPhone;
            s.MobilePhone = this.Subject.MobilePhone;
            s.ModifyDate = DateTime.Now;
            s.CreateDate = DateTime.Now;

            try
            {                
                MasterGDPRHelper.GDPRDatabase.Entry(s).State = System.Data.Entity.EntityState.Added;
                MasterGDPRHelper.GDPRDatabase.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MasterGDPRHelper.GDPRDatabase.Entry(s).State = System.Data.Entity.EntityState.Detached;
            }

            return false;
        }
    }
}
