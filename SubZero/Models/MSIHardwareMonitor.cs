namespace SubZero.Models
{
    /// <summary>
    /// Monitors MSI Laptop Hardware
    /// </summary>
    public class MSIHardwareMonitor
    {
        #region Public Constructors

        /// <summary>
        /// Initializes MSI Hardware monitoring
        /// </summary>
        /// <param name="wmiHelper">WMI Helper to use</param>
        public MSIHardwareMonitor(MSIWmiHelper wmiHelper)
        {
            FanController = new Hardware.MSIFans(wmiHelper);
            TemperatureSensors = new Hardware.MSITemperatureSensors(wmiHelper);
        }

        #endregion Public Constructors

        #region Public Properties

        //TODO: Move to different DLL, and add more functionality for all MSI hidden APIs
        /// <summary>
        /// Fan Controller
        /// </summary>
        public Hardware.MSIFans FanController { get; }

        /// <summary>
        /// Temperature Sensors
        /// </summary>
        public Hardware.MSITemperatureSensors TemperatureSensors { get; }

        #endregion Public Properties
    }
}