using System;

namespace SubZero
{
    /// <summary>
    /// Profile information
    /// </summary>
    [Serializable]
    public class Profile
    {
        #region Public Properties

        /// <summary>
        /// CPU Fan speeds
        /// </summary>
        public TemperatureSettings CPU { get; set; }

        /// <summary>
        /// GPU Fan speeds
        /// </summary>
        public TemperatureSettings GPU { get; set; }

        /// <summary>
        /// Name of the profile
        /// </summary>
        public string Name { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Global application settings to be saved in XML format
    /// </summary>
    [Serializable]
    public class Settings
    {
        #region Public Properties

        /// <summary>
        /// Model of laptop where settings were generated, if different, user must be informed
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Used profiles, can never be null or empty array!
        /// </summary>
        public Profile[] Profiles { get; set; }

        /// <summary>
        /// Version of settings, if older is detected, upgrade is required
        /// </summary>
        public int Version { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Fan profiles for all six possible values
    /// </summary>
    [Serializable]
    public class TemperatureSettings
    {
        #region Public Properties

        /// <summary>
        /// Factory defaults for most laptops, for CPU Fan speeds
        /// </summary>
        public static TemperatureSettings FactoryCPU => new TemperatureSettings()
        {
            Value1 = 0,
            Value2 = 40,
            Value3 = 48,
            Value4 = 56,
            Value5 = 64,
            Value6 = 72
        };

        /// <summary>
        /// Factory defaults for most laptops, for GPU Fan speeds
        /// </summary>
        public static TemperatureSettings FactoryGPU => new TemperatureSettings()
        {
            Value1 = 0,
            Value2 = 50,
            Value3 = 60,
            Value4 = 70,
            Value5 = 77,
            Value6 = 84
        };

        /// <summary>
        /// MSI Fan Value 1
        /// </summary>
        public double Value1 { get; set; }

        /// <summary>
        /// MSI Fan Value 2
        /// </summary>
        public double Value2 { get; set; }

        /// <summary>
        /// MSI Fan Value 3
        /// </summary>
        public double Value3 { get; set; }

        /// <summary>
        /// MSI Fan Value 4
        /// </summary>
        public double Value4 { get; set; }

        /// <summary>
        /// MSI Fan Value 5
        /// </summary>
        public double Value5 { get; set; }

        /// <summary>
        /// MSI Fan Value 6
        /// </summary>
        public double Value6 { get; set; }

        #endregion Public Properties
    }
}