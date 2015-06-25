using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace BillsManager.Localization
{
    public abstract class SimpleTranslationDictionary : ITranslationDictionary
    {
        private const char KeyValueSeparator = '\t';

        private readonly Dictionary<string, string> _translationsDictionary;

        public SimpleTranslationDictionary(string translationDictionaryFilePath)
        {
            if (string.IsNullOrWhiteSpace(translationDictionaryFilePath))
                throw new ArgumentNullException("translationDictionaryFilePath");

            if (!File.Exists(translationDictionaryFilePath))
                throw new FileNotFoundException("Couldn't locate the translation dictionary file.", translationDictionaryFilePath);

            this._translationsDictionary = new Dictionary<string, string>();

            var lines = new List<string>(File.ReadAllLines(translationDictionaryFilePath));

            if (lines.Count > 0)
            {
                var lcid = int.Parse(lines[0]);
                this._language = new CultureInfo(lcid);
                lines.RemoveAt(0);

                lines.ForEach(line =>
                {
                    var kv = line.Split(SimpleTranslationDictionary.KeyValueSeparator);
                    this._translationsDictionary.Add(kv[0], kv[1]);
                });
            }
        }

        private readonly CultureInfo _language;
        public CultureInfo Language
        {
            get { return this._language; }
        }

        protected string GetTranslation([CallerMemberName] string key = null)
        {
            var translation = this._translationsDictionary[key];
            return translation ?? "!" + key + "!";
        }
    }
}