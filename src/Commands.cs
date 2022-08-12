using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;

namespace ShowKeybindings
{
    public record KeyItem(string Name, string KeyBinding);
    
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

                    int scopeIndex = binding.IndexOf("::");
                    if (scopeIndex >= 0)
                    {
                        binding = binding.Substring(scopeIndex + 2);
                    }

                    items.Add(new KeyItem(command.Name, binding));
                }
            }

            return items.OrderBy(i => i.Name);
        }
    }
}
