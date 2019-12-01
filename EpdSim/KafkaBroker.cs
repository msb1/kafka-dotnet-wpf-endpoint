using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EpdSim
{
    class KafkaBroker
    {

        // Kafka Consumer will stop when Global Cancellation Token is cancelled
        public void RunConsumer()
        {
            var conf = new ConsumerConfig
            {
                GroupId = Buffer.Instance.KConfig.GroupId,
                BootstrapServers = Buffer.Instance.KConfig.BootstrapServer,
                //AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                List<string> ctopics = new List<string>() { Buffer.Instance.KConfig.ConsumerTopic };
                c.Subscribe(ctopics);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(Buffer.Instance.token);
                            Buffer.Instance.ConsumerMessageQueue.Add(cr);
                            //Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                            KafkaMessage.Instance.CMessage = MakeData.MakeViewRecord(cr.Value, KafkaMessage.Instance.LastCMessages);
                            Console.WriteLine("CONSUMER: " + KafkaMessage.Instance.PMessage);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                    // Stop Producer and Consumer Message Queues
                    Buffer.Instance.ProducerMessageQueue.CompleteAdding();
                    Buffer.Instance.ConsumerMessageQueue.CompleteAdding();
                }
            }

            Buffer.Instance.ConsumerShutdown = true;
        }

        // Task method will end when complete adding is added to producer message queue
        // Foreach consuming enumerable will stop when it reads CompleteAdding()
        public void RunProducer()
        {
            var config = new ProducerConfig { BootstrapServers = Buffer.Instance.KConfig.BootstrapServer};

            Action<DeliveryReport<Null, string>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                foreach (string msg in Buffer.Instance.ProducerMessageQueue.GetConsumingEnumerable())
                {
                    p.Produce(Buffer.Instance.KConfig.ProducerTopic, new Message<Null, string> { Value = msg }, handler);
                    KafkaMessage.Instance.PMessage = " ";
                    KafkaMessage.Instance.PMessage = MakeData.MakeViewRecord(msg, KafkaMessage.Instance.LastPMessages);
                    Console.WriteLine("PRODUCER:  " + KafkaMessage.Instance.PMessage);
                }
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }

            Buffer.Instance.ProducerShutdown = true;
        }
    }

}
