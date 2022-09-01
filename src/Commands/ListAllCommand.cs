using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.Win32;

namespace ShowKeybindings
{
    [Command(PackageIds.ListAll)]
    internal sealed class ListAllCommand : BaseCommand<ListAllCommand>
    {
        private static readonly RatingPrompt _rating = new("MadsKristensen.ShowKeybindings", Vsix.Name, General.Instance, 2);

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Filter = "VS Settings File (.vssettings)|*.vssettings|Text File (.txt)|*.txt|HTML Document (.html)|*.html|JSON (.json)|*.json|XML (.xml)|*.xml",
                FileName = "VS Shortcuts"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                IEnumerable<KeyItem> bindings = await Commands.GetCommandsAsync();
                string content = GetStringRepresentation(dialog.FileName, bindings);

                using (StreamWriter writer = new(dialog.FileName, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(content);
                }

                Process.Start(dialog.FileName);

                await Task.Delay(5000);
                _rating.RegisterSuccessfulUsage();
            }
        }

        private static string GetStringRepresentation(string fileName, IEnumerable<KeyItem> items)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();

            TelemetryEvent tel = Telemetry.CreateEvent("export");
            tel.Properties["fileextension"] = ext;

            IConverter converter = ext switch
            {
                ".html" or ".html" => new HtmlConverter(),
                ".json" or ".json5" or ".jsonc" => new JsonConverter(),
                ".xml" or ".vssettings" => new XmlConverter(),
                _ => new TextConverter()
            };

            try
            {
                return converter.Convert(items);
            }
            finally
            {
                Telemetry.TrackEvent(tel);
            }
        }
    }
}
