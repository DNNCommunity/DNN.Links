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
using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using System.Xml;
using Dnn.Links;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using System.Collections;

namespace DotNetNuke.Modules.Links
{

    /// -----------------------------------------------------------------------------
    ///     ''' <summary>
    ///     ''' The Settings ModuleSettingsBase is used to manage the 
    ///     ''' settings for the Links Module
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' </remarks>
    ///     ''' <history>
    ///     ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    ///     ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
    ///     '''                       and localisation
    ///     '''		[cnurse]	10/20/2004	Converted to a ModuleSettingsBase class
    ///     ''' </history>
    ///     ''' -----------------------------------------------------------------------------
    partial class Settings : ModuleSettingsBase
    {
        public const string BUSINESSCARDMODE = "BusinessCardMode";
        public const string NORMALMODE = "NormalMode";


        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' LoadSettings loads the settings from the Databas and displays them
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         '''		[cnurse]	10/20/2004	created
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if ((Page.IsPostBack == false))
                {
                    // 2014 TODO: Menu
                    if (System.Convert.ToString(ModuleSettings[SettingName.MenuAllUsers]) != string.Empty)
                    {
                        ListItem item = optMenuAllUsers.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.MenuAllUsers]));
                        if (item != null)
                            item.Selected = true;
                    }
                    else
                        optMenuAllUsers.SelectedIndex = 0;

                    if (System.Convert.ToString(ModuleSettings[SettingName.DisplayMode]) != string.Empty)
                    {
                        optControl.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.DisplayMode])).Selected = true;
                        pnlWrap.Visible = ShowWrapPanel;
                    }
                    else
                        optControl.SelectedIndex = 0;// list

                    if (System.Convert.ToString(ModuleSettings[SettingName.Direction]) != string.Empty)
                        optView.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.Direction])).Selected = true;
                    else
                        optView.SelectedIndex = 0;// vertical

                    if (System.Convert.ToString(ModuleSettings[SettingName.LinkDescriptionMode]) != "")
                        optInfo.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.LinkDescriptionMode])).Selected = true;
                    else
                        optInfo.SelectedIndex = 1;

                    trOptView.Visible = optControl.SelectedValue != Consts.DisplayModeDropdown;

                    if (DotNetNuke.Application.DotNetNukeContext.Current.Application.Version.Major > 5)
                        optInfo.Items[2].Enabled = true;
                    else
                        optInfo.Items[2].Enabled = false;

                    if (System.Convert.ToString(ModuleSettings[SettingName.Icon]) != string.Empty)
                        ctlIcon.Url = System.Convert.ToString(ModuleSettings[SettingName.Icon]);

                    if (System.Convert.ToString(ModuleSettings["nowrap"]) != string.Empty)
                        optNoWrap.Items.FindByValue(System.Convert.ToString(ModuleSettings["nowrap"])).Selected = true;
                    else
                        optNoWrap.SelectedIndex = 1;

                    if (System.Convert.ToString(ModuleSettings[SettingName.UsePermissions]) != string.Empty)
                        optUsePermissions.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.UsePermissions])).Selected = true;
                    else
                        optUsePermissions.SelectedIndex = 0;

                    if (ModuleSettings[SettingName.ModuleContentType] != null)
                    {
                        string moduleContenttype = ModuleSettings[SettingName.ModuleContentType].ToString();
                        if (!string.IsNullOrEmpty(moduleContenttype))
                        {
                            optLinkModuleType.Items.FindByValue(moduleContenttype).Selected = true;
                            pnlIcon.Visible = ShowIconPanel;
                            LoadContentData(System.Convert.ToString(ModuleSettings[SettingName.ModuleContentType]));
                        }
                    }

                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public void LoadContentData(string pContentType)
        {
            // optTypeContentSelection.Visible = True
            plControl.Visible = true;
            optControl.Visible = true;
            ploptView.Visible = true;
            optView.Visible = true;
            plInfo.Visible = true;
            optInfo.Visible = true;
            plNoWrap.Visible = true;
            optNoWrap.Visible = true;
            plUsePermissions.Visible = true;
            optUsePermissions.Visible = true;
            plIcon.Visible = true;
            ctlIcon.Visible = true;

            plDisplayAttribute.Visible = false;
            optDisplayAttribute.Visible = false;
            optDisplayOrder.Visible = false;
            // 2014 TODO: Menu
            plMenuAllUsers.Visible = false;
            optMenuAllUsers.Visible = false;
            switch ((Enums.ModuleContentTypes)int.Parse(pContentType))
            {
                case Enums.ModuleContentTypes.Links:
                    {
                        optUsePermissions.Visible = true;
                        plUsePermissions.Visible = true;
                        optTypeContentSelection.Visible = false;
                        break;
                    }

                case Enums.ModuleContentTypes.Menu:
                    {
                        optTypeContentSelection.Visible = true;

                        TabController dnnTabController = new TabController();
                        TabCollection portalTabs = dnnTabController.GetTabsByPortal(PortalId);
                        TabCollection hostTabs = dnnTabController.GetTabsByPortal(-1);

                        TabCollection tabs = new TabCollection();
                        AddTabsToCollection(portalTabs, tabs);
                        AddTabsToCollection(hostTabs, tabs);

                        List<DotNetNuke.Entities.Tabs.TabInfo> listTabs = new List<DotNetNuke.Entities.Tabs.TabInfo>();
                        foreach (System.Collections.Generic.KeyValuePair<int, DotNetNuke.Entities.Tabs.TabInfo> kvp in tabs)
                            listTabs.Add(kvp.Value);

                        optTypeContentSelection.DataSource = listTabs;
                        optTypeContentSelection.DataValueField = "TabID";
                        optTypeContentSelection.DataTextField = "TabPath";
                        optTypeContentSelection.DataBind();

                        if (System.Convert.ToString(ModuleSettings[Consts.ModuleContentItem]) != string.Empty)
                        {
                            ListItem item = optTypeContentSelection.Items.FindByValue(System.Convert.ToString(ModuleSettings[Consts.ModuleContentItem]));

                            if (item != null)
                                item.Selected = true;
                        }

                        optUsePermissions.Visible = false;
                        plUsePermissions.Visible = false;
                        // 2014 TODO: Menu
                        plMenuAllUsers.Visible = true;
                        optMenuAllUsers.Visible = true;
                        break;
                    }

                case Enums.ModuleContentTypes.Folder:
                    {


                        var dic = FolderManager.Instance.GetFolders(PortalId);

                        var folders = new List<FolderInfo>();

                        FolderPermissionController folderPermissionsController = new FolderPermissionController();

                        foreach (var item in dic)
                        {
                            if (FolderPermissionController.HasFolderPermission(this.PortalId, item.FolderPath, "READ"))
                                folders.Add(item as FolderInfo);
                        }

                        optTypeContentSelection.DataSource = folders;
                        optTypeContentSelection.DataValueField = "FolderID";
                        optTypeContentSelection.DataTextField = "FolderPath";
                        optTypeContentSelection.DataBind();

                        foreach (ListItem item in optTypeContentSelection.Items)
                        {
                            if (string.IsNullOrEmpty(item.Text))
                                item.Text = "Root";
                        }

                        optTypeContentSelection.Visible = true;
                        optUsePermissions.Visible = false;
                        plUsePermissions.Visible = false;

                        if (!string.IsNullOrEmpty(ModuleSettings[Consts.ModuleContentItem].ToString()))
                        {
                            string moduleContentItem = ModuleSettings[Consts.ModuleContentItem].ToString();

                            bool hasItem = false;
                            foreach (ListItem item in optTypeContentSelection.Items)
                            {
                                if (item.Value == moduleContentItem)
                                {
                                    hasItem = true;
                                    break;
                                }
                            }

                            if (hasItem)
                                optTypeContentSelection.SelectedValue = ModuleSettings[Consts.ModuleContentItem].ToString();
                        }

                        break;
                    }

                case Enums.ModuleContentTypes.Friends:
                    {
                        optTypeContentSelection.Visible = true;
                        plControl.Visible = true;
                        optControl.Visible = true;
                        ploptView.Visible = true;
                        optView.Visible = true;
                        plInfo.Visible = true;
                        optInfo.Visible = true;
                        plNoWrap.Visible = true;
                        optNoWrap.Visible = true;
                        plDisplayAttribute.Visible = true;
                        optDisplayAttribute.Visible = true;
                        optDisplayOrder.Visible = true;
                        plUsePermissions.Visible = false;
                        optUsePermissions.Visible = false;
                        plIcon.Visible = false;
                        ctlIcon.Visible = false;

                        optTypeContentSelection.Items.Clear();
                        optTypeContentSelection.ClearSelection();
                        optDisplayAttribute.ClearSelection();
                        optDisplayOrder.ClearSelection();
                        // normal und business card
                        ArrayList modeList = new ArrayList();
                        object modeNormal = new { ModeID = "NormalMode", Mode = "Normal" };
                        object modeBusinessCard = new { ModeID = "BusinessCardMode", Mode = "Business Card" };
                        modeList.Add(modeNormal);
                        modeList.Add(modeBusinessCard);
                        optTypeContentSelection.DataSource = modeList;
                        optTypeContentSelection.DataValueField = "ModeID";
                        optTypeContentSelection.DataTextField = "Mode";
                        optTypeContentSelection.DataBind();

                        if (System.Convert.ToString(ModuleSettings[Consts.ModuleContentItem]) != string.Empty)
                        {
                            ListItem item = optTypeContentSelection.Items.FindByValue(System.Convert.ToString(ModuleSettings[Consts.ModuleContentItem]));
                            if (item != null)
                            {
                                item.Selected = true;
                                if (item.Value.Equals(BUSINESSCARDMODE))
                                {
                                    plControl.Visible = false;
                                    optControl.Visible = false;
                                    ploptView.Visible = false;
                                    optView.Visible = false;
                                    plInfo.Visible = false;
                                    optInfo.Visible = false;
                                    plNoWrap.Visible = false;
                                    optNoWrap.Visible = false;
                                    plDisplayAttribute.Visible = false;
                                    optDisplayAttribute.Visible = false;
                                    optDisplayOrder.Visible = false;
                                }
                            }
                        }

                        if (System.Convert.ToString(ModuleSettings[SettingName.DisplayAttribute]) != string.Empty)
                        {
                            ListItem item = optDisplayAttribute.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.DisplayAttribute]));

                            if (item != null)
                                item.Selected = true;
                        }
                        if (System.Convert.ToString(ModuleSettings[SettingName.DisplayAttribute]) != string.Empty)
                        {
                            ListItem item = optDisplayOrder.Items.FindByValue(System.Convert.ToString(ModuleSettings[SettingName.DisplayOrder]));

                            if (item != null)
                                item.Selected = true;
                        }

                        break;
                    }
            }
        }

        private void AddTabsToCollection(TabCollection inTabs, TabCollection outtabs)
        {

            // 1.0.2 - Workaround for dnn powered content localization - [alexander.zimmer] & [simon.meraner]
            // serialize tab object in order to be able to identify whether the content localization is active. 
            System.Xml.Serialization.XmlSerializer tabSerializer = new System.Xml.Serialization.XmlSerializer(typeof(DotNetNuke.Entities.Tabs.TabInfo));
            foreach (System.Collections.Generic.KeyValuePair<int, DotNetNuke.Entities.Tabs.TabInfo> kvp in inTabs)
            {
                if (!outtabs.ContainsKey(kvp.Key))
                {
                    System.IO.StringWriter stream = new System.IO.StringWriter();

                    tabSerializer.Serialize(stream, kvp.Value);

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
                        outtabs.Add(kvp.Key, kvp.Value);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' UpdateSettings saves the modified settings to the Database
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         '''		[cnurse]	10/20/2004	created
        ///         '''		[cnurse]	10/25/2004	upated to use TabModuleId rather than TabId/ModuleId
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
                // 2014 TODO: Menu
                objModules.UpdateModuleSetting(ModuleId, SettingName.DisplayMode, optControl.SelectedItem.Value);
                objModules.UpdateModuleSetting(ModuleId, SettingName.Direction, optView.SelectedItem.Value);
                objModules.UpdateModuleSetting(ModuleId, SettingName.LinkDescriptionMode, optInfo.SelectedItem.Value);
                objModules.UpdateModuleSetting(ModuleId, SettingName.Icon, ctlIcon.Url);
                objModules.UpdateModuleSetting(ModuleId, "nowrap", optNoWrap.SelectedItem.Value);
                objModules.UpdateModuleSetting(ModuleId, SettingName.ModuleContentType, optLinkModuleType.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, SettingName.UsePermissions, optUsePermissions.SelectedValue);
                // objModules.UpdateModuleSetting(ModuleId, "usepopup", optUsePopup.SelectedValue)
                objModules.UpdateModuleSetting(ModuleId, SettingName.DisplayAttribute, optDisplayAttribute.SelectedValue);
                objModules.UpdateModuleSetting(ModuleId, SettingName.DisplayOrder, optDisplayOrder.SelectedValue);
                // 2014 TODO
                objModules.UpdateModuleSetting(ModuleId, SettingName.MenuAllUsers, optMenuAllUsers.SelectedValue);

