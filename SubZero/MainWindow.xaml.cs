using MaterialDesignExtensions.Controls;

using Newtonsoft.Json;

using SubZero.Models;
using SubZero.Resources;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Threading;

namespace SubZero
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow, INotifyPropertyChanged
    {
        public string CPUTemperature { get; set; }
        public string GPUTemperature { get; set; }
        public string CPURPM { get; set; }
        public string GPURPM { get; set; }
        private bool temperatureInCelsius = true; //TODO: Move to better place
        private MSIHardwareMonitor HardwareMonitor { get; set; }
        #region Private Fields

        /// <summary>
        /// Config file location
        /// </summary>
        private const string configFileName = "subzero.config";

        /// <summary>
        /// Version of config file
        /// </summary>
        private const int configFileVersion = 1;

        private MSIWmiHelper MSIWmiHelper { get; set; }

        #endregion Private Fields

        #region Public Constructors

        public MainWindow()
        {
            InitializeComponent();
            //Init icons
            var imgSource = Helpers.ImageTools.ImageSourceFromBitmap(DesignSource.logo_icon);
            this.Icon = imgSource;
            this.TitleBarIcon = imgSource;
        }

        #endregion Public Constructors

        #region Public Events

#pragma warning disable 67

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore 67

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Can we delete this profile?
        /// </summary>
        public bool DeleteAllowed { get; set; }

        /// <summary>
        /// Did user edit any settings?
        /// </summary>
        public bool IsEdited { get; set; } = false;

        /// <summary>
        /// Is current profile saved?
        /// </summary>
        public bool IsSaved { get; set; } = false;

        /// <summary>
        /// Laptop model we are running on
        /// </summary>
        public string LaptopModel { get; set; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Apply settings to laptop
        /// </summary>
        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Check we have correct fan setting (Advanced, not simple)
            Profile appliedProfile = ((profilesList.SelectedItem as ListBoxItem).Tag as Profile);
            appliedProfile.CPU.Value1.FanSpeed = (int)Math.Round(cpu1.Value * 100);
            appliedProfile.CPU.Value2.FanSpeed = (int)Math.Round(cpu2.Value * 100);
            appliedProfile.CPU.Value3.FanSpeed = (int)Math.Round(cpu3.Value * 100);
            appliedProfile.CPU.Value4.FanSpeed = (int)Math.Round(cpu4.Value * 100);
            appliedProfile.CPU.Value5.FanSpeed = (int)Math.Round(cpu5.Value * 100);
            appliedProfile.CPU.Value6.FanSpeed = (int)Math.Round(cpu6.Value * 100);

            appliedProfile.GPU.Value1.FanSpeed = (int)Math.Round(gpu1.Value * 100);
            appliedProfile.GPU.Value2.FanSpeed = (int)Math.Round(gpu2.Value * 100);
            appliedProfile.GPU.Value3.FanSpeed = (int)Math.Round(gpu3.Value * 100);
            appliedProfile.GPU.Value4.FanSpeed = (int)Math.Round(gpu4.Value * 100);
            appliedProfile.GPU.Value5.FanSpeed = (int)Math.Round(gpu5.Value * 100);
            appliedProfile.GPU.Value6.FanSpeed = (int)Math.Round(gpu6.Value * 100);

            ((profilesList.SelectedItem as ListBoxItem)).Tag = appliedProfile; //Synchronize changes first

            int counter = 0; //Because Microsoft does not allow indexing of ManagmentObjectCollection
            using (var cpuData = MSIWmiHelper.MSI_CPU.Get())
            using (var gpudata = MSIWmiHelper.MSI_GPU.Get())
            {
                foreach (ManagementObject cpuValue in cpuData)
                {
                    //Write CPU Fan speeds to PC
                    //TODO: Write temperatures as well
                    switch (counter)
                    {
                        case 11:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value1.FanSpeed);
                            cpuValue.Put();
                            break;

                        case 12:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value2.FanSpeed);
                            cpuValue.Put();
                            break;

                        case 13:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value3.FanSpeed);
                            cpuValue.Put();
                            break;

                        case 14:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value4.FanSpeed);
                            cpuValue.Put();
                            break;

                        case 15:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value5.FanSpeed);
                            cpuValue.Put();
                            break;

                        case 16:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value6.FanSpeed);
                            cpuValue.Put();
                            break;
                    }
                    counter++;
                }
                counter = 0; //Reset for GPU
                foreach (ManagementObject gpuValue in gpudata)
                {
                    //Write GPU Fan speeds to PC
                    switch (counter)
                    {
                        case 11:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value1.FanSpeed);
                            gpuValue.Put();
                            break;

                        case 12:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value2.FanSpeed);
                            gpuValue.Put();
                            break;

                        case 13:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value3.FanSpeed);
                            gpuValue.Put();
                            break;

                        case 14:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value4.FanSpeed);
                            gpuValue.Put();
                            break;

                        case 15:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value5.FanSpeed);
                            gpuValue.Put();
                            break;

                        case 16:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value6.FanSpeed);
                            gpuValue.Put();
                            break;
                    }
                    counter++;
                }
            }
            IsEdited = false; //Editation finished
        }

        /// <summary>
        /// User pressed Discard!
        /// </summary>
        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Warn user about Discarding unsaved changes
            profilesList_SelectionChanged(sender, new SelectionChangedEventArgs(e.RoutedEvent, new List<object>(), new List<object>
            {
                profilesList.SelectedItem //Load back original item
            }));
        }

        /// <summary>
        /// User pressed factory settings, load factory configs into current profile
        /// </summary>
        private void factoryButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Warn user about Discarding unsaved changes
            var current = (profilesList.SelectedItem as ListBoxItem);
            (current.Tag as Profile).CPU = TemperatureSettings.FactoryCPU;
            (current.Tag as Profile).GPU = TemperatureSettings.FactoryGPU;
            profilesList_SelectionChanged(sender, new SelectionChangedEventArgs(e.RoutedEvent, new List<object>(), new List<object>
            {
                 current//Load factory item
            }));
        }

        /// <summary>
        /// Loads currently running system profile
        /// </summary>
        /// <returns>Profile system is running</returns>
        private Profile LoadCurrentSystemProfile()
        {
            TemperatureSettings cpuTemps = new TemperatureSettings();
            TemperatureSettings gpuTemps = new TemperatureSettings();
            int counter = 0; //Because Microsoft does not allow indexing of ManagmentObjectCollection
            //Prepare WMI
            using (var cpuData = MSIWmiHelper.MSI_CPU.Get())
            using (var gpudata = MSIWmiHelper.MSI_GPU.Get())
            {
                foreach (ManagementObject cpuValue in cpuData)
                {
                    //Load CPU values
                    switch (counter)
                    {
                        case 4:
                            cpuTemps.Value1.Temperature = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 5:
                            cpuTemps.Value2.Temperature = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 6:
                            cpuTemps.Value3.Temperature = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 7:
                            cpuTemps.Value4.Temperature = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 8:
                            cpuTemps.Value5.Temperature = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 9:
                            cpuTemps.Value6.Temperature = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 11:
                            cpuTemps.Value1.FanSpeed = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 12:
                            cpuTemps.Value2.FanSpeed = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 13:
                            cpuTemps.Value3.FanSpeed = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 14:
                            cpuTemps.Value4.FanSpeed = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 15:
                            cpuTemps.Value5.FanSpeed = Convert.ToInt32(cpuValue["CPU"]);
                            break;

                        case 16:
                            cpuTemps.Value6.FanSpeed = Convert.ToInt32(cpuValue["CPU"]);
                            break;
                    }
                    counter++;
                }
                counter = 0; //Reset for GPU
                foreach (ManagementObject gpuValue in gpudata)
                {
                    //Load GPU Values
                    switch (counter)
                    {
                        case 4:
                            gpuTemps.Value1.Temperature = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 5:
                            gpuTemps.Value2.Temperature = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 6:
                            gpuTemps.Value3.Temperature = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 7:
                            gpuTemps.Value4.Temperature = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 8:
                            gpuTemps.Value5.Temperature = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 9:
                            gpuTemps.Value6.Temperature = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 11:
                            gpuTemps.Value1.FanSpeed = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 12:
                            gpuTemps.Value2.FanSpeed = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 13:
                            gpuTemps.Value3.FanSpeed = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 14:
                            gpuTemps.Value4.FanSpeed = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 15:
                            gpuTemps.Value5.FanSpeed = Convert.ToInt32(gpuValue["VGA"]);
                            break;

                        case 16:
                            gpuTemps.Value6.FanSpeed = Convert.ToInt32(gpuValue["VGA"]);
                            break;
                    }
                    counter++;
                }
            }
            return new Profile() { CPU = cpuTemps, GPU = gpuTemps, Name = "Current" };//Return our new profile
        }

        /// <summary>
        /// Happens when rendering is done
        /// </summary>
        private void MaterialWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Window is Loaded, start initialization, load Managment Objects
            MSIWmiHelper = new MSIWmiHelper();
            //Disposable, test components if we are on MSI laptop
            using (var mthbInfo = MSIWmiHelper.MSI_LaptopModel.Get())
            {
                if (!MSIWmiHelper.IsAvailableMSI_CPU || !MSIWmiHelper.IsAvailableMSI_GPU)
                {
                    //Not detected MSI laptop, show dialog to user
                    return;
                }
                if (!MSIWmiHelper.IsAvailableMSI_LaptopModel)
                {
                    //Huh, is this MSI Laptop? Inform user, but continue execution
                    LaptopModel = "Unknown";
                }
                else
                {
                    foreach (ManagementObject item in mthbInfo) //Laptop model
                        LaptopModel = $"{item["Model"]}";
                }
            }
            if (File.Exists(configFileName)) //Do we have config?
            {
                try
                {
                    var loadedSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configFileName));
                    if (loadedSettings.Version != configFileVersion)
                    {
                        //TODO: Show upgrade dialog to user
                        //TODO: Upgrade config
                    }
                    if (loadedSettings.ModelName != LaptopModel)
                    {
                        //TODO: Show upgrade Model Dialog to user
                        //TODO: Upgrade config
                    }
                    ProcessSettings(loadedSettings);
                    return;
                }
                catch (JsonException)
                {
                    //TODO: Config file is not valid for some reason, show dialog to user
                }
            }
            //We dont have config, lets make one!
            var profile = LoadCurrentSystemProfile(); //Load what we have in System now
            //Initialize Settings
            Settings set = new Settings();
            set.Profiles = new Profile[] { profile, new Profile() { Name = "Factory", CPU = TemperatureSettings.FactoryCPU, GPU = TemperatureSettings.FactoryGPU } };
            set.Version = configFileVersion; //Set version
            set.ModelName = LaptopModel;
            ProcessSettings(set);
        }

        /// <summary>
        /// Load Settings and display them
        /// </summary>
        /// <param name="settings">Settings to show</param>
        private void ProcessSettings(Settings settings)
        {
            //Let's start sensors first
            HardwareMonitor = new MSIHardwareMonitor(MSIWmiHelper);
            //Let's start update Thread
            Thread updateThread = new Thread(() =>
            {
                while (true)
                {
                    HardwareMonitor.FanController.RefreshFans();
                    var rpm = HardwareMonitor.FanController.GetFanSpeed(Models.Hardware.MSIFanType.CPUFan);
                    CPURPM = rpm == -1 ? "N/A" : $"{rpm}";
                    rpm = HardwareMonitor.FanController.GetFanSpeed(Models.Hardware.MSIFanType.GPUFan);
                    GPURPM = rpm == -1 ? "N/A" : $"{rpm}";
                    CPUTemperature = $"{HardwareMonitor.TemperatureSensors.GetCPUTemperatureCelsius()} °C";
                    GPUTemperature = $"{HardwareMonitor.TemperatureSensors.GetGPUTemperatureCelsius()} °C";
                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true, Name = "FanAndTemperatureUpdateThread" };
            updateThread.Start();
            //Now check what state we should have, Auto or SubZero?
            enabledSubZero.IsChecked = settings.TurnedOn;
            using (ManagementObjectCollection system = MSIWmiHelper.MSI_System.Get())
            {
                var fanObject = system.Cast<ManagementObject>().ElementAt(9);
                if (settings.TurnedOn)
                {
                    fanObject.SetPropertyValue("System",((Convert.ToInt32(fanObject["System"]) | 128) & 191));
                    fanObject.Put();
                }
                else
                {
                    fanObject.SetPropertyValue("System", (Convert.ToInt32(fanObject["System"]) & 63));
                    fanObject.Put();
                }
                fanObject.Dispose();
            }
            //Now load profiles
            profilesList.Items.Clear(); //Clear current settings
            foreach (var profile in settings.Profiles) //Add profiles
            {
                ListBoxItem itemToAdd = new ListBoxItem();
                TextBlock blockToAdd = new TextBlock();
                itemToAdd.Content = blockToAdd;
                blockToAdd.FontSize = 14;
                //End of same settings
                blockToAdd.Text = profile.Name;
                itemToAdd.Tag = profile; //Save current profile in a tag

                profilesList.Items.Add(itemToAdd); //Add tab
            }
            if (settings.Profiles.Length == 1) //Is this only profile?
                DeleteAllowed = false;
            else
                DeleteAllowed = true;
        }

        /// <summary>
        /// Happens when profile TAB is changed, we need to load different settings
        /// </summary>
        /// <param name="sender">We are ignoring this param</param>
        /// <param name="e">We are checking AddedItems as ListBoxItem</param>
        private void profilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: Add check if we have edited state
            var profileInfo = ((e.AddedItems[0] as ListBoxItem).Tag as Profile);
            cpu1.Value = profileInfo.CPU.Value1.FanSpeed / 100d;
            cpu2.Value = profileInfo.CPU.Value2.FanSpeed / 100d;
            cpu3.Value = profileInfo.CPU.Value3.FanSpeed / 100d;
            cpu4.Value = profileInfo.CPU.Value4.FanSpeed / 100d;
            cpu5.Value = profileInfo.CPU.Value5.FanSpeed / 100d;
            cpu6.Value = profileInfo.CPU.Value6.FanSpeed / 100d;

            gpu1.Value = profileInfo.GPU.Value1.FanSpeed / 100d;
            gpu2.Value = profileInfo.GPU.Value2.FanSpeed / 100d;
            gpu3.Value = profileInfo.GPU.Value3.FanSpeed / 100d;
            gpu4.Value = profileInfo.GPU.Value4.FanSpeed / 100d;
            gpu5.Value = profileInfo.GPU.Value5.FanSpeed / 100d;
            gpu6.Value = profileInfo.GPU.Value6.FanSpeed / 100d;

            IsEdited = false;
            IsSaved = false;
        }

        /// <summary>
        /// User pressed save, apply and save XML
        /// </summary>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEdited)
                applyButton_Click(sender, e); //Apply settings first
            Settings set = new Settings
            {
                Version = configFileVersion,
                ModelName = LaptopModel,
                TurnedOn = enabledSubZero.IsChecked.GetValueOrDefault()
            };
            List<Profile> profilesTemp = new List<Profile>();
            foreach (var item in profilesList.Items)
            {
                profilesTemp.Add(((item as ListBoxItem).Tag as Profile));
            }
            set.Profiles = profilesTemp.ToArray();
            try
            {
                File.WriteAllText(configFileName, JsonConvert.SerializeObject(set, Formatting.Indented)); //Try to save
                IsSaved = true;
            }
            catch (IOException ex)
            {
                //TODO: Saving failed, show user dialog
            }
        }

        /// <summary>
        /// This happens when slider changes, user edited something!
        /// </summary>
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IsEdited = true; //User made some change!
            IsSaved = false;
        }

        #endregion Private Methods

        private void enabledSubZero_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Show dialog so by enabling/disabling profile will be saved to HDD
            saveButton_Click(sender, e);
            using (ManagementObjectCollection system = MSIWmiHelper.MSI_System.Get())
            {
                var fanObject = system.Cast<ManagementObject>().ElementAt(9);
                if (enabledSubZero.IsChecked.GetValueOrDefault())
                {
                    fanObject.SetPropertyValue("System", ((Convert.ToInt32(fanObject["System"]) | 128) & 191));
                    fanObject.Put();
                }
                else
                {
                    fanObject.SetPropertyValue("System", (Convert.ToInt32(fanObject["System"]) & 63));
                    fanObject.Put();
                }
                fanObject.Dispose();
            }
        }
    }
}