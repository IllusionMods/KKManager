using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KKManager.Properties;
using KKManager.Util;

namespace KKManager
{
    public static class LanguageManager
    {
        // Add newly supported languages to this list. Languages need to be in the "xx-yy" format, even if resources are only named "xx".
        private static readonly string[] _supportedLanguageNames =
        {
            "en-US",
            "en-GB",
            "zh-Hans",
            "zh-Hant",
        };

        private static IEnumerable<CultureInfo> _supportedLanguages;
        public static IEnumerable<CultureInfo> SupportedLanguages => _supportedLanguages ?? (_supportedLanguages = GetSupportedLanguages());

        private static IEnumerable<CultureInfo> GetSupportedLanguages()
        {
            // Check what translations are available in program dir
            var location = typeof(LanguageManager).Assembly.Location;
            if (location.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) location = Path.GetDirectoryName(location);
            var translationDirectories = new DirectoryInfo(location).GetDirectories()
                .Where(x =>
                {
                    if (x.Name.Length < 2)
                        return false;
                    try
                    {
                        return x.GetFiles("KKManager.resources.dll", SearchOption.TopDirectoryOnly).Any();
                    }
                    catch (SystemException e)
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                })
                .Select(x => x.Name.Substring(0, 2).ToLower())
                .ToList();
            var supportedCultures = _supportedLanguageNames.Attempt(CultureInfo.GetCultureInfo).ToList();

            var missingLanguageCodes = translationDirectories
                .Where(x => supportedCultures.Select(c => c.Name.Substring(0, 2)).All(z => !z.Equals(x, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (missingLanguageCodes.Any())
                Console.WriteLine($"Following language files are present but they are not specified as supported in {nameof(LanguageManager)}: " + string.Join(", ", missingLanguageCodes));

            return supportedCultures.Where(x =>
            {
                var code = x.Name.Substring(0, 2).ToLower();
                return code.Equals("en", StringComparison.Ordinal) || translationDirectories.Contains(code);
            }).OrderBy(x => x.DisplayName).ToList().AsEnumerable();
        }

        public static CultureInfo CurrentCulture
        {
            get
            {
                return SupportedLanguages.FirstOrDefault(x => x.Name == Settings.Default.Language) ?? CultureInfo.GetCultureInfo("en-US");
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                Settings.Default.Language = value.Name;
            }
        }

        /// <summary>
        /// Set Window Language
        /// </summary>
        public static void ApplyCurrentCulture(Form form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            var resources = new System.ComponentModel.ComponentResourceManager(form.GetType());
            resources.ApplyResources(form, "$this");
            ApplyCurrentCulture(form, resources);
        }

        private static void ApplyCurrentCulture(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            if (control is MenuStrip ms)
            {
                resources.ApplyResources(control, ms.Name);
                if (ms.Items.Count > 0)
                {
                    foreach (ToolStripMenuItem c in ms.Items)
                    {
                        ApplyCurrentCulture(c, resources);
                    }
                }
            }

            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                ApplyCurrentCulture(c, resources);
            }
        }

        private static void ApplyCurrentCulture(ToolStripMenuItem item, System.ComponentModel.ComponentResourceManager resources)
        {
            resources.ApplyResources(item, item.Name);
            if (item.DropDownItems.Count > 0)
            {
                foreach (var c in item.DropDownItems)
                {
                    if (c is ToolStripMenuItem menuItem)
                    {
                        ApplyCurrentCulture(menuItem, resources);
                    }
                }
            }
        }
    }
}
