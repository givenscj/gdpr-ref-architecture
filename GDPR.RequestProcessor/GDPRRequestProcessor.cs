
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
    public static class Function1
    {
        static GDPRDatabaseEntities gdprDB = new GDPRDatabaseEntities();

        [FunctionName("GDPRRequestProcessor")]
        public static void Run([EventHubTrigger("gdprmessagehub", Connection = "EventHubConnectionAppSetting")] EventData myEventHubMessage, TraceWriter log)
        {
            string data = Encoding.UTF8.GetString(myEventHubMessage.GetBytes());
            log.Info($"{data}");            
            GDPRMessageWrapper message = (GDPRMessageWrapper)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            MasterGDPRHelper.ProcessRequest(message);            
        }                
    }
}
