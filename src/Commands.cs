using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;

namespace ShowKeybindings
{
    public record KeyItem(string Name, string KeyBinding, string Category);

    public class Commands
    {

        public static async Task<IEnumerable<KeyItem>> GetCommandsAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            List<KeyItem> items = new();
            DTE2 dte = await VS.GetServiceAsync<DTE, DTE2>();

            foreach (Command command in dte.Commands)
            {
                if (string.IsNullOrEmpty(command.Name))
                {
                    continue;
                }

                if (command.Bindings is object[] bindings && bindings.Length > 0)
                {
                    string binding = bindings[0].ToString();

                    if (binding.Contains("Ctrl") || binding.Contains("Alt") || binding.Contains("Shift"))
                    {
                        int scopeIndex = binding.IndexOf("::");
                        if (scopeIndex >= 0)
                        {
                            binding = binding.Substring(scopeIndex + 2);
                        }

                        int index = command.Name.IndexOf('.');
                        string prefix = index > 0 ? CleanName(command.Name.Substring(0, index)) : "Misc";

                        items.Add(new KeyItem(command.Name, binding, prefix));
                    }
                }
            }

            return items.OrderBy(i => i.Category);
        }

        public static string CleanName(string name)
        {
            StringBuilder sb = new();
            sb.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                char c = name[i];

                if (char.IsUpper(c) && !char.IsUpper(name[i - 1]))
                {
                    sb.Append(" ");
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
