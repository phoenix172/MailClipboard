using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHotkey;

namespace MailClipboard
{
    class HotkeyManager : IDisposable
    {
        private const string HotkeyName = "Email clipboard";
        private readonly NHotkey.Wpf.HotkeyManager _hotkeyManager;

        public event EventHandler HotkeyPressed;

        public HotkeyManager()
        {
            _hotkeyManager = NHotkey.Wpf.HotkeyManager.Current;
        }

        public void BindHotkey(Hotkey hotkey)
        {
            _hotkeyManager.AddOrReplace(HotkeyName, hotkey.Key, hotkey.Modifiers, OnHotkeyPressed);
        }

        private void OnHotkeyPressed(object sender, HotkeyEventArgs e)
        {
            HotkeyPressed?.Invoke(sender, EventArgs.Empty);
        }

        public void Dispose()
        {
            _hotkeyManager.Remove(HotkeyName);
        }
    }
}
