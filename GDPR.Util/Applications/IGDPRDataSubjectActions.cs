using GDPR.Util.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util
{
    public interface IGDPRDataSubjectActions
    {
        void ProcessRequest(GDPRMessage message);
        void ValidateSubject(GDPRSubject subject);
        void RecordCreateIn(GDPRSubject subject);
        void RecordCreateOut(GDPRSubject subject);
        List<GDPRSubject> RecordSearch(GDPRSubject search);
        void RecordNotify(GDPRSubject subject);
        void RecordDeleteIn(GDPRSubject subject);
        void RecordDeleteOut(GDPRSubject subject);
        void RecordHold(GDPRSubject subject);
        void RecordUpdateIn(GDPRSubject subject);
        void RecordUpdateOut(GDPRSubject subject);
        List<GDPRMessage> GetAllRecords();
        List<GDPRMessage> GetChanges(DateTime changeDate);
        string ExportData(string applicationSubjectId);
    }    
}
