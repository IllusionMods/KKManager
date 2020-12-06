using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KKManager.Properties;

namespace KKManager.Utils
{
    public static class WindowLanguageHelper
    {

        private static Dictionary<string, string> Languages = new Dictionary<string, string>()
        {
            { "English",                "en" },
            { "Japanese",               "ja" },
            { "SChinese(简体中文)",     "zh-Hans" },
            { "TChinese(繁体中文)",     "zh-Hant" },
            { "Russian",                "ru" },
            { "German",                 "de" },
            { "French",                 "fr" },
        };

        public static string GetLanguageCode(string languageName)
        {
            if (Languages.ContainsKey(languageName.Trim()))
            {
                return Languages[languageName.Trim()];
            }
            return null;
        }

        public static void SetCurrentCulture()
        {
            SetCurrentCulture(Settings.Default.Language);
        }

        public static void SetCurrentCulture(string languageName)
        {
            var lang = GetLanguageCode(languageName);
            if (lang != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            }
        }

            /// <summary>
            /// Set Window Language
            /// </summary>
            /// <param name="lang">language:zh-CN, en-US</param>
            /// <param name="form"></param>
        public static void SetLang(string lang, Form form)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            if (form != null)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(form.GetType());
                resources.ApplyResources(form, "$this");
                AppLang(form, resources);
            }
        }

        /// <summary>
        /// Set All Controls
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resources"></param>
        private static void AppLang(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            if (control is MenuStrip)
            {
                resources.ApplyResources(control, control.Name);
                MenuStrip ms = (MenuStrip)control;
                if (ms.Items.Count > 0)
                {
                    foreach (ToolStripMenuItem c in ms.Items)
                    {
                        AppLang(c, resources);
                    }
                }
            }

            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                AppLang(c, resources);
            }
        }
        /// <summary>
        /// Set All Menus
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resources"></param>
        private static void AppLang(ToolStripMenuItem item, System.ComponentModel.ComponentResourceManager resources)
        {
            if (item is ToolStripMenuItem)
            {
                resources.ApplyResources(item, item.Name);
                ToolStripMenuItem tsmi = (ToolStripMenuItem)item;
                if (tsmi.DropDownItems.Count > 0)
                {
                    foreach (var c in tsmi.DropDownItems)
                    {
                        if (tsmi is ToolStripMenuItem)
                        {
                            AppLang(c as ToolStripMenuItem, resources);
                        }
                    }
                }
            }
        }
    }
}
