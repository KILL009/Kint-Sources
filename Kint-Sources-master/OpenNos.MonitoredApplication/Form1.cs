using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitoredApplication
{
    /// <summary>
    /// The Monitored Application
    /// </summary>
    public partial class MonitoredApplicationForm : Form
    {
        private WatchdogWatcher watchdogWatcher = null;

        public MonitoredApplicationForm()
        {
            InitializeComponent();

            #region WatchdogWatcher initialization

            int watchDogMonitorInterval = 5000;

            try
            {
                watchDogMonitorInterval = Convert.ToInt32(ConfigurationManager.AppSettings["WatchDogMonitorInterval"]);
                if (watchDogMonitorInterval != 0)
                {
                    watchDogMonitorInterval = 5000;
                }
            }
            catch (Exception ex)
            {
                watchDogMonitorInterval = 5000;
                MessageBox.Show("Exception WatchdogMonitor1: " + ex.StackTrace);
            }

            watchdogWatcher = new WatchdogWatcher("WatchDog", "WatchDog.exe", watchDogMonitorInterval);

            #endregion
        }

        /// <summary>
        /// Terminate the monitored application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Terminate_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Kill the watchdog apllication process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_KillWatchDog_Click(object sender, EventArgs e)
        {
            watchdogWatcher.KillWatchDog();
        }

        /// <summary>
        /// Lock/Unlock the watchdog mechanism by creating/deleting the watchdog lock file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lockWatchDog_Click(object sender, EventArgs e)
        {

            string watchdogLockFileName = "watchdoglock";

            try
            {
                if (!File.Exists(watchdogLockFileName))
                {
                    File.Create(watchdogLockFileName);
                    btn_lockWatchDog.Text = "Unlock WatchDog";
                }
                else
                {
                    File.Delete(watchdogLockFileName);
                    btn_lockWatchDog.Text = "Lock WatchDog";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception MonitoredApplicationForm : " + ex.StackTrace);
            }
        }
    }
}
