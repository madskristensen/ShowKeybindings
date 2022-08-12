using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
                return ConvertToHtml(items);
            }

            return ConvertToText(items);
        }

        private static string ConvertToHtml(IEnumerable<KeyItem> items)
        {
            StringBuilder sb = new();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <title>Visual Studio Shortcuts</title>");
            sb.AppendLine("  <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body, tabler {font-family:'Cascadia Code', verdana, sans-serif}");
            sb.AppendLine("    caption {text-align:left; font-size: 1.4em; padding-top: 1em}");
            sb.AppendLine("    th {text-align:left; font-weight:bold}");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            string category = "";
            bool first = true;
            foreach (KeyItem item in items)
            {
                if (item.Category != category)
                {
                    if (!first)
                    {
                        sb.AppendLine($"</table>");
                    }
                    
                    sb.AppendLine($"<table><caption>{item.Category}</caption>");
                    sb.AppendLine($"<tr>");
                    sb.AppendLine($"  <th>Command</th>");
                    sb.AppendLine($"  <th>Shorcut</th>");
                    sb.AppendLine($"</tr>");
                }

                sb.AppendLine($"<tr>");
                sb.AppendLine($"  <td>{item.Name}</td>");
                sb.AppendLine($"  <td>{WebUtility.HtmlEncode(item.KeyBinding)}</td>");
                sb.AppendLine($"</tr>");

                if (item.Category != category)
                {
                    category = item.Category;
                }
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static string ConvertToText(IEnumerable<KeyItem> items)
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
    }
}
