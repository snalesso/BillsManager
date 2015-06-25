using System;
using System.Windows;

namespace BillsManager.Localization
{
    public class LanguageChangedEventManager : WeakEventManager
    {
        #region methods

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            this.DeliverEvent(sender, e);
        }

        protected override void StartListening(object source)
        {
            var manager = (TranslationManager)source;
            manager.LanguageChanged += this.OnLanguageChanged;
        }

        protected override void StopListening(Object source)
        {
            var manager = (TranslationManager)source;
            manager.LanguageChanged -= this.OnLanguageChanged;
        }

        #region static methods

        public static void AddListener(TranslationManager source, IWeakEventListener listener)
        {
            LanguageChangedEventManager.CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(TranslationManager source, IWeakEventListener listener)
        {
            LanguageChangedEventManager.CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private static LanguageChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(LanguageChangedEventManager);
                var manager = (LanguageChangedEventManager)LanguageChangedEventManager.GetCurrentManager(managerType);

                if (manager == null)
                {
                    manager = new LanguageChangedEventManager();
                    LanguageChangedEventManager.SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }

        #endregion
        
        #endregion
    }
}