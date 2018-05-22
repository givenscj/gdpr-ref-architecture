using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class GDPRMessage
    {
        public GDPRMessage()
        {            
        }

        public GDPRSubject Subject { get; set; }
        public string ApplicationId { get; set; }
        public string SubjectRequestId { get; set; }
        public string Direction { get; set; }        

        public virtual bool Process()
        {
            return true;
        }
    }
}
