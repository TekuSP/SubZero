using System.Management;

namespace SubZero.Models
{
    /// <summary>
    /// Helper for working with MSI WMI Objects
    /// </summary>
    public class MSIWmiHelper
    {
        #region Public Constructors

        /// <summary>
        /// Initializes WMI helper for MSI Laptops
        /// </summary>
        public MSIWmiHelper()
        {
            //Init Windows Management Instrumentation, specific to MSI
            MSI_CPU = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_CPU");
            MSI_GPU = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_VGA");
            MSI_AP = new ManagementObjectSearcher("root\\WMI", "SELECT AP FROM MSI_AP");
            MSI_LaptopModel = new ManagementObjectSearcher("root\\CIMV2", "SELECT Model FROM Win32_ComputerSystem");
            MSI_System = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_System");
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
            using (ManagementObjectCollection test = MSI_LaptopModel.Get())
            {
                IsAvailableMSI_LaptopModel = test.Count != 0;
            }
            using (ManagementObjectCollection test = MSI_System.Get())
            {
                IsAvailableMSI_System = test.Count != 0;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Is MSI AP Available to be used?
        /// </summary>
        public bool IsAvailableMSI_AP { get; private set; }

        /// <summary>
        /// Is MSI CPU Available to be used?
        /// </summary>
        public bool IsAvailableMSI_CPU { get; private set; }

        /// <summary>
        /// Is MSI GPU Available to be used?
        /// </summary>
        public bool IsAvailableMSI_GPU { get; private set; }

        /// <summary>
        /// Is MSI Laptop Model ready to be used?
        /// </summary>
        public bool IsAvailableMSI_LaptopModel { get; private set; }

        /// <summary>
        /// Is MSI System Available to be used?
        /// </summary>
        public bool IsAvailableMSI_System { get; private set; }

        /// <summary>
        /// AP WMI
        /// </summary>
        public ManagementObjectSearcher MSI_AP { get; private set; }

        /// <summary>
        /// CPU WMI
        /// </summary>
        public ManagementObjectSearcher MSI_CPU { get; private set; }

        /// <summary>
        /// GPU WMI
        /// </summary>
        public ManagementObjectSearcher MSI_GPU { get; private set; }

        /// <summary>
        /// MSI Laptop Model
        /// </summary>
        public ManagementObjectSearcher MSI_LaptopModel { get; private set; }

        /// <summary>
        /// MSI System WMI
        /// </summary>
        public ManagementObjectSearcher MSI_System { get; private set; }

        #endregion Public Properties
    }
}