using OpenNos.ChatLog.Networking;
using OpenNos.ChatLog.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OpenNos.ChatLog.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Members

        private bool _orderDesc;

        #endregion

        #region Instantiation

        public MainWindow()
        {
            InitializeComponent();
            List<ChatLogEntry> logs = ChatLogServiceClient.Instance.GetChatLogEntries(null, null, null, null, null, DateTime.UtcNow.AddMinutes(-15), null, null);
            resultlistbox.ItemsSource = logs;
            typedropdown.Items.Add("All");
            typedropdown.Items.Add(ChatLogType.Map);
            typedropdown.Items.Add(ChatLogType.Speaker);
            typedropdown.Items.Add(ChatLogType.Whisper);
            typedropdown.Items.Add(ChatLogType.Group);
            typedropdown.Items.Add(ChatLogType.Family);
            typedropdown.Items.Add(ChatLogType.BuddyTalk);
            typedropdown.SelectedIndex = 0;

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

            resultlistbox.ContextMenu = rmbMenu;
        }

        #endregion

        #region Methods

        private void CloseFile(object sender, RoutedEventArgs e)
        {
        }

        private void CopyLogEntryOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.ToString());
            }
        }

        private void CopyMessageOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.Message);
            }
        }

        private void CopyReceiverIdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.ReceiverId.ToString());
            }
        }

        private void CopyReceiverOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.Receiver);
            }
        }

        private void CopySenderIdOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.SenderId.ToString());
            }
        }

        private void CopySenderOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                Clipboard.SetText(entry.Sender);
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
        }

        private void SearchBidirectionallyOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (resultlistbox.SelectedItem is ChatLogEntry entry)
            {
                if (entry.MessageType == ChatLogType.Whisper || entry.MessageType == ChatLogType.BuddyTalk)
                {
                    IEnumerable<ChatLogEntry> tmp = ChatLogServiceClient.Instance.GetChatLogEntries(
                        entry.Sender, null, entry.Receiver, null, null, null, null, entry.MessageType)
                        .Concat(ChatLogServiceClient.Instance.GetChatLogEntries(
                        entry.Receiver, null, entry.Sender, null, null, null, null, entry.MessageType));
                    if (_orderDesc)
                    {
                        resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
                    }
                    else
                    {
                        resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
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
            string _sender = null;
            long? _senderid = null;
            string _receiver = null;
            long? _receiverid = null;
            string _message = null;
            DateTime? _start = null;
            DateTime? _end = null;
            ChatLogType? _logType = null;

            if (!string.IsNullOrWhiteSpace(senderbox.Text))
            {
                _sender = senderbox.Text;
            }
            if (!string.IsNullOrWhiteSpace(senderidbox.Text) && long.TryParse(senderidbox.Text, out long senderid))
            {
                _senderid = senderid;
            }
            if (!string.IsNullOrWhiteSpace(receiverbox.Text))
            {
                _receiver = receiverbox.Text;
            }
            if (!string.IsNullOrWhiteSpace(receiveridbox.Text) && long.TryParse(receiveridbox.Text, out long receiverid))
            {
                _receiverid = receiverid;
            }
            if (!string.IsNullOrWhiteSpace(messagebox.Text))
            {
                _message = messagebox.Text;
            }
            _start = datestartpicker.Value;
            _end = dateendpicker.Value;
            if (typedropdown.SelectedIndex != 0)
            {
                _logType = (ChatLogType)typedropdown.SelectedValue;
            }

            IEnumerable<ChatLogEntry> tmp = ChatLogServiceClient.Instance.GetChatLogEntries(_sender, _senderid,
                _receiver, _receiverid, _message, _start, _end, _logType);
            if (_orderDesc)
            {
                resultlistbox.ItemsSource = tmp.OrderByDescending(s => s.Timestamp);
            }
            else
            {
                resultlistbox.ItemsSource = tmp.OrderBy(s => s.Timestamp);
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