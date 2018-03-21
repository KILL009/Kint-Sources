using System;
using System.Configuration;
using System.Diagnostics;

namespace WatchDog
{
    /// <summary>
    /// The Watchdog Application
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ///ApplicationWatcher initialization

            int monitoringInterval = 5000;

            try
            {
                monitoringInterval = Convert.ToInt32(ConfigurationManager.AppSettings["MonitoringInterval"]);

                if (monitoringInterval == 0)
                {
                    monitoringInterval = 5000;
                }
            }
            catch (Exception ex)
            {
                monitoringInterval = 5000;
                Debug.WriteLine("ApplicationWatcher Exception2: " + ex.StackTrace);
            }

            ApplicationWatcher applicationWatcher = new ApplicationWatcher("MonitoredApplication", "WatchDog", 5000);
        }
    }
}
