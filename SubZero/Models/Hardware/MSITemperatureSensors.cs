using System;

namespace SubZero.Models.Hardware
{
    /// <summary>
    /// Hardware Temperature Sensors for MSI
    /// </summary>
    public class MSITemperatureSensors
    {
        #region Public Constructors

        /// <summary>
        /// Intializes sensors with WMIHelper
        /// </summary>
        /// <param name="helper">WMIHelper to use</param>
        public MSITemperatureSensors(MSIWmiHelper helper)
        {
            WMIHelper = helper;
        }

        #endregion Public Constructors

        #region Private Properties

        private MSIWmiHelper WMIHelper { get; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Gets CPU Temperature in Celsius
        /// </summary>
        /// <returns>Temperature in Celsius</returns>
        public double GetCPUTemperatureCelsius()
        {
            using (var cpu = WMIHelper.MSI_CPU.Get())
            {
                bool skip = true;
                foreach (var item in cpu)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                    return Convert.ToDouble(item["CPU"]);
                }
            }
            return -1; //This should never happen, we are not on MSI if we are here
        }

        /// <summary>
        /// Gets CPU Temperature in Fahrenheit
        /// </summary>
        /// <returns>Temperature in Fahrenheit</returns>
        public double GetCPUTemperatureFahrenheit() => ((double)GetCPUTemperatureCelsius() / 5.0 * 9.0 + 32.0);

        /// <summary>
        /// Gets GPU Temperature in Celsius
        /// </summary>
        /// <returns>Temperature in Celsius</returns>
        public double GetGPUTemperatureCelsius()
        {
            using (var gpu = WMIHelper.MSI_GPU.Get())
            {
                bool skip = true;
                foreach (var item in gpu)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                    return Convert.ToDouble(item["VGA"]);
                }
            }
            return -1; //This should never happen, we are not on MSI if we are here
        }

        /// <summary>
        /// Gets GPU Temperature in Fahrenheit
        /// </summary>
        /// <returns>Temperature in Fahrenheit</returns>
        public double GetGPUTemperatureFahrenheit() => ((double)GetGPUTemperatureCelsius() / 5.0 * 9.0 + 32.0);

        #endregion Public Methods
    }
}