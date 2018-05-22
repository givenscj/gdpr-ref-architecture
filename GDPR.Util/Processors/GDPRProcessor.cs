using GDPR.Util.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Processors
{
    public abstract class GDPRProcessor : IGDPRProcessor
    {
        abstract public string SendRequest(GDPRMessage message);
    }
}
