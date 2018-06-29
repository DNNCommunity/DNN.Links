using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DotNetNuke.Data;
using DotNetNuke.Entities.Tabs;

namespace DotNetNuke.Modules.Links.Components
{
    /// <summary>
    /// Provides data access to the links in the database
    /// </summary>
    public static class LinkController
    {
        /// <summary>
        /// Save a new link in the database
        /// </summary>
        /// <param name="link">The link info object to be saved</param>
        public static void AddLink(Link link)
        {
            using (var ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Link>();
                rep.Insert(link);
            }
        }

        /// <summary>
        /// Removed a link in the database
        /// </summary>
        /// <param name="itemId">The item ID of the link to delete</param>
        /// <param name="moduleId">The moduleID of the link to delete</param>
        public static void DeleteLink(int itemId, int moduleId)
        {
            using (var ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Link>();
                rep.Delete("WHERE ItemID = @0 AND ModuleID = @1", itemId, moduleId);
            }
        }

        /// <summary>
        /// Update an existing link in the database
        /// </summary>
        /// <param name="link">The link info object</param>
        public static void UpdateLink(Link link)
        {
            using (var ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Link>();
                rep.Update(link);
            }
        }
        
        /// <summary>
        /// Evaluates the targets content and tries to build a summary about the targets title and description
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <returns>A short summary of the content</returns>
        /// <history>
        /// [alex] 9/1/2009 Implementing fontionality
        /// </history>
        public static TargetInfo GetTargetContent(string url)
        {
            // verify passed arguments
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("URL", "Necessary parameter for reading content from a target is missing. Execution aborted.");
            }

            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                // use proxy if defined in host settings
                if (!string.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.ProxyServer))
                {
                    int proxyPort = DotNetNuke.Entities.Host.Host.ProxyPort;                    
                    System.Net.WebProxy proxy = new System.Net.WebProxy(DotNetNuke.Entities.Host.Host.ProxyServer, proxyPort);
                    webClient.Proxy = proxy;
                }

                System.IO.Stream contentStream = webClient.OpenRead(url);
                System.IO.StreamReader contentReader = new System.IO.StreamReader(contentStream, true);

                string contentInformation = string.Empty;
                bool endTagFound = false;
                bool startTagFound = false;

                TargetInfo targetInfo = new TargetInfo(url);

                while (!contentReader.EndOfStream)
                {
                    var currentContent = contentReader.ReadLine();

                    if (currentContent.Contains("<head"))
                    {
                        // start recording information
                        startTagFound = true;
                    }

                    if (startTagFound)
                    {
                        contentInformation += currentContent;
                    }

                    if (contentInformation.Contains("</head"))
                    {
                        // stop recording information and abort streaming after content evaluation
                        endTagFound = true;
                    }

                    if (startTagFound && endTagFound)
                    {
                        // evaluate targets content
                        if (contentInformation.ToLower().Contains("<meta") && contentInformation.ToLower().Contains("name=\"description\""))
                        {
                            // iterate through meta tags and try to find the description meta element
                            string tmpContentInformation = contentInformation;
                            while (tmpContentInformation.ToLower().Contains("<meta"))
                            {
                                string meta = tmpContentInformation.Remove(0, tmpContentInformation.IndexOf("<meta"));
                                meta = meta.Remove(meta.IndexOf(">"));

                                if (meta.ToLower().Contains("name=\"description\""))
                                {
                                    targetInfo.Description = meta.Remove(0, meta.ToLower().IndexOf("content=\"") + 9);
                                    targetInfo.Description = targetInfo.Description.Remove(targetInfo.Description.IndexOf("\""));
                                    targetInfo.Description = WebUtility.HtmlDecode(targetInfo.Description.Trim());
                                }

                                tmpContentInformation = tmpContentInformation.Remove(tmpContentInformation.IndexOf("<meta"), +5);
                            }
                        }

                        if (contentInformation.ToLower().Contains("<title>") && contentInformation.ToLower().Contains("</title>"))
                        {
                            // try to get targets title
                            targetInfo.Title = contentInformation.Substring(contentInformation.IndexOf("<title>") + 7);
                            targetInfo.Title = targetInfo.Title.Remove(targetInfo.Title.IndexOf("</title>"));
                            targetInfo.Title = WebUtility.HtmlDecode(targetInfo.Title).Trim();
                        }

                        break;
                    }
                }

