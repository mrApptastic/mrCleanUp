using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MrCleanUp
{
    class Program
    {
        static string output = "";
        static Regex RemoveChars = new Regex(@"[\\/:*?""<>|]");
        static bool exit = false;

        static void Main(string[] args)
        {
            if (args.Length >= 2) {
                output = args[1];
                MapFiles(args[0]);
                if (args.Length >= 3 && args[2] == "1") {
                    Console.WriteLine("All Done!");
                   string bob = Console.ReadLine();
                }               
            } else {
                while (!exit) {
                    Console.WriteLine("Write the absolute destination path:");
                    output = Console.ReadLine();
                    Console.WriteLine("Write the absolute input path to activate the clean up script:");
                    string input = Console.ReadLine();
                    MapFiles(input);
                }     
            }                  
        }

        static void MapFiles(string directory)
        {
            try
            {
                var files = Directory.GetFiles(directory);

                foreach (var file in files)
                {
                    string type = file.Substring(file.LastIndexOf(".") +1);
                    /* Skip unhandled file types */
                    if (!Helpers.CheckFileType(type)) {
                        continue;
                    }

                    int year = File.GetLastWriteTime(file).Year;

                    if (Helpers.IsImage(type) || Helpers.IsVideo(type) || Helpers.IsDocument(type)) {
                        try
                        {
                            TagLib.File f = TagLib.File.Create(file);

                            type = f.MimeType.Split(@"/")[1];

                            if (type != "taglib/ini")
                            {
                                string artists = f.Tag.JoinedPerformers != null ? f.Tag.JoinedPerformers : "";
                                string album = f.Tag.Album != null ? f.Tag.Album : "";

                                SaveMediaFile(file,
                                        RemoveChars.Replace(type, "").Trim(),
                                        RemoveChars.Replace(artists, "").Trim(),
                                        RemoveChars.Replace(album, "").Trim(),
                                        year
                                        );
                            }
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                SaveFile(file, RemoveChars.Replace(type, "").Trim(), year);
                            }

                            catch (Exception expt)
                            {
                                Console.WriteLine(expt.Message);
                                continue;
                            }
                            Console.WriteLine(e.Message);
                            continue;
                        }
                    } else {
                          try
                            {
                                SaveFile(file, RemoveChars.Replace(type, ""), year);
                            }

                            catch (Exception expt)
                            {
                                Console.WriteLine(expt.Message);
                                continue;
                            }
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

        static void SaveMediaFile (string file, string type, string artist, string album, int year)
        {
            string fileName = file.Split('.')[0];

            if (fileName.Contains(@"\"))
            {
                int index = fileName.LastIndexOf(@"\");
                fileName = fileName.Substring(index);
            }

            string folder = @"\Media\";

            if (Helpers.IsAudio(type)) {
                folder = @"\Music\";
            } else if (Helpers.IsImage(type)) {
                folder = @"\Pictures\";
            } else if (Helpers.IsVideo(type)) {
                folder = @"\Movies\";          
            }
            
            string basePath = output + folder + @"\" + artist + @"\" + album + @"\" + year + @"\";

            string path = basePath + fileName + "." + type.ToLower();
            Directory.CreateDirectory(basePath);

            string checkPath = path.Contains("-") ? path.Split("-")[0] + "."  + type.ToLower() : path;

            if (File.Exists(checkPath)) {
                /* This is indeed a doublet */
                if (Helpers.IsExactSameSize(file, checkPath)) {
                    string doubletBasePath = output + @"\Doublets\" + @"\";
                    Directory.CreateDirectory(doubletBasePath);
                    Console.WriteLine("File already exists: " + path);
                    path = doubletBasePath + fileName + "-" + (!fileName.Contains("-DUBLET-") ? ("DUBLET" + "-" + DateTime.Now.Ticks) : "") + "." + type.ToLower();
                } 
                /* This is - after all - not a doublet - run roll back */
                else {
                    if (fileName.Contains("-DUBLET-")) {
                        string dubby = path.Replace("-DUBLET-", "-");
                        dubby = Helpers.RemoveDateSuffix(path);
                        if (!File.Exists(dubby)) {
                            path = dubby;
                        }                        
                    } else if (File.Exists(path)) {
                        path = basePath + fileName + "-" + DateTime.Now.Ticks + "." + type.ToLower();
                    } else {
                        string uha = Helpers.RemoveDateSuffix(path);
                        if (!File.Exists(uha)) {
                            path = uha;
                        } 
                    }
                }
 
            }
            /* This is - after all - not a doublet - run roll back */
            else if (fileName.Contains("-DUBLET-")) {
                string dubby = path.Replace("-DUBLET-", "-");
                dubby = Helpers.RemoveDateSuffix(path);
                if (!File.Exists(dubby)) {
                    path = dubby;
                } 
            }

            File.Move(file, path, false);

            Console.WriteLine(path);
        }

        static void SaveFile(string file, string type, int year)
        {
            string prePath = @"\Other\";
            string author;

            try {
                author = Helpers.GetAuthor(file);
            } catch {
                author = "";
            }

            DateTime modified = File.GetLastWriteTime(file);

            string fileName = file.Substring(0, file.LastIndexOf("."));
            // string type = file.Substring(file.LastIndexOf(".") + 1);

            if (fileName.Contains(@"\"))
            {
                int index = fileName.LastIndexOf(@"\");
                fileName = fileName.Substring(index);
            }
            
            if (Helpers.IsBook(type))
            {
                prePath = @"\Books\";
            }
            else if (Helpers.IsImage(type))
            {
                prePath = @"\Pictures\";
            }
            else if (Helpers.IsAudio(type))
            {
                prePath = @"\Music\";
            }
            else if (Helpers.IsVideo(type))
            {
                prePath = @"\Movies\";
            }
            else if (Helpers.IsDocument(type))
            {
                prePath = @"\Documents\";
            }
            else if (Helpers.IsDisc(type))
            {
                prePath = @"\Discs\";
            }
            else if (Helpers.IsFont(type))
            {
                prePath = @"\Fonts\";
            }
            else if (Helpers.IsArchive(type))
            {
                prePath = @"\Archives\";
            }
            else if (Helpers.IsGraphics(type))
            {
                prePath = @"\Graphics\";
            }

            string basePath = output + @"\" + prePath + @"\" + (author.Length > 0 ? (author + @"\") : "") + year + @"\";
            string path = basePath + fileName + "." + type.ToLower();
            Directory.CreateDirectory(basePath);

            string checkPath = path.Contains("-") ? path.Split("-")[0] + "."  + type.ToLower() : path;

            if (File.Exists(checkPath)) {
                /* This is indeed a doublet */
                if (Helpers.IsExactSameSize(file, checkPath)) {
                    string doubletBasePath = output + @"\Doublets\" + @"\";
                    Directory.CreateDirectory(doubletBasePath);
                    Console.WriteLine("File already exists: " + path);
                    path = doubletBasePath + fileName + "-" + (!fileName.Contains("-DUBLET-") ? ("DUBLET" + "-" + DateTime.Now.Ticks) : "") + "." + type.ToLower();
                } 
                /* This is - after all - not a doublet - run roll back */
                else {
                    if (fileName.Contains("-DUBLET-")) {
                        string dubby = path.Replace("-DUBLET-", "-");
                        dubby = Helpers.RemoveDateSuffix(path);
                        if (!File.Exists(dubby)) {
                            path = dubby;
                        }                        
                    } else if (File.Exists(path)) {
                        path = basePath + fileName + "-" + DateTime.Now.Ticks + "." + type.ToLower();
                    } else {
                        string uha = Helpers.RemoveDateSuffix(path);
                        if (!File.Exists(uha)) {
                            path = uha;
                        } 
                    }
                }
 
            }
            /* This is - after all - not a doublet - run roll back */
            else if (fileName.Contains("-DUBLET-")) {
                string dubby = path.Replace("-DUBLET-", "-");
                dubby = Helpers.RemoveDateSuffix(path);
                if (!File.Exists(dubby)) {
                    path = dubby;
                } 
            }

            File.Move(file, path, false);

            Console.WriteLine(path);
        }      

    }
}
