using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class DataSubjectMessage : GDPRMessage
    {

        public DataSubjectMessage()
        {
            this.SubjectRequestId = Guid.NewGuid().ToString();
        }

        public override bool Process()
        {            
            this.SubjectRequestId = MasterGDPRHelper.SaveConsumerRequest(this).ToString();

            return base.Process();
        }
    }
}
