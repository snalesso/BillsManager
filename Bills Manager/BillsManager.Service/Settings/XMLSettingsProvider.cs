using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace BillsManager.Services.Settings
{
    public class XMLSettingsProvider : ISettingsProvider
    {
        #region fields

        // FILE PATHS
        private const string SETTINGS_FILE_DOT_EXT = ".bmse";
        private const string SETTINGS_FILE_NAMEWO_EXT = "settings";
        private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILE_NAMEWO_EXT + SETTINGS_FILE_DOT_EXT);

        // NAMESPACES
        private const string NS_SETTINGS_ROOT = "settings";
        private const string ITEM_STARTUP_DB_LOAD = "startup_db_load";
        private const string ITEM_LANGUAGE = "language";
        private const string ITEM_FEEDBACK_TO_EMAIL_ADDRESS = "feedback_to_email_address";

        // DEFAULT VALUES
        private const string DV_FEEDBACK_TO_EMAIL_ADDRESS = "nalesso.sergio@gmail.com";
        private readonly string DV_LANGUAGE = "it-IT";
        private const bool DV_STARTUP_DB_LOAD = true;

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
                return XDocument.Load(settingsFilePath); // TODO: is parsing error possible?
            }
            catch (Exception)
            {
                return this.CreateDefaultXSettings();
            }
        }

        public bool Save()
        {
            this.xmlSettings.Root.Element(ITEM_LANGUAGE).SetValue(this.Settings.Language.Name);
            this.xmlSettings.Root.Element(ITEM_STARTUP_DB_LOAD).SetValue(this.Settings.StartupDBLoad);
            this.xmlSettings.Root.Element(ITEM_FEEDBACK_TO_EMAIL_ADDRESS).SetValue(this.Settings.FeedbackToEmailAddress);

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
                 new XElement(NS_SETTINGS_ROOT,
                     new XElement(ITEM_LANGUAGE, DV_LANGUAGE),
                     new XElement(ITEM_STARTUP_DB_LOAD, DV_STARTUP_DB_LOAD),
                     new XElement(ITEM_FEEDBACK_TO_EMAIL_ADDRESS, DV_FEEDBACK_TO_EMAIL_ADDRESS)));

            return newXMLSetts;
        }

        protected Models.Settings ExtractSettings(XDocument XSettings)
        {
            var ci = CultureInfo.GetCultureInfo(XSettings.Root.Element(ITEM_LANGUAGE).Value);
            var load = (bool)XSettings.Root.Element(ITEM_STARTUP_DB_LOAD);
            var toEmAdd = XSettings.Root.Element(ITEM_FEEDBACK_TO_EMAIL_ADDRESS).Value;

            return
                new Models.Settings(
                    ci,
                    load,
                    toEmAdd);
        }

        #endregion
    }
}