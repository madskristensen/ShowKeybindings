using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace ShowKeybindings
{
    [Command(PackageIds.ListAll)]
    internal sealed class ListAllCommand : BaseCommand<ListAllCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Filter = "Text File (.txt)|*.txt|HTML Document (.html)|*.html",
                FileName = "VS Shortcuts"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                IEnumerable<KeyItem> bindings = await Commands.GetCommandsAsync();
                string content = GetStringRepresentation(dialog.FileName, bindings);

                using (StreamWriter writer = new(dialog.FileName))
                {
                    await writer.WriteAsync(content);
                }

                Process.Start(dialog.FileName);
            }
        }

        private static string GetStringRepresentation(string fileName, IEnumerable<KeyItem> items)
        {
            if (fileName.EndsWith(".html"))
            {
                return ConvertToText(items);
            }

            return ConvertToText(items);
        }

        private static string ConvertToText(IEnumerable<KeyItem> items)
        {
            StringBuilder sb = new();

            foreach (KeyItem item in items)
            {
                sb.AppendLine($"{item.Name} {item.KeyBinding}");
            }

            return sb.ToString();
        }
    }
}
