using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MrCleanUp
{
    class Program
    {
        static List<string> allowFormats = new List<string> { "taglib/wma", "taglib/mp3" };
        static string input = @"C:\Users\Kong Heinse\Desktop\SharingBandit\Musik"; // @"C:\Users\Kong Heinse\Desktop\SharingBandit\Musik"; 
        static string output = @"c:\temp\output";
        static Regex RemoveChars = new Regex(@"[\\/:*?""<>|]");

        static void Main(string[] args)
        {
            // var dir = Directory.GetFiles(input);
            
            // Directory.CreateDirectory(output);

            MapFiles(input);

            Console.ReadLine();
        }

        static void MapFiles(string directory)
        {
            Console.WriteLine(directory);

            var files = Directory.GetFiles(directory);

            foreach (var file in files)
            {
                try
                {
                    TagLib.File f = TagLib.File.Create(file);
                    /*
                    Console.WriteLine(f.Tag.JoinedPerformers);
                    Console.WriteLine(f.Tag.Title);
                    Console.WriteLine(f.Tag.Album);
                    Console.WriteLine(f.MimeType);   
                    */

                    string type = f.MimeType.Split(@"/")[1];

                    if (type != "taglib/ini")
                    {
                        string artists = f.Tag.JoinedPerformers != null ? f.Tag.JoinedPerformers : "";
                        string album = f.Tag.Album != null ? f.Tag.Album : "";

                        SaveMediaFile(file,
                                 RemoveChars.Replace(type, ""),
                                 RemoveChars.Replace(artists, ""),
                                 RemoveChars.Replace(album, "")
                                 );
                    }
                }
                catch (Exception e)
                {

                    // Console.ReadKey();
                    try
                    {
                        SaveFile(file);
                    }

                    catch (Exception expt)
                    {
                        Console.WriteLine(expt.Message);
                        continue;
                    }
                    Console.WriteLine(e.Message);
                    continue;
                }
            }             

            var dirs = Directory.GetDirectories(directory);

            foreach (var dir in dirs)
            {
                MapFiles(dir);
            }

            if (Directory.GetFiles(directory).Length == 0 &&
            Directory.GetDirectories(directory).Length == 0)
            {               
                // Directory.Delete(directory, false);
            }
        }

        static void SaveMediaFile (string file, string type, string artist, string album)
        {
            string fileName = file.Split('.')[0];

            if (fileName.Contains(@"\"))
            {
                int index = fileName.LastIndexOf(@"\");
                fileName = fileName.Substring(index);
            }

            // var fil = File.ReadAllBytes(file);
            string basePath = output + @"\" + type.ToUpper() + @"\" + artist + @"\" + album + @"\";
            string path = basePath + fileName + "." + type.ToLower();
            Directory.CreateDirectory(basePath);
            File.Move(file, path, true);
            // File.WriteAllBytes(path, fil);

            Console.WriteLine(path);
        }

        static void SaveFile(string file)
        {
            string fileName = file.Split('.')[0];
            string type = file.Split('.')[1];

            if (fileName.Contains(@"\"))
            {
                int index = fileName.LastIndexOf(@"\");
                fileName = fileName.Substring(index);
            }

            string basePath = output + @"\" + type.ToUpper() + @"\";
            string path = basePath + fileName + "." + type.ToLower();
            Directory.CreateDirectory(basePath);
            File.Move(file, path, true);

            Console.WriteLine(path);
        }
    }
}
