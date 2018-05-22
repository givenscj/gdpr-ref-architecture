using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using GDPR.Util;

namespace GDPRWindowsService
{
    public partial class Service1 : ServiceBase
    {
        static EventProcessorHost eventProcessorHost;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string applicationId = System.Configuration.ConfigurationManager.AppSettings["applicationId"];
            string eventHubConnectionString = System.Configuration.ConfigurationManager.AppSettings["eventHubConnectionString"];
            string eventHubName = System.Configuration.ConfigurationManager.AppSettings["eventHubName"] ;
            string storageAccountName = System.Configuration.ConfigurationManager.AppSettings["storageAccountName"];
            string storageAccountKey = System.Configuration.ConfigurationManager.AppSettings["storageAccountKey"];
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering EventProcessor...");
            var options = new EventProcessorOptions();
            options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<GDPREventProcessor>(options).Wait();            
        }

        protected override void OnStop()
        {
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
