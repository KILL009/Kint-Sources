using OpenNos.ChatLog.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenNos.ChatLog.Standalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Members

        private readonly List<ChatLogEntry> _logs;
        private readonly LogFileReader _reader;
        private bool _orderDesc;

        #endregion

        #region Instantiation

        public MainWindow()
        {
            InitializeComponent();
            _logs = new List<ChatLogEntry>();
            _reader = new LogFileReader();
            RecursiveFileOpen("chatlogs");
            _logs = _logs.OrderBy(s => s.Timestamp).ToList();
            Resultlistbox.ItemsSource = _logs;
            Typedropdown.Items.Add("All");
            Typedropdown.Items.Add(ChatLogType.Map);
            Typedropdown.Items.Add(ChatLogType.Speaker);
            Typedropdown.Items.Add(ChatLogType.Whisper);
            Typedropdown.Items.Add(ChatLogType.Group);
            Typedropdown.Items.Add(ChatLogType.Family);
            Typedropdown.Items.Add(ChatLogType.BuddyTalk);
            Typedropdown.SelectedIndex = 0;

            ContextMenu rmbMenu = new ContextMenu();

            MenuItem copySender = new MenuItem
            {
                Header = "Copy Sender"
            };
            copySender.Click += CopySenderOnClick;
            rmbMenu.Items.Add(copySender);

            MenuItem copySenderId = new MenuItem
            {
                Header = "Copy SenderId"
            };
            copySenderId.Click += CopySenderIdOnClick;
            rmbMenu.Items.Add(copySenderId);

            MenuItem copyReceiver = new MenuItem
            {
                Header = "Copy Receiver"
            };
            copyReceiver.Click += CopyReceiverOnClick;
            rmbMenu.Items.Add(copyReceiver);

            MenuItem copyReceiverId = new MenuItem
            {
                Header = "Copy ReceiverId"
            };
            copyReceiverId.Click += CopyReceiverIdOnClick;
            rmbMenu.Items.Add(copyReceiverId);

            MenuItem copyMessage = new MenuItem
            {
                Header = "Copy Message"
            };
            copyMessage.Click += CopyMessageOnClick;
            rmbMenu.Items.Add(copyMessage);

            MenuItem copyLogEntry = new MenuItem
            {
                Header = "Copy LogEntry"
            };
            copyLogEntry.Click += CopyLogEntryOnClick;
            rmbMenu.Items.Add(copyLogEntry);

            MenuItem searchBidirectionally = new MenuItem
            {
                Header = "Search Bidirectionally"
            };
            searchBidirectionally.Click += SearchBidirectionallyOnClick;
            rmbMenu.Items.Add(searchBidirectionally);

            Resultlistbox.ContextMenu = rmbMenu;
        }

        #endregion

        #region Methods

        private void CloseFile(object sender, RoutedEventArgs e)
        {
        }

        private void CopyLogEntryOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.ToString());
            }
        }

        private void CopyMessageOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.Message);
            }
        }

        private void CopyReceiverIdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.ReceiverId.ToString());
            }
        }

        private void CopyReceiverOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.Receiver);
            }
        }

        private void CopySenderIdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.SenderId.ToString());
            }
        }

        private void CopySenderOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.Sender);
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
        }

        private void RecursiveFileOpen(string dir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string s in Directory.GetFiles(d).Where(s => s.EndsWith(".onc")))
                    {
                        _logs.AddRange(_reader.ReadLogFile(s));
                    }

                    RecursiveFileOpen(d);
                }
            }
            catch
            {
                MessageBox.Show("Something went wrong while opening Chat Log Files. Exiting...", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        private void SearchBidirectionallyOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                if (entry.MessageType == ChatLogType.Whisper || entry.MessageType == ChatLogType.BuddyTalk)
                {
                    List<ChatLogEntry> tmp = _logs;
                    tmp = tmp.Where(s =>
                        s.MessageType == entry.MessageType
                        && ((s.Sender == entry.Sender && s.Receiver == entry.Receiver)
                         || (s.Sender == entry.Receiver && s.Receiver == entry.Sender))).ToList();

                    if (_orderDesc)
                    {
                        Resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
                    }
                    else
                    {
                        Resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
                    }
                }
                else
                {
                    MessageBox.Show("You can ony search Bidirectionally for Whisper and BuddyTalk messages", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SearchButton(object sender, RoutedEventArgs e)
        {
            List<ChatLogEntry> tmp = _logs;
            if (!string.IsNullOrWhiteSpace(Senderbox.Text))
            {
                tmp = tmp.Where(s => s.Sender.IndexOf(Senderbox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Senderidbox.Text) && long.TryParse(Senderidbox.Text, out long senderid))
            {
                tmp = tmp.Where(s => s.SenderId == senderid).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Receiverbox.Text))
            {
                tmp = tmp.Where(s => s.Receiver.IndexOf(Receiverbox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Receiveridbox.Text)
                && long.TryParse(Receiveridbox.Text, out long receiverid))
            {
                tmp = tmp.Where(s => s.ReceiverId == receiverid).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Messagebox.Text))
            {
                tmp = tmp.Where(s => s.Message.IndexOf(Messagebox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
            }

            if (Datestartpicker.Value != null)
            {
                tmp = tmp.Where(s => s.Timestamp >= Datestartpicker.Value).ToList();
            }

            if (Dateendpicker.Value != null)
            {
                tmp = tmp.Where(s => s.Timestamp <= Dateendpicker.Value).ToList();
            }

            if (Typedropdown.SelectedIndex != 0)
            {
                tmp = tmp.Where(s => s.MessageType == (ChatLogType)Typedropdown.SelectedValue).ToList();
            }

            if (_orderDesc)
            {
                Resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
            }
            else
            {
                Resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
            }
        }

        private void SettingsOrderAscending(object sender, RoutedEventArgs e)
        {
            _orderDesc = false;
            Orderdesc.IsChecked = true;
            Orderasc.IsChecked = false;
        }

        private void SettingsOrderDescending(object sender, RoutedEventArgs e)
        {
            _orderDesc = true;
            Orderdesc.IsChecked = true;
            Orderasc.IsChecked = false;
        }

        #endregion
    }
}