using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupLanguages
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var newLangsFolder = args[0];

                if (newLangsFolder == null || newLangsFolder == string.Empty)
                    return;

                foreach (var ic in Path.GetInvalidFileNameChars())
                    if (newLangsFolder.Contains(ic))
                        return;

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
            }
            catch (Exception ex)
            {
                var now = DateTime.Now;
                File.AppendAllText("Group Languages Exceptions.txt", 
                    now.ToShortDateString() + " " +  now.ToShortTimeString() + " " + ex.Message + Environment.NewLine);
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
                    File.Move(file, movedFile);
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