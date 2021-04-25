// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2008
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using System.Xml;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Social;
using Telerik.Web.UI;
using DotNetNuke.Common.Utilities;
using Dnn.Links;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Modules.Links.Components;
using System.Collections;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using System.Web.UI.HtmlControls;

namespace DotNetNuke.Modules.Links
{

    /// -----------------------------------------------------------------------------
    ///     ''' <summary>
    ///     ''' The Links Class provides the UI for displaying the Links
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' </remarks>
    ///     ''' <history>
    ///     ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    ///     '''		[cnurse]	10/20/2004	Removed ViewOptions from Action menu
    ///     ''' </history>
    ///     ''' -----------------------------------------------------------------------------
    partial class Links : PortalModuleBase, IActionable
    {
        private int _FileId = Null.NullInteger;
        public int FileId
        {
            get
            {
                return _FileId;
            }
            set
            {
                _FileId = value;
            }
        }

        public Enums.ModuleContentTypes ModuleContentType
        {
            get
            {
                Enums.ModuleContentTypes result = Enums.ModuleContentTypes.Links;

                if (Settings[SettingName.ModuleContentType] != null && int.TryParse(Settings[SettingName.ModuleContentType].ToString(), out int contenttype))
                    result = (Enums.ModuleContentTypes)contenttype;

                return result;
            }
        }

        public int FolderId
        {
            get
            {
                int result = Null.NullInteger;

                if (Settings[Consts.FolderId] != null)
                    result = System.Convert.ToInt32(Settings[Consts.FolderId]);

                return result;
            }
        }

        public DotNetNuke.Services.FileSystem.FolderInfo FolderInfo
        {
            get
            {                
                if (this.FolderId != Null.NullInteger)
                {
                    return FolderManager.Instance.GetFolder(FolderId) as FolderInfo;
                }
                return null;
            }
        }

        public DotNetNuke.Security.Permissions.FolderPermissionCollection FolderPermissions
        {
            get
            {
                FolderPermissionCollection result = new FolderPermissionCollection();

                if (FolderInfo != null)
                    result = FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalId, FolderInfo.FolderPath);

                return result;
            }
        }

        public new bool IsEditable
        {
            get
            {
                switch (this.ModuleContentType)
                {
                    case Enums.ModuleContentTypes.Links:
                        {
                            if (base.IsEditable)
                                return true;
                            break;
                        }

                    case Enums.ModuleContentTypes.Menu:
                        {
                            return false;
                        }

                    case Enums.ModuleContentTypes.Folder:
                        {
                            return false;
                        }

                    case Enums.ModuleContentTypes.Friends:
                        {
                            return false;
                        }
                }
                return false;
            }
        }

        public string DisplayMode
        {
            get
            {
                string result = Consts.DisplayModeLink;

                if (Settings[SettingName.DisplayMode] != null)
                    result = Settings[SettingName.DisplayMode].ToString();

                return result;
            }
        }
        // 2014 TODO: Menu
        public string MenuAllUser
        {
            get
            {
                string result = Consts.MenuAllUser;

                if (Settings[SettingName.MenuAllUsers] != null)
                    result = Settings[SettingName.MenuAllUsers].ToString();

                return result;
            }
        }

        public string LinkDescriptionMode
        {
            get
            {
                string result = Consts.ShowLinkDescriptionNo;

                if (Settings[SettingName.LinkDescriptionMode] != null)
                    result = Settings[SettingName.LinkDescriptionMode].ToString();

                return result;
            }
        }

        public bool UsePermissions
        {
            get
            {
                bool result = false;

                if (Settings[SettingName.UsePermissions] != null)
                    result = bool.Parse(Settings[SettingName.UsePermissions].ToString());

                return result;
            }
        }



