using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubZero.Models
{
    /// <summary>
    /// Monitors MSI Laptop Hardware
    /// </summary>
    public class MSIHardwareMonitor
    {
        /// <summary>
        /// WMI Helper to use
        /// </summary>
        private MSIWmiHelper MSIWmiHelper { get; }
        /// <summary>
        /// Fan Controller
        /// </summary>
        private Hardware.MSIFans FanController { get; }
        /// <summary>
        /// Initializes MSI Hardware monitoring
        /// </summary>
        /// <param name="wmiHelper">WMI Helper to use</param>
        public MSIHardwareMonitor(MSIWmiHelper wmiHelper)
        {
            MSIWmiHelper = wmiHelper;
            FanController = new Hardware.MSIFans();
        }


    }
}
