using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ShowKeybindings
{
    internal class HtmlConverter : IConverter
    {
        public string Convert(IEnumerable<KeyItem> items)
        {
            StringBuilder sb = new();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <title>Visual Studio Shortcuts</title>");
            sb.AppendLine("  <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body, table {font-family:'Cascadia Code', verdana, sans-serif}");
            sb.AppendLine("    caption {text-align:left; font-size: 1.4em; padding-top: 1em}");
            sb.AppendLine("    tr:nth-child(odd) {background: #f1f1f1}");
            sb.AppendLine("    th {text-align:left; font-weight:bold, background: #d1d1d1}");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            string category = "";
            bool first = true;
            foreach (KeyItem item in items.Where(i => i.Scope != "Unknown Editor"))
            {
                if (item.Category != category)
                {
                    if (!first)
                    {
                        sb.AppendLine($"</table>");
                    }

                    sb.AppendLine($"<table><caption>{item.Category}</caption>");
                    sb.AppendLine($"<tr>");
                    sb.AppendLine($"  <th scope=\"col\">Command</th>");
                    sb.AppendLine($"  <th scope=\"col\">Scope</th>");
                    sb.AppendLine($"  <th scope=\"col\">Shorcut</th>");
                    sb.AppendLine($"</tr>");
                }

                sb.AppendLine($"<tr>");
                sb.AppendLine($"  <td>{item.Name}</td>");
                sb.AppendLine($"  <td>{item.Scope}</td>");
                sb.AppendLine($"  <td>{WebUtility.HtmlEncode(item.KeyBinding)}</td>");
                sb.AppendLine($"</tr>");

                if (item.Category != category)
                {
                    category = item.Category;
                }

                first = false;
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}