                switch ((Enums.ModuleContentTypes)int.Parse(optLinkModuleType.SelectedValue))
                {
                    case Enums.ModuleContentTypes.Menu:
                        {
                            objModules.UpdateModuleSetting(ModuleId, Consts.ModuleContentItem, optTypeContentSelection.SelectedValue);
                            break;
                        }

                    case Enums.ModuleContentTypes.Folder:
                        {
                            objModules.UpdateModuleSetting(ModuleId, Consts.ModuleContentItem, optTypeContentSelection.SelectedValue);
                            objModules.UpdateModuleSetting(ModuleId, Consts.FolderId, optTypeContentSelection.SelectedValue);
                            break;
                        }

                    case Enums.ModuleContentTypes.Friends:
                        {
                            objModules.UpdateModuleSetting(ModuleId, Consts.ModuleContentItem, optTypeContentSelection.SelectedValue);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                bool allowCaching = true;

                if (optControl.SelectedItem.Value == Consts.DisplayModeDropdown)
                    allowCaching = false;

                if (optInfo.SelectedItem.Value == "Y")
                    allowCaching = false;

                if (!allowCaching)
                {
                    DotNetNuke.Entities.Modules.ModuleInfo objModule = objModules.GetModule(ModuleId, TabId, false);
                    if (objModule.CacheTime > 0)
                    {
                        objModule.CacheTime = 0;
                        objModules.UpdateModule(objModule);
                    }
                }

                ModuleController.SynchronizeModule(ModuleId);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }



        public void optLinkModuleType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                pnlIcon.Visible = ShowIconPanel;

                LoadContentData(optLinkModuleType.SelectedValue);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        public void optControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            pnlWrap.Visible = ShowWrapPanel;

            if (ShowWrapPanel)
                optNoWrap.SelectedValue = "NW";

            trOptView.Visible = optControl.SelectedValue != Consts.DisplayModeDropdown;
        }

