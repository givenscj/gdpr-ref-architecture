using GDPR.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Messages
{
    public class DeleteMessage : ApplicationMessage
    {
        public override bool Process()
        {
            return true;

            //return base.Process();
        }
    }
}
