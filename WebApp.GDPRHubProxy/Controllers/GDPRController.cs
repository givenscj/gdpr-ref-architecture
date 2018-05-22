using GDPR.Util;
using GDPR.Util.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.GDPRHubProxy.Controllers
{
    public class GDPRController : ApiController
    {
        public string SubjectDelete([FromUri]string applicationId, [FromUri]string subjectId, [FromUri]string emailAddress)
        {
            try
            {
                GDPRMessage msg = new GDPRMessage();

                DeleteMessage dm = new DeleteMessage();
                dm.ApplicationId = applicationId;
                dm.ApplicationSubjectId = subjectId;
                dm.Direction = "in";
                msg = dm;

                GDPRSubject s = new GDPRSubject();
                s.Email = emailAddress;
                msg.Subject = s;

                MasterGDPRHelper.SendMessage(msg);
            }
            catch
            {
                return "Failure";
            }

            return "Success";
        }

        public string SubjectCreate([FromUri]string applicationId, [FromUri]string subjectId, [FromUri]string emailAddress)
        {
            try
            {
                GDPRMessage msg = new GDPRMessage();

                CreateMessage dm = new CreateMessage();
                dm.ApplicationId = applicationId;
                dm.ApplicationSubjectId = subjectId;
                dm.Direction = "in";
                msg = dm;

                GDPRSubject s = new GDPRSubject();
                s.Email = emailAddress;
                msg.Subject = s;

                MasterGDPRHelper.SendMessage(msg);
            }
            catch
            {
                return "Failure";
            }

            return "Success";
        }

        public string SubjectHold([FromUri]string applicationId, [FromUri]string subjectId, [FromUri]string emailAddress)
        {
            try
            {
                GDPRMessage msg = new GDPRMessage();

                HoldMessage dm = new HoldMessage();
                dm.ApplicationId = applicationId;
                dm.ApplicationSubjectId = subjectId;
                dm.Direction = "in";
                msg = dm;

                GDPRSubject s = new GDPRSubject();
                s.Email = emailAddress;
                msg.Subject = s;

                MasterGDPRHelper.SendMessage(msg);
            }
            catch
            {
                return "Failure";
            }

            return "Success";
        }

        public string CreateGDPRRequest([FromUri]string emailAddress, [FromUri]string type)
        {
            try
            {
                return MasterGDPRHelper.CreateGDPRRequest(emailAddress, type);                
            }
            catch
            {
                return "Failure";
            }            
        }        
    }
}
