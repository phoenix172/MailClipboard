using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CredentialManagement;

namespace MailClipboard
{
    public class AppConfiguration
    {
        private AppConfiguration()
        {
        }

        private static class Keys
        {
            public const string RecipientEmail = "RecipientEmail";
            public const string Hotkey = "Hotkey";
            public const string SenderEmail = "SenderEmail";
            public const string RecipientName = "RecipientName";
            public const string SenderName = "SenderName";
            public const string EmailBody = "EmailBody";
        }

        public static AppConfiguration Load()
        {
            HotkeyConverter converter = new HotkeyConverter();
            return new AppConfiguration
            {
                Hotkey = converter.ConvertFromInvariantString(ConfigurationManager.AppSettings.Get(Keys.Hotkey)),
                SenderEmail = AppSettings.Get(Keys.SenderEmail),
                RecipientEmail = AppSettings.Get(Keys.RecipientEmail),
                SenderName =  AppSettings.Get(Keys.SenderName),
                RecipientName = AppSettings.Get(Keys.RecipientName),
                EmailBody = AppSettings.Get(Keys.EmailBody)
            };
        }

        private static NameValueCollection AppSettings
            => ConfigurationManager.AppSettings;

        public string EmailBody { get; private set; }
        public Hotkey Hotkey { get; private set; }
        public string SenderEmail { get; private set; }
        public string RecipientEmail { get; private set; }
        public string SenderName { get; private set; }
        public string RecipientName { get; private set; }
    }
}
