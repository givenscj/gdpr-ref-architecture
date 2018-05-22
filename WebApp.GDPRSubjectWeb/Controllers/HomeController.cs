using GDPR.Util;
using GDPR.Util.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void SetupViewBag(dynamic ViewBag)
        {
        }

        [Authorize]
        public ActionResult GDPRRequestPost(string type)
        {
            MasterGDPRHelper.CreateGDPRRequest(this.User.Identity.Name, type);
            return View("Index", "Home");
        }

        [Authorize]
        public ActionResult GDPRRequest()
        {
            return View();
        }

        [Authorize]
        public ActionResult DataExport()
        {
            GDPRDatabaseEntities e = Util.GetGDPRDBContext(Util.GDPRSQLConnectionString);

            SetupViewBag(ViewBag);

            GDPRSubject s = new GDPRSubject();
            s.Email = this.User.Identity.Name;            
            string subjectId = MasterGDPRHelper.FindSubject(s).SubjectId.ToString();
            List<GDPRSubjectRequestApplication> requests = e.Database.SqlQuery<GDPRSubjectRequestApplication>("select sar.* from SubjectRequest sr, SubjectRequestApplication sar where sr.subjectrequestid = sar.subjectrequestid and sr.subjectid = '" + subjectId + "'").ToList();

            ViewBag.Requests = requests;

            return View();
        }

        [Authorize]
        public ActionResult Notifications()
        {
            GDPRDatabaseEntities e = Util.GetGDPRDBContext(Util.GDPRSQLConnectionString);

            SetupViewBag(ViewBag);

            GDPRSubject s = new GDPRSubject();
            s.Email = this.User.Identity.Name;            
            string subjectId = MasterGDPRHelper.FindSubject(s).SubjectId.ToString();
            List<GDPRSubjectRequestApplication> requests = e.Database.SqlQuery<GDPRSubjectRequestApplication>("select sar.* from SubjectRequest sr, SubjectRequestApplication sar where sr.subjectrequestid = sar.subjectrequestid and sr.subjectid = '" + subjectId + "'").ToList();

            ViewBag.Requests = requests;

            return View();
        }

        public ActionResult Consents()
        {
            GDPRDatabaseEntities e = Util.GetGDPRDBContext(Util.GDPRSQLConnectionString);

            SetupViewBag(ViewBag);

            GDPRSubject s = new GDPRSubject();
            s.Email = this.User.Identity.Name;            
            string subjectId = MasterGDPRHelper.FindSubject(s).SubjectId.ToString();
            List<GDPRSubjectRequestApplication> requests = e.Database.SqlQuery<GDPRSubjectRequestApplication>("select sar.* from SubjectRequest sr, SubjectRequestApplication sar where sr.subjectrequestid = sar.subjectrequestid and sr.subjectid = '" + subjectId + "'").ToList();

            ViewBag.Requests = requests;

            return View();
        }
    }
}