using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace MailClipboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowState _currentWindowState = WindowState.Normal;
        private readonly HotkeyManager _hotkeyManager;
        private readonly MailSender _sender;

        public MainWindow()
        {
            LogMessages = new ObservableCollection<string>();

            Config = AppConfiguration.Load();
            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.BindHotkey(Config.Hotkey);
            _hotkeyManager.HotkeyPressed += (s,e) => SendEmail();
            _sender = new MailSender(Config, GetPassword());

            this.DataContext = this;

            InitializeComponent();
        }

        public ObservableCollection<string> LogMessages { get; }
        public AppConfiguration Config { get; }

        private void SendEmail()
        {
            HandleException(() =>
            {
                string emailText = Clipboard.GetText();
                _sender.SendEmail(emailText);
                LogMessages.Add(emailText);
                TrayIcon.ShowBalloonTip("Email sent", emailText, BalloonIcon.Info);
            });
        }

        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            ForgetPassword();
            _sender.Password = GetPassword();
        }

        private SecureString GetPassword() =>
            HandleException(() => AuthenticationHelper.GetPassword(Config.SenderEmail));

        private void ForgetPassword() =>
            HandleException(()=>AuthenticationHelper.ForgetPassword(Config.SenderEmail));

        private T HandleException<T>(Func<T> action)
        {
            T result = default;
            HandleException((Action)(() => result = action()));
            return result;
        }

        private void HandleException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Activate();
                MessageBox.Show($"An error occured: {e}", "Send error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _hotkeyManager.Dispose();
        }

        private void TaskbarIcon_OnTrayClick(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = _currentWindowState;
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            else
            {
                _currentWindowState = WindowState;
            }
        }
    }
}
