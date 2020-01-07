using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
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
                var portalId = module?.PortalID ?? Null.NullInteger;
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
                    xml.AppendFormat("<logactivity>{0}</logactivity>", XmlUtils.XMLEncode(link.LogActivity.ToString()));
                    xml.AppendFormat("<refreshinterval>{0}</refreshinterval>", XmlUtils.XMLEncode(link.RefreshInterval.ToString()));
                    xml.AppendFormat("<grantroles>{0}</grantroles>", XmlUtils.XMLEncode(ConvertToRoleNames(portalId, link.GrantRoles)));
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
            var module = ModuleController.Instance.GetModule(moduleID, DotNetNuke.Common.Utilities.Null.NullInteger, false);
            var portalId = module?.PortalID ?? Null.NullInteger;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            var xmlLinks = xmlDoc.SelectNodes("links/link");
            foreach (XmlNode xmlLink in xmlLinks)
            {
                int viewOrder = int.TryParse(GetXmlNodeValue(xmlLink ,"vieworder"), out viewOrder) ? viewOrder : 0;
                bool newWindow = bool.TryParse(GetXmlNodeValue(xmlLink, "newwindow"), out newWindow) ? newWindow : false;
                Link link = new Link
                {
                    ModuleId = moduleID,
                    Title = GetXmlNodeValue(xmlLink, "title"),
                    Url = DotNetNuke.Common.Globals.ImportUrl(moduleID, GetXmlNodeValue(xmlLink, "url")),
                    ViewOrder = viewOrder,
                    Description = GetXmlNodeValue(xmlLink, "description"),
                    GrantRoles = ConvertToRoleIds(portalId, GetXmlNodeValue(xmlLink, "grantroles")),
                };

                link.NewWindow = newWindow;

                if (bool.TryParse(GetXmlNodeValue(xmlLink, "trackclicks"), out bool trackClicks))
                {
                    link.TrackClicks = trackClicks;
                }
                if (bool.TryParse(GetXmlNodeValue(xmlLink, "logactivity"), out bool logActivity))
                {
                    link.LogActivity = logActivity;
                }

                if (int.TryParse(GetXmlNodeValue(xmlLink, "refreshinterval"), out int refreshInterval))
                {
                    link.RefreshInterval = refreshInterval;
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
                    link.LogActivity,
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

        private string GetXmlNodeValue(XmlNode parent, string nodeName)
        {
            var node = parent.SelectSingleNode(nodeName);
            if (node == null)
            {
                return string.Empty;
            }

            return node.Value ?? node.InnerText;
        }

        private string ConvertToRoleNames(int portalId, string grantRoles)
        {
            if (Null.IsNull(portalId) || string.IsNullOrEmpty(grantRoles))
            {
                return string.Empty;
            }

            var roles = string.Empty;
            foreach (var roleId in grantRoles.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt32(i)))
            {
                var role = RoleController.Instance.GetRoleById(portalId, roleId);
                if (role != null)
                {
                    roles += $"{role.RoleName};";
                }
            }

            if (!string.IsNullOrWhiteSpace(roles) && !roles.StartsWith(";"))
            {
                roles = $";{roles}";
            }

            return roles;
        }

        private string ConvertToRoleIds(int portalId, string roleNames)
        {
            if (Null.IsNull(portalId) || string.IsNullOrEmpty(roleNames))
            {
                return string.Empty;
            }

            var roles = string.Empty;
            foreach (var roleName in roleNames.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var role = RoleController.Instance.GetRoleByName(portalId, roleName);
                if (role != null)
                {
                    roles += $"{role.RoleID};";
                }
            }

            if (!string.IsNullOrWhiteSpace(roles) && !roles.StartsWith(";"))
            {
                roles = $";{roles}";
            }

            return roles;
        }
    }
}
