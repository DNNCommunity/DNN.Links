// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Links
{
    using System;
    using System.Security;
    using System.Web.UI.WebControls;

    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Modules.Links.Components;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.Utilities;

    /// <summary>
    /// Codebehind for editing links user interface.
    /// </summary>
    public partial class EditLinks : PortalModuleBase
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// The URL control.
        /// </summary>
        protected UI.UserControls.UrlControl ctlURL;

        /// <summary>
        /// The audit control.
        /// </summary>
        protected UI.UserControls.ModuleAuditControl ctlAudit;

        /// <summary>
        /// The urlTracking control.
        /// </summary>
        protected UI.UserControls.URLTrackingControl ctlTracking;
#pragma warning restore SA1401 // Fields should be private

        private int itemId = -1;

        /// <summary>
        /// Handles the Click event of the cmdCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void cmdCancel_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                this.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void cmdDelete_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                if (this.itemId != -1)
                {
                    LinkController.DeleteLink(this.itemId, this.ModuleId);
                    ModuleController.SynchronizeModule(this.ModuleId);
                }

                // Redirect back to the portal home page
                this.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public void cmdUpdate_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            try
            {
                if (this.Page.IsValid == true & this.ctlURL.Url != string.Empty)
                {
                    Link objLink = new Link();

                    // bind text values to object
                    objLink.ItemId = this.itemId;
                    objLink.ModuleId = this.ModuleId;
                    objLink.CreatedByUser = this.UserInfo.UserID;
                    objLink.CreatedDate = DateTime.Now;
                    objLink.Title = this.txtTitle.Text;
                    objLink.Url = this.ctlURL.Url;

                    int refreshInterval = 0;

                    if (this.ctlURL.UrlType == "U")
                    {
                        refreshInterval = System.Convert.ToInt32(this.ddlGetContentInterval.SelectedValue);
                    }

                    objLink.RefreshInterval = refreshInterval;

                    if (this.ddlViewOrderLinks.Items.Count > 0)
                    {
                        switch (this.ddlViewOrder.SelectedValue)
                        {
                            case "B":
                                {
                                    objLink.ViewOrder = Convert.ToInt32(this.ddlViewOrderLinks.SelectedValue) - 1;
                                    LinkController.UpdateViewOrder(objLink, -1, this.ModuleId);
                                    break;
                                }

                            case "A":
                                {
                                    objLink.ViewOrder = Convert.ToInt32(this.ddlViewOrderLinks.SelectedValue) + 1;
                                    LinkController.UpdateViewOrder(objLink, 1, this.ModuleId);
                                    break;
                                }

                            default:
                                {
                                    objLink.ViewOrder = Null.NullInteger;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        objLink.ViewOrder = Null.NullInteger;
                    }

                    objLink.Description = this.txtDescription.Text;
                    objLink.GrantRoles = ";";

                    foreach (ListItem cb in this.cblGrantRoles.Items)
                    {
                        if (cb.Selected)
                        {
                            objLink.GrantRoles += cb.Value + ";";
                        }
                    }

                    if (objLink.GrantRoles.Equals(";"))
                    {
                        objLink.GrantRoles += "0;";
                    }

                    // Create an instance of the Link DB component
                    if (Common.Utilities.Null.IsNull(this.itemId))
                    {
                        LinkController.AddLink(objLink);
                    }
                    else
                    {
                        LinkController.UpdateLink(objLink);
                    }

                    ModuleController.SynchronizeModule(this.ModuleId);

                    // url tracking
                    UrlController objUrls = new UrlController();
                    objUrls.UpdateUrl(this.PortalId, this.ctlURL.Url, this.ctlURL.UrlType, this.ctlURL.Log, this.ctlURL.Track, this.ModuleId, this.ctlURL.NewWindow);

                    // Redirect back to the portal home page
                    this.Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <inheritdoc/>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += this.Page_Load;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Determine ItemId of Link to Update
                if (this.Request.QueryString["ItemId"] != null)
                {
                    int.TryParse(this.Request.QueryString["ItemId"], out this.itemId);
                }

                // If the page is being requested the first time, determine if an
                // link itemId value is specified, and if so populate page
                // contents with the link details
                if (this.Page.IsPostBack == false)
                {
                    var permissionSet = new PermissionSet(System.Security.Permissions.PermissionState.None);
                    permissionSet.AddPermission(new System.Net.WebPermission(System.Net.NetworkAccess.Connect, "http://www.dotnetnuke.com"));
                    this.tblGetContent.Visible = permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);

                    var roles = RoleController.Instance.GetRoles(this.PortalId);

                    this.ddlViewOrderLinks.DataSource = LinkController.GetLinks(this.ModuleId);
                    this.ddlViewOrderLinks.DataTextField = "Title";
                    this.ddlViewOrderLinks.DataValueField = "ViewOrder";
                    this.ddlViewOrderLinks.DataBind();

                    this.cblGrantRoles.DataSource = roles;
                    this.cblGrantRoles.DataTextField = "RoleName";
                    this.cblGrantRoles.DataValueField = "RoleID";
                    this.cblGrantRoles.DataBind();

                    ClientAPI.AddButtonConfirm(this.cmdDelete, Services.Localization.Localization.GetString("DeleteItem"));

                    if (this.itemId != -1)
                    {
                        // Obtain a single row of link information
                        var objLink = LinkController.GetLink(this.itemId, this.ModuleId);

                        if (objLink != null)
                        {
                            this.ddlViewOrderLinks.Items.Remove(this.ddlViewOrderLinks.Items.FindByText(objLink.Title));

                            if (this.ddlViewOrderLinks.Items.Count > 0)
                            {
                                this.ddlViewOrderLinks.SelectedValue = LinkController.GetLinkByHigherViewOrder(objLink.ViewOrder, this.ModuleId).ToString();

                                if (int.Parse(this.ddlViewOrderLinks.SelectedValue) < objLink.ViewOrder)
                                {
                                    this.ddlViewOrder.SelectedValue = "A";
                                }
                                else
                                {
                                    this.ddlViewOrder.SelectedValue = "B";
                                }
                            }

                            this.txtTitle.Text = objLink.Title.ToString();
                            this.ctlURL.Url = objLink.Url;
                            string urlType = LinkController.ConvertUrlType(DotNetNuke.Common.Globals.GetURLType(objLink.Url));

                            if (urlType != "U")
                            {
                                this.tblGetContent.Visible = false;
                            }

                            this.txtDescription.Text = objLink.Description.ToString();
                            this.ddlGetContentInterval.SelectedValue = objLink.RefreshInterval.ToString();

                            this.ctlAudit.CreatedByUser = objLink.CreatedByUser.ToString();
                            this.ctlAudit.CreatedDate = objLink.CreatedDate.ToString();

                            this.ctlTracking.URL = objLink.Url;
                            this.ctlTracking.ModuleID = this.ModuleId;

                            foreach (ListItem cb in this.cblGrantRoles.Items)
                            {
                                cb.Selected = objLink.GrantRoles.Contains(";" + cb.Value + ";");
                            }
                        }
                        else
                        {
                            this.Response.Redirect(Common.Globals.NavigateURL(), true);
                        }
                    }
                    else
                    {
                        this.cmdDelete.Visible = false;
                        this.ctlAudit.Visible = false;
                        this.ctlTracking.Visible = false;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            // hide get content functionality when externel url isn`t selected
            if (!string.IsNullOrEmpty(this.ctlURL.UrlType) && this.ctlURL.UrlType != "U")
            {
                this.tblGetContent.Visible = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the lbtGetContent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        protected void lbtGetContent_Click(object sender, EventArgs e)
#pragma warning restore SA1300 // Element should begin with upper-case letter
        {
            string targetUrl = this.ctlURL.Url;

            if (!string.IsNullOrEmpty(targetUrl))
            {
                string retrieveMessage = Localization.GetString("msgGetContentSucceeded.Text", this.LocalResourceFile);
                string retrieveMessageCssClass = "MessageSuccees";

                try
                {
                    // get content from target url
                    TargetInfo targetInfo = LinkController.GetTargetContent(targetUrl);

                    this.txtTitle.Text = targetInfo.Title;
                    this.txtDescription.Text = targetInfo.Description;
                }
                catch (System.Net.WebException)
                {
                    retrieveMessage = Localization.GetString("msgGetContentFailed.Text", this.LocalResourceFile);
                    retrieveMessageCssClass = "MessageFailure";
                }

                this.lblGetContentResult.Text = retrieveMessage;
                this.lblGetContentResult.CssClass = retrieveMessageCssClass;

                this.valTitle.Validate();
            }
        }
    }
}
