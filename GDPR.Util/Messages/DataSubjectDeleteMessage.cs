using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class DataSubjectDeleteMessage : DataSubjectMessage
    {
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
                    DeleteMessage dm = new DeleteMessage();
                    dm.Subject = this.Subject;
                    dm.SubjectRequestId = this.SubjectRequestId;
                    dm.ApplicationId = app.ApplicationId.ToString();
                    GDPR.Util.MasterGDPRHelper.SendMessage((GDPRMessage)dm);
                }
            }

            return true;
        }
    }
}
