
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using GDPR.Util;
using GDPR.Util.Messages;
using System.Text;
using Microsoft.Azure.WebJobs.ServiceBus;
using System.Collections.Generic;

namespace GDPRRequestProcessor
{
    public static class GDPRRequest
    {
        static GDPRDatabaseEntities gdprDB = new GDPRDatabaseEntities();

        [FunctionName("GDPRRequest")]
        public static void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            string data = "";
            GDPRMessageWrapper message = (GDPRMessageWrapper)Newtonsoft.Json.JsonConvert.DeserializeObject(data);            
        }
    }
}
