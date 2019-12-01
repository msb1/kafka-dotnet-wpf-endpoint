using System.Collections.Generic;

namespace EpdSim
{
    // Config class defines overall endpoint setup
    class Config
    {
        public bool Simulator { get; set; } = true;         // True = run simulator
        public List<Category> Cats { get; set; }
        public List<Sensor> Sensors { get; set; }
        public double Success { get; set; }                 // success rate (fraction of one value outputs for simulator)
        public double ErrRate { get; set; }                 // error rate for category (switch threshold to opposite

        //public Config() { }

        public Config()
        {
            Cats = new List<Category>();
            Sensors = new List<Sensor>();
        }
    }

    // Category class defines category parameters
    class Category
    {
        public string Label { get; set; }
        public List<string> Level { get; set; }             // all levels are defined as strings (even numbers)
        public List<double> ZeroThreshold { get; set; }     // thresholds for uniform [0,1] rv simulator zero output
        public List<double> OneThreshold { get; set; }      // thresholds for uniform [0,1] rv simulator one output

        //public Category() { }

        public Category()
        {
            Level = new List<string>();
            ZeroThreshold = new List<double>();
            OneThreshold = new List<double>();
        }

        //public Category(string label, List<string> level, List<double> zeroThreshold, List<double> oneThreshold)
        //{
        //    Label = label;
        //    Level = level;
        //    ZeroThreshold = zeroThreshold;
        //    OneThreshold = oneThreshold;
        //}
    }

    // Sensor class defines a sensor
    // first four parameters define sensor; remaining parameters are for simulator
    // sensor simulators are as follows:
    //  	First sublist entry (case)
    //            0 = two means correlated with two class (normal distributions)
    //            1 = two means anti-correlated with  two class (normal distributions)
    //            2 = one mean -- no correlation (normal distribution) - use only zero mean and std dev
    //            3 = uniformly distributed [0, 1] with no correlation
    //      The scale factor multiplies the rv for the output
    //		Two output classes are assumed (0 and 1 or pass and fail)
    class Sensor
    {
        public string Label { get; set; }
        public double UpperLimit { get; set; }      // max value for sensor
        public double LowerLimit { get; set; }      // min value for sensor
        public double UpperControl { get; set; }    // upper warning or control for sensor
        public double LowerControl { get; set; }    // lower warning or control for sensor
        public int SimType { get; set; }            // simulator type
        public double ZeroMean { get; set; }        // mean of zero output simulator
        public double ZeroStdDev { get; set; }      // std dev of zero output simulator
        public double OneMean { get; set; }         // mean of one output simulator
        public double OneStdDev { get; set; }       // std dev of one output simulator    
        public double Scale { get; set; }           // scale of simulator  

        public Sensor() { }

        public Sensor(string label, double upperLimit, double lowerLimit, double upperControl, double lowerControl,
            int simType, double zeroMean, double zeroStdDev, double oneMean, double oneStdDev, double scale)
        {
            Label = label;
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
            UpperControl = upperControl;
            LowerControl = lowerControl;
            SimType = simType;
            ZeroMean = zeroMean;
            ZeroStdDev = zeroStdDev;
            OneMean = oneMean;
            OneStdDev = oneStdDev;
            Scale = scale;
        }
    }

    // EpdData class has simulator or endpoint outputs for categories and sensors
    class EpdData
    {
        public string CurrentTime { get; set; }
        public string Topic { get; set; }
        public Dictionary<string, string> CatMap { get; set; }
        public Dictionary<string, string> SensorMap { get; set; }
        public int Result { get; set; }

        public EpdData()
        {
            CatMap = new Dictionary<string, string>();
            SensorMap = new Dictionary<string, string>();
        }
    }


}
