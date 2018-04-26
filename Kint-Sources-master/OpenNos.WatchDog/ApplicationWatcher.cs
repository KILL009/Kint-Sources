using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WatchDog
{
    public class ApplicationWatcher
    {
        private const string HEARTBEAT_FILE_NAME = "heartbeat";

        private string monitoredAppName = "MonitoredApplication";
        private string watchdogAppName = "OpenNos.World";
        private int monitoringInterval = 5000;

        /// <summary>
        /// The ApplicationWatcher class takes in the monitored application name, watchdog application name and the preffered monitoring interval and initiates the watchdog process.
        /// </summary>
        /// <param name="monitoredApplicationName">Name of the application to be monitored</param>
        /// <param name="watchdogApplicationName">Name of the watchdog application</param>
        /// <param name="monitoringInterval">Monitoring interval in Milliseconds</param>
        public ApplicationWatcher(string monitoredApplicationName, string watchdogApplicationName, int monitoringInterval)
        {
            this.monitoredAppName = monitoredApplicationName;
            this.watchdogAppName = watchdogApplicationName;
            this.monitoringInterval = monitoringInterval;

            /// Check if another instance of this application is running
            /// If True self-terminate
            /// Else continue
            try
            {
                Process[] pname = Process.GetProcessesByName(watchdogAppName);

                if (pname.Length > 1)
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ApplicationWatcher Exception1: " + ex.StackTrace);
            }

            try
            {
                Thread watchDogThread = new Thread(new ThreadStart(StartAppMonitoring));
                watchDogThread.Start();
            }
            catch (Exception e)
            {
            }
        }

        ///<summary>
        /// Utility method to check if a process belonging to the monitored application is running
        /// </summary>
        /// <returns> Bool
        /// True : A monitored app process exists
        /// False : A monitored app process does not exist
        /// </returns>
        private bool MonitoredAppExists()
        {
            try
            {
                Process[] processList = Process.GetProcessesByName(monitoredAppName);
                if (processList.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ApplicationWatcher MonitoredAppExists Exception: " + ex.StackTrace);
                return true;
            }
        }


        ///<summary>
        /// Worker method of the Watchdog application
        /// Monitors the relevant application process availability
        /// Restarts the application if the process is not available
        /// </summary>
        private void StartAppMonitoring()
        {
            while (true)
            {
                /// A lock file is used to temporarily disable the watchdog from monitoring and reviving the monitored application for debugging purposes.
                if (!File.Exists("watchdoglock"))
                {
                    string monitoredAppExePath = monitoredAppName + ".exe";
                    if (!MonitoredAppExists())
                    {
                        if (File.Exists(monitoredAppExePath))
                        {
                            Process.Start(monitoredAppExePath);
                        }
                        else
                        {
                            Debug.WriteLine("ApplicationWatcher StartAppMonitoring Monitored Application exe not found at: " + monitoredAppExePath);
                        }
                    }
                    else
                    {
                        if (File.Exists(HEARTBEAT_FILE_NAME))
                        {
                            try
                            {
                                File.Delete(HEARTBEAT_FILE_NAME);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("ApplicationWatcher StartAppMonitoring Exception1: " + ex.StackTrace);
                            }
                        }
                        else
                        {
                            /// If the heartbeat file is not created, this could mean that the Monitored Application could be frozen
                            while (MonitoredAppExists())
                            {
                                try
                                {
                                    Process[] pname = Process.GetProcessesByName(monitoredAppName);

                                    foreach (Process process in pname)
                                    {
                                        process.Kill();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("ApplicationWatcher StartAppMonitoring Exception2: " + ex.StackTrace);
                                }
                            }
                            Process.Start(monitoredAppExePath);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("ApplicationWatcher StartAppMonitoring watchdoglock found.");
                }
                Thread.Sleep(monitoringInterval);
            }
        }
    }
}
