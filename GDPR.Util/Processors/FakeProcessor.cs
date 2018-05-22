using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDPR.Util.Messages;

namespace GDPR.Util.Processors
{
    public class FakeProcessor : GDPRProcessor
    {
        public override string SendRequest(GDPRMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