        public void optTypeContentSelection_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((Enums.ModuleContentTypes)int.Parse(optLinkModuleType.SelectedValue) == Enums.ModuleContentTypes.Friends)
            {
                if (optTypeContentSelection.Text.Equals(BUSINESSCARDMODE))
                {
                    plControl.Visible = false;
                    optControl.Visible = false;
                    ploptView.Visible = false;
                    optView.Visible = false;
                    plInfo.Visible = false;
                    optInfo.Visible = false;
                    plNoWrap.Visible = false;
                    optNoWrap.Visible = false;
                    plDisplayAttribute.Visible = false;
                    optDisplayAttribute.Visible = false;
                    optDisplayOrder.Visible = false;
                }
                else
                {
                    plControl.Visible = true;
                    optControl.Visible = true;
                    ploptView.Visible = true;
                    optView.Visible = true;
                    plInfo.Visible = true;
                    optInfo.Visible = true;
                    plNoWrap.Visible = true;
                    optNoWrap.Visible = true;
                    plDisplayAttribute.Visible = true;
                    optDisplayAttribute.Visible = true;
                    optDisplayOrder.Visible = true;
                }
            }
        }

        public bool ShowIconPanel
        {
            get
            {
                bool result = false;

                if (int.Parse(optLinkModuleType.SelectedValue) == 1)
                    result = true;

                return result;
            }
        }

        public bool ShowWrapPanel
        {
            get
            {
                bool result = false;

                if (optControl.SelectedValue != Consts.DisplayModeDropdown)
                    result = true;

                return result;
            }
        }
    }
}