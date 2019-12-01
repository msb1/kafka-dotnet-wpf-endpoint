using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpdSim
{
    public sealed class KafkaMessage : INotifyPropertyChanged
    {
        public string PMessage { get; set; }
        public string CMessage { get; set; }

        public List<string> LastPMessages { get; set; }
        public List<string> LastCMessages { get; set; }

        public int NumMessages { get; set; } = 3;

        public event PropertyChangedEventHandler PropertyChanged;

        // instantiate as a Singelton
        private static readonly Lazy<KafkaMessage> lazy = new Lazy<KafkaMessage>(() => new KafkaMessage());
        public static KafkaMessage Instance { get { return lazy.Value; } }

        private KafkaMessage()
        {
            LastPMessages = new List<string>();
            LastCMessages = new List<string>();
        }

    }
}
