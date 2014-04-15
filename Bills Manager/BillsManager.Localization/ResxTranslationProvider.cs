using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace BillsManager.Localization
{
    public class ResxTranslationProvider : ITranslationProvider
    {
        #region Private Members

        private readonly ResourceManager resourceManager;

        #endregion

        #region Construction

        public ResxTranslationProvider(string baseName, Assembly assembly)
        {
            resourceManager = new ResourceManager(baseName, assembly);

            this.availableLanguages = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Where(c =>
                    {
                        try
                        {
                            assembly.GetSatelliteAssembly(c);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    });
        }

        #endregion

        #region ITranslationProvider Members

        public object Translate(string key)
        {
            var value = resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);
            return value ;
        }

        #endregion

        #region ITranslationProvider Members

        private readonly IEnumerable<CultureInfo> availableLanguages;
        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                // TODO: Resolve the available languages
                //yield return new CultureInfo("de");
                //yield return new CultureInfo("en");
                return this.availableLanguages;
            }
        }

        #endregion
    }
}