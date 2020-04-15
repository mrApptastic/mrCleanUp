using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MrCleanUp
{
    class Program
    {
        static string output = @"c:\temp\output";
        static Regex RemoveChars = new Regex(@"[\\/:*?""<>|]");
        static bool exit = false;

        static void Main(string[] args)
        {
            while (!exit) {
                Console.WriteLine("Write the absolute root path to activate the clean up script:");
                string input = Console.ReadLine();
                MapFiles(input);
            }                       
        }

        static void MapFiles(string directory)
        {
            try
            {
                var files = Directory.GetFiles(directory);

                foreach (var file in files)
                {
                    try
                    {
                        TagLib.File f = TagLib.File.Create(file);

                        string type = f.MimeType.Split(@"/")[1];

                        if (type != "taglib/ini")
                        {
                            string artists = f.Tag.JoinedPerformers != null ? f.Tag.JoinedPerformers : "";
                            string album = f.Tag.Album != null ? f.Tag.Album : "";

                            SaveMediaFile(file,
                                     RemoveChars.Replace(type, "").Trim(),
                                     RemoveChars.Replace(artists, "").Trim(),
                                     RemoveChars.Replace(album, "").Trim()
                                     );
                        }
                    }
                    catch (Exception e)
                    {
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
                    MapFiles(dir + @"\");
                }

                if (Directory.GetFiles(directory).Length == 0 &&
                Directory.GetDirectories(directory).Length == 0)
                {
                    try
                    {
                        Directory.Delete(directory, false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
            catch (Exception AllExpt)
            {
                Console.WriteLine(AllExpt.Message);
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

            string basePath = output + @"\Media\" + type.ToUpper() + @"\" + artist + @"\" + album + @"\";
            string path = basePath + fileName + "." + type.ToLower();
            Directory.CreateDirectory(basePath);
            if (File.Exists(path))
            {
                Console.WriteLine("File already exists: " + path);
                path = basePath + fileName + "-" + "DUBLET" + "-" + DateTime.Now.Ticks + "." + type.ToLower();

            }
            File.Move(file, path, false);

            Console.WriteLine(path);
        }

        static void SaveFile(string file)
        {
            string prePath = @"\Other\";
            string author = "";
            DateTime modified = File.GetLastWriteTime(file);

            string fileName = file.Substring(0, file.LastIndexOf("."));
            string type = file.Substring(file.LastIndexOf(".") +1);

            if (fileName.Contains(@"\"))
            {
                int index = fileName.LastIndexOf(@"\");
                fileName = fileName.Substring(index);
            }

            if (type.ToLower() == "jpg" || 
                type.ToLower() == "png" || 
                type.ToLower() == "gif" ||
                type.ToLower() == "tiff" 
                )
            {
                prePath = @"\Images\";
            }
            else if (type.ToLower() == "doc" || 
                type.ToLower() == "docx" || 
                type.ToLower() == "txt" || 
                type.ToLower() == "rtf" ||
                type.ToLower() == "odt" || 
                type.ToLower() == "xls" || 
                type.ToLower() == "pdf" ||
                type.ToLower() == "xlsx" ||
                type.ToLower() == "xlsm" ||
                type.ToLower() == "pps" ||
                type.ToLower() == "pptx" ||
                type.ToLower() == "ods" ||
                type.ToLower() == "pub" ||
                type.ToLower() == "xps"
                     )
            {
                prePath = @"\Documents\";
            }
            else if (type.ToLower() == "vcd" ||
                type.ToLower() == "bin" ||
                type.ToLower() == "cue" ||
                type.ToLower() == "daa" ||
                type.ToLower() == "iso"
                )
            {
                prePath = @"\Discs\";
            }
            else if (type.ToLower() == "eot" ||
                type.ToLower() == "ttf" ||
                type.ToLower() == "woff" ||
                type.ToLower() == "woff2" ||
                type.ToLower() == "otf")
            {
                prePath = @"\Fonts\";
            }
            else if (type.ToLower() == "exe" ||
                type.ToLower() == "html" ||
                type.ToLower() == "css" ||
                type.ToLower() == "cs" ||
                type.ToLower() == "sql" ||
                type.ToLower() == "json" ||
                type.ToLower() == "js" ||
                type.ToLower() == "rar" ||
                type.ToLower() == "zip" ||
                type.ToLower() == "rss" ||
                type.ToLower() == "bat")
            {
                prePath = @"\Software\";
            }
            else if (type.ToLower() == "psd" ||
                type.ToLower() == "eps" ||
                type.ToLower() == "svg" ||
                type.ToLower() == "ai" ||
                type.ToLower() == "indd")
            {
                prePath = @"\Graphics\";
            }

            // string basePath = output + @"\" + prePath + @"\" + type.ToUpper() + @"\" + modified.ToString("yyyy-MM-dd") + @"\";
            string basePath = output + @"\" + prePath + @"\" + type.ToUpper() + @"\";
            string path = basePath + fileName + "." + type.ToLower();
            Directory.CreateDirectory(basePath);
            if (File.Exists(path))
            {
                Console.WriteLine("File already exists: " + path);
                path = basePath + fileName + "-" + "DUBLET" + "-" + DateTime.Now.Ticks + "." + type.ToLower();

            }
            File.Move(file, path, false);

            Console.WriteLine(path);
        }
    }
}
