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
using DotNetNuke.UI.Utilities;
using DotNetNuke.Security.Roles;
using DotNetNuke.Modules.Links.Components;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Links
{

    /// -----------------------------------------------------------------------------
    ///     ''' <summary>
    /// 	''' The EditLinks PortalModuleBase is used to manage the Links
    /// 	''' </summary>
    ///     ''' <remarks>
    /// 	''' </remarks>
    /// 	''' <history>
    /// 	''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    /// 	''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
    /// 	'''                       and localisation
    /// 	''' </history>
    /// 	''' -----------------------------------------------------------------------------
    public partial class EditLinks : PortalModuleBase
    {
        protected DotNetNuke.UI.UserControls.UrlControl ctlURL;
        protected DotNetNuke.UI.UserControls.ModuleAuditControl ctlAudit;
        protected DotNetNuke.UI.UserControls.URLTrackingControl ctlTracking;


        private int itemId = -1;

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
        ///         ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        ///         '''                       and localisation
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        private void Page_Load(System.Object sender, System.EventArgs e)
        {
            try
            {
                // Determine ItemId of Link to Update

                if (Request.QueryString["ItemId"] != null)
                {
                    int.TryParse(Request.QueryString["ItemId"], out itemId);
                }

                // If the page is being requested the first time, determine if an
                // link itemId value is specified, and if so populate page
                // contents with the link details
                if (Page.IsPostBack == false)
                {
                    // Deprecated, implemented below but kept here for some time just for reference
                    // tblGetContent.Visible = System.Security.SecurityManager.IsGranted(new System.Net.WebPermission(System.Net.NetworkAccess.Connect, "http://www.dotnetnuke.com"));                    
                    var permissionSet = new PermissionSet(System.Security.Permissions.PermissionState.None);
                    permissionSet.AddPermission(new System.Net.WebPermission(System.Net.NetworkAccess.Connect, "http://www.dotnetnuke.com"));
                    tblGetContent.Visible = permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);

                    var roles = RoleController.Instance.GetRoles(this.PortalId);

                    ddlViewOrderLinks.DataSource = LinkController.GetLinks(this.ModuleId);
                    ddlViewOrderLinks.DataTextField = "Title";
                    ddlViewOrderLinks.DataValueField = "ViewOrder";
                    ddlViewOrderLinks.DataBind();

                    cblGrantRoles.DataSource = roles;
                    cblGrantRoles.DataTextField = "RoleName";
                    cblGrantRoles.DataValueField = "RoleID";
                    cblGrantRoles.DataBind();

                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"));

                    if (itemId != -1)
                    {

                        // Obtain a single row of link information
                        
                        var objLink = LinkController.GetLink(itemId, ModuleId);

                        if (objLink != null)
                        {
                            ddlViewOrderLinks.Items.Remove(ddlViewOrderLinks.Items.FindByText(objLink.Title));

                            if (ddlViewOrderLinks.Items.Count > 0)
                            {
                                ddlViewOrderLinks.SelectedValue = LinkController.GetLinkByHigherViewOrder(objLink.ViewOrder, this.ModuleId).ToString();

                                if (int.Parse(ddlViewOrderLinks.SelectedValue) < objLink.ViewOrder)
                                    ddlViewOrder.SelectedValue = "A";
                                else
                                    ddlViewOrder.SelectedValue = "B";
                            }

                            txtTitle.Text = objLink.Title.ToString();
                            
                            ctlURL.Url = objLink.Url;

                            // Probably no longer needed
                            //ctlURL.ShowDatabase = true;
                            //ctlURL.ShowSecure = true;

                            // chkGetContent.Checked = objLink.RefreshContent
                            
                            string urlType = LinkController.ConvertUrlType(DotNetNuke.Common.Globals.GetURLType(objLink.Url));

                            if (urlType != "U")
                                tblGetContent.Visible = false;

                            txtDescription.Text = objLink.Description.ToString();

                            // If (Common.Utilities.Null.IsNull(objLink.ViewOrder) = False) Then
                            // txtViewOrder.Text = Convert.ToString(objLink.ViewOrder)
                            // End If

                            ddlGetContentInterval.SelectedValue = objLink.RefreshInterval.ToString();

                            ctlAudit.CreatedByUser = objLink.CreatedByUser.ToString();
                            ctlAudit.CreatedDate = objLink.CreatedDate.ToString();

                            ctlTracking.URL = objLink.Url;
                            ctlTracking.ModuleID = ModuleId;

                            foreach (ListItem cb in cblGrantRoles.Items)
                                cb.Selected = objLink.GrantRoles.Contains(";" + cb.Value + ";");
                        }
                        else
                            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
                    }
                    else
                    {
                        cmdDelete.Visible = false;
                        ctlAudit.Visible = false;
                        ctlTracking.Visible = false;
                    }
                }
            }
            catch (Exception exc)
            {                
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///         ''' Hide get content functionality if resource url type is none external url
        ///         ''' </summary>
        ///         ''' <param name="sender"></param>
        ///         ''' <param name="e"></param>
        ///         ''' <remarks></remarks>
        protected void Page_PreRender(object sender, System.EventArgs e)
        {

            // hide get content functionality when externel url isn`t selected
            if (!string.IsNullOrEmpty(ctlURL.UrlType) && ctlURL.UrlType != "U")
                tblGetContent.Visible = false;
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' cmdCancel_Click runs when the cancel button is clicked
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        ///         '''                       and localisation
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' cmdDelete_Click runs when the delete button is clicked
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        ///         '''                       and localisation
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (itemId != -1)
                {                    
                    LinkController.DeleteLink(itemId, ModuleId);
                    ModuleController.SynchronizeModule(ModuleId);
                }

                // Redirect back to the portal home page
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        ///         ''' <summary>
        ///         ''' cmdUpdate_Click runs when the update button is clicked
        ///         ''' </summary>
        ///         ''' <remarks>
        ///         ''' </remarks>
        ///         ''' <history>
        ///         ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        ///         '''                       and localisation
        ///         ''' </history>
        ///         ''' -----------------------------------------------------------------------------
        public void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid == true & ctlURL.Url != "")
                {
                    Link objLink = new Link();                    

                    // bind text values to object
                    objLink.ItemId = itemId;
                    objLink.ModuleId = ModuleId;
                    objLink.CreatedByUser = UserInfo.UserID;
                    objLink.CreatedDate = DateTime.Now;
                    objLink.Title = txtTitle.Text;
                    objLink.Url = ctlURL.Url;

                    int refreshInterval = 0;

                    if (ctlURL.UrlType == "U")
                        refreshInterval = System.Convert.ToInt32(ddlGetContentInterval.SelectedValue);

                    objLink.RefreshInterval = refreshInterval;

                    if ((ddlViewOrderLinks.Items.Count > 0))
                    {
                        switch (ddlViewOrder.SelectedValue)
                        {
                            case "B":
                                {
                                    objLink.ViewOrder = Convert.ToInt32(ddlViewOrderLinks.SelectedValue) - 1;
                                    LinkController.UpdateViewOrder(objLink, -1, this.ModuleId);
                                    break;
                                }

                            case "A":
                                {
                                    objLink.ViewOrder = Convert.ToInt32(ddlViewOrderLinks.SelectedValue) + 1;
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
                        objLink.ViewOrder = Null.NullInteger;

                    objLink.Description = txtDescription.Text;
                    objLink.GrantRoles = ";";

                    foreach (ListItem cb in cblGrantRoles.Items)
                    {
                        if (cb.Selected)
                            objLink.GrantRoles += cb.Value + ";";
                    }

                    if (objLink.GrantRoles.Equals(";"))
                        objLink.GrantRoles += "0;";

                    // Create an instance of the Link DB component

                    if (Common.Utilities.Null.IsNull(itemId))
                        LinkController.AddLink(objLink);
                    else
                        LinkController.UpdateLink(objLink);

                    ModuleController.SynchronizeModule(ModuleId);

                    // url tracking
                    UrlController objUrls = new UrlController();
                    objUrls.UpdateUrl(PortalId, ctlURL.Url, ctlURL.UrlType, ctlURL.Log, ctlURL.Track, ModuleId, ctlURL.NewWindow);

                    // Redirect back to the portal home page
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///         ''' Try to retrieve meta description and title from the url specified
        ///         ''' </summary>
        ///         ''' <param name="sender"></param>
        ///         ''' <param name="e"></param>
        ///         ''' <remarks></remarks>
        ///         ''' <history>
        ///         '''     [alex]      10/02/2009   First implementation 
        ///         ''' </history>
        protected void lbtGetContent_Click(object sender, System.EventArgs e)
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

                lblGetContentResult.Text = retrieveMessage;
                lblGetContentResult.CssClass = retrieveMessageCssClass;

                valTitle.Validate();
            }
        }
    }
}
