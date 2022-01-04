using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using DocumentFormat.OpenXml.Packaging;
using Novacode;

namespace MrCleanUp
{
    class Helpers
    {
        public static bool CheckFileType(string type) {
            return (
                IsBook(type) ||
                IsDocument(type) ||
                IsAudio(type) ||
                IsDisc(type) ||
                IsFont(type) ||
                IsImage(type) ||
                IsVideo(type) ||
                IsArchive(type) ||
                IsGraphics(type)
            );
        }

        public static bool IsBook(string type) {
            if (type.ToLower() == "pdf" || 
                type.ToLower() == "epub" || 
                type.ToLower() == "mobi") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsDocument(string type) {
            if (type.ToLower() == "doc" || 
                type.ToLower() == "docx" || 
                type.ToLower() == "txt" || 
                type.ToLower() == "rtf" ||
                type.ToLower() == "odt" || 
                type.ToLower() == "xls" || 
                type.ToLower() == "xlsx" ||
                type.ToLower() == "xlsm" ||
                type.ToLower() == "pps" ||
                type.ToLower() == "pptx" ||
                type.ToLower() == "ods" ||
                type.ToLower() == "pub" ||
                type.ToLower() == "xps") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsImage(string type) {
            if (type.ToLower() == "jpg" || 
                type.ToLower() == "png" || 
                type.ToLower() == "gif" ||
                type.ToLower() == "jpeg" ||
                type.ToLower() == "bmp" ||  
                type.ToLower() == "tif" ||                   
                type.ToLower() == "tiff" 
                ) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsAudio(string type) {
            if (type.ToLower() == "wav" || 
                type.ToLower() == "mp3" || 
                type.ToLower() == "wma" || 
                type.ToLower() == "ogg") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsVideo(string type) {
            if (type.ToLower() == "flv" ||
                type.ToLower() == "mkv" ||
                type.ToLower() == "ogv" ||  
                type.ToLower() == "mov" ||         
                type.ToLower() == "mpeg" ||
                type.ToLower() == "mpg" ||
                type.ToLower() == "wmv" ||
                type.ToLower() == "3gp" ||
                type.ToLower() == "flac" ||
                type.ToLower() == "avi" ||
                type.ToLower() == "m4a" ||
                type.ToLower() == "mp4") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsDisc(string type) {
            if (type.ToLower() == "vcd" ||
                type.ToLower() == "bin" ||
                type.ToLower() == "cue" ||
                type.ToLower() == "daa" ||
                type.ToLower() == "iso"
                ) {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsFont(string type) {
            if (type.ToLower() == "eot" ||
                type.ToLower() == "ttf" ||
                type.ToLower() == "woff" ||
                type.ToLower() == "woff2" ||
                type.ToLower() == "otf") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsArchive(string type) {
            if (type.ToLower() == "zip" ||
                type.ToLower() == "rar") {
                return true;
            } else {
                return false;
            }
        }

        public static bool IsGraphics(string type) {
            if (type.ToLower() == "psd" ||
                type.ToLower() == "eps" ||
                type.ToLower() == "svg" ||
                type.ToLower() == "svgz" ||
                type.ToLower() == "indd" ||
                type.ToLower() == "ai" ||
                type.ToLower() == "xcf") {
                return true;
            } else {
                return false;
            }
        }

        public static string GetAuthor(string path)
        {
            try {
                return GetAuthorFromMSOffice(path);
            } catch {
                return GetAuthorFromOtherFileType(path);
            }
        }

        private static string GetAuthorFromMSOffice(string path)
        {
            DocX doc = DocX.Load(path);
            
            var props = doc.CoreProperties;

            return props["dc:creator"].Replace("|", "");            
        }

        private static string GetAuthorFromOtherFileType(string path)
        {
             var security = new FileSecurity(path, 
                AccessControlSections.Owner | 
                AccessControlSections.Group |
                AccessControlSections.Access);

            var owner = security.GetOwner(typeof(NTAccount)).ToString();

            if (owner.Contains(@"\")) {
                owner = owner.Split(@"\")[1];
            }

            return owner.Substring(0, 1).ToUpper() + owner.Substring(1).ToLower();
        }

        public static string GetAuthor2(string documentPath)
        {
            try {
                using (var document = WordprocessingDocument.Open(documentPath, false))
                {
                    return document.PackageProperties.Creator;
                }
            } catch {
                return "";
            }
        }

        public static bool IsExactSameSize(string file1, string file2)
        {
            return new FileInfo(file1).Length ==  new FileInfo(file2).Length;
        }

        public static string RemoveDateSuffix(string ib) {
            ib = ib.Replace("-", "").Trim();

            while (DateSuffixCheck(ib)) {
                string type = ib.Substring(ib.LastIndexOf("."));
                ib = ib.Substring(0, ib.Length - (18 + type.Length)) + type;
            }

            return ib;
        }

        private static bool DateSuffixCheck(string lilleIb) {
            if (lilleIb.Length >= 18) {
                string bo = lilleIb.Substring(0, lilleIb.LastIndexOf("."));
                string tom = bo.Substring(bo.Length - 18, 18);
                long.TryParse(tom, out long numberOfTicks);
                return new DateTime(numberOfTicks).Year > 2015;
            }

            return false;                                 
        }
    }
}