        protected string ImageFileId
        {
            get
            {
                string result = "";

                if (Settings[SettingName.Icon] != null)
                    result = Settings[SettingName.Icon].ToString();

                return result;
            }
        }




        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' DisplayInfo displays the description/info on the link
        ///         ''' </summary>
        ///         ''' <param name="strDescription"></param>
        ///         ''' <returns>The description text</returns>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         '''     [erikvb]    5/29/2008   Added strDescription parameter, inorder to prevent displaying info when description is empty
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public bool DisplayInfo(string strDescription)
        {
            bool result = false;

            try
            {
                if ((System.Convert.ToString(Settings[SettingName.LinkDescriptionMode]) == Consts.ShowLinkDescriptionYes) && (!string.IsNullOrEmpty(strDescription)))
                    result = true;
                else
                    result = false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return result;
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' DisplayToolTip gets the tooltip to display
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <param name="strDescription">The description</param>
        ///         ''' <returns>The tooltip text</returns>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' 	[bonosoft]	11/3/2004	Added default option, if option not set. (DNN-115)
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public string DisplayToolTip(string strDescription)
        {
            string result = strDescription;

            if (System.Convert.ToString(Settings[SettingName.LinkDescriptionMode]) != Consts.ShowLinkDescriptionNo)
                result = string.Empty;

            return result;
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' FormatURL correctly formats the links url
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <param name="Link">The link</param>
        ///         ''' <returns>The correctly formatted url</returns>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public string FormatURL(string Link, bool TrackClicks)
        {
            return Common.Globals.LinkClick(Link, TabId, ModuleId, TrackClicks);
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' FormatIcon correctly formats the link icon
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <returns>The correctly formatted url</returns>
        ///         ''' <history>
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public string FormatIcon(string ImageURL)
        {
            string result = string.Empty;

            switch (this.ModuleContentType)
            {
                case Enums.ModuleContentTypes.Links:
                    {
                        if (System.Convert.ToString(Settings[SettingName.Icon]) != string.Empty)
                            result = Common.Globals.LinkClick(System.Convert.ToString(Settings[SettingName.Icon]), TabId, ModuleId, false);
                        break;
                    }

                case Enums.ModuleContentTypes.Folder:
                    {
                        if (!string.IsNullOrEmpty(ImageURL))
                            result = ResolveUrl(ImageURL);
                        break;
                    }
            }

            return result;
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' DisplayIcon displays the icon
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <returns>true or false</returns>
        ///         ''' <history>
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public bool DisplayIcon
        {
            get
            {
                bool result = false;

                try
                {
                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                if (System.Convert.ToString(Settings[SettingName.Icon]) != string.Empty)
                                    result = true;
                                else
                                    result = false;
                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {
                                result = true;
                                break;
                            }
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }

                return result;
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' HtmlDecode decodes the html string
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <param name="strValue">The string to decode</param>
        ///         ''' <returns>The decoded html</returns>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public string HtmlDecode(string strValue)
        {
            string result = string.Empty;

            try
            {
                result = Server.HtmlDecode(strValue);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return result;
        }

        public string NoWrap
        {
            get
            {
                if (Settings.ContainsKey("nowrap") && Settings["nowrap"].ToString() != "W")
                    return "style=\"white-space: nowrap\"";

                return string.Empty;
            }
        }

        public string Horizontal
        {
            get
            {
                string result = string.Empty;

                if (Settings.ContainsKey(SettingName.Direction) && System.Convert.ToString(Settings[SettingName.Direction]) == Consts.DirectionHorizontal)
                    result = "Horizontal";

                return result;
            }
        }

        public string PopupTrigger
        {
            get
            {
                string result = string.Empty;

                if (this.LinkDescriptionMode == Consts.ShowLinkDescriptionJQuery)
                    result = " trigger";

                return result;
            }
        }

        public bool ShowPopup
        {
            get
            {
                bool result = false;

                if (this.LinkDescriptionMode == Consts.ShowLinkDescriptionJQuery)
                    result = true;

                return result;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += Page_Load;
        }       

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' Page_Load runs when the control is loaded
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        private void Page_Load(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (System.Convert.ToString(this.Settings[SettingName.LinkDescriptionMode]) == Consts.ShowLinkDescriptionYes)
                {
                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(Consts.ToogleJS))
                    {
                        string jspath = Page.ResolveUrl("~/DesktopModules/Links/js/toggle.js");
                        this.Page.ClientScript.RegisterClientScriptInclude(Consts.PopupJS, jspath);
                    }
                }

                if (this.ModuleContentType.Equals(Enums.ModuleContentTypes.Friends) && this.Settings[Consts.ModuleContentItem].Equals("BusinessCardMode"))
                {
                    pnlList.Visible = false;
                    pnlDropdown.Visible = false;
                }
                else if (System.Convert.ToString(Settings[SettingName.DisplayMode]) == Consts.DisplayModeDropdown)
                {
                    pnlList.Visible = false;
                    pnlDropdown.Visible = true;
                }
                else
                {
                    pnlList.Visible = true;
                    pnlDropdown.Visible = false;
                }

                if (!Page.IsPostBack)
                {                    
                    if (Request.Params[Consts.FileId] != null)
                        this.FileId = System.Convert.ToInt32(Request.Params[Consts.FileId]);

                    if (this.FileId != Null.NullInteger)
                    {
                        var fileInfo = FileManager.Instance.GetFile(FileId);

                        if (fileInfo != null)
                        {
                            FolderController folderCont = new DotNetNuke.Services.FileSystem.FolderController();

                            var folderInfo = FolderManager.Instance.GetFolder(FolderId);

                            if (folderInfo != null)
                            {
                                if (DotNetNuke.Security.Permissions.FolderPermissionController.HasFolderPermission(this.PortalId, folderInfo.FolderPath, "READ"))
                                {
                                    FileManager.Instance.WriteFileToResponse(fileInfo, ContentDisposition.Attachment);
                                }
                            }
                        }
                    }

                    DotNetNuke.Security.Roles.RoleController roleController = new DotNetNuke.Security.Roles.RoleController();

                    var roles = roleController.GetUserRoles(UserInfo, true);
                    bool isInARole = false;
                    ArrayList linksToShow = new ArrayList();

                    List<Link> links = new List<Link>();

                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                links = LinkController.GetLinks(ModuleId).ToList();

                                string imgUrl = DotNetNuke.Common.Globals.LinkClick(ImageFileId, TabId, ModuleId);
                                foreach (var link in links)
                                    link.ImageURL = imgUrl;
                                break;
                            }

                        case Enums.ModuleContentTypes.Menu:
                            {
                                List<TabInfo> tabsToShow = new List<TabInfo>();
                                if (Settings.ContainsKey(Consts.ModuleContentItem)) {
                                    string moduleContentItem = Settings[Consts.ModuleContentItem].ToString();
                                    int.TryParse(moduleContentItem, out int moduleContentItemInt);
                                    tabsToShow = TabController.GetTabsByParent(moduleContentItemInt, this.PortalId);
                                }

                                foreach (DotNetNuke.Entities.Tabs.TabInfo tabinfo in tabsToShow)
                                {

                                    // 1.0.2 - Workaround for dnn powered content localization - [alexander.zimmer] & [simon.meraner]
                                    // serialize tab object in order to be able to identify whether the content localization is active. 
                                    System.Xml.Serialization.XmlSerializer tabSerializer = new System.Xml.Serialization.XmlSerializer(typeof(DotNetNuke.Entities.Tabs.TabInfo));
                                    System.IO.StringWriter stream = new System.IO.StringWriter();
                                    tabSerializer.Serialize(stream, tabinfo);

                                    System.IO.StringReader sr = new System.IO.StringReader(stream.ToString());
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.Load(sr);

                                    stream.Close();

                                    var tabCulture = xmlDoc.SelectSingleNode("/tab/cultureCode");

                                    bool showCulture = true;

                                    if (tabCulture != null)
                                    {
                                        XmlNode cultureNode = tabCulture.FirstChild;

                                        // dnn 5.5.x detected ... exclude tabs where the cultures doesn`t matcj the current culture
                                        if (cultureNode != null && cultureNode.Value != System.Threading.Thread.CurrentThread.CurrentCulture.Name)
                                            showCulture = false;
                                    }

                                    // -------------------------------------------------------------------------------------------------------

                                    if (showCulture)
                                    {
                                        Link link = new Link();

                                        link.ModuleId = ModuleId;
                                        link.Url = DotNetNuke.Common.Globals.NavigateURL(tabinfo.TabID);
                                        link.NewWindow = false;
                                        link.Title = tabinfo.TabName;
                                        link.GrantRoles = ";";
                                        link.Description = tabinfo.Description;
                                        link.ItemId = tabinfo.TabID;
                                        // 2014 TODO: Menu
                                        if (MenuAllUser.Equals("No"))
                                        {
                                            foreach (var role in tabinfo.TabPermissions.ToList())
                                                link.GrantRoles += role.RoleID + ";";
                                        }
                                        else
                                            // -2: All Users; -1: Unauthenticated Users
                                            link.GrantRoles += "-2;";

                                        links.Add(link);
                                    }
                                }

                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {                                
                                var folder = FolderManager.Instance.GetFolder(FolderId);
                                var files = FolderManager.Instance.GetFiles(folder);

                                foreach (DotNetNuke.Services.FileSystem.FileInfo file in files)
                                {
                                    Link link = new Link();

                                    link.NewWindow = false;
                                    link.Title = file.FileName;
                                    link.ItemId = file.FileId;
                                    link.Url = DotNetNuke.Common.Globals.NavigateURL(this.TabId, string.Empty, "FileId=" + file.FileId.ToString());
                                    link.GrantRoles = ";";
                                    link.Description =  Utils.GetFileSizeString(file.Size);
                                    link.ImageURL = Utils.GetImageURL(file.Extension);

                                    foreach (var permission in this.FolderPermissions.ToList())
                                    {
                                        if (permission.AllowAccess == true && permission.PermissionKey == "READ" && permission.UserID == Null.NullInteger)
                                            link.GrantRoles += permission.RoleID + ";";
                                    }

                                    links.Add(link);
                                }

                                break;
                            }

                        case Enums.ModuleContentTypes.Friends:
                            {
                                if (this.Settings[Consts.ModuleContentItem].Equals("NormalMode"))
                                    pnlFriends.Visible = false;
                                else
                                {
                                    pnlFriends.Visible = true;
                                    pnlDropdown.Visible = false;
                                    pnlList.Visible = false;
                                }

                                // list friends
                                UserInfo currentUser = UserInfo;
                                if (currentUser != null & currentUser.UserID != -1)
                                {
                                    foreach (LinksFriend lFriend in GetFriendsSource(currentUser))
                                    {
                                        Link link = new Link();
                                        link.NewWindow = false;
                                        switch ((Enums.DisplayAttribute)(int)this.Settings[SettingName.DisplayAttribute])
                                        {
                                            case Enums.DisplayAttribute.Username:
                                                {
                                                    link.Title = lFriend.UserName;
                                                    break;
                                                }

                                            case Enums.DisplayAttribute.DisplayName:
                                                {
                                                    link.Title = lFriend.DisplayName;
                                                    break;
                                                }

                                            case Enums.DisplayAttribute.FirstName:
                                                {
                                                    link.Title = lFriend.UserFirstName;
                                                    break;
                                                }

                                            case Enums.DisplayAttribute.LastName:
                                                {
                                                    link.Title = lFriend.UserLastName;
                                                    break;
                                                }

                                            case Enums.DisplayAttribute.FullName:
                                                {
                                                    link.Title = lFriend.UserFullName;
                                                    break;
                                                }
                                        }
                                        // link.Title = lFriend.UserName
                                        link.Url = DotNetNuke.Common.Globals.LinkClick("UserID=" + lFriend.UserID, this.TabId, this.ModuleId);
                                        link.GrantRoles = ";";
                                        link.ItemId = lFriend.UserID;
                                        link.Description = "Status: " + lFriend.Status + "<br />Username: " + lFriend.UserName + "<br />Displayname: " + lFriend.DisplayName + "<br />Full Name: " + lFriend.UserFullName;
                                        links.Add(link);
                                    }
                                    lvFriends.DataSource = GetFriendsSource(currentUser);
                                    lvFriends.DataBind();
                                }
                                if ((Enums.DisplayOrder)int.Parse(Settings[SettingName.DisplayOrder].ToString()) == Enums.DisplayOrder.ASC)
                                    links = links.OrderBy(l => l.Title).ToList();
                                else
                                    links = links.OrderByDescending(l => l.Title).ToList();
                                break;
                            }
                    }

                    if (this.DisplayMode == Consts.DisplayModeDropdown)
                    {
                        if (IsEditable)
                            cmdEdit.Visible = true;
                        else
                            cmdEdit.Visible = false;

                        cmdGo.ToolTip = Localization.GetString("cmdGo");

                        if (this.LinkDescriptionMode == Consts.ShowLinkDescriptionYes)
                            cmdInfo.Visible = true;
                        else
                            cmdInfo.Visible = false;

                        cmdInfo.ToolTip = Localization.GetString("cmdInfo", this.LocalResourceFile);

                        foreach (Link link in links)
                        {
                            if (this.ModuleContentType == Enums.ModuleContentTypes.Links && this.UsePermissions)
                                link.GrantRoles += "-2;";

                            isInARole = false;

                            if (link.RefreshContent)
                                LinkController.RefreshLinkContent(link);

                            foreach (RoleInfo role in roles)
                            {
                                if (link.GrantRoles.Contains(";" + role.RoleID + ";"))
                                    isInARole = true;
                            }

                            // 
                            if (this.ModuleContentType == Enums.ModuleContentTypes.Friends)
                                isInARole = true;

                            if (!this.UsePermissions | isInARole | link.GrantRoles.Contains(";-2;") | link.GrantRoles.Contains(";-1;") | this.UserInfo.IsSuperUser | this.UserInfo.IsInRole(this.PortalSettings.AdministratorRoleName))

                                linksToShow.Add(link);
                        }

                        cboLinks.DataSource = linksToShow;

                        switch (this.ModuleContentType)
                        {
                            case Enums.ModuleContentTypes.Menu:
                                {
                                    cboLinks.DataValueField = "ItemId";
                                    break;
                                }

                            case Enums.ModuleContentTypes.Folder:
                                {
                                    cboLinks.DataValueField = "ItemId";
                                    cboLinks.DataTextField = "Title";
                                    break;
                                }

                            case Enums.ModuleContentTypes.Friends:
                                {
                                    cboLinks.DataValueField = "ItemId";
                                    break;
                                }
                        }

                        cboLinks.DataBind();
                    }
                    else
                    {
                        foreach (Link link in links)
                        {
                            if (this.ModuleContentType == Enums.ModuleContentTypes.Links)
                            {
                                if (this.UsePermissions == false)
                                    link.GrantRoles += "-2;";
                            }

                            isInARole = false;

                            if (link.RefreshContent)
                                LinkController.RefreshLinkContent(link);

                            foreach (RoleInfo role in roles)
                            {
                                if (link.GrantRoles.Contains(";" + role.RoleID + ";"))
                                    isInARole = true;
                            }
                            // 
                            if (this.ModuleContentType == Enums.ModuleContentTypes.Friends)
                                isInARole = true;

                            if (isInARole | link.GrantRoles.Contains("-2") | link.GrantRoles.Contains("-1") | this.UserInfo.IsSuperUser | this.UserInfo.IsInRole(this.PortalSettings.AdministratorRoleName))

                                linksToShow.Add(link);
                        }

                        lstLinks.DataSource = linksToShow;
                        lstLinks.DataBind();
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' cmdEdit_Click runs when the edit button is clciked
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public void cmdEdit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                if (cboLinks.SelectedItem != null)
                    Response.Redirect(EditUrl("ItemID", cboLinks.SelectedItem.Value), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' cmdGo_Click runs when the Go Button is clicked
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public void cmdGo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (cboLinks.SelectedItem != null)
                {
                    string strURL = string.Empty;

                    Link objLink = new Link();

                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                objLink = LinkController.GetLink(int.Parse(cboLinks.SelectedValue), ModuleId);
                                strURL = FormatURL(objLink.Url, objLink.TrackClicks);
                                break;
                            }

                        case Enums.ModuleContentTypes.Menu:
                            {
                                objLink = new Link();

                                DotNetNuke.Entities.Tabs.TabController tabCont = new DotNetNuke.Entities.Tabs.TabController();
                                DotNetNuke.Entities.Tabs.TabInfo tabInfo = tabCont.GetTab(int.Parse(cboLinks.SelectedItem.Value), this.PortalId, false);

                                strURL = DotNetNuke.Common.Globals.NavigateURL(tabInfo.TabID);
                                objLink.TrackClicks = false;
                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {
                                objLink = new Link();
                                strURL = DotNetNuke.Common.Globals.NavigateURL(this.TabId, string.Empty, "FileId", cboLinks.SelectedValue);
                                break;
                            }

                        case Enums.ModuleContentTypes.Friends:
                            {
                                objLink = new Link();
                                strURL = DotNetNuke.Common.Globals.LinkClick("UserID=" + int.Parse(cboLinks.SelectedItem.Value), this.TabId, this.ModuleId);
                                break;
                            }
                    }

                    if (objLink != null)
                    {
                        if (objLink.NewWindow)
                            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "OpenLink", "window.open('" + strURL + "','_blank')", true);
                        else
                            Response.Redirect(strURL, true);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' cmdInfo_Click runs when the Info (...) button is clicked
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public void cmdInfo_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (cboLinks.SelectedItem != null)
                {
                    string desc = string.Empty;

                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                Link objLink = new Link();

                                objLink = LinkController.GetLink(int.Parse(cboLinks.SelectedItem.Value), ModuleId);

                                if (objLink == null)
                                    desc = HtmlDecode(objLink.Description);
                                break;
                            }

                        case Enums.ModuleContentTypes.Menu:
                            {
                                DotNetNuke.Entities.Tabs.TabController tabCont = new DotNetNuke.Entities.Tabs.TabController();
                                DotNetNuke.Entities.Tabs.TabInfo tabInfo = tabCont.GetTab(int.Parse(cboLinks.SelectedItem.Value), this.PortalId, false);

                                if (tabInfo != null)
                                    desc = tabInfo.Description;
                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {
                                var fileInfo = FileManager.Instance.GetFile(int.Parse(cboLinks.SelectedItem.Value));

                                if (fileInfo != null)
                                    desc = Utils.GetFileSizeString(fileInfo.Size);
                                break;
                            }

                        case Enums.ModuleContentTypes.Friends:
                            {
                                UserInfo currentUser = UserInfo;
                                UserInfo friendUser = UserController.GetUserById(this.PortalId, int.Parse(cboLinks.SelectedItem.Value));
                                UserRelationship updatedRelation = RelationshipController.Instance.GetFriendRelationship(currentUser, friendUser);
                                if (updatedRelation.Status.ToString().Equals("Accepted"))
                                    desc = "Status: " + updatedRelation.Status.ToString();
                                else if (currentUser.UserID != updatedRelation.UserId)
                                    // current user not initialize
                                    desc = "Status: " + "Receive Request from " + friendUser.Username;
                                else
                                    desc = "Status: " + "Request " + updatedRelation.Status.ToString();
                                desc += "<br />Username: " + friendUser.Username + "<br />Displayname: " + friendUser.DisplayName + "<br />Full Name: " + friendUser.DisplayName;
                                break;
                            }
                    }

                    if (!string.IsNullOrEmpty(desc))
                    {
                        if (lblDescription.Text == string.Empty)
                            lblDescription.Text = desc;
                        else
                            lblDescription.Text = string.Empty;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' lstLinks_Select runs when an item in the Links list is selected
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        private void lstLinks_Select(object Sender, RepeaterCommandEventArgs e)
        {
            try
            {
                lstLinks.Items[e.Item.ItemIndex].FindControl("pnlDescription").Visible = !lstLinks.Items[e.Item.ItemIndex].FindControl("pnlDescription").Visible;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void lstLinks_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            try
            {
                var link = e.Item.DataItem as Link;

                //if (e.Item.ItemType == ListItemType.Header)
                //{
                //    var header = e.Item.FindControl("ulHeader") as HtmlControl;
                //    header.Attributes["class"] += " " + Horizontal;
                //}
                if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HtmlAnchor linkHyp = (HtmlAnchor)e.Item.FindControl("linkHyp");
                    Label lblMoreInfo = (Label)e.Item.FindControl("lblMoreInfo");
                    Panel pnlDescription = (Panel)e.Item.FindControl("pnlDescription");
                    var lbldescrdiv = e.Item.FindControl("lbldescrdiv") as Label;
                    var spnSelect = e.Item.FindControl("spnSelect");
                    var radToolTip = e.Item.FindControl("radToolTip") as RadToolTip;
                    var editLink = e.Item.FindControl("editLink") as HyperLink;

                    lblMoreInfo.Attributes.Add("onclick", "toggleVisibility('" + pnlDescription.ClientID + "')");
                    lblMoreInfo.Attributes.Add("style", "cursor: pointer;");
                    lblMoreInfo.Visible = !ShowPopup;

                    linkHyp.HRef = FormatURL(link.Url, link.TrackClicks);
                    linkHyp.Target = link.NewWindow ? "_blank" : "_self";

                    lbldescrdiv.Text = HtmlDecode(link.Description);

                    spnSelect.Visible = DisplayInfo(link.Description);

                    radToolTip.Visible = (ShowPopup && link.Description != "");

                    editLink.NavigateUrl = EditUrl("ItemId", link.ItemId.ToString());
                }
            }

            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' Page_Unload runs when the page is unloaded. It is used to solve caching problems with the core that cause certain items to not work.
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[awhittington]	01/04/2007	Added Page_Unload
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        private void Page_Unload(object sender, System.EventArgs e)
        {
            // Should no longer bee needed, but kept as a comment just in case:

            //if (DisplayMode == Consts.DisplayModeDropdown | DisplayMode == "Y")

            //    DataCache.RemoveCache(ModuleController.CacheKey(TabModuleId));

            //if (!IsPostBack)
            //    DataCache.RemoveCache(ModuleController.CacheKey(TabModuleId));
        }


        protected void btnRemoveFriend_OnCommand(object sender, CommandEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.CommandName))
            {
                UserInfo selectUser = UserController.GetUserById(this.PortalId, Convert.ToInt16(e.CommandName));
                // remove friend
                UserInfo currentUser = UserInfo;
                try
                {
                    // beta api
                    // RelationshipController.Instance.DeleteFriend(currentUser, selectUser)
                    FriendsController.Instance.DeleteFriend(currentUser, selectUser);
                    // update friends list
                    lvFriends.DataSource = GetFriendsSource(currentUser);
                    lvFriends.DataBind();
                }
                catch (Exception ex)
                {
                    Exceptions.ProcessModuleLoadException(this, ex);
                }
            }
        }

        protected void btnAcceptFriendRequest_OnCommand(object sender, CommandEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.CommandName))
            {
                try
                {
                    UserInfo currentUser = UserInfo;
                    // accept friend request
                    RelationshipController.Instance.AcceptUserRelationship(Convert.ToInt32(e.CommandName));
                    // update friends list
                    lvFriends.DataSource = GetFriendsSource(currentUser);
                    lvFriends.DataBind();
                    // refresh
                    Page.Response.Redirect(Page.Request.Url.ToString(), true);
                }
                catch (Exception ex)
                {
                    Exceptions.ProcessModuleLoadException(this, ex);
                }
            }
        }

        protected void cmbPageSize_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            lvFriends.PageSize = int.Parse(e.Value);
            lvFriends.CurrentPageIndex = 0;
            lvFriends.Rebind();
        }

        public string RedirectUserProfile(int userId)
        {
            string profileUrl = DotNetNuke.Common.Globals.LinkClick("UserID=" + userId, this.TabId, this.ModuleId);
            return "window.location='" + profileUrl + "'";
        }

        public bool MakeVisible(string status)
        {
            if (status.ToString().Equals("Accepted"))
                return true;
            else
                return false;
        }

        public bool MakeAcceptFriendRequestVisible(string status, int userId)
        {
            if (status.ToString().Equals("Accepted"))
                return false;
            else
            {
                UserInfo currentUser = UserInfo;
                UserInfo relationUser = UserController.GetUserById(this.PortalId, userId);
                UserRelationship relationship = RelationshipController.Instance.GetFriendRelationship(currentUser, relationUser);
                if (currentUser.UserID != relationship.UserId)
                    // current user not initialize
                    return true;
                else
                    return false;
            }
        }


        public ArrayList GetFriendsSource(UserInfo currentUser)
        {
            ArrayList friendSourceList = new ArrayList();
            // get friends
            // beta api
            // Dim relationshipList As List(Of UserRelationship) = RelationshipController.Instance.GetFriends(currentUser)
            var relationshipList = RelationshipController.Instance.GetUserRelationships(currentUser);
            if (relationshipList != null)
            {
                foreach (UserRelationship relation in relationshipList)
                {
                    UserInfo friendInfo;
                    // check who makes request 
                    if (currentUser.UserID == relation.UserId)
                        friendInfo = UserController.GetUserById(this.PortalId, relation.RelatedUserId);
                    else
                        friendInfo = UserController.GetUserById(this.PortalId, relation.UserId);
                    // get updated relationship
                    UserRelationship updatedRelation = RelationshipController.Instance.GetFriendRelationship(currentUser, friendInfo);
                    if (updatedRelation != null)
                    {
                        LinksFriend friendSource = new LinksFriend();
                        {
                            var withBlock = friendSource;
                            withBlock.UserID = friendInfo.UserID;
                            withBlock.PhotoUrl = friendInfo.Profile.PhotoURL;
                            withBlock.UserName = friendInfo.Username;
                            withBlock.DisplayName = friendInfo.DisplayName;
                            withBlock.UserFirstName = friendInfo.FirstName;
                            withBlock.UserLastName = friendInfo.LastName;
                            withBlock.UserFullName = friendInfo.DisplayName;
                            withBlock.UserRelationshipID = relation.UserRelationshipId;
                        }
                        if (updatedRelation.Status.ToString().Equals("Accepted"))
                            friendSource.Status = updatedRelation.Status.ToString();
                        else if (currentUser.UserID != relation.UserId)
                            // current user not initialize
                            friendSource.Status = "Receive Request from " + friendInfo.Username;
                        else
                            friendSource.Status = "Request " + updatedRelation.Status.ToString();
                        friendSourceList.Add(friendSource);
                    }
                }
            }
            return friendSourceList;
        }



        public Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                Entities.Modules.Actions.ModuleActionCollection Actions = new Entities.Modules.Actions.ModuleActionCollection();

                switch (this.ModuleContentType)
                {
                    case Enums.ModuleContentTypes.Links:
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "add.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                            break;
                        }
                }

                return Actions;
            }
        }
    }
}
