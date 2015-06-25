using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GroupLanguages
{
    class Program
    {
        static void Main(string[] args)
        {
            var newLangsFolder = args[0];

            if (newLangsFolder == null ||
                newLangsFolder == string.Empty ||
                Path.GetInvalidFileNameChars().Any(ic => newLangsFolder.Contains(ic)))
            {
                Console.WriteLine();
                Console.WriteLine("Languages folder name '" + newLangsFolder + "' not valid.");
                Console.WriteLine();
                return;
            }

            try
            {
                Console.WriteLine();
                Console.WriteLine("Started grouping languages into '" + newLangsFolder + "'...");
                Console.WriteLine();

                var langsDirInfo = Directory.CreateDirectory(newLangsFolder);

                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                var specificCultures = cultures.Where(c => c.CultureTypes.HasFlag(CultureTypes.SpecificCultures));
                var cultureNames = specificCultures.Select(sc => sc.Name);

                foreach (var dir in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory))
                {
                    var dirName = Path.GetFileName(dir);
                    if (cultureNames.Contains(dirName))
                    {
                        MoveDirectory(dir, langsDirInfo.FullName, true);
                    }
                }

                Console.WriteLine("Finished moving languages.");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var now = DateTime.Now;
                File.AppendAllText("Group Languages Exceptions.txt",
                    now.ToShortDateString() + " " + now.ToShortTimeString() + " " + ex.Message + Environment.NewLine);

                Console.WriteLine("Error moving languages.");
                Console.WriteLine();
            }
        }

        private static void MoveDirectory(string source, string target, bool overwrite = true)
        {
            if (Directory.Exists(source) & Directory.Exists(target))
            {
                var movedSource = Path.Combine(target, Path.GetFileName(source));
                
                if (!Directory.Exists(movedSource))
                    Directory.CreateDirectory(movedSource);

                var sourceFiles = Directory.GetFiles(source);

                foreach (var file in sourceFiles)
                {
                    var movedFile = Path.Combine(movedSource, Path.GetFileName(file));
                    
                    if (File.Exists(movedFile))
                        File.Delete(movedFile);
                    
                    File.Copy(file, movedFile, true);
                    File.Delete(file);

                    Console.WriteLine("From " + file);
                    Console.WriteLine("To " + movedFile);
                    Console.WriteLine();

                }
                foreach (var dir in Directory.GetDirectories(source))
                {
                    MoveDirectory(dir, movedSource, overwrite);
                }
                Directory.Delete(source);
            }
        }
    }
}