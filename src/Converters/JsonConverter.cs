using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShowKeybindings
{
    internal class JsonConverter : IConverter
    {
        public string Convert(IEnumerable<KeyItem> items)
        {
            return JsonConvert.SerializeObject(items, Formatting.Indented);
        }
    }
}
