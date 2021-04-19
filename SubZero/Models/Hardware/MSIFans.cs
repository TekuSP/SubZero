﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubZero.Models.Hardware
{
    /// <summary>
    /// Hardware FAN Controller
    /// </summary>
    public class MSIFans
    {
        private Dictionary<MSIFanType, int> rpms = new Dictionary<MSIFanType, int>();
        private bool MonitorRunning { get; set; }
        /// <summary>
        /// Returns RPM for selected fan
        /// </summary>
        /// <param name="fanType">Fan to return</param>
        /// <returns>Returns RPM, or -1 if fan does not exist, or does not run</returns>
        public int GetFanSpeed(MSIFanType fanType)
        {
            lock (this)
            {
                if (rpms.ContainsKey(fanType))
                    return rpms[fanType];
                return -1;
            }
        }
        /// <summary>
        /// Reloads all Fans and their RPMs from motherboard
        /// </summary>
        /// <param name="helper">WMI Helper to use</param>
        public void RefreshFans(MSIWmiHelper helper)
        {
            lock (this)
            {
                rpms.Clear(); //Clear dict
                using (System.Management.ManagementObjectCollection ap = helper.MSI_AP.Get())
                {
                    bool even = false;
                    int fanNumber = 0;
                    short oddValue = 0;
                    foreach (var item in ap)
                    {
                        if (even)
                        {
                            rpms.Add((MSIFanType)fanNumber, GetRPM(oddValue, Convert.ToInt16(item["AP"]))); //Laptops have CPU on 1 and GPU on 2
                            fanNumber++;
                        }
                        else
                        {
                            oddValue = Convert.ToInt16(item["AP"]);
                        }
                        even = !even;
                    } 
                }
            }
        }
        /// <summary>
        /// This is PWM related, returns RPM from two values
        /// TODO: Get to know what those values are
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns>Returns RPM Fan speed</returns>
        private static int GetRPM(short firstValue, short secondValue)
        {
            int rpm = -1;
            if (secondValue != 0 || firstValue != 0)
                rpm = (int)(60000000.0 / (((secondValue << 8) + firstValue) * 2 * 62.5)); //WTH MSI?
            if (rpm > 0)
                return rpm;
            return -1;
        }
        /// <summary>
        /// Starts intervalled polling of Fan Speeds in a PC
        /// </summary>
        /// <param name="helper">WMI Helper to use</param>
        /// <param name="interval">How fast to poll</param>
        /// <returns>Returns false if Monitor is Running already, true if started succesfully</returns>
        public bool StartMonitor(MSIWmiHelper helper, TimeSpan interval)
        {
            if (MonitorRunning) //False if is it running already
                return false;
            Thread th = new Thread(() =>
            {
                while (MonitorRunning)
                {
                    RefreshFans(helper); //Refresh
                    Thread.Sleep(interval); //Wait
                }
            })
            { IsBackground = true, Name = "FanMonitor" };
            th.Start(); //Start the thread
            return true;
        }
        /// <summary>
        /// Stops intervalled polling of Fan Speeds in a PC
        /// </summary>
        public void StopMonitor()
        {
            MonitorRunning = false; //Stop the monitor
        }
    }
    /// <summary>
    /// MSI Laptop default fan order
    /// </summary>
    public enum MSIFanType
    {
        /// <summary>
        /// CPU Fan
        /// </summary>
        CPUFan = 1,
        /// <summary>
        /// GPU Fan
        /// </summary>
        GPUFan = 2
    }
}
