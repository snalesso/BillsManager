using System;
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
            this.resourceManager = new ResourceManager(baseName, assembly);

            var avLangs = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Where(culture =>
                {
                    try
                    {
                        assembly.GetSatelliteAssembly(culture);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                })
                .Concat(
                new CultureInfo[]
                { 
                    CultureInfo.GetCultureInfo(Assembly.GetEntryAssembly().GetCustomAttribute<NeutralResourcesLanguageAttribute>().CultureName)
                })
                /*.ToList()*/;

            this.availableLanguages = avLangs;
        }

        #endregion

        #region ITranslationProvider Members

        public string Translate(string key)
        {
            var value = resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);
            return value;
        }

        #endregion

        #region ITranslationProvider Members

        private readonly IEnumerable<CultureInfo> availableLanguages;
        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                return this.availableLanguages;
            }
        }

        #endregion
    }
}