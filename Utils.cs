// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links
{
    /// <summary>
    /// Utilities for the Links module.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="fileExtension">The file extension for the image.</param>
        /// <returns>A string representing the url to the image.</returns>
        public static string GetImageURL(string fileExtension)
        {
            string result = string.Empty;
            string baseURL = "~/images/FileManager/Icons/";

            switch (fileExtension)
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

        /// <summary>
        /// Gets a friendly string for a file size.
        /// </summary>
        /// <param name="fileSize">Size of the file.</param>
        /// <returns>A human readable file size string.</returns>
        public static string GetFileSizeString(int fileSize)
        {
            string result = string.Empty;

            if (fileSize < 100)
            {
                result = System.Convert.ToString(fileSize) + " B";
            }
            else if (fileSize >= 100 & fileSize < 100000)
            {
                result = (fileSize / 1000D).ToString("0.0") + " KB";
            }
            else if (fileSize >= 100000)
            {
                result = (fileSize / 1000000D).ToString("0.0") + " MB";
            }

            return result;
        }
    }
}
