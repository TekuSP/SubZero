using MaterialDesignExtensions.Controls;
using SubZero.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Management;
using System.Diagnostics;
using System.ComponentModel;

namespace SubZero
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MaterialWindow, INotifyPropertyChanged
    {
        const string configFileName = "subzero.config";
        const int configFileVersion = 1;
        private ManagementObjectSearcher MSI_CPU;
        private ManagementObjectSearcher MSI_GPU;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEdited { get; set; } = false;
        public bool DeleteAllowed { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            var imgSource = Convertors.ImageTools.ImageSourceFromBitmap(DesignSource.logo_icon);
            this.Icon = imgSource;
            this.TitleBarIcon = imgSource;
        }

        private void MaterialWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MSI_CPU = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_CPU");
            MSI_GPU = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_VGA");
            using (var cpuTest = MSI_CPU.Get())
            using (var gpuTest = MSI_GPU.Get())
            {
                if (cpuTest.Count == 0 || gpuTest.Count == 0)
                {
                    //Not detected MSI laptop, show dialog to user
                    return;
                }
            }
            if (File.Exists(configFileName)) //Do we have config?
            {
                try
                {
                    var loadedSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configFileName));
                    if (loadedSettings.Version != configFileVersion)
                    {
                        //Show upgrade dialog to user
                        //Upgrade config
                    }
                    ProcessSettings(loadedSettings);
                    return;
                }
                catch (JsonException)
                {
                    //Config file is not valid for some reason, show dialog to user
                }
            }
            //We dont have config, lets make one!
            var profile = LoadCurrentSystemProfile(); //Load what we have in System now
            //Initialize Settings
            Settings set = new Settings();
            set.Profiles = new Profile[] { profile, new Profile() { Name = "Factory", CPU = TemperatureSettings.FactoryCPU, GPU = TemperatureSettings.FactoryGPU } };
            set.Version = configFileVersion; //Set version
            ProcessSettings(set);
        }
        private void ProcessSettings(Settings settings)
        {
            profilesList.Items.Clear();
            foreach (var profile in settings.Profiles)
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
            if (settings.Profiles.Length == 1)
                DeleteAllowed = false;
            else
                DeleteAllowed = true;
        }
        private Profile LoadCurrentSystemProfile()
        {
            TemperatureSettings cpuTemps = new TemperatureSettings();
            TemperatureSettings gpuTemps = new TemperatureSettings();
            int counter = 0; //Because Microsoft does not allow indexing of ManagmentObjectCollection
            using (var cpuData = MSI_CPU.Get())
            using (var gpudata = MSI_GPU.Get())
            {
                foreach (ManagementObject cpuValue in cpuData)
                {
                    switch (counter)
                    {
                        case 11:
                            cpuTemps.Value1 = double.Parse(cpuValue["CPU"].ToString());
                            break;
                        case 12:
                            cpuTemps.Value2 = double.Parse(cpuValue["CPU"].ToString());
                            break;
                        case 13:
                            cpuTemps.Value3 = double.Parse(cpuValue["CPU"].ToString());
                            break;
                        case 14:
                            cpuTemps.Value4 = double.Parse(cpuValue["CPU"].ToString());
                            break;
                        case 15:
                            cpuTemps.Value5 = double.Parse(cpuValue["CPU"].ToString());
                            break;
                        case 16:
                            cpuTemps.Value6 = double.Parse(cpuValue["CPU"].ToString());
                            break;
                    }
                    counter++;
                }
                counter = 0; //Reset for GPU
                foreach (ManagementObject gpuValue in gpudata)
                {
                    switch (counter)
                    {
                        case 11:
                            gpuTemps.Value1 = double.Parse(gpuValue["VGA"].ToString());
                            break;
                        case 12:
                            gpuTemps.Value2 = double.Parse(gpuValue["VGA"].ToString());
                            break;
                        case 13:
                            gpuTemps.Value3 = double.Parse(gpuValue["VGA"].ToString());
                            break;
                        case 14:
                            gpuTemps.Value4 = double.Parse(gpuValue["VGA"].ToString());
                            break;
                        case 15:
                            gpuTemps.Value5 = double.Parse(gpuValue["VGA"].ToString());
                            break;
                        case 16:
                            gpuTemps.Value6 = double.Parse(gpuValue["VGA"].ToString());
                            break;
                    }
                    counter++;
                }
            }
            return new Profile() { CPU = cpuTemps, GPU = gpuTemps, Name = "Current" };
        }

        private void profilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Add check if we have edited state
            var profileInfo = ((e.AddedItems[0] as ListBoxItem).Tag as Profile);
            cpu1.Value = profileInfo.CPU.Value1 / 100;
            cpu2.Value = profileInfo.CPU.Value2 / 100;
            cpu3.Value = profileInfo.CPU.Value3 / 100;
            cpu4.Value = profileInfo.CPU.Value4 / 100;
            cpu5.Value = profileInfo.CPU.Value5 / 100;
            cpu6.Value = profileInfo.CPU.Value6 / 100;

            gpu1.Value = profileInfo.GPU.Value1 / 100;
            gpu2.Value = profileInfo.GPU.Value2 / 100;
            gpu3.Value = profileInfo.GPU.Value3 / 100;
            gpu4.Value = profileInfo.GPU.Value4 / 100;
            gpu5.Value = profileInfo.GPU.Value5 / 100;
            gpu6.Value = profileInfo.GPU.Value6 / 100;

            IsEdited = false;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IsEdited = true; //User made some change!
        }

        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            profilesList_SelectionChanged(sender, new SelectionChangedEventArgs(e.RoutedEvent, new List<object>(), new List<object>
            {
                profilesList.SelectedItem
            }));
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            Profile appliedProfile = new Profile();
            appliedProfile.CPU = new TemperatureSettings();
            appliedProfile.GPU = new TemperatureSettings();
            appliedProfile.Name = ((profilesList.SelectedItem as ListBoxItem).Tag as Profile).Name;
            appliedProfile.CPU.Value1 = Math.Round(cpu1.Value * 100);
            appliedProfile.CPU.Value2 = Math.Round(cpu2.Value * 100);
            appliedProfile.CPU.Value3 = Math.Round(cpu3.Value * 100);
            appliedProfile.CPU.Value4 = Math.Round(cpu4.Value * 100);
            appliedProfile.CPU.Value5 = Math.Round(cpu5.Value * 100);
            appliedProfile.CPU.Value6 = Math.Round(cpu6.Value * 100);

            appliedProfile.GPU.Value1 = Math.Round(gpu1.Value * 100);
            appliedProfile.GPU.Value2 = Math.Round(gpu2.Value * 100);
            appliedProfile.GPU.Value3 = Math.Round(gpu3.Value * 100);
            appliedProfile.GPU.Value4 = Math.Round(gpu4.Value * 100);
            appliedProfile.GPU.Value5 = Math.Round(gpu5.Value * 100);
            appliedProfile.GPU.Value6 = Math.Round(gpu6.Value * 100);

            ((profilesList.SelectedItem as ListBoxItem)).Tag = appliedProfile; //Synchronize changes first

            int counter = 0; //Because Microsoft does not allow indexing of ManagmentObjectCollection
            using (var cpuData = MSI_CPU.Get())
            using (var gpudata = MSI_GPU.Get())
            {
                foreach (ManagementObject cpuValue in cpuData)
                {
                    switch (counter)
                    {
                        case 11:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value1);
                            cpuValue.Put();
                            break;
                        case 12:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value2);
                            cpuValue.Put();
                            break;
                        case 13:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value3);
                            cpuValue.Put();
                            break;
                        case 14:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value4);
                            cpuValue.Put();
                            break;
                        case 15:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value5);
                            cpuValue.Put();
                            break;
                        case 16:
                            cpuValue.SetPropertyValue("CPU", appliedProfile.CPU.Value6);
                            cpuValue.Put();
                            break;
                    }
                    counter++;
                }
                counter = 0; //Reset for GPU
                foreach (ManagementObject gpuValue in gpudata)
                {
                    switch (counter)
                    {
                        case 11:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value1);
                            gpuValue.Put();
                            break;
                        case 12:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value2);
                            gpuValue.Put();
                            break;
                        case 13:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value3);
                            gpuValue.Put();
                            break;
                        case 14:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value4);
                            gpuValue.Put();
                            break;
                        case 15:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value5);
                            gpuValue.Put();
                            break;
                        case 16:
                            gpuValue.SetPropertyValue("VGA", appliedProfile.GPU.Value6);
                            gpuValue.Put();
                            break;
                    }
                    counter++;
                }
            }
            IsEdited = false; //Editation finished
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEdited)
                applyButton_Click(sender, e); //Apply settings first
            Settings set = new Settings
            {
                Version = configFileVersion
            };
            List<Profile> profilesTemp = new List<Profile>();
            foreach (var item in profilesList.Items)
            {
                profilesTemp.Add(((item as ListBoxItem).Tag as Profile));
            }
            set.Profiles = profilesTemp.ToArray();
            try
            {
                File.WriteAllText(configFileName, JsonConvert.SerializeObject(set)); //Try to save
            }
            catch (IOException ex)
            {
                //Saving failed, show user dialog
            }
        }
    }
}
