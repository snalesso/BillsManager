using Caliburn.Micro;
using System;
using System.Windows;

namespace BillsManager.Localization
{
    public class TranslationData : PropertyChangedBase, IWeakEventListener
    {
        #region fields

        private string key;

        #endregion

        #region ctor

        public TranslationData(string key)
        {
            this.key = key;
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }

        #endregion

        #region properties

        public object Value
        {
            get { return TranslationManager.Instance.Translate(this.key); }
        }

        #endregion

        #region IWeakEventListener Members

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                this.NotifyOfPropertyChange(() => this.Value);
                return true;
            }
            return false;
        }

        #endregion
    }
}