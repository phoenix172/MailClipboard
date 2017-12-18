using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MailClipboard
{
    public class HotkeyConverter
    {
        private readonly KeyConverter _keyConverter;
        private readonly ModifierKeysConverter _modifierKeyConverter;

        public HotkeyConverter()
        {
            _modifierKeyConverter = new ModifierKeysConverter();
            _keyConverter = new KeyConverter();
        }

        public Hotkey ConvertFromInvariantString(string hotkey)
        {
            var allKeysString = hotkey.Split('+')
                .Select(x => x.Trim());

            ModifierKeys modifiers = default;
            Key key = default;

            foreach (var keyString in allKeysString)
            {
                if (TryConvertKey(keyString, _modifierKeyConverter, out ModifierKeys modifierKey))
                    modifiers |= modifierKey;
                else TryConvertKey(keyString, _keyConverter, out key);
            }
            if(modifiers == default || key == default)
                throw new InvalidCastException($"Unable to convert '{hotkey}' to hotkey");
            return new Hotkey(key, modifiers);
        }

        private static bool TryConvertKey<T>(string keyString, TypeConverter converter, out T key)
        {
            if (converter.IsValid(keyString)
                && converter.ConvertFromInvariantString(keyString)
                    is T convertedKey)
            {
                key = convertedKey;
                return true;
            }
            key = default;
            return false;
        }
    }
}