                // clean up memory
                contentReader.Close();
                contentReader.Dispose();
                contentStream.Close();
                contentStream.Dispose();

                return targetInfo;
            }
        }

        public static Link RefreshLinkContent(Link objLink)
        {
            if (DateTime.Now.Subtract(new TimeSpan(0, objLink.RefreshInterval, 0)) >= objLink.CreatedDate)
            {
                var targetInfo = GetTargetContent(objLink.Url);

                objLink.Title = targetInfo.Title;
                objLink.Description = targetInfo.Description;
                objLink.CreatedDate = DateTime.Now;

                UpdateLink(objLink);
            }

            return objLink;
        }

        public static int GetLinkByHigherViewOrder(int viewOrder, int moduleId)
        {
            int prevOrder = int.MinValue;

            foreach (var link in GetLinks(moduleId))
            {
                if (link.ViewOrder > prevOrder & link.ViewOrder < viewOrder)
                {
                    prevOrder = link.ViewOrder;
                }
            }

            if (prevOrder != int.MinValue)
            {
                return prevOrder;
            }
            else
            {
                return viewOrder;
            }
        }

        /// <summary>
        /// Updates the view order of a link up or down, use negative increaseValue to move down
        /// </summary>
        /// <param name="objLink">The link object</param>
        /// <param name="increaseValue">how many places to move up or down (use negative numbers to move down)</param>
        /// <param name="moduleId">The module ID</param>
        public static void UpdateViewOrder(Link objLink, int increaseValue, int moduleId)
        {
            foreach (var link in GetLinks(moduleId))
            {
                if (increaseValue == -1 && link.ViewOrder <= objLink.ViewOrder && link.ItemId != objLink.ItemId)
                {
                    link.ViewOrder += increaseValue;
                }
                else if (increaseValue == 1 && link.ViewOrder >= objLink.ViewOrder && link.ItemId != objLink.ItemId)
                {
                    link.ViewOrder += increaseValue;
                }

                UpdateLink(link);
            }
        }

        /// <summary>
        /// Gets a single link bye the itemID and ModuleID
        /// </summary>
        /// <param name="itemID">The ID of the item</param>
        /// <param name="moduleId">The module ID</param>
        /// <returns>Link info object</returns>
        public static Link GetLink(int itemID, int moduleId)
        {
            using (var ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Link>(System.Data.CommandType.StoredProcedure, "dnnLinks_GetLink", itemID, moduleId).FirstOrDefault();
            }                
        }

        public static IEnumerable<Link> GetLinks(int moduleId)
        {
            using (var ctx = DataContext.Instance())
            {
                return ctx.ExecuteQuery<Link>(System.Data.CommandType.StoredProcedure, "dnnLinks_GetLinks", moduleId);
            }
        }

        public static string ConvertUrlType(TabType tabType)
        {
            switch (tabType)
            {
                case DotNetNuke.Entities.Tabs.TabType.File:
                    {
                        return "F";
                    }

                case DotNetNuke.Entities.Tabs.TabType.Member:
                    {
                        return "M";
                    }

                case DotNetNuke.Entities.Tabs.TabType.Normal:
                case DotNetNuke.Entities.Tabs.TabType.Tab:
                    {
                        return "T";
                    }

                default:
                    {
                        return "U";
                    }
            }
        }

        public static void DeleteLinkIfItExistsForModule(int moduleId, Link link)
        {
            foreach (var oldLink in GetLinks(moduleId))
            {
                if (oldLink.Title == link.Title && oldLink.Url == link.Url)
                {
                    DeleteLink(link.ItemId, moduleId);
                }
            }
        }
    }
}
