using GDPR.Util;
using GDPR.Util.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GDPR.Applications
{
    public class Office365 : GDPRApplication
    {
        public override string ExportData(string applicationSubjectId)
        {
            //login to office 365
            string username = "";
            string password = "";
            Util.Util.LoginOffice365(username, password);

            string name = this.Request.Subject.Email;
            string json = "{\"identity\":\"\",\"name\":\"" + name + "\",\"language\":\"\",\"includeUserAppContent\":false,\"contentQuery\":{\"ShouldUseRawContent\":false,\"RawContent\":\"\",\"Root\":{\"$type\":\"Microsoft.Office.Compliance.Repository.ComplexCondition, Microsoft.Office.Compliance.Repository\",\"Logic\":\"And\",\"Conditions\":[{\"$type\":\"Microsoft.Office.Compliance.Repository.SimpleCondition, Microsoft.Office.Compliance.Repository\",\"IsExclusive\":false,\"IsLeaf\":true,\"Operator\":\"ContainsAllof\",\"Property\":\"Keywords\",\"Values\":[\"" + name + "\"],\"ValueType\":\"String\"}],\"IsExclusive\":false}},\"searchAllHoldLocations\":false,\"searchAllExchange\":true,\"searchAllSharepoint\":true,\"searchAllPublicFolders\":true}";

            //create a compliance search...
            string url = "https://protection.office.com/api/ComplianceSearch/StartSearch?id=" + name + "&retry=false";
            string html = Util.Util.DoPost(url, json, null);

            //start the search...
            url = "https://protection.office.com/api/ComplianceSearch/StartSearch?id=" + name +"&retry=false";
            html = Util.Util.DoPut(url, null);
            
            url = "https://protection.office.com/api/ComplianceSearch/StartSearch?id=" + name + "&retry=false";
            html = Util.Util.DoGet(url, null);
            dynamic searchJson = JsonConvert.DeserializeObject(html);
            
            //search has to be completed...
            if (searchJson.Status == 4)
            {
                //start an export...                
                json = "{\"searchName\":\"" + name + "\",\"actionType\":2,\"options\":{\"scenario\":1,\"scope\":1,\"includeSharePointDocumentVersions\":false,\"enableDedupe\":false,\"format\":1,\"exchangeArchiveFormat\":1,\"exportToArchive\":false,\"sharePointArchiveFormat\":3,\"scopeDetails\":[]}}";                
                url = "https://protection.office.com/api/ComplianceSearchAction";
                html = Util.Util.DoPost(url, json, null);

                bool notReady = true;

                //download the package...
                while (notReady)
                {
                    html = Util.Util.DoGet("https://protection.office.com/api/ComplianceSearchAction?getDetails=true&id=" + name, null);
                    dynamic statusJson = JsonConvert.DeserializeObject(html);

                    //running
                    if (statusJson.Status == 3)
                    {
                        //return download...
                        return statusJson.DownloadLink;
                    }

                    Thread.Sleep(100);
                }                
            }

            return base.ExportData(applicationSubjectId);
        }

        public override List<GDPRMessage> GetAllRecords()
        {
            //get all users from active directory...

            //get all invited users...

            return null;
        }

        
        void RecordCreateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordCreateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        
        void RecordDeleteIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordDeleteOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordHold(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordNotify(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordSearch(GDPRSubject search, List<GDPRSubject> results)
        {
            throw new NotImplementedException();
        }        

        void RecordUpdateIn(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void RecordUpdateOut(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }

        void ValidateSubject(GDPRSubject subject)
        {
            throw new NotImplementedException();
        }
    }
}
