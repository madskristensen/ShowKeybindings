using System.Collections.Generic;

namespace ShowKeybindings
{
    internal interface IConverter
    {
        public string Convert(IEnumerable<KeyItem> items);
    }
}
