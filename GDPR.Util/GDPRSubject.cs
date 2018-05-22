using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util
{
    public class GDPRSubject
    {
        public GDPRSubject()
        {
        }
        public GDPRSubject(GDPRSubject subject)
        {
            this.SubjectId = subject.SubjectId;
            this.FirstName = subject.FirstName;
            this.LastName = subject.LastName;
            this.Email = subject.Email;
            this.HomePhone = subject.HomePhone;
            this.WorkPhone = subject.WorkPhone;
            this.MobilePhone = subject.MobilePhone;
        }

        public string ApplicationSubjectId { get; set; }
        public string SubjectId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Address { get; set; }
        public System.Collections.Hashtable Attributes { get; set; }
        public string GovernmentId { get; set; }
    }
}
