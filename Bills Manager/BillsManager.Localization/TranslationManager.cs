using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace BillsManager.Localization
{
    public class TranslationManager
    {
        #region ctor

        private TranslationManager()
        {
        }

        #endregion

        #region properties

        private static TranslationManager instance;
        public static TranslationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TranslationManager();

                return instance;
            }
        }

        public CultureInfo CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (value != Thread.CurrentThread.CurrentUICulture)
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    this.OnLanguageChanged();
                    FrameworkElement.LanguageProperty.OverrideMetadata(
                        typeof(FrameworkElement),
                        new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(value.IetfLanguageTag)));
                }
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                if (this.TranslationProvider != null)
                {
                    return this.TranslationProvider.Languages;
                }
                return Enumerable.Empty<CultureInfo>();
            }
        }

        // TODO: if translation provider changes, available languages have to be updated
        public ITranslationProvider TranslationProvider { get; set; }

        #endregion

        #region methods

        public object Translate(string key)
        {
            if (this.TranslationProvider != null)
            {
                object translatedValue = this.TranslationProvider.Translate(key);

                if (translatedValue != null)
                {
                    return translatedValue;
                }
            }
            return string.Format("!{0}!", key);
        }

        #endregion

        #region events

        public event EventHandler LanguageChanged;

        private void OnLanguageChanged()
        {
            if (this.LanguageChanged != null)
            {
                this.LanguageChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}