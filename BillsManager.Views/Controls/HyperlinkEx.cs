using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Documents;

namespace BillsManager.Views.Controls
{
    public class HyperlinkEx : Hyperlink
    {
        //public bool IsMailto { get; set; }

        protected override void OnClick()
        {
            base.OnClick();

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo();

            var format = this.GetBindingExpression(HyperlinkEx.NavigateUriProperty).ParentBindingBase.StringFormat;

            string link;
            if (!string.IsNullOrWhiteSpace(format))
                link = string.Format(format, this.NavigateUri.ToString());
            else
                link = this.NavigateUri.ToString();

            if (!link.StartsWith("mailto", System.StringComparison.OrdinalIgnoreCase))
            {
                string browserPath = this.GetBrowserPath();
                if (browserPath == string.Empty)
                    browserPath = "iexplore";

                process.StartInfo.FileName = browserPath;
                process.StartInfo.Arguments = "\"" + link + "\"";
            }
            else
            {
                process.StartInfo.FileName = link;
            }

            process.Start();
        }

        private string GetBrowserPath()
        {
            string browser = string.Empty;
            RegistryKey key = null;

            try
            {
                // try location of default browser path in XP
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                // try location of default browser path in Vista
                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http", false); ;
                }

                if (key != null)
                {
                    //trim off quotes
                    browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                    if (!browser.EndsWith("exe"))
                    {
                        //get rid of everything after the ".exe"
                        browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
                    }

                    key.Close();
                }
            }
            catch
            {
                return string.Empty;
            }

            return browser;
        }
    }
}