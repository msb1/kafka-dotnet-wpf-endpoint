using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EpdSim
{
    class MakeData
    {
        private static readonly Random rand = new Random();
        private static int dataRecordCtr = 0;

        public static Config ReadConfigData(string configFile)
        {
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
            Console.WriteLine(config);
            return config;
        }

        public static string MakeViewRecord(string currentMessage, List<string> lastMessages)
        {
            lastMessages.Add(currentMessage);
            if (lastMessages.Count > KafkaMessage.Instance.NumMessages)
            {
                lastMessages.RemoveAt(0);
            }

            StringBuilder sb = new StringBuilder();
            foreach(string s in lastMessages)
            {
                sb.Append(s).Append("\n\n");
            }
            return sb.ToString();
        }

        public static double NextGaussian(double mu, double sigma)
        {
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mu + sigma * randStdNormal;
            return randNormal;
        }

        public static string MakeSimulatedRecord(Config config, string topic)
        {
            dataRecordCtr++;
            // initialize epdData class
            EpdData epd = new EpdData();
            epd.Topic = topic;
            // get current time and convert to formatted string
            epd.CurrentTime = DateTime.Now.ToString("G");

            // determine output (1 or 0) for data record
            if (rand.NextDouble() < config.Success)
            {
                epd.Result = 1;
            }
            else
            {
                epd.Result = 0;
            }
            // generate sensor endpoint (numerics) simulated data
            foreach (Sensor s in config.Sensors)
            {
                double output = 0;
                switch (s.SimType)
                {
                    case 0:
                        if (epd.Result == 1) output = NextGaussian(s.OneMean, s.OneStdDev);
                        else output = NextGaussian(s.ZeroMean, s.ZeroStdDev);
                        break;
                    case 1:
                        if (epd.Result == 1) output = NextGaussian(s.ZeroMean, s.ZeroStdDev);
                        else output = NextGaussian(s.OneMean, s.OneStdDev);
                        break;
                    case 2:
                        output = NextGaussian(s.OneMean, s.OneStdDev);
                        break;
                    case 3:
                        output = rand.NextDouble();
                        break;
                    default:
                        Console.WriteLine("Improper simulator type for record: {0}", dataRecordCtr);
                        break;
                }
                output *= s.Scale;
                epd.SensorMap.Add(s.Label, output.ToString("N3"));
            }

            // generate category endpoint simulated data
            foreach (Category c in config.Cats)
            {
                int result = 0;
                double rnd = rand.NextDouble();
                if (epd.Result == 1)
                {
                    for (int i = 0; i < c.Level.Count; i++)
                    {
                        if (rnd < c.OneThreshold[i])
                        {
                            result = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < c.Level.Count; i++)
                    {
                        if (rnd < c.ZeroThreshold[i])
                        {
                            result = i;
                            break;
                        }
                    }
                }
                // adjust category with random error rate
                if (rand.NextDouble() < config.ErrRate)
                {
                    result = rand.Next(0, c.Level.Count);
                }
                epd.CatMap.Add(c.Label, c.Level[result]);
            }
            return JsonConvert.SerializeObject(epd);
        }
    }
}