using GDPR.Applications;
using GDPR.Util;
using GDPR.Util.Messages;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Console.Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            //message processor...
            StartReceiveMessages("$default");

            //message processor...
            StartReceiveMessages("gdprgroup");
        }

        static void CheckForChanges()
        {
            foreach (Application a in MasterGDPRHelper.GDPRDatabase.Applications)
            {
                if (a.IsActive)
                {
                    Type t = Type.GetType(a.ProcessorClass + ",GDPR.Util");
                    GDPRApplication app = (GDPRApplication)Activator.CreateInstance(t);

                    List<GDPRMessage> messages = app.GetChanges(a.ChangeDate);

                    foreach (GDPRMessage msg in messages)
                    {
                        MasterGDPRHelper.SendMessage(msg);
                    }

                    MasterGDPRHelper.UpdateApplicationChangeDate(a.ApplicationId, DateTime.Now);
                }
            }
        }

        static private void StartReceiveMessages(string consumerGroup)
        {
            MainAsync(consumerGroup).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string consumerGroup)
        {            
            System.Console.WriteLine("Registering EventProcessor...");

            MasterGDPRHelper.StartMessageProcessing(consumerGroup, out EventProcessorHost eventProcessorHost);

            System.Console.WriteLine("Receiving. Press ENTER to stop worker.");
            System.Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
