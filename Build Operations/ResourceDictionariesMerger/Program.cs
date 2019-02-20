using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ResourceDictionariesMerger
{
    class Program
    {
        const string ResourceDictionaryRootName = "ResourceDictionary";
        const string ResourceDictionaryMergedDictionariesPropertyName = "MergedDictionaries";
        const string ResourceDictionarySourcePropertyName = "Source";

        static void Main(string[] args)
        {
//#if !DEBUG
//            if (args.Length < 2)
//            {
//                Console.WriteLine("Both start and destionation dictionaries' paths are needed.");
//                Console.WriteLine();
//                Console.WriteLine("Press [Enter] to exit ...");
//                Console.ReadLine();
//                return;
//            }

//            if (!File.Exists(args[0]))
//            {
//                Debug.WriteLine("Start dictionary file not found.");
//                Console.WriteLine();
//                Console.WriteLine("Press [Enter] to exit ...");
//                return;
//            }

//            string startDictionaryPath = args[0];
//            string destinationDictionaryPath = args[1];
//#else
            //const string startDictionaryPath = @"D:\Source\Workspaces\billsmanager\Bills Manager\BillsManager.Views\Dictionaries\Styles.xaml";
            //const string destinationDictionaryPath = @"D:\Test\MergedDictionary.xaml";
            const string startDictionaryPath = @"D:\Source\Workspaces\billsmanager\Bills Manager\BillsManager.Views\Dictionaries\Styles.xaml";
            const string destinationDictionaryPath = @"D:\Source\Workspaces\billsmanager\Bills Manager\BillsManager.Views\Themes\Generic.xaml";
//#endif
            XNamespace ResourceDictionaryRootNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            // prepare the xml for the new resource dictionary
            XDocument destinationDictionary = new XDocument();
            var destinationXResourceDictionaryRoot = new XElement(ResourceDictionaryRootNamespace + ResourceDictionaryRootName);
            destinationDictionary.Add(destinationXResourceDictionaryRoot);
            destinationDictionary.Root.Add(new XAttribute(XNamespace.Xmlns + "x", @"http://schemas.microsoft.com/winfx/2006/xaml"));

            IList<string> mergedDictionariesLog = new List<string>();

            AddDictionary(destinationDictionary, startDictionaryPath, mergedDictionariesLog);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(destinationDictionaryPath, settings))
            {
                destinationDictionary.Save(xw);
            }
        }

        private static void AddDictionary(XDocument destinationDictionary, string toBeMergedDictionaryPath, IList<string> mergedDictionariesLog)
        {
            // if the dictionary has laready been merged, skip it
            if (mergedDictionariesLog.Contains(toBeMergedDictionaryPath))
                return;

            // load the dictionary that has to be merged
            XDocument toBeMergedDictionary = XDocument.Load(toBeMergedDictionaryPath);

            var toBeMergedDictionaryMergedDictionariesNode = toBeMergedDictionary.Root.Elements().FirstOrDefault(e => e.Name.LocalName == ResourceDictionaryRootName + "." + ResourceDictionaryMergedDictionariesPropertyName);

            // if the dictionary that has to be merged contains merged dictionaries add em
            if (toBeMergedDictionaryMergedDictionariesNode != null)
            {
                var mergedDictionaries = toBeMergedDictionaryMergedDictionariesNode.Elements().Where(e => e.Name.LocalName == ResourceDictionaryRootName);

                var toBeMergedDictionaryFolderPath = toBeMergedDictionaryPath.Replace(Path.GetFileName(toBeMergedDictionaryPath), string.Empty);

                var toBeMergedDictionaryMergedDictionariesPaths = mergedDictionaries.Select(md =>
                {
                    var sourceAtt = md.Attribute(ResourceDictionarySourcePropertyName);
                    return sourceAtt != null ? Path.Combine(toBeMergedDictionaryFolderPath, sourceAtt.Value) : null;
                });

                foreach (var path in toBeMergedDictionaryMergedDictionariesPaths)
                {
                    AddDictionary(destinationDictionary, path, mergedDictionariesLog);
                }

                /* remove the merged dictionaries from the dictionary that has to be merged 
                so we can add all root elements generically without adding the merged dictionaries node */
                toBeMergedDictionaryMergedDictionariesNode.Remove();
            }

            // add resource dictionary'ss imported namespaces
            var toBeMergedDictionaryNamespaceAttributes = toBeMergedDictionary.Root.Attributes().Where(att => !string.IsNullOrEmpty(att.Name.NamespaceName));
            foreach (var nsAtt in toBeMergedDictionaryNamespaceAttributes)
            {
                var sameNsInRoot = destinationDictionary.Root.GetNamespaceOfPrefix(nsAtt.Name.LocalName);

                if (sameNsInRoot == null)
                {
                    destinationDictionary.Root.Add(new XAttribute(XNamespace.Xmlns + nsAtt.Name.LocalName, nsAtt.Value));
                }
            }

            // import the resource dictionary's resources
            var toBeMergedDictionaryResources = toBeMergedDictionary.Root.Elements().ToList();

            foreach (var el in toBeMergedDictionaryResources)
            {
                // add the element namespace
                if (el.Name.Namespace != destinationDictionary.Root.Name.Namespace)
                {
                    var rooPrefix = destinationDictionary.Root.GetPrefixOfNamespace(el.Name.Namespace);
                    if (string.IsNullOrEmpty(rooPrefix))
                    {
                        var elPrefix = el.GetPrefixOfNamespace(el.Name.Namespace);
                        var newAtt = new XAttribute(XNamespace.Xmlns + elPrefix, el.Name.NamespaceName);
                        destinationDictionary.Root.Add(newAtt);
                    }
                }

                el.Remove();
                destinationDictionary.Root.Add(el);
            }

            // register this dictionary as already imported
            mergedDictionariesLog.Add(toBeMergedDictionaryPath);
        }
    }
}