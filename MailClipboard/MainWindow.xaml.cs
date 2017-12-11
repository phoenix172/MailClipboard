using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;
using WindowsInput.Native;
using NHotkey;
using NHotkey.Wpf;

namespace MailClipboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NHotkey.Wpf.HotkeyManager _hotkeyManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _hotkeyManager = HotkeyManager.Current;
            _hotkeyManager.AddOrReplace("Email selected text", Key.Q,
                ModifierKeys.Control | ModifierKeys.Shift , HotkeyPressed);
        }

        private void HotkeyPressed(object sender, HotkeyEventArgs e)
        {
            string selectedText = Clipboard.GetText();
            SendEmail(selectedText);
        }

        private static void SendEmail(string selectedText)
        {
            var fromAddress = new MailAddress("vani172@gmail.com", "Evernote");
            var toAddress = new MailAddress("bc13127ea6@nirvanahq.in", "NirvanaHq");
            const string fromPassword = "rmiuvgnlkarfyyce";
            string subject = selectedText.Trim();
            string body = "";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject
            })
            { 
                smtp.Send(message);
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _hotkeyManager.Remove("Email selected text");
        }
    }
}
