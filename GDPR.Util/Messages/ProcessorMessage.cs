using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class ProcessorMessage : GDPRMessage
    {
        public string ProcessorId { get; set; }
        public string ProcessorRequestId { get; set; }
        public string ExportLocation { get; set; }
    }
}
