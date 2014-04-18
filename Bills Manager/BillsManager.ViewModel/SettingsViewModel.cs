using BillsManager.Localization;
using BillsManager.Models;
using BillsManager.Services.Settings;
using BillsManager.ViewModels.Commanding;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Globalization;

namespace BillsManager.ViewModels
{
    public partial class SettingsViewModel : Screen
    {
        #region fields

        private readonly ISettingsProvider settingsProvider;

        private readonly Settings oldSettings;

        #endregion

        #region ctor

        public SettingsViewModel(
            ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;

            this.oldSettings = (Settings)this.settingsProvider.Settings.Clone();

            this.DisplayName = TranslationManager.Instance.Translate("Settings").ToString();
        }

        #endregion

        #region properties

        public IEnumerable<CultureInfo> AvailableLanguages
        {
            get { return TranslationManager.Instance.Languages; }
        }

        public CultureInfo CurrentLanguage
        {
            get { return this.settingsProvider.Settings.Language; }
            set
            {
                if (this.CurrentLanguage == value) return;

                this.settingsProvider.Settings.Language = value;
                this.NotifyOfPropertyChange(() => this.CurrentLanguage);
                TranslationManager.Instance.CurrentLanguage = value;
            }
        }

        public bool StartupDBLoad
        {
            get { return this.settingsProvider.Settings.StartupDBLoad; }
            set
            {
                if (this.StartupDBLoad == value) return;

                this.settingsProvider.Settings.StartupDBLoad = value;
                this.NotifyOfPropertyChange(() => this.StartupDBLoad);
            }
        }

        public string FeedbackToEmailAddress
        {
            get { return this.settingsProvider.Settings.FeedbackToEmailAddress; }
            set
            {
                if (this.FeedbackToEmailAddress == value) return;

                this.settingsProvider.Settings.FeedbackToEmailAddress = value;
                this.NotifyOfPropertyChange(() => this.FeedbackToEmailAddress);
            }
        }

        #endregion

        #region methods

        protected void RevertChanges()
        {
            this.settingsProvider.Settings.Language = this.oldSettings.Language;
            this.settingsProvider.Settings.StartupDBLoad = this.oldSettings.StartupDBLoad;
            this.settingsProvider.Settings.FeedbackToEmailAddress = this.oldSettings.FeedbackToEmailAddress;
        }

        protected void SaveAndClose()
        {
            this.settingsProvider.Save();
            this.TryClose();
        }

        protected void CancelAndClose()
        {
            this.RevertChanges();
            this.SaveAndClose();
        }

        #endregion

        #region commands

        private RelayCommand saveAndCloseCommand;
        public RelayCommand SaveAndCloseCommand
        {
            get
            {
                if (this.saveAndCloseCommand == null)
                    this.saveAndCloseCommand = new RelayCommand(
                        () => this.SaveAndClose());

                return this.saveAndCloseCommand;
            }
        }

        private RelayCommand cancelAndCloseCommand;
        public RelayCommand CancelAndCloseCommand
        {
            get
            {
                if (this.cancelAndCloseCommand == null)
                    this.cancelAndCloseCommand = new RelayCommand(
                        () => this.CancelAndClose());

                return this.cancelAndCloseCommand;
            }
        }

        #endregion
    }
}