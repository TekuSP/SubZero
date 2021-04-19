using System;

namespace SubZero
{
    [Serializable]
    public class TemperatureSettings
    {
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
        public double Value4 { get; set; }
        public double Value5 { get; set; }
        public double Value6 { get; set; }
        public static TemperatureSettings FactoryCPU => new TemperatureSettings()
        {
            Value1 = 0,
            Value2 = 40,
            Value3 = 48,
            Value4 = 56,
            Value5 = 64,
            Value6 = 72
        };
        public static TemperatureSettings FactoryGPU => new TemperatureSettings()
        {
            Value1 = 0,
            Value2 = 50,
            Value3 = 60,
            Value4 = 70,
            Value5 = 77,
            Value6 = 84
        };
    }
    [Serializable]
    public class Settings
    {
        public Profile[] Profiles { get; set; }
        public int Version { get; set; }
    }
    [Serializable]
    public class Profile
    {
        public TemperatureSettings CPU { get; set; }
        public TemperatureSettings GPU { get; set; }
        public string Name { get; set; }
    }
}
