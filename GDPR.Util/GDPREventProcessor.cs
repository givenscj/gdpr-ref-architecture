using GDPR.Util.Messages;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util
{
    public class GDPREventProcessor : IEventProcessor
    {

        Stopwatch checkpointStopWatch;

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Console.WriteLine("SimpleEventProcessor initialized.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset);
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            DateTime checkPoint = MasterGDPRHelper.GetOffset(context.ConsumerGroupName, context.Lease.PartitionId);
            DateTime lastMessageDate = checkPoint;

            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());

                try
                {
                    Type t = typeof(GDPRMessageWrapper);
                    GDPRMessageWrapper w = (GDPRMessageWrapper)Newtonsoft.Json.JsonConvert.DeserializeObject(data, t);

                    if (w.MessageDate > checkPoint)
                    {
                        if (w.MessageDate > lastMessageDate)
                        {
                            lastMessageDate = w.MessageDate;
                        }

                        Console.WriteLine(string.Format("Message received.  Partition: '{0}', Data: '{1}'", context.Lease.PartitionId, data));
                        MasterGDPRHelper.ProcessRequest(w);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Skiping message {0}", eventData.Offset));
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(data);
                }                                
            }

            MasterGDPRHelper.SetOffSet(context.ConsumerGroupName, context.Lease.PartitionId, lastMessageDate);

            //Call checkpoint every 5 minutes, so that worker can resume processing from 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }
    }
}
