using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.Links
{
    public class Utils
    {
        public static string GetImageURL(string FileExtension)
        {
            string result = string.Empty;
            string baseURL = "~/images/FileManager/Icons/";

            switch (FileExtension)
            {
                case "bmp":
                    {
                        result = baseURL + "bmp.gif";
                        break;
                    }

                case "avi":
                    {
                        result = baseURL + "avi.gif";
                        break;
                    }

                case "doc":
                case "docx":
                    {
                        result = baseURL + "doc.gif";
                        break;
                    }

                case "exe":
                    {
                        result = baseURL + "exe.gif";
                        break;
                    }

                case "gif":
                    {
                        result = baseURL + "gif.gif";
                        break;
                    }

                case "html":
                case "htm":
                    {
                        result = baseURL + "htm.gif";
                        break;
                    }

                case "jpg":
                    {
                        result = baseURL + "jpg.gif";
                        break;
                    }

                case "js":
                    {
                        result = baseURL + "js.gif";
                        break;
                    }

                case "move":
                    {
                        result = baseURL + "move.gif";
                        break;
                    }

                case "mp3":
                    {
                        result = baseURL + "mp3.gif";
                        break;
                    }

                case "mpeg":
                case "mpg":
                    {
                        result = baseURL + "mpeg.gif";
                        break;
                    }

                case "pdf":
                    {
                        result = baseURL + "pdf.gif";
                        break;
                    }

                case "ppt":
                case "pptx":
                    {
                        result = baseURL + "ppt.gif";
                        break;
                    }

                case "tif":
                    {
                        result = baseURL + "tif.gif";
                        break;
                    }

                case "txt":
                    {
                        result = baseURL + "txt.gif";
                        break;
                    }

                case "vb":
                case "vbs":
                    {
                        result = baseURL + "vb.gif";
                        break;
                    }

                case "wav":
                    {
                        result = baseURL + "wav.gif";
                        break;
                    }

                case "xls":
                case "xlsx":
                    {
                        result = baseURL + "xls.gif";
                        break;
                    }

                case "xml":
                    {
                        result = baseURL + "xml.gif";
                        break;
                    }

                case "zip":
                    {
                        result = baseURL + "zip.gif";
                        break;
                    }

                default:
                    {
                        result = baseURL + "txt.gif";
                        break;
                    }
            }

            return result;
        }

        public static string GetFileSizeString(int FileSize)
        {
            string result = string.Empty;

            if (FileSize < 100)
                result = System.Convert.ToString(FileSize) + " B";
            else if (FileSize >= 100 & FileSize < 100000)
                result = (FileSize / (double)1000).ToString("0.0") + " KB";
            else if (FileSize >= 100000)
                result = (FileSize / (double)1000000).ToString("0.0") + " MB";

            return result;
        }
    }
}
