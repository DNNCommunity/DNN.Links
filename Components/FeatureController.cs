using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;

namespace DotNetNuke.Modules.Links.Components
{
    /// <summary>
    /// Modules interface implementations
    /// </summary>
    public class FeatureController : ModuleSearchBase, IPortable
    {
        /// <summary>
        /// Exports the module (Implements the IPortable interface)
        /// </summary>
        /// <param name="moduleID">The module ID</param>
        /// <returns>XML String of the module data</returns>
        public string ExportModule(int moduleID)
        {
            StringBuilder xml = new StringBuilder();
            var links = LinkController.GetLinks(moduleID);
            if (links.Count() != 0)
            {
                var module = ModuleController.Instance.GetModule(moduleID, DotNetNuke.Common.Utilities.Null.NullInteger, false);
                xml.Append("<links>");                
                foreach (var link in links)
                {
                    xml.Append("<link>");
                    xml.AppendFormat("<title>{0}</title>", XmlUtils.XMLEncode(link.Title));
                    xml.AppendFormat("<url>{0}</url>", XmlUtils.XMLEncode(link.Url));
                    xml.AppendFormat("<vieworder>{0}</vieworder>", XmlUtils.XMLEncode(link.ViewOrder.ToString()));
                    xml.AppendFormat("<description>{0}</description>", XmlUtils.XMLEncode(link.Description));
                    xml.AppendFormat("<newwindow>{0}</newwindow>", XmlUtils.XMLEncode(link.NewWindow.ToString()));
                    xml.AppendFormat("<trackclicks>{0}</trackclicks>", XmlUtils.XMLEncode(link.TrackClicks.ToString()));
                    xml.Append("</link>");
                }

                xml.Append("</links>");
            }

            return xml.ToString();
        }
        
        /// <summary>
        /// Imports xml to fill the module data
        /// </summary>
        /// <param name="moduleID">The module ID importing</param>
        /// <param name="content">The data representation to import in an XML string</param>
        /// <param name="version">The version of the export</param>
        /// <param name="userId">The user ID of the user importing the data</param>
        public void ImportModule(int moduleID, string content, string version, int userId)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            var xmlLinks = xmlDoc.SelectNodes("links");
            foreach (XmlNode xmlLink in xmlLinks)
            {
                int viewOrder = int.TryParse(xmlLink.SelectSingleNode("vieworder").Value, out viewOrder) ? viewOrder : 0;
                bool newWindow = bool.TryParse(xmlLink.SelectSingleNode("newwindow").Value, out newWindow) ? newWindow : false;
                Link link = new Link
                {
                    ModuleId = moduleID,
                    Title = xmlLink.SelectSingleNode("title").Value,
                    Url = DotNetNuke.Common.Globals.ImportUrl(moduleID, xmlLink.SelectSingleNode("url").Value),
                    ViewOrder = viewOrder,
                    Description = xmlLink.SelectSingleNode("description").Value
                };

                link.NewWindow = newWindow;
                
                try
                {
                    link.TrackClicks = bool.Parse(xmlLink.SelectSingleNode("trackclicks").Value);
                }
                catch
                {
                    link.TrackClicks = false;
                }

                link.CreatedDate = DateTime.Now;
                link.CreatedByUser = userId;
                LinkController.DeleteLinkIfItExistsForModule(moduleID, link);
                LinkController.AddLink(link);

                // url tracking
                UrlController objUrls = new UrlController();
                var moduleInfo = ModuleController.Instance.GetModule(moduleID, Null.NullInteger, false);
                objUrls.UpdateUrl(
                    moduleInfo.PortalID, 
                    link.Url,
                    LinkController.ConvertUrlType(DotNetNuke.Common.Globals.GetURLType(link.Url)), 
                    false,
                    link.TrackClicks, 
                    moduleID, 
                    link.NewWindow);
            }
        }

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDateUtc)
        {
            // TODO: Would be better performing if we had a last modified date and soft deletes
            DotNetNuke.Services.Search.Internals.InternalSearchController.Instance.DeleteSearchDocumentsByModule(moduleInfo.PortalID, moduleInfo.ModuleID, moduleInfo.ModuleDefID);
            List<SearchDocument> searchDocuments = new List<SearchDocument>();
            var links = LinkController.GetLinks(moduleInfo.ModuleID);
            foreach (var link in links)
            {
                var searchDoc = new SearchDocument
                {
                    UniqueKey = moduleInfo.ModuleID.ToString(),
                    PortalId = moduleInfo.PortalID,
                    Title = link.Title,
                    Description = link.Description,
                    Body = link.Description,
                    ModifiedTimeUtc = link.CreatedDate
                };
                searchDocuments.Add(searchDoc);
            }

            return searchDocuments;
        }
    }
}
