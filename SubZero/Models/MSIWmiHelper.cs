using System;
using System.Management;

namespace SubZero.Models
{
    /// <summary>
    /// Helper for working with MSI WMI Objects
    /// </summary>
    public class MSIWmiHelper : IDisposable
    {
        #region Private Fields

        private bool disposedValue;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes WMI helper for MSI Laptops
        /// </summary>
        public MSIWmiHelper()
        {
            //Init Windows Management Instrumentation Roots, specific to MSI
            var WMIRoot = new ManagementScope("root\\WMI", new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Connect,
                EnablePrivileges = true
            });
            var CIMV2Root = new ManagementScope("root\\CIMV2", new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Connect,
                EnablePrivileges = true
            });
            //Init Windows Management Instrumentation, specific to MSI
            MSI_CPU = new ManagementObjectSearcher(WMIRoot, new ObjectQuery("SELECT * FROM MSI_CPU"));
            MSI_GPU = new ManagementObjectSearcher(WMIRoot, new ObjectQuery("SELECT * FROM MSI_VGA"));
            MSI_AP = new ManagementObjectSearcher(WMIRoot, new ObjectQuery("SELECT AP FROM MSI_AP"));
            MSI_LaptopModel = new ManagementObjectSearcher(CIMV2Root, new ObjectQuery("SELECT Model FROM Win32_ComputerSystem"));
            MSI_System = new ManagementObjectSearcher(WMIRoot, new ObjectQuery("SELECT * FROM MSI_System"));
            //Test if they are valid
            using (ManagementObjectCollection test = MSI_CPU.Get())
            {
                try
                {
                    IsAvailableMSI_CPU = test.Count != 0;
                }
                catch
                {
                    IsAvailableMSI_CPU = false;
                }
            }
            using (ManagementObjectCollection test = MSI_GPU.Get())
            {
                try
                {
                    IsAvailableMSI_GPU = test.Count != 0;
                }
                catch
                {
                    IsAvailableMSI_GPU = false;
                }
            }
            using (ManagementObjectCollection test = MSI_AP.Get())
            {
                try
                {
                    IsAvailableMSI_AP = test.Count != 0;
                }
                catch
                {
                    IsAvailableMSI_AP = false;
                }
            }
            using (ManagementObjectCollection test = MSI_LaptopModel.Get())
            {
                try
                {
                    IsAvailableMSI_LaptopModel = test.Count != 0;
                }
                catch
                {
                    IsAvailableMSI_LaptopModel = false;
                }
            }
            using (ManagementObjectCollection test = MSI_System.Get())
            {
                try
                {
                    IsAvailableMSI_System = test.Count != 0;
                }
                catch
                {
                    IsAvailableMSI_System = false;
                }
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

        #region Public Methods

        /// <summary>
        /// Dispose implementation
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Dispose implementation
        /// </summary>
        /// <param name="disposing">Is managed disposing?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //Managed disposing
                    MSI_AP.Dispose();
                    MSI_CPU.Dispose();
                    MSI_GPU.Dispose();
                    MSI_LaptopModel.Dispose();
                    MSI_System.Dispose();
                }
                //Unmanaged disposing
                MSI_AP = null;
                MSI_CPU = null;
                MSI_GPU = null;
                MSI_LaptopModel = null;
                MSI_System = null;
                //Confirm dispose
                disposedValue = true;
            }
        }

        #endregion Protected Methods
    }
}