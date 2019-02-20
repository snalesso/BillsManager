using BillsManager.Models;
using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace BillsManager.Services
{
    public class XMLSettingsProvider : ISettingsProvider
    {
        #region fields

        // FILE PATHS
        private const string SettingsFileDotExtension = ".bmse";
        private const string SettingsFileNameWithoutExtension = "settings";
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileNameWithoutExtension + SettingsFileDotExtension);

        // NAMESPACES
        private const string Namespace_SettingsRoot = "settings";
        private const string Item_StartupDBLoad = "startup_db_load";
        private const string Item_Language = "language";
        private const string Item_FeedbackToEmailAddress = "feedback_to_email_address";

        // DEFAULT VALUES
        private const string Default_Language = "it-IT";
        private const string Default_FeedbackEmailAddressee = "nalessosergio@gmail.com";
        private const bool Default_LoadDbAtStartup = true;

        private readonly XDocument xmlSettings;

        #endregion

        #region ctor

        public XMLSettingsProvider()
        {
            if (File.Exists(settingsFilePath))
                this.xmlSettings = this.LoadXSettings();
            else
            {
                this.xmlSettings = this.CreateDefaultXSettings();
                this.Settings = this.ExtractSettings(this.xmlSettings);
                this.Save();
            }

            this.Settings = this.ExtractSettings(this.xmlSettings);
        }

        #endregion

        #region properties

        public Models.Settings Settings { get; protected set; }

        #endregion

        #region methods

        protected XDocument LoadXSettings()
        {
            try
            {
                return XDocument.Load(settingsFilePath);
            }
            catch (Exception)
            {
                return this.CreateDefaultXSettings();
            }
        }

        public bool Save()
        {
            this.xmlSettings.Root.Element(Item_Language).SetValue(this.Settings.Language.Name);
            this.xmlSettings.Root.Element(Item_StartupDBLoad).SetValue(this.Settings.StartupDBLoad);
            this.xmlSettings.Root.Element(Item_FeedbackToEmailAddress).SetValue(this.Settings.FeedbackToEmailAddress);

            try
            {
                this.xmlSettings.Save(settingsFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected XDocument CreateDefaultXSettings()
        {
            var newXMLSetts = new XDocument();

            newXMLSetts.Declaration = new XDeclaration("1.0", "utf-8", null);

            newXMLSetts.Add(
                 new XElement(Namespace_SettingsRoot,
                     new XElement(Item_Language, Default_Language),
                     new XElement(Item_StartupDBLoad, Default_LoadDbAtStartup),
                     new XElement(Item_FeedbackToEmailAddress, Default_FeedbackEmailAddressee)));

            return newXMLSetts;
        }

        protected Models.Settings ExtractSettings(XDocument XSettings)
        {
            var ci = CultureInfo.GetCultureInfo(XSettings.Root.Element(Item_Language).Value);
            var load = (bool)XSettings.Root.Element(Item_StartupDBLoad);
            var toEmAdd = XSettings.Root.Element(Item_FeedbackToEmailAddress).Value;

            return
                new Models.Settings(
                    ci,
                    load,
                    toEmAdd);
        }

        #endregion
    }
}