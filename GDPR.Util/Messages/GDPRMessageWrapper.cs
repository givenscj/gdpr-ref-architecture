using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class GDPRMessageWrapper
    {
        public string ApplicationId { get; set; }
        public string Type { get; set; }
        public string Object { get; set; }
        public DateTime MessageDate { get; set; }
    }
}
