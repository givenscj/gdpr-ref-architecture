using GDPR.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class StatusMessage : ApplicationMessage
    {
        GDPRMessage OriginalMessage { get; set; }
        public override bool Process()
        {
            //check that all applications have deleted the data subject            

            return false;
        }
    }
}
