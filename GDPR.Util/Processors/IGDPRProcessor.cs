using GDPR.Util.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Processors
{
    public interface IGDPRProcessor
    {
        string SendRequest(GDPRMessage message);
    }
}
