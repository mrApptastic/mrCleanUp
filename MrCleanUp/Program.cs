using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MrCleanUp
{
    class Program
    {
        static List<string> allowFormats = new List<string> { "taglib/wma", "taglib/mp3" };
        static string input = @"C:\Users\Kong Heinse\Desktop\SharingBandit\Musik\Musik til oprydning\Top 500 Rock And Roll Songs"; // @"C:\Users\Kong Heinse\Desktop\SharingBandit\Musik"; C:\Users\Kong Heinse\Desktop\SharingBandit\Musik\Musik til oprydning\Fimses Musik
        static string output = @"c:\temp\output";
        static Regex RemoveChars = new Regex(@"[\\/:*?""<>|]");

        static void Main(string[] args)
        {
            // var dir = Directory.GetFiles(input);
            
            // Directory.CreateDirectory(output);

            MapFiles(input);

            Console.ReadLine();
        }

        static void MapFiles (string directory)        
        {
            Console.WriteLine(directory);

            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                TagLib.File f = TagLib.File.Create(file);
                /*
                Console.WriteLine(f.Tag.JoinedPerformers);
                Console.WriteLine(f.Tag.Title);
                Console.WriteLine(f.Tag.Album);
                Console.WriteLine(f.MimeType);   
                */

                string type = f.MimeType.Split(@"/")[1];
                string artists = f.Tag.JoinedPerformers != null ? f.Tag.JoinedPerformers : "";
                string album = f.Tag.Album != null ? f.Tag.Album : "";

                SaveFile(file, 
                         RemoveChars.Replace(type, ""), 
                         RemoveChars.Replace(artists, ""), 
                         RemoveChars.Replace(album, "")
                         );
            }

            var dirs = Directory.GetDirectories(directory);

            foreach (var dir in dirs)
            {
                MapFiles(dir);
            }
        }

        static void SaveFile (string file, string type, string artist, string album)
        {
            // var fil = File.ReadAllBytes(file);
            string basePath = output + @"\" + type.ToUpper() + @"\" + artist + @"\" + album + @"\";
            string path = basePath + file.Split('.')[0].Substring(file.LastIndexOf(@"\")) + "." + type.ToLower();
            Directory.CreateDirectory(basePath);
            File.Move(file, path, true);
            // File.WriteAllBytes(path, fil);

            Console.WriteLine(path);
        }
    }
}
