using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class NotifyMessage : GDPRMessage
    {
        public string Title { get; set; }
        public string ShortMessage { get; set; }
        public string LongMessage { get; set; }

        public override bool Process()
        {
            Util.NotifyDataSubject(this);

            return base.Process();
        }
    }
}
