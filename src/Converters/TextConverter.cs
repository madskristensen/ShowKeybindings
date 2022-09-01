using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowKeybindings
{
    internal class TextConverter : IConverter
    {
        public string Convert(IEnumerable<KeyItem> items)
        {
            StringBuilder sb = new();
            string category = items.First().Category;
            sb.AppendLine(category);

            foreach (KeyItem item in items)
            {
                if (item.Category != category)
                {
                    sb.AppendLine();
                    sb.AppendLine(item.Category);
                    category = item.Category;
                }

                sb.AppendLine($"\t{item.Name} {item.KeyBinding}");
            }

            return sb.ToString().Trim();
        }

        public async Task OpenAsync(string filePath)
        {
            await VS.Documents.OpenAsync(filePath);
        }
    }
}
