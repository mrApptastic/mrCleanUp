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
                IsVideo(type)
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
                type.ToLower() == "wmv") {
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


    }
}