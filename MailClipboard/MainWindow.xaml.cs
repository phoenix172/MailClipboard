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

namespace MailClipboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _trayIcon;
        private WindowState _currentWindowState = WindowState.Normal;
        private readonly HotkeyManager _hotkeyManager;
        private readonly MailSender _sender;


        public MainWindow()
        {
            InitializeTray();

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

        private void InitializeTray()
        {
            _trayIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipText = "The app has been minimised. Click the tray icon to show.",
                BalloonTipTitle = "MailClipboard",
                Text = "MailClipboard",
                Icon = new Icon(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("MailClipboard.Resources.AppIcon.ico"))
            };
            _trayIcon.Click += TrayIcon_Click;
        }

        private void SendEmail()
        {
            HandleException(() =>
            {
                string emailText = Clipboard.GetText();
                _sender.SendEmail(emailText);
                LogMessages.Add(emailText);
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
            _trayIcon.Dispose();
            _trayIcon = null;
        }
        
        private void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (_trayIcon != null)
                    _trayIcon.ShowBalloonTip(2000);
            }
            else
                _currentWindowState = WindowState;
        }
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = _currentWindowState;
        }
        private void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        private void ShowTrayIcon(bool show)
        {
            if (_trayIcon != null)
                _trayIcon.Visible = show;
        }
    }
}
