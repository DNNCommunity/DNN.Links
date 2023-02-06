// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Dnn.Links;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Modules.Links.Components;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;

    /// <summary>
    /// Provides the UI logic for the module.
    /// </summary>
    public partial class Links : PortalModuleBase, IActionable
    {
        /// <summary>
        /// Gets or sets the file ID.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets the type of the module content.
        /// </summary>
        public Enums.ModuleContentTypes ModuleContentType
        {
            get
            {
                Enums.ModuleContentTypes result = Enums.ModuleContentTypes.Links;

                if (this.Settings[SettingName.ModuleContentType] != null
                    && int.TryParse(this.Settings[SettingName.ModuleContentType].ToString(), out int contenttype))
                {
                    result = (Enums.ModuleContentTypes)contenttype;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the folder ID.
        /// </summary>
        public int FolderId
        {
            get
            {
                if (this.Settings[Consts.FolderId] != null)
                {
                    return Convert.ToInt32(this.Settings[Consts.FolderId]);
                }

                return Null.NullInteger;
            }
        }

        /// <summary>
        /// Gets the folder information.
        /// </summary>
        public FolderInfo FolderInfo
        {
            get
            {
                if (this.FolderId != Null.NullInteger)
                {
                    return FolderManager.Instance.GetFolder(this.FolderId) as FolderInfo;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the folder permissions.
        /// </summary>
        public FolderPermissionCollection FolderPermissions
        {
            get
            {
                if (this.FolderInfo != null)
                {
                    return FolderPermissionController.GetFolderPermissionsCollectionByFolder(this.PortalId, this.FolderInfo.FolderPath);
                }

                return new FolderPermissionCollection();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editable.
        /// </summary>
        public new bool IsEditable
        {
            get
            {
                if (this.ModuleContentType == Enums.ModuleContentTypes.Links)
                {
                    return base.IsEditable;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the module display mode.
        /// </summary>
        public string DisplayMode
        {
            get
            {
                if (this.Settings[SettingName.DisplayMode] != null)
                {
                    return this.Settings[SettingName.DisplayMode].ToString();
                }

                return Consts.DisplayModeLink;
            }
        }

        /// <summary>
        /// Gets the menu all user setting.
        /// </summary>
        public string MenuAllUser
        {
            get
            {
                if (this.Settings[SettingName.MenuAllUsers] != null)
                {
                    return this.Settings[SettingName.MenuAllUsers].ToString();
                }

                return Consts.MenuAllUser;
            }
        }

        /// <summary>
        /// Gets the link description mode.
        /// </summary>
        /// <value>
        /// The link description mode.
        /// </value>
        public string LinkDescriptionMode
        {
            get
            {
                if (this.Settings[SettingName.LinkDescriptionMode] != null)
                {
                    return this.Settings[SettingName.LinkDescriptionMode].ToString();
                }

                return Consts.ShowLinkDescriptionNo;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use permissions.
        /// </summary>
        public bool UsePermissions
        {
            get
            {
                if (this.Settings[SettingName.UsePermissions] != null)
                {
                    return bool.Parse(this.Settings[SettingName.UsePermissions].ToString());
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to display an icon.
        /// </summary>
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
                                if (System.Convert.ToString(this.Settings[SettingName.Icon]) != string.Empty)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }

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

        /// <summary>
        /// Gets a style string to use "nowrap" in css.
        /// </summary>
        public string NoWrap
        {
            get
            {
                if (this.Settings.ContainsKey("nowrap") && this.Settings["nowrap"].ToString() != "W")
                {
                    return "style=\"white-space: nowrap\"";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show the links horizontaly.
        /// </summary>
        public string Horizontal
        {
            get
            {
                string result = string.Empty;

                if (this.Settings.ContainsKey(SettingName.Direction) && System.Convert.ToString(this.Settings[SettingName.Direction]) == Consts.DirectionHorizontal)
                {
                    result = "Horizontal";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the popup trigger css class.
        /// </summary>
        public string PopupTrigger
        {
            get
            {
                string result = string.Empty;

                if (this.LinkDescriptionMode == Consts.ShowLinkDescriptionJQuery)
                {
                    result = " trigger";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show a popup.
        /// </summary>
        public bool ShowPopup
        {
            get
            {
                bool result = false;

                if (this.LinkDescriptionMode == Consts.ShowLinkDescriptionJQuery)
                {
                    result = true;
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                Entities.Modules.Actions.ModuleActionCollection actions = new Entities.Modules.Actions.ModuleActionCollection();

                switch (this.ModuleContentType)
                {
                    case Enums.ModuleContentTypes.Links:
                        {
                            actions.Add(this.GetNextActionID(), Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, this.LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, string.Empty, "add.gif", this.EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                            break;
                        }
                }

                return actions;
            }
        }

        /// <summary>
        /// Gets the image file identifier.
        /// </summary>
        protected string ImageFileId
        {
            get
            {
                if (this.Settings[SettingName.Icon] != null)
                {
                    return this.Settings[SettingName.Icon].ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to display information about each link.
        /// </summary>
        /// <param name="strDescription">The description to show.</param>
        /// <returns>A value indicating whether to display the info.</returns>
        public bool DisplayInfo(string strDescription)
        {
            bool result = false;

            try
            {
                if ((System.Convert.ToString(this.Settings[SettingName.LinkDescriptionMode]) == Consts.ShowLinkDescriptionYes) && (!string.IsNullOrEmpty(strDescription)))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return result;
        }

        /// <summary>
        /// If settings allow, gets the tooltip to display.
        /// </summary>
        /// <param name="strDescription">The link description.</param>
        /// <returns>The tooltip string (could be empty).</returns>
        public string DisplayToolTip(string strDescription)
        {
            if (System.Convert.ToString(this.Settings[SettingName.LinkDescriptionMode]) != Consts.ShowLinkDescriptionNo)
            {
                return string.Empty;
            }

            return strDescription;
        }

        /// <summary>
        /// Formats the URL correctly.
        /// </summary>
        /// <param name="link">The link t use.</param>
        /// <param name="trackClicks">A value indicating whether to track the user clicks on the link.</param>
        /// <returns>A proper link for the options provided.</returns>
        public string FormatURL(string link, bool trackClicks)
        {
            return Common.Globals.LinkClick(link, this.TabId, this.ModuleId, trackClicks);
        }

        /// <summary>
        /// Formats the icon.
        /// </summary>
        /// <param name="imageURL">The image URL.</param>
        /// <returns>A string representing the icon to display.</returns>
        public string FormatIcon(string imageURL)
        {
            string result = string.Empty;

            switch (this.ModuleContentType)
            {
                case Enums.ModuleContentTypes.Links:
                    {
                        if (System.Convert.ToString(this.Settings[SettingName.Icon]) != string.Empty)
                        {
                            result = Common.Globals.LinkClick(System.Convert.ToString(this.Settings[SettingName.Icon]), this.TabId, this.ModuleId, false);
                        }

                        break;
                    }

                case Enums.ModuleContentTypes.Folder:
                    {
                        if (!string.IsNullOrEmpty(imageURL))
                        {
                            result = this.ResolveUrl(imageURL);
                        }

                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// Decodes an html string.
        /// </summary>
        /// <param name="strValue">The string to decode.</param>
        /// <returns>A decoded html string.</returns>
        public string HtmlDecode(string strValue)
        {
            string result = string.Empty;

            try
            {
                result = this.Server.HtmlDecode(strValue);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return result;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (System.Convert.ToString(this.Settings[SettingName.LinkDescriptionMode]) ==
                    Consts.ShowLinkDescriptionYes)
                {
                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(Consts.ToogleJS))
                    {
                        string jspath = this.Page.ResolveUrl("~/DesktopModules/Links/js/toggle.js");
                        this.Page.ClientScript.RegisterClientScriptInclude(Consts.PopupJS, jspath);
                    }
                }

                if (Convert.ToString(this.Settings[SettingName.DisplayMode]) == Consts.DisplayModeDropdown)
                {
                    this.pnlList.Visible = false;
                    this.pnlDropdown.Visible = true;
                }
                else
                {
                    this.pnlList.Visible = true;
                    this.pnlDropdown.Visible = false;
                }

                if (!this.Page.IsPostBack)
                {
                    if (this.Request.Params[Consts.FileId] != null)
                    {
                        this.FileId = System.Convert.ToInt32(this.Request.Params[Consts.FileId]);
                    }

                    if (this.FileId != Null.NullInteger)
                    {
                        var fileInfo = FileManager.Instance.GetFile(this.FileId);

                        if (fileInfo != null)
                        {
                            FolderController folderCont = new FolderController();

                            var folderInfo = FolderManager.Instance.GetFolder(this.FolderId);

                            if (folderInfo != null)
                            {
                                if (DotNetNuke.Security.Permissions.FolderPermissionController.HasFolderPermission(
                                        this.PortalId, folderInfo.FolderPath, "READ"))
                                {
                                    FileManager.Instance.WriteFileToResponse(fileInfo, ContentDisposition.Attachment);
                                }
                            }
                        }
                    }

                    RoleController roleController =
                        new RoleController();

                    var roles = roleController.GetUserRoles(this.UserInfo, true);
                    bool isInARole = false;
                    ArrayList linksToShow = new ArrayList();

                    List<Link> links = new List<Link>();

                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                links = LinkController.GetLinks(this.ModuleId).ToList();

                                string imgUrl = DotNetNuke.Common.Globals.LinkClick(this.ImageFileId, this.TabId, this.ModuleId);
                                foreach (var link in links)
                                {
                                    link.ImageURL = imgUrl;
                                }

                                break;
                            }

                        case Enums.ModuleContentTypes.Menu:
                            {
                                List<TabInfo> tabsToShow = new List<TabInfo>();
                                if (this.Settings.ContainsKey(Consts.ModuleContentItem))
                                {
                                    string moduleContentItem = this.Settings[Consts.ModuleContentItem].ToString();
                                    int.TryParse(moduleContentItem, out int moduleContentItemInt);
                                    tabsToShow = TabController.GetTabsByParent(moduleContentItemInt, this.PortalId);
                                }

                                foreach (TabInfo tabinfo in tabsToShow)
                                {
                                    // 1.0.2 - Workaround for dnn powered content localization - [alexander.zimmer] & [simon.meraner]
                                    // serialize tab object in order to be able to identify whether the content localization is active.
                                    System.Xml.Serialization.XmlSerializer tabSerializer =
                                        new System.Xml.Serialization.XmlSerializer(
                                            typeof(TabInfo));
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
                                        if (cultureNode != null && cultureNode.Value !=
                                            Thread.CurrentThread.CurrentCulture.Name)
                                        {
                                            showCulture = false;
                                        }
                                    }

                                    if (showCulture)
                                    {
                                        Link link = new Link();

                                        link.ModuleId = this.ModuleId;
                                        link.Url = DotNetNuke.Common.Globals.NavigateURL(tabinfo.TabID);
                                        link.NewWindow = false;
                                        link.Title = tabinfo.TabName;
                                        link.GrantRoles = ";";
                                        link.Description = tabinfo.Description;
                                        link.ItemId = tabinfo.TabID;

                                        if (this.MenuAllUser.Equals("No"))
                                        {
                                            foreach (var role in tabinfo.TabPermissions.ToList())
                                            {
                                                link.GrantRoles += role.RoleID + ";";
                                            }
                                        }
                                        else
                                        {
                                            // -2: All Users; -1: Unauthenticated Users
                                            link.GrantRoles += "-2;";
                                        }

                                        links.Add(link);
                                    }
                                }

                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {
                                var folder = FolderManager.Instance.GetFolder(this.FolderId);
                                var files = FolderManager.Instance.GetFiles(folder);

                                foreach (FileInfo file in files)
                                {
                                    Link link = new Link();

                                    link.NewWindow = false;
                                    link.Title = file.FileName;
                                    link.ItemId = file.FileId;
                                    link.Url = DotNetNuke.Common.Globals.NavigateURL(
                                        this.TabId,
                                        string.Empty,
                                        "FileId=" + file.FileId.ToString());
                                    link.GrantRoles = ";";
                                    link.Description = Utils.GetFileSizeString(file.Size);
                                    link.ImageURL = Utils.GetImageURL(file.Extension);

                                    foreach (var permission in this.FolderPermissions.ToList())
                                    {
                                        if (permission.AllowAccess == true && permission.PermissionKey == "READ" &&
                                            permission.UserID == Null.NullInteger)
                                        {
                                            link.GrantRoles += permission.RoleID + ";";
                                        }
                                    }

                                    links.Add(link);
                                }

                                break;
                            }
                    }

                    if (this.DisplayMode == Consts.DisplayModeDropdown)
                    {
                        if (this.IsEditable)
                        {
                            this.cmdEdit.Visible = true;
                        }
                        else
                        {
                            this.cmdEdit.Visible = false;
                        }

                        this.cmdGo.ToolTip = Localization.GetString("cmdGo");

                        if (this.LinkDescriptionMode == Consts.ShowLinkDescriptionYes)
                        {
                            this.cmdInfo.Visible = true;
                        }
                        else
                        {
                            this.cmdInfo.Visible = false;
                        }

                        this.cmdInfo.ToolTip = Localization.GetString("cmdInfo", this.LocalResourceFile);

                        foreach (Link link in links)
                        {
                            if (this.ModuleContentType == Enums.ModuleContentTypes.Links && this.UsePermissions)
                            {
                                link.GrantRoles += "-2;";
                            }

                            isInARole = false;

                            if (link.RefreshContent)
                            {
                                LinkController.RefreshLinkContent(link);
                            }

                            foreach (RoleInfo role in roles)
                            {
                                if (link.GrantRoles.Contains(";" + role.RoleID + ";"))
                                {
                                    isInARole = true;
                                }
                            }

                            if (!this.UsePermissions | isInARole | link.GrantRoles.Contains(";-2;") |
                                link.GrantRoles.Contains(";-1;") | this.UserInfo.IsSuperUser |
                                this.UserInfo.IsInRole(this.PortalSettings.AdministratorRoleName))
                            {
                                linksToShow.Add(link);
                            }
                        }

                        this.cboLinks.DataSource = linksToShow;

                        switch (this.ModuleContentType)
                        {
                            case Enums.ModuleContentTypes.Menu:
                                {
                                    this.cboLinks.DataValueField = "ItemId";
                                    break;
                                }

                            case Enums.ModuleContentTypes.Folder:
                                {
                                    this.cboLinks.DataValueField = "ItemId";
                                    this.cboLinks.DataTextField = "Title";
                                    break;
                                }
                        }

                        this.cboLinks.DataBind();
                    }
                    else
                    {
                        foreach (Link link in links)
                        {
                            if (this.ModuleContentType == Enums.ModuleContentTypes.Links)
                            {
                                if (this.UsePermissions == false)
                                {
                                    link.GrantRoles += "-2;";
                                }
                            }

                            isInARole = false;

                            if (link.RefreshContent)
                            {
                                LinkController.RefreshLinkContent(link);
                            }

                            foreach (RoleInfo role in roles)
                            {
                                if (link.GrantRoles.Contains(";" + role.RoleID + ";"))
                                {
                                    isInARole = true;
                                }
                            }

                            if (isInARole | link.GrantRoles.Contains("-2") | link.GrantRoles.Contains("-1") |
                                this.UserInfo.IsSuperUser |
                                this.UserInfo.IsInRole(this.PortalSettings.AdministratorRoleName))
                            {
                                linksToShow.Add(link);
                            }
                        }

                        this.lstLinks.DataSource = linksToShow;
                        this.lstLinks.DataBind();
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // This is to be expected when serving a file from Page_load.
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void cmdEdit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                if (this.cboLinks.SelectedItem != null)
                {
                    this.Response.Redirect(this.EditUrl("ItemID", this.cboLinks.SelectedItem.Value), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void cmdGo_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                if (this.cboLinks.SelectedItem != null)
                {
                    string strURL = string.Empty;

                    Link objLink = new Link();

                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                objLink = LinkController.GetLink(int.Parse(this.cboLinks.SelectedValue), this.ModuleId);
                                strURL = this.FormatURL(objLink.Url, objLink.TrackClicks);
                                break;
                            }

                        case Enums.ModuleContentTypes.Menu:
                            {
                                objLink = new Link();

                                TabController tabCont = new TabController();
                                TabInfo tabInfo = tabCont.GetTab(int.Parse(this.cboLinks.SelectedItem.Value), this.PortalId, false);

                                strURL = DotNetNuke.Common.Globals.NavigateURL(tabInfo.TabID);
                                objLink.TrackClicks = false;
                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {
                                objLink = new Link();
                                strURL = DotNetNuke.Common.Globals.NavigateURL(this.TabId, string.Empty, "FileId", this.cboLinks.SelectedValue);
                                break;
                            }
                    }

                    if (objLink != null)
                    {
                        if (objLink.NewWindow)
                        {
                            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "OpenLink", "window.open('" + strURL + "','_blank')", true);
                        }
                        else
                        {
                            this.Response.Redirect(strURL, true);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void cmdInfo_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                if (this.cboLinks.SelectedItem != null)
                {
                    string desc = string.Empty;

                    switch (this.ModuleContentType)
                    {
                        case Enums.ModuleContentTypes.Links:
                            {
                                Link objLink = new Link();

                                objLink = LinkController.GetLink(int.Parse(this.cboLinks.SelectedItem.Value), this.ModuleId);

                                if (objLink == null)
                                {
                                    desc = this.HtmlDecode(objLink.Description);
                                }

                                break;
                            }

                        case Enums.ModuleContentTypes.Menu:
                            {
                                TabController tabCont = new TabController();
                                TabInfo tabInfo = tabCont.GetTab(int.Parse(this.cboLinks.SelectedItem.Value), this.PortalId, false);

                                if (tabInfo != null)
                                {
                                    desc = tabInfo.Description;
                                }

                                break;
                            }

                        case Enums.ModuleContentTypes.Folder:
                            {
                                var fileInfo = FileManager.Instance.GetFile(int.Parse(this.cboLinks.SelectedItem.Value));

                                if (fileInfo != null)
                                {
                                    desc = Utils.GetFileSizeString(fileInfo.Size);
                                }

                                break;
                            }
                    }

                    if (!string.IsNullOrEmpty(desc))
                    {
                        if (this.lblDescription.Text == string.Empty)
                        {
                            this.lblDescription.Text = desc;
                        }
                        else
                        {
                            this.lblDescription.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the lstLinks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        protected void lstLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                var link = e.Item.DataItem as Link;

                if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HtmlAnchor linkHyp = (HtmlAnchor)e.Item.FindControl("linkHyp");
                    Label lblMoreInfo = (Label)e.Item.FindControl("lblMoreInfo");
                    Panel pnlDescription = (Panel)e.Item.FindControl("pnlDescription");
                    var lbldescrdiv = e.Item.FindControl("lbldescrdiv") as Label;
                    var spnSelect = e.Item.FindControl("spnSelect");
                    var editLink = e.Item.FindControl("editLink") as HyperLink;

                    lblMoreInfo.Attributes.Add("onclick", "toggleVisibility('" + pnlDescription.ClientID + "')");
                    lblMoreInfo.Attributes.Add("style", "cursor: pointer;");
                    lblMoreInfo.Visible = !this.ShowPopup;

                    linkHyp.HRef = this.FormatURL(link.Url, link.TrackClicks);
                    linkHyp.Target = link.NewWindow ? "_blank" : "_self";
                    if (this.ShowPopup && link.Description != string.Empty)
                    {
                        linkHyp.Title = link.Description;
                    }

                    lbldescrdiv.Text = this.HtmlDecode(link.Description);

                    spnSelect.Visible = this.DisplayInfo(link.Description);

                    editLink.NavigateUrl = this.EditUrl("ItemId", link.ItemId.ToString());
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Select event of the lstLinks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        private void lstLinks_Select(object sender, RepeaterCommandEventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                this.lstLinks.Items[e.Item.ItemIndex].FindControl("pnlDescription").Visible = !this.lstLinks.Items[e.Item.ItemIndex].FindControl("pnlDescription").Visible;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
