// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Dnn.Links;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;

    /// <summary>
    /// Represents the module settings.
    /// </summary>
    public partial class Settings : ModuleSettingsBase
    {
        /// <summary>
        /// Gets a value indicating whether to show the icons panel.
        /// </summary>
        public bool ShowIconPanel
        {
            get
            {
                bool result = false;

                if (int.Parse(this.optLinkModuleType.SelectedValue) == 1)
                {
                    result = true;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show the wrap panel.
        /// </summary>
        public bool ShowWrapPanel
        {
            get
            {
                bool result = false;

                if (this.optControl.SelectedValue != Consts.DisplayModeDropdown)
                {
                    result = true;
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public override void LoadSettings()
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    if (Convert.ToString(this.ModuleSettings[SettingName.MenuAllUsers]) != string.Empty)
                    {
                        ListItem item = this.optMenuAllUsers.Items.FindByValue(System.Convert.ToString(this.ModuleSettings[SettingName.MenuAllUsers]));
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                    }
                    else
                    {
                        this.optMenuAllUsers.SelectedIndex = 0;
                    }

                    if (Convert.ToString(this.ModuleSettings[SettingName.DisplayMode]) != string.Empty)
                    {
                        this.optControl.Items.FindByValue(System.Convert.ToString(this.ModuleSettings[SettingName.DisplayMode])).Selected = true;
                        this.pnlWrap.Visible = this.ShowWrapPanel;
                    }
                    else
                    {
                        this.optControl.SelectedIndex = 0; // list
                    }

                    if (System.Convert.ToString(this.ModuleSettings[SettingName.Direction]) != string.Empty)
                    {
                        this.optView.Items.FindByValue(System.Convert.ToString(this.ModuleSettings[SettingName.Direction])).Selected = true;
                    }
                    else
                    {
                        this.optView.SelectedIndex = 0; // vertical
                    }

                    if (System.Convert.ToString(this.ModuleSettings[SettingName.LinkDescriptionMode]) != string.Empty)
                    {
                        this.optInfo.Items.FindByValue(System.Convert.ToString(this.ModuleSettings[SettingName.LinkDescriptionMode])).Selected = true;
                    }
                    else
                    {
                        this.optInfo.SelectedIndex = 1;
                    }

                    this.trOptView.Visible = this.optControl.SelectedValue != Consts.DisplayModeDropdown;

                    if (DotNetNuke.Application.DotNetNukeContext.Current.Application.Version.Major > 5)
                    {
                        this.optInfo.Items[2].Enabled = true;
                    }
                    else
                    {
                        this.optInfo.Items[2].Enabled = false;
                    }

                    if (System.Convert.ToString(this.ModuleSettings[SettingName.Icon]) != string.Empty)
                    {
                        this.ctlIcon.Url = System.Convert.ToString(this.ModuleSettings[SettingName.Icon]);
                    }

                    if (System.Convert.ToString(this.ModuleSettings["nowrap"]) != string.Empty)
                    {
                        this.optNoWrap.Items.FindByValue(System.Convert.ToString(this.ModuleSettings["nowrap"])).Selected = true;
                    }
                    else
                    {
                        this.optNoWrap.SelectedIndex = 1;
                    }

                    if (System.Convert.ToString(this.ModuleSettings[SettingName.UsePermissions]) != string.Empty)
                    {
                        this.optUsePermissions.Items.FindByValue(System.Convert.ToString(this.ModuleSettings[SettingName.UsePermissions])).Selected = true;
                    }
                    else
                    {
                        this.optUsePermissions.SelectedIndex = 0;
                    }

                    if (this.ModuleSettings[SettingName.ModuleContentType] != null)
                    {
                        string moduleContenttype = this.ModuleSettings[SettingName.ModuleContentType].ToString();
                        if (!string.IsNullOrEmpty(moduleContenttype))
                        {
                            this.optLinkModuleType.Items.FindByValue(moduleContenttype).Selected = true;
                            this.pnlIcon.Visible = this.ShowIconPanel;
                            this.LoadContentData(System.Convert.ToString(this.ModuleSettings[SettingName.ModuleContentType]));
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
        /// Loads the content data.
        /// </summary>
        /// <param name="pContentType">The type of content to load.</param>
        public void LoadContentData(string pContentType)
        {
            this.plControl.Visible = true;
            this.optControl.Visible = true;
            this.ploptView.Visible = true;
            this.optView.Visible = true;
            this.plInfo.Visible = true;
            this.optInfo.Visible = true;
            this.plNoWrap.Visible = true;
            this.optNoWrap.Visible = true;
            this.plUsePermissions.Visible = true;
            this.optUsePermissions.Visible = true;
            this.plIcon.Visible = true;
            this.ctlIcon.Visible = true;

            this.plDisplayAttribute.Visible = false;
            this.optDisplayAttribute.Visible = false;
            this.optDisplayOrder.Visible = false;

            this.plMenuAllUsers.Visible = false;
            this.optMenuAllUsers.Visible = false;
            switch ((Enums.ModuleContentTypes)int.Parse(pContentType))
            {
                case Enums.ModuleContentTypes.Links:
                    {
                        this.optUsePermissions.Visible = true;
                        this.plUsePermissions.Visible = true;
                        this.optTypeContentSelection.Visible = false;
                        break;
                    }

                case Enums.ModuleContentTypes.Menu:
                    {
                        this.optTypeContentSelection.Visible = true;

                        TabController dnnTabController = new TabController();
                        TabCollection portalTabs = dnnTabController.GetTabsByPortal(this.PortalId);
                        TabCollection hostTabs = dnnTabController.GetTabsByPortal(-1);

                        TabCollection tabs = new TabCollection();
                        this.AddTabsToCollection(portalTabs, tabs);
                        this.AddTabsToCollection(hostTabs, tabs);

                        List<DotNetNuke.Entities.Tabs.TabInfo> listTabs = new List<DotNetNuke.Entities.Tabs.TabInfo>();
                        foreach (System.Collections.Generic.KeyValuePair<int, DotNetNuke.Entities.Tabs.TabInfo> kvp in tabs)
                        {
                            listTabs.Add(kvp.Value);
                        }

                        this.optTypeContentSelection.DataSource = listTabs;
                        this.optTypeContentSelection.DataValueField = "TabID";
                        this.optTypeContentSelection.DataTextField = "TabPath";
                        this.optTypeContentSelection.DataBind();

                        if (System.Convert.ToString(this.ModuleSettings[Consts.ModuleContentItem]) != string.Empty)
                        {
                            ListItem item = this.optTypeContentSelection.Items.FindByValue(System.Convert.ToString(this.ModuleSettings[Consts.ModuleContentItem]));

                            if (item != null)
                            {
                                item.Selected = true;
                            }
                        }

                        this.optUsePermissions.Visible = false;
                        this.plUsePermissions.Visible = false;
                        this.plMenuAllUsers.Visible = true;
                        this.optMenuAllUsers.Visible = true;
                        break;
                    }

                case Enums.ModuleContentTypes.Folder:
                    {
                        var dic = FolderManager.Instance.GetFolders(this.PortalId);

                        var folders = new List<FolderInfo>();

                        FolderPermissionController folderPermissionsController = new FolderPermissionController();

                        foreach (var item in dic)
                        {
                            if (FolderPermissionController.HasFolderPermission(this.PortalId, item.FolderPath, "READ"))
                            {
                                folders.Add(item as FolderInfo);
                            }
                        }

                        this.optTypeContentSelection.DataSource = folders;
                        this.optTypeContentSelection.DataValueField = "FolderID";
                        this.optTypeContentSelection.DataTextField = "FolderPath";
                        this.optTypeContentSelection.DataBind();

                        foreach (ListItem item in this.optTypeContentSelection.Items)
                        {
                            if (string.IsNullOrEmpty(item.Text))
                            {
                                item.Text = "Root";
                            }
                        }

                        this.optTypeContentSelection.Visible = true;
                        this.optUsePermissions.Visible = false;
                        this.plUsePermissions.Visible = false;

                        if (!string.IsNullOrEmpty(this.ModuleSettings[Consts.ModuleContentItem].ToString()))
                        {
                            string moduleContentItem = this.ModuleSettings[Consts.ModuleContentItem].ToString();

                            bool hasItem = false;
                            foreach (ListItem item in this.optTypeContentSelection.Items)
                            {
                                if (item.Value == moduleContentItem)
                                {
                                    hasItem = true;
                                    break;
                                }
                            }

                            if (hasItem)
                            {
                                this.optTypeContentSelection.SelectedValue = this.ModuleSettings[Consts.ModuleContentItem].ToString();
                            }
                        }

                        break;
                    }
            }
        }

        /// <inheritdoc/>
        public override void UpdateSettings()
        {
            try
            {
                ModuleController objModules = new ModuleController();

                objModules.UpdateModuleSetting(this.ModuleId, SettingName.DisplayMode, this.optControl.SelectedItem.Value);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.Direction, this.optView.SelectedItem.Value);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.LinkDescriptionMode, this.optInfo.SelectedItem.Value);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.Icon, this.ctlIcon.Url);
                objModules.UpdateModuleSetting(this.ModuleId, "nowrap", this.optNoWrap.SelectedItem.Value);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.ModuleContentType, this.optLinkModuleType.SelectedValue);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.UsePermissions, this.optUsePermissions.SelectedValue);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.DisplayAttribute, this.optDisplayAttribute.SelectedValue);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.DisplayOrder, this.optDisplayOrder.SelectedValue);
                objModules.UpdateModuleSetting(this.ModuleId, SettingName.MenuAllUsers, this.optMenuAllUsers.SelectedValue);

                switch ((Enums.ModuleContentTypes)int.Parse(this.optLinkModuleType.SelectedValue))
                {
                    case Enums.ModuleContentTypes.Menu:
                        {
                            objModules.UpdateModuleSetting(this.ModuleId, Consts.ModuleContentItem, this.optTypeContentSelection.SelectedValue);
                            break;
                        }

                    case Enums.ModuleContentTypes.Folder:
                        {
                            objModules.UpdateModuleSetting(this.ModuleId, Consts.ModuleContentItem, this.optTypeContentSelection.SelectedValue);
                            objModules.UpdateModuleSetting(this.ModuleId, Consts.FolderId, this.optTypeContentSelection.SelectedValue);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                bool allowCaching = true;

                if (this.optControl.SelectedItem.Value == Consts.DisplayModeDropdown)
                {
                    allowCaching = false;
                }

                if (this.optInfo.SelectedItem.Value == "Y")
                {
                    allowCaching = false;
                }

                if (!allowCaching)
                {
                    DotNetNuke.Entities.Modules.ModuleInfo objModule = objModules.GetModule(this.ModuleId, this.TabId, false);
                    if (objModule.CacheTime > 0)
                    {
                        objModule.CacheTime = 0;
                        objModules.UpdateModule(objModule);
                    }
                }

                ModuleController.SynchronizeModule(this.ModuleId);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the optLinkModuleType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void optLinkModuleType_SelectedIndexChanged(object sender, System.EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                this.pnlIcon.Visible = this.ShowIconPanel;

                this.LoadContentData(this.optLinkModuleType.SelectedValue);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the optControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void optControl_SelectedIndexChanged(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            this.pnlWrap.Visible = this.ShowWrapPanel;

            if (this.ShowWrapPanel)
            {
                this.optNoWrap.SelectedValue = "NW";
            }

            this.trOptView.Visible = this.optControl.SelectedValue != Consts.DisplayModeDropdown;
        }

        private void AddTabsToCollection(TabCollection inTabs, TabCollection outtabs)
        {
            // 1.0.2 - Workaround for dnn powered content localization - [alexander.zimmer] & [simon.meraner]
            // serialize tab object in order to be able to identify whether the content localization is active.
            System.Xml.Serialization.XmlSerializer tabSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TabInfo));
            foreach (KeyValuePair<int, TabInfo> kvp in inTabs)
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
                        {
                            showCulture = false;
                        }
                    }

                    if (showCulture)
                    {
                        outtabs.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }
    }
}
