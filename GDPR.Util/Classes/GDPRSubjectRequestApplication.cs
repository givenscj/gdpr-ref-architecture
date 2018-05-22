using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util.Classes
{
    public class GDPRSubjectRequestApplication
    {
        public string ApplicationName { get; set; }
        public DateTime CompletedDate { get; set; }
        public string DataExportLink { get; set; }
        public string Status { get; set; }
        public string ApplicationId { get; set; }
    }
}
