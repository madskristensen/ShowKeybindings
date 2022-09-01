using System.Collections.Generic;
using System.Xml.Linq;

namespace ShowKeybindings
{
    internal class XmlConverter : IConverter
    {
        private readonly Version _version;

        public XmlConverter(Version version)
        {
            _version = version;
        }

        public string Convert(IEnumerable<KeyItem> items)
        {
            XElement userShortcuts = CreateUserShortcuts(items);

            XDocument doc = new(new XElement("UserSettings",
                                    new XElement("ApplicationIdentity",
                                        new XAttribute("version", $"{_version.Major}.0")),
                                    new XElement("ToolsOptions",
                                        new XElement("ToolsOptionsCategory",
                                            new XAttribute("name", "Environment"),
                                            new XAttribute("RegisteredName", "Environment"))
                                    ),
                                    new XElement("Category",
                                        new XAttribute("name", "Environment_Group"),
                                        new XAttribute("RegisteredName", "Environment_Group"),
                                        new XElement("Category",
                                            new XAttribute("name", "Environment_KeyBindings"),
                                            new XAttribute("Category", "{F09035F1-80D2-4312-8EC4-4D354A4BCB4C}"),
                                            new XAttribute("Package", "{DA9FB551-C724-11d0-AE1F-00A0C90FFFC3}"),
                                            new XAttribute("RegisteredName", "Environment_KeyBindings"),
                                            new XAttribute("PackageName", "Visual Studio Environment Package"),
                                            new XElement("Version", $"{_version.Major}.0.0.0"),
                                            new XElement("KeyboardShortcuts", userShortcuts))
                                        )
                                    )
                                );

            return doc.ToString();
        }

        private XElement CreateUserShortcuts(IEnumerable<KeyItem> items)
        {
            List<XElement> elements = new();

            foreach (KeyItem item in items)
            {
                XElement shortcut = new("Shortcut", item.KeyBinding);
                shortcut.SetAttributeValue("Command", item.Name);
                shortcut.SetAttributeValue("Scope", item.Scope);

                elements.Add(shortcut);
            }

            return new XElement("DefaultShortcuts", elements);
        }

        public async Task OpenAsync(string filePath)
        {
            await VS.Documents.OpenAsync(filePath);
        }
    }
}
