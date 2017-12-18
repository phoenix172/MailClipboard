using System.Windows.Input;

namespace MailClipboard
{
    public class Hotkey
    {
        public Hotkey(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }
        
        public Key Key { get; }
        public ModifierKeys Modifiers { get; }

        public override string ToString() => $"{Modifiers} + {Key}";
    }
}