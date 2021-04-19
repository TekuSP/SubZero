using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SubZero.Models
{
    /// <summary>
    /// Helper for working with MSI WMI Objects
    /// </summary>
    public class MSIWmiHelper
    {
        /// <summary>
        /// CPU WMI
        /// </summary>
        public ManagementObjectSearcher MSI_CPU { get; private set; }
        /// <summary>
        /// Is MSI CPU Available to be used?
        /// </summary>
        public bool IsAvailableMSI_CPU { get; private set; }
        /// <summary>
        /// GPU WMI
        /// </summary>
        public ManagementObjectSearcher MSI_GPU { get; private set; }
        /// <summary>
        /// Is MSI GPU Available to be used?
        /// </summary>
        public bool IsAvailableMSI_GPU { get; private set; }
        /// <summary>
        /// AP WMI
        /// </summary>
        public ManagementObjectSearcher MSI_AP { get; private set; }
        /// <summary>
        /// Is MSI AP Available to be used?
        /// </summary>
        public bool IsAvailableMSI_AP { get; private set; }
        /// <summary>
        /// Master Battery WMI
        /// </summary>
        public ManagementObjectSearcher MSI_Master_Battery { get; private set; }
        /// <summary>
        /// Is MSI Master Battery Available to be used?
        /// </summary>
        public bool IsAvailableMSI_Master_Battery { get; private set; }
        /// <summary>
        /// Power WMI
        /// </summary>
        public ManagementObjectSearcher MSI_Power { get; private set; }
        /// <summary>
        /// Is MSI Power Available to be used?
        /// </summary>
        public bool IsAvailableMSI_Power { get; private set; }
        /// <summary>
        /// MSI Laptop Model
        /// </summary>
        public ManagementObjectSearcher MSI_LaptopModel { get; private set; }
        /// <summary>
        /// Is MSI Laptop Model ready to be used?
        /// </summary>
        public bool IsAvailableMSI_LaptopModel { get; private set; }
        /// <summary>
        /// Windows PowerPlan WMI, do we need this?
        /// </summary>
        public ManagementObjectSearcher WIN_PowerPlan { get; private set; }
        /// <summary>
        /// Is Windows PowerPlan Available to be used?
        /// </summary>
        public bool IsAvailableWIN_PowerPlan { get; private set; }
        /// <summary>
        /// Windows Display WMI, do we need this?
        /// </summary>
        public ManagementObjectSearcher WIN_DisplayConfig { get; private set; }
        /// <summary>
        /// Is Windows DisplayConfig Available to be used?
        /// </summary>
        public bool IsAvailableWIN_DisplayConfig { get; private set; }
        /// <summary>
        /// Initializes WMI helper for MSI Laptops
        /// </summary>
        public MSIWmiHelper()
        {
            //Init Windows Management Instrumentation, specific to MSI
            MSI_CPU = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_CPU");
            MSI_GPU = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_VGA");
            MSI_AP = new ManagementObjectSearcher("root\\WMI", "SELECT AP FROM MSI_AP");
            MSI_Master_Battery = new ManagementObjectSearcher("root\\WMI", "SELECT Master_Battery FROM MSI_Master_Battery");
            MSI_Power = new ManagementObjectSearcher("root\\WMI", "SELECT Power FROM MSI_Power");
            MSI_LaptopModel = new ManagementObjectSearcher("root\\CIMV2", "SELECT Model FROM Win32_ComputerSystem");
            WIN_PowerPlan = new ManagementObjectSearcher("root\\CIMV2\\power", "SELECT * FROM Win32_PowerPlan"); //TODO: ???
            WIN_DisplayConfig = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DisplayConfiguration"); //TODO: ???
            //Test if they are valid
            using (ManagementObjectCollection test = MSI_CPU.Get())
            {
                IsAvailableMSI_CPU = test.Count != 0;
            }
            using (ManagementObjectCollection test = MSI_GPU.Get())
            {
                IsAvailableMSI_GPU = test.Count != 0;
            }
            using (ManagementObjectCollection test = MSI_AP.Get())
            {
                IsAvailableMSI_AP = test.Count != 0;
            }
            using (ManagementObjectCollection test = MSI_Master_Battery.Get())
            {
                IsAvailableMSI_Master_Battery = test.Count != 0;
            }
            using (ManagementObjectCollection test = MSI_Power.Get())
            {
                IsAvailableMSI_Power = test.Count != 0;
            }
            using (ManagementObjectCollection test = MSI_LaptopModel.Get())
            {
                IsAvailableMSI_LaptopModel = test.Count != 0;
            }
            using (ManagementObjectCollection test = WIN_PowerPlan.Get())
            {
                IsAvailableWIN_PowerPlan = test.Count != 0;
            }
            using (ManagementObjectCollection test = WIN_DisplayConfig.Get())
            {
                IsAvailableWIN_DisplayConfig = test.Count != 0;
            }
        }
    }
}
