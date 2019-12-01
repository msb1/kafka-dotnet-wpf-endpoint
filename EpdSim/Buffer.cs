using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace EpdSim
{
    class Buffer
    {
        // producer and consumer messages queues for Kafka Broker (for Producer/Consumer design pattern)
        public BlockingCollection<ConsumeResult<Ignore, string>> ConsumerMessageQueue { get; set; }
        public BlockingCollection<string> ProducerMessageQueue { get; set; }
        public KafkaConfig KConfig { get; set; }
        public CancellationTokenSource cts { get; }
        public  CancellationToken token { get; }
        public bool ProducerShutdown { get; set; } = false;
        public bool ConsumerShutdown { get; set; } = false;

        // instantiate as a Singelton
        private static readonly Lazy<Buffer> lazy = new Lazy<Buffer>(() => new Buffer());
        public static Buffer Instance { get { return lazy.Value; } }

        private Buffer()
        {
            ConsumerMessageQueue = new BlockingCollection<ConsumeResult<Ignore, string>>();
            ProducerMessageQueue = new BlockingCollection<string>();
            KConfig = new KafkaConfig();
            cts = new CancellationTokenSource();
            token = cts.Token;
        }
    }

    public sealed class KafkaConfig
    {
        public string SimName { get; set; } = "EPDSIM";
        public string BootstrapServer { get; set; }
        public string ProducerTopic { get; set; }
        public string ConsumerTopic { get; set; }
        public string GroupId { get; set; } = "barnwaldo";
    }

}

