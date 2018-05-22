using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class DataSubjectUpdateMessage : DataSubjectMessage
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
                    UpdateMessage um = new UpdateMessage();
                    um.Subject = this.Subject;
                    um.SubjectRequestId = this.SubjectRequestId;
                    um.ApplicationId = app.ApplicationId.ToString();
                    GDPR.Util.MasterGDPRHelper.SendMessage((GDPRMessage)um);
                }
            }

            return true;
        }
    }
}
