using System.Collections.Generic;
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
                Filter = "HTML Document (.html)|*.html|VS Settings File (.vssettings)|*.vssettings|Text File (.txt)|*.txt|JSON (.json)|*.json",
                FileName = "VS Shortcuts"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                IEnumerable<KeyItem> bindings = await Commands.GetCommandsAsync();
                IConverter converter = GetConverter(dialog.FileName, bindings);

                string content = converter.Convert(bindings);

                using (StreamWriter writer = new(dialog.FileName, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(content);
                }

                await converter.OpenAsync(dialog.FileName);

                await Task.Delay(5000);
                _rating.RegisterSuccessfulUsage();
            }
        }

        private static IConverter GetConverter(string fileName, IEnumerable<KeyItem> items)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();

            TelemetryEvent tel = Telemetry.CreateEvent("export");
            tel.Properties["fileextension"] = ext;
            Telemetry.TrackEvent(tel);

            return ext switch
            {
                ".html" or ".html" => new HtmlConverter(),
                ".json" or ".json5" or ".jsonc" => new JsonConverter(),
                ".xml" or ".vssettings" => new XmlConverter(ShowKeybindingsPackage.Version),
                _ => new TextConverter()
            };
        }
    }
}
