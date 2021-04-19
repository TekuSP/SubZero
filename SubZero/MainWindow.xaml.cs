using MaterialDesignExtensions.Controls;

using Newtonsoft.Json;

using SubZero.Models;
using SubZero.Resources;
using SubZero.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using SubZero.Dialogs;
using System.Threading.Tasks;

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
        private MSIHardwareMonitor HardwareMonitor { get; set; }
        public Settings ApplicationSettings { get; set; }
        public Profile ActiveProfile { get; set; }

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
        public bool IsSaved { get; set; } = true;

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
        private async void discardButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = YesNoDialog.ShowWarningDialog("Are you sure you want to discard?", () => { DialogHost.Close("mainDialog"); });
            _ = await DialogHost.Show(dialog, "mainDialog");
            if (!dialog.DialogResult)
                return; //User said no
            profilesList_SelectionChanged(sender, new SelectionChangedEventArgs(e.RoutedEvent, new List<object>(), new List<object>
            {
                profilesList.SelectedItem //Load back original item
            }));
        }

        /// <summary>
        /// User pressed factory settings, load factory configs into current profile
        /// </summary>
        private async void factoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = YesNoDialog.ShowWarningDialog("Are you sure you want to load\nfactory defaults?", () => { DialogHost.Close("mainDialog"); });
            _ = await DialogHost.Show(dialog, "mainDialog");
            if (!dialog.DialogResult)
                return; //User said no
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
        private async void MaterialWindow_Loaded(object sender, RoutedEventArgs e)
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
                        var dialog = YesNoDialog.ShowWarningDialog($"Current version of application uses version {configFileVersion},but configuration you are trying to load has version {loadedSettings.Version}. Do you want to upgrade configuration (Will require restart of application)?", () => { DialogHost.Close("mainDialog"); });
                        _ = await DialogHost.Show(dialog, "mainDialog");
                        if (!dialog.DialogResult)
                            Environment.Exit(1); //User said no
                        ApplicationSettings = loadedSettings;
                        ApplicationSettings.Version = configFileVersion;
                        saveButton_Click(sender, e);
                        Environment.Exit(0);
                    }
                    if (loadedSettings.ModelName != LaptopModel)
                    {
                        var dialog = YesNoDialog.ShowWarningDialog($"Currently loading configuration is for laptop {loadedSettings.ModelName} but you have {LaptopModel}. Do you want to upgrade configuration (Will require restart of application)?", () => { DialogHost.Close("mainDialog"); });
                        _ = await DialogHost.Show(dialog, "mainDialog");
                        if (!dialog.DialogResult)
                            Environment.Exit(1); //User said no
                        ApplicationSettings = loadedSettings;
                        ApplicationSettings.ModelName = LaptopModel;
                        saveButton_Click(sender, e);
                        Environment.Exit(0);
                    }
                    ProcessSettings(loadedSettings);
                    return;
                }
                catch (JsonException)
                {
                    var dialog = OKDialog.ShowErrorDialog($"Configuration file subzero.config is corrupted. Please repair or delete the file. Application cannot continue.", () => { DialogHost.Close("mainDialog"); });
                    _ = await DialogHost.Show(dialog, "mainDialog");
                    Environment.Exit(2); //User said no
                }
            }
            //We dont have config, lets make one!
            var profile = LoadCurrentSystemProfile(); //Load what we have in System now
            //Initialize Settings
            Settings set = new Settings
            {
                Profiles = new Profile[] { profile, new Profile() { Name = "Factory", CPU = TemperatureSettings.FactoryCPU, GPU = TemperatureSettings.FactoryGPU } },
                Version = configFileVersion, //Set version
                ModelName = LaptopModel
            };
            ProcessSettings(set);
        }

        /// <summary>
        /// Load Settings and display them
        /// </summary>
        /// <param name="settings">Settings to show</param>
        private void ProcessSettings(Settings settings)
        {
            //Initialize global settings based on incoming data
            ApplicationSettings = settings;
            //Let's start sensors first
            HardwareMonitor = new MSIHardwareMonitor(MSIWmiHelper);
            //Let's start update Thread
            Thread updateThread = new Thread(TemperatureAndFanThread) { IsBackground = true, Name = "FanAndTemperatureUpdateThread" };
            updateThread.Start();
            //Now check what state we should have, Auto or SubZero?
            enabledSubZero.IsChecked = settings.TurnedOn;
            using (ManagementObjectCollection system = MSIWmiHelper.MSI_System.Get())
            {
                var fanObject = system.Cast<ManagementObject>().ElementAt(9);
                if (settings.TurnedOn)
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
        /// This is way to stop <see cref="TemperatureAndFanThread"/>
        /// </summary>
        private bool runTemperatureAndFanThread = true;

        /// <summary>
        /// This is dedicated thread method for measuring RPM and Temperatures
        /// </summary>
        private void TemperatureAndFanThread()
        {
            while (runTemperatureAndFanThread)
            {
                HardwareMonitor.FanController.RefreshFans();
                var rpm = HardwareMonitor.FanController.GetFanSpeed(Models.Hardware.MSIFanType.CPUFan);
                CPURPM = rpm == -1 ? "N/A" : $"{rpm}";
                rpm = HardwareMonitor.FanController.GetFanSpeed(Models.Hardware.MSIFanType.GPUFan);
                GPURPM = rpm == -1 ? "N/A" : $"{rpm}";
                if (ApplicationSettings.UseCelsius)
                {
                    CPUTemperature = $"{HardwareMonitor.TemperatureSensors.GetCPUTemperatureCelsius()} °C";
                    GPUTemperature = $"{HardwareMonitor.TemperatureSensors.GetGPUTemperatureCelsius()} °C";
                }
                else
                {
                    CPUTemperature = $"{HardwareMonitor.TemperatureSensors.GetCPUTemperatureFahrenheit()} °F";
                    GPUTemperature = $"{HardwareMonitor.TemperatureSensors.GetGPUTemperatureFahrenheit()} °F";
                }
                Thread.Sleep(ApplicationSettings.PollingSpeed);
            }
        }

        /// <summary>
        /// Happens when profile TAB is changed, we need to load different settings
        /// </summary>
        /// <param name="sender">We are ignoring this param</param>
        /// <param name="e">We are checking AddedItems as ListBoxItem</param>
        private async void profilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: Add check if we have edited state
            if (!DialogHost.IsDialogOpen("mainDialog"))
            {
                if (!IsSaved)
                {
                    var dialog = YesNoDialog.ShowWarningDialog("You have unsaved data which will be lost.\nDo you want to save first?", () => { DialogHost.Close("mainDialog"); });
                    _ = await DialogHost.Show(dialog, "mainDialog");
                    if (dialog.DialogResult)
                    {
                        saveButton_Click(sender, new RoutedEventArgs(e.RoutedEvent)); //Save first
                    }
                }
            }
            if (e.AddedItems.Count == 0)
                return;
            var profileInfo = ((e.AddedItems[0] as ListBoxItem).Tag as Profile);
            ActiveProfile = profileInfo; //Set active profile to real value
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

            if (ApplicationSettings.UseCelsius)
            {
                cpuTemp1.Text = $"{profileInfo.CPU.Value1.Temperature} °C";
                cpuTemp2.Text = $"{profileInfo.CPU.Value2.Temperature} °C";
                cpuTemp3.Text = $"{profileInfo.CPU.Value3.Temperature} °C";
                cpuTemp4.Text = $"{profileInfo.CPU.Value4.Temperature} °C";
                cpuTemp5.Text = $"{profileInfo.CPU.Value5.Temperature} °C";
                cpuTemp6.Text = $"{profileInfo.CPU.Value6.Temperature} °C";

                gpuTemp1.Text = $"{profileInfo.GPU.Value1.Temperature} °C";
                gpuTemp2.Text = $"{profileInfo.GPU.Value2.Temperature} °C";
                gpuTemp3.Text = $"{profileInfo.GPU.Value3.Temperature} °C";
                gpuTemp4.Text = $"{profileInfo.GPU.Value4.Temperature} °C";
                gpuTemp5.Text = $"{profileInfo.GPU.Value5.Temperature} °C";
                gpuTemp6.Text = $"{profileInfo.GPU.Value6.Temperature} °C";
            }
            else
            {
                cpuTemp1.Text = $"{profileInfo.CPU.Value1.Temperature.ToFahrenheit()} °F";
                cpuTemp2.Text = $"{profileInfo.CPU.Value2.Temperature.ToFahrenheit()} °F";
                cpuTemp3.Text = $"{profileInfo.CPU.Value3.Temperature.ToFahrenheit()} °F";
                cpuTemp4.Text = $"{profileInfo.CPU.Value4.Temperature.ToFahrenheit()} °F";
                cpuTemp5.Text = $"{profileInfo.CPU.Value5.Temperature.ToFahrenheit()} °F";
                cpuTemp6.Text = $"{profileInfo.CPU.Value6.Temperature.ToFahrenheit()} °F";

                gpuTemp1.Text = $"{profileInfo.GPU.Value1.Temperature.ToFahrenheit()} °F";
                gpuTemp2.Text = $"{profileInfo.GPU.Value2.Temperature.ToFahrenheit()} °F";
                gpuTemp3.Text = $"{profileInfo.GPU.Value3.Temperature.ToFahrenheit()} °F";
                gpuTemp4.Text = $"{profileInfo.GPU.Value4.Temperature.ToFahrenheit()} °F";
                gpuTemp5.Text = $"{profileInfo.GPU.Value5.Temperature.ToFahrenheit()} °F";
                gpuTemp6.Text = $"{profileInfo.GPU.Value6.Temperature.ToFahrenheit()} °F";
            }


            IsEdited = false;
        }

        /// <summary>
        /// User pressed save, apply and save XML
        /// </summary>
        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            bool ourDialog = false;
            if (!DialogHost.IsDialogOpen("mainDialog"))
            {
                _ = DialogHost.Show(new LoadingDialog("Saving..."), "mainDialog");
                ourDialog = true;
            }
            if (IsEdited)
                applyButton_Click(sender, e); //Apply settings first
            List<Profile> profilesTemp = new List<Profile>();
            foreach (var item in profilesList.Items)
            {
                profilesTemp.Add(((item as ListBoxItem).Tag as Profile));
            }
            if (profilesTemp.Count > 0)
                ApplicationSettings.Profiles = profilesTemp.ToArray();
            try
            {
                File.WriteAllText(configFileName, JsonConvert.SerializeObject(ApplicationSettings, Formatting.Indented)); //Try to save
                IsSaved = true;
                if (ourDialog)
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(3000);
                        saveButton.Dispatcher.Invoke(() =>
                        {
                            DialogHost.Close("mainDialog");
                        });
                    });
                }
                IsSaved = true;
            }
            catch (IOException)
            {
                if (DialogHost.IsDialogOpen("mainDialog"))
                    DialogHost.Close("mainDialog");
                await DialogHost.Show(OKDialog.ShowWarningDialog("Saving data to drive failed. Is file open or disk full?", () => { DialogHost.Close("mainDialog"); }), "mainDialog");
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

        private async void enabledSubZero_Click(object sender, RoutedEventArgs e)
        {
            if (enabledSubZero.IsChecked.GetValueOrDefault())
            {
                var dialog = YesNoDialog.ShowInformationDialog("Enabling profile will apply current settings and save them.\nDo you want to continue?", () => { DialogHost.Close("mainDialog"); });
                _ = await DialogHost.Show(dialog, "mainDialog");
                if (!dialog.DialogResult)
                    return; //User said no
                _ = DialogHost.Show(new LoadingDialog("Enabling..."), "mainDialog");
            }
            else
            {
                _ = DialogHost.Show(new LoadingDialog("Disabling..."), "mainDialog");
            }
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
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
                saveButton.Dispatcher.Invoke(() =>
                {
                    DialogHost.Close("mainDialog");
                });
            });
        }

        private async void settings_Click(object sender, RoutedEventArgs e)
        {
            _ = await DialogHost.Show(YesNoDialog.ShowErrorDialog("Fucked up", null), "mainDialog");
        }
    }
}