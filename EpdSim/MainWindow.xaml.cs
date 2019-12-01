using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EpdSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool RunSim = false;
        private static readonly Random rand = new Random();
        private string endCode = "**ENDCODE**";
        private KafkaBroker kBroker;

        public MainWindow()
        {
            InitializeComponent();

            // Establish binding with view
            KProducer.DataContext = KafkaMessage.Instance;
            KConsumer.DataContext = KafkaMessage.Instance;

            SimName.Text = Buffer.Instance.KConfig.SimName;
            BootstrapServer.Text = Buffer.Instance.KConfig.BootstrapServer;
            ProducerTopic.Text = Buffer.Instance.KConfig.ProducerTopic;
            ConsumerTopic.Text = Buffer.Instance.KConfig.ConsumerTopic;
            GroupId.Text = Buffer.Instance.KConfig.GroupId;

            // Initialize Kafka Broker class
            kBroker = new KafkaBroker();
        }

        private void SaveKafkaSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            Buffer.Instance.KConfig.SimName = SimName.Text;
            Buffer.Instance.KConfig.BootstrapServer = BootstrapServer.Text;
            Buffer.Instance.KConfig.ProducerTopic = ProducerTopic.Text;
            Buffer.Instance.KConfig.ConsumerTopic = ConsumerTopic.Text;
            Buffer.Instance.KConfig.GroupId = GroupId.Text;
        }

        private void RunKafkaBroker_Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => Parallel.Invoke(
                () => kBroker.RunConsumer(),
                () => kBroker.RunProducer()
            ));
        }


        private async void StartSimulator_Button_Click(object sender, RoutedEventArgs e)
        {
            Config epdConfig = MakeData.ReadConfigData("epd.conf");
            RunSim = true;
            await Task.Run(() =>
            {
                while (RunSim && !Buffer.Instance.cts.IsCancellationRequested)
                {
                    string dataRecord = MakeData.MakeSimulatedRecord(epdConfig, Buffer.Instance.KConfig.ProducerTopic);
                    Buffer.Instance.ProducerMessageQueue.Add(dataRecord);
                    Thread.Sleep(rand.Next(1000, 2000));
                }
            });
        }

        private void StopSimulator_Button_Click(object sender, RoutedEventArgs e)
        {
            RunSim = false;
        }
        private void CloseProgram_Button_Click(object sender, RoutedEventArgs e)
        {
            RunSim = false;
            Thread.Sleep(2000);
            Buffer.Instance.ProducerMessageQueue.Add(endCode);
            Buffer.Instance.cts.Cancel();
            while (!(Buffer.Instance.ConsumerShutdown && Buffer.Instance.ProducerShutdown))
            {
                Thread.Sleep(1000);
            }
            Application.Current.Shutdown();
        }
    }
}
