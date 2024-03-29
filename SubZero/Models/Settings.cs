﻿using System;

namespace SubZero.Models
{
    /// <summary>
    /// Profile information
    /// </summary>
    [Serializable]
    public class Profile
    {
        #region Public Constructors

        public Profile()
        {
            CPU = new TemperatureSettings();
            GPU = new TemperatureSettings();
            Guid = Guid.NewGuid();
        }
        public Profile(Profile profileBasedOn)
        {
            Name = profileBasedOn.Name.Clone().ToString();
            CPU = new TemperatureSettings(profileBasedOn.CPU);
            GPU = new TemperatureSettings(profileBasedOn.GPU);
            Guid = Guid.NewGuid();
        }
        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Name of the profile
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CPU Fan speeds
        /// </summary>
        public TemperatureSettings CPU { get; set; }

        /// <summary>
        /// GPU Fan speeds
        /// </summary>
        public TemperatureSettings GPU { get; set; }

        /// <summary>
        /// Unique profile GUID
        /// </summary>
        public Guid Guid { get; set; }
        #endregion Public Properties
    }

    /// <summary>
    /// Global application settings to be saved in XML format
    /// </summary>
    [Serializable]
    public class Settings
    {
        #region Public Constructors

        public Settings()
        {
            Profiles = Array.Empty<Profile>();
            PollingSpeed = 2000;
            TurnedOn = false;
            UseCelsius = true;
            SelectedProfileIndex = 0;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Model of laptop where settings were generated, if different, user must be informed
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Polling speed in milliseconds
        /// </summary>
        public int PollingSpeed { get; set; }

        /// <summary>
        /// Used profiles, can never be null or empty array!
        /// </summary>
        public Profile[] Profiles { get; set; }

        /// <summary>
        /// Which profile is currently selected?
        /// </summary>
        public int SelectedProfileIndex { get; set; }

        /// <summary>
        /// Get current profile by index
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]//IGNORE!
        public Profile CurrentProfile => Profiles[SelectedProfileIndex];

        /// <summary>
        /// Is SubZero active?
        /// </summary>
        public bool TurnedOn { get; set; }

        /// <summary>
        /// Use Celsius or Fahrenheit?
        /// </summary>
        public bool UseCelsius { get; set; }

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
        #region Public Constructors

        public TemperatureSettings()
        {
            Value1 = new TempFan();
            Value2 = new TempFan();
            Value3 = new TempFan();
            Value4 = new TempFan();
            Value5 = new TempFan();
            Value6 = new TempFan();
        }
        public TemperatureSettings(TemperatureSettings basedOn)
        {
            Value1 = new TempFan(basedOn.Value1);
            Value2 = new TempFan(basedOn.Value2);
            Value3 = new TempFan(basedOn.Value3);
            Value4 = new TempFan(basedOn.Value4);
            Value5 = new TempFan(basedOn.Value5);
            Value6 = new TempFan(basedOn.Value6);
        }
        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Factory defaults for most laptops, for CPU Fan speeds
        /// </summary>
        public static TemperatureSettings FactoryCPU => new TemperatureSettings()
        {
            Value1 = new TempFan(0, 0),
            Value2 = new TempFan(50, 40),
            Value3 = new TempFan(56, 48),
            Value4 = new TempFan(62, 56),
            Value5 = new TempFan(70, 64),
            Value6 = new TempFan(75, 72)
        };

        /// <summary>
        /// Factory defaults for most laptops, for GPU Fan speeds
        /// </summary>
        public static TemperatureSettings FactoryGPU => new TemperatureSettings()
        {
            Value1 = new TempFan(0, 0),
            Value2 = new TempFan(55, 50),
            Value3 = new TempFan(60, 60),
            Value4 = new TempFan(65, 70),
            Value5 = new TempFan(70, 77),
            Value6 = new TempFan(75, 84)
        };

        /// <summary>
        /// MSI Trigger temp / Fan Value 1
        /// </summary>
        public TempFan Value1 { get; set; }

        /// <summary>
        /// MSI Trigger temp / Fan Value 2
        /// </summary>
        public TempFan Value2 { get; set; }

        /// <summary>
        /// MSI Trigger temp / Fan Value 3
        /// </summary>
        public TempFan Value3 { get; set; }

        /// <summary>
        /// MSI Trigger temp / Fan Value 4
        /// </summary>
        public TempFan Value4 { get; set; }

        /// <summary>
        /// MSI Trigger temp / Fan Value 5
        /// </summary>
        public TempFan Value5 { get; set; }

        /// <summary>
        /// MSI Trigger temp / Fan Value 6
        /// </summary>
        public TempFan Value6 { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Structure for Fan Speeds and Temperature
    /// </summary>
    [Serializable]
    public record TempFan
    {
        /// <summary>
        /// Constructs TempFan
        /// </summary>
        /// <param name="temperature">Temperature</param>
        /// <param name="fanSpeed">Fan Speed</param>
        public TempFan(int temperature, int fanSpeed)
        {
            Temperature = temperature;
            FanSpeed = fanSpeed;
        }
        public TempFan(TempFan basedOn)
        {
            Temperature = basedOn.Temperature;
            FanSpeed = basedOn.FanSpeed;
        }
        /// <summary>
        /// Constructs empty TempFan (Serialization)
        /// </summary>
        public TempFan()
        {
        }
        /// <summary>
        /// Temperature in degrees Celsius
        /// </summary>
        public int Temperature { get; set; }
        /// <summary>
        /// Fan Speed in percentages
        /// </summary>
        public int FanSpeed { get; set; }
    }
}