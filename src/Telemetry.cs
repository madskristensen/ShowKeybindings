using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Telemetry;

namespace ShowKeybindings
{
    public class Telemetry
    {
        private const string _namespace = "VS/Extension/" + Vsix.Name + "/";

        public static TelemetryEvent CreateEvent(string name)
        {
            return new TelemetryEvent(CleanName(name));
        }

        public static void TrackEvent(TelemetryEvent telemetryEvent)
        {
            TelemetryService.DefaultSession.PostEvent(telemetryEvent);
        }

        public static void TrackUserTask(string name, TelemetryResult result = TelemetryResult.Success)
        {
            TelemetryService.DefaultSession.PostUserTask(CleanName(name), result);
        }

        private static string CleanName(string name)
        {
            return Regex.Replace(_namespace + name, @"\s*", "");
        }
    }
}
