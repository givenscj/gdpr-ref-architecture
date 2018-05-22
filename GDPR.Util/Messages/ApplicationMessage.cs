using GDPR.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class ApplicationMessage : GDPRMessage
    {        
        public GDPRApplication Application { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockedDate { get; set; }
        public string ApplicationSubjectId { get; set; }
        public override bool Process()
        {
            //Save the application request for future status update
            MasterGDPRHelper.SaveApplicationRequest(this);

            Application app = MasterGDPRHelper.GDPRDatabase.Applications.Find(Guid.Parse(this.ApplicationId));

            if (app.IsActive)
            {
                string typeclass = app.ProcessorClass;

                //initalize the Application stub
                Type pType = Type.GetType(typeclass);
                Application = (GDPRApplication)Activator.CreateInstance(pType);                

                //fire the request
                Application.ProcessRequest(this);
            }

            return base.Process();
        }
    }
}
