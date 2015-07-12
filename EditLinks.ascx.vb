'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2008
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Links

    ''' -----------------------------------------------------------------------------
    ''' <summary>
	''' The EditLinks PortalModuleBase is used to manage the Links
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
	''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
	'''                       and localisation
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial  Class EditLinks
		Inherits Entities.Modules.PortalModuleBase

#Region " Private Members "

        Private itemId As Integer = -1

#End Region

#Region " Event Handlers "


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                Dim objModules As New Entities.Modules.ModuleController
                Dim objLinks As New LinkController
                Dim roleController As New DotNetNuke.Security.Roles.RoleController
                ' Determine ItemId of Link to Update

                If Not (Request.QueryString("ItemId") Is Nothing) Then
                    itemId = Int32.Parse(Request.QueryString("ItemId"))
                End If

                ' If the page is being requested the first time, determine if an
                ' link itemId value is specified, and if so populate page
                ' contents with the link details
                If Page.IsPostBack = False Then

                    tblGetContent.Visible = System.Security.SecurityManager.IsGranted(New System.Net.WebPermission(System.Net.NetworkAccess.Connect, "http://www.dotnetnuke.com"))

                    Dim allUsersRole As New DotNetNuke.Security.Roles.RoleInfo()
                    allUsersRole.RoleID = -2
                    allUsersRole.RoleName = "All Users"
                    Dim unAuthorizedUsersRole As New DotNetNuke.Security.Roles.RoleInfo()
                    unAuthorizedUsersRole.RoleID = -1
                    unAuthorizedUsersRole.RoleName = "Unauthenticated Users"

                    Dim roles = roleController.GetPortalRoles(Me.PortalId)
                    roles.Add(unAuthorizedUsersRole)
                    roles.Add(allUsersRole)

                    ddlViewOrderLinks.DataSource = objLinks.GetLinks(Me.ModuleId)
                    ddlViewOrderLinks.DataTextField = "Title"
                    ddlViewOrderLinks.DataValueField = "ViewOrder"
                    ddlViewOrderLinks.DataBind()

                    cblGrantRoles.DataSource = roles
                    cblGrantRoles.DataTextField = "RoleName"
                    cblGrantRoles.DataValueField = "RoleID"
                    cblGrantRoles.DataBind()

                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    If itemId <> -1 Then

                        ' Obtain a single row of link information

                        Dim objLink As LinkInfo = objLinks.GetLink(itemId, ModuleId)

                        If Not objLink Is Nothing Then

                            ddlViewOrderLinks.Items.Remove(ddlViewOrderLinks.Items.FindByText(objLink.Title))

                            If ddlViewOrderLinks.Items.Count > 0 Then

                                ddlViewOrderLinks.SelectedValue = objLinks.GetLinkByHigherViewOrder(objLink.ViewOrder, Me.ModuleId).ToString

                                If Integer.Parse(ddlViewOrderLinks.SelectedValue) < objLink.ViewOrder Then
                                    ddlViewOrder.SelectedValue = "A"
                                Else
                                    ddlViewOrder.SelectedValue = "B"
                                End If

                            End If

                            txtTitle.Text = objLink.Title.ToString

                            ctlURL.Url = objLink.Url
                            ctlURL.ShowDatabase = True
                            ctlURL.ShowSecure = True
                            'chkGetContent.Checked = objLink.RefreshContent

                            Dim urlType As String = LinkController.convertURLType(GetURLType(objLink.Url))

                            If urlType <> "U" Then
                                tblGetContent.Visible = False
                            End If

                            txtDescription.Text = objLink.Description.ToString

                            'If (Common.Utilities.Null.IsNull(objLink.ViewOrder) = False) Then
                            '    txtViewOrder.Text = Convert.ToString(objLink.ViewOrder)
                            'End If

                            ddlGetContentInterval.SelectedValue = objLink.RefreshInterval

                            ctlAudit.CreatedByUser = objLink.CreatedByUser.ToString
                            ctlAudit.CreatedDate = objLink.CreatedDate.ToString

                            ctlTracking.URL = objLink.Url
                            ctlTracking.ModuleID = ModuleId

                            For Each cb As ListItem In cblGrantRoles.Items
                                cb.Selected = objLink.GrantRoles.Contains(";" & cb.Value & ";")
                            Next

                        Else       ' security violation attempt to access item not related to this Module

                            Response.Redirect(NavigateURL(), True)

                        End If

                    Else

                        cmdDelete.Visible = False
                        ctlAudit.Visible = False
                        ctlTracking.Visible = False

                    End If

                End If

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' <summary>
        ''' Hide get content functionality if resource url type is none external url
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            ' hide get content functionality when externel url isn`t selected
            If Not String.IsNullOrEmpty(ctlURL.UrlType) AndAlso ctlURL.UrlType <> "U" Then tblGetContent.Visible = False

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click

            Try

                Response.Redirect(NavigateURL(), True)

            Catch exc As Exception           'Module failed to load

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the delete button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click

            Try

                If itemId <> -1 Then

                    Dim links As New LinkController
                    links.DeleteLink(itemId, ModuleId)
                    ModuleController.SynchronizeModule(ModuleId)

                End If

                ' Redirect back to the portal home page
                Response.Redirect(NavigateURL(), True)

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click

            Try

                If Page.IsValid = True And ctlURL.Url <> "" Then

                    Dim objLink As New LinkInfo
                    Dim objLinks As New LinkController

                    objLink = CType(CBO.InitializeObject(objLink, GetType(LinkInfo)), LinkInfo)
                    'bind text values to object
                    objLink.ItemId = itemId
                    objLink.ModuleId = ModuleId
                    objLink.CreatedByUser = UserInfo.UserID
                    objLink.CreatedDate = DateTime.Now()
                    objLink.Title = txtTitle.Text
                    objLink.Url = ctlURL.Url

                    Dim refreshInterval As Integer = 0

                    If ctlURL.UrlType = "U" Then
                        refreshInterval = CInt(ddlGetContentInterval.SelectedValue)
                    End If

                    objLink.RefreshInterval = refreshInterval

                    If (ddlViewOrderLinks.Items.Count > 0) Then

                        Select Case ddlViewOrder.SelectedValue
                            Case "B"
                                objLink.ViewOrder = Convert.ToInt32(ddlViewOrderLinks.SelectedValue) - 1
                                objLinks.UpdateViewOrder(objLink, -1, Me.ModuleId)
                            Case "A"
                                objLink.ViewOrder = Convert.ToInt32(ddlViewOrderLinks.SelectedValue) + 1
                                objLinks.UpdateViewOrder(objLink, 1, Me.ModuleId)
                            Case Else
                                objLink.ViewOrder = Null.NullInteger
                        End Select

                    Else
                        objLink.ViewOrder = Null.NullInteger
                    End If

                    objLink.Description = txtDescription.Text
                    objLink.GrantRoles = ";"

                    For Each cb As ListItem In cblGrantRoles.Items
                        If cb.Selected Then
                            objLink.GrantRoles += cb.Value & ";"
                        End If
                    Next

                    If objLink.GrantRoles.Equals(";") Then
                        objLink.GrantRoles += "0;"
                    End If

                    ' Create an instance of the Link DB component

                    If Common.Utilities.Null.IsNull(itemId) Then
                        objLinks.AddLink(objLink)
                    Else
                        objLinks.UpdateLink(objLink)
                    End If

                    ModuleController.SynchronizeModule(ModuleId)

                    ' url tracking
                    Dim objUrls As New UrlController
                    objUrls.UpdateUrl(PortalId, ctlURL.Url, ctlURL.UrlType, ctlURL.Log, ctlURL.Track, ModuleId, ctlURL.NewWindow)

                    ' Redirect back to the portal home page
                    Response.Redirect(NavigateURL(), True)

                End If

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' <summary>
        ''' Try to retrieve meta description and title from the url specified
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' <history>
        '''     [alex]      10/02/2009   First implementation 
        ''' </history>
        Protected Sub lbtGetContent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbtGetContent.Click

            Dim targetUrl As String = Me.ctlURL.Url

            If Not String.IsNullOrEmpty(targetUrl) Then

                Dim retrieveMessage As String = Localization.GetString("msgGetContentSucceeded.Text", Me.LocalResourceFile)
                Dim retrieveMessageCssClass As String = "MessageSuccees"

                Try
                    ' get content from target url
                    Dim targetInfo As TargetInfo = New LinkController().GetTargetContent(targetUrl)

                    Me.txtTitle.Text = targetInfo.Title
                    Me.txtDescription.Text = targetInfo.Description

                Catch ex As System.Net.WebException

                    retrieveMessage = Localization.GetString("msgGetContentFailed.Text", Me.LocalResourceFile)
                    retrieveMessageCssClass = "MessageFailure"

                End Try

                lblGetContentResult.Text = retrieveMessage
                lblGetContentResult.CssClass = retrieveMessageCssClass

                valTitle.Validate()

            End If

        End Sub

#End Region

    End Class

End Namespace
