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

'Option Strict On

Imports System.Web.UI.WebControls

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Roles
Imports System.IO
Imports DotNetNuke.Services.FileSystem
Imports System.Xml
Imports DotNetNuke.Entities.Users
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Users.Social
Imports System.Linq
Imports Telerik.Web.UI

Namespace DotNetNuke.Modules.Links

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Links Class provides the UI for displaying the Links
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    '''		[cnurse]	10/20/2004	Removed ViewOptions from Action menu
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Links
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

#Region " Properties "

        Private _FileId As Integer = Null.NullInteger
        Public Property FileId() As Integer
            Get
                Return _FileId
            End Get
            Set(ByVal value As Integer)
                _FileId = value
            End Set
        End Property

        Public ReadOnly Property ModuleContentType() As Enums.ModuleContentTypes



            Get
                Dim result As Enums.ModuleContentTypes = Enums.ModuleContentTypes.Links

                If Settings(SettingName.ModuleContentType) IsNot Nothing Then

                    result = CType(Settings(SettingName.ModuleContentType), Enums.ModuleContentTypes)

                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property FolderId() As Integer
            Get

                Dim result As Integer = Null.NullInteger

                If Settings(Consts.FolderId) IsNot Nothing Then

                    result = CType(Settings(Consts.FolderId), Integer)

                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property FolderInfo() As DotNetNuke.Services.FileSystem.FolderInfo
            Get

                Dim result As DotNetNuke.Services.FileSystem.FolderInfo = Nothing

                Dim cont As New DotNetNuke.Services.FileSystem.FolderController

                If Me.FolderId <> Null.NullInteger Then

                    result = cont.GetFolderInfo(Me.PortalId, Me.FolderId)

                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property FolderPermissions() As DotNetNuke.Security.Permissions.FolderPermissionCollection
            Get

                Dim result As New DotNetNuke.Security.Permissions.FolderPermissionCollection

                Dim cont As New DotNetNuke.Security.Permissions.FolderPermissionController

                If FolderInfo IsNot Nothing Then

                    result = cont.GetFolderPermissionsCollectionByFolderPath(Me.PortalId, FolderInfo.FolderPath)

                End If

                Return result

            End Get
        End Property

        Public Shadows Function IsEditable() As Boolean

            Dim result As Boolean = False

            Select Case Me.ModuleContentType

                Case Enums.ModuleContentTypes.Links

                    If MyBase.IsEditable Then

                        Return True

                    End If

                Case Enums.ModuleContentTypes.Menu

                Case Enums.ModuleContentTypes.Folder

                Case Enums.ModuleContentTypes.Friends


            End Select

        End Function

        Public ReadOnly Property DisplayMode() As String
            Get

                Dim result As String = Consts.DisplayModeLink

                If Settings(SettingName.DisplayMode) IsNot Nothing Then

                    result = Settings(SettingName.DisplayMode)

                End If

                Return result

            End Get
        End Property
        ' 2014 TODO: Menu
        Public ReadOnly Property MenuAllUser() As String
            Get

                Dim result As String = Consts.MenuAllUser

                If Settings(SettingName.MenuAllUsers) IsNot Nothing Then

                    result = Settings(SettingName.MenuAllUsers)

                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property LinkDescriptionMode() As String
            Get

                Dim result As String = Consts.ShowLinkDescriptionNo

                If Settings(SettingName.LinkDescriptionMode) IsNot Nothing Then

                    result = Settings(SettingName.LinkDescriptionMode)

                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property UsePermissions() As Boolean
            Get

                Dim result As String = False

                If Settings(SettingName.UsePermissions) IsNot Nothing Then

                    result = CBool(Settings(SettingName.UsePermissions))

                End If

                Return result

            End Get
        End Property



        Protected ReadOnly Property ImageFileId As String
            Get

                Dim result As String = ""

                If Settings(SettingName.Icon) IsNot Nothing Then

                    result = CStr(Settings(SettingName.Icon))

                End If

                Return result

            End Get
        End Property


#End Region

#Region " Public Methods "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayInfo displays the description/info on the link
        ''' </summary>
        ''' <param name="strDescription"></param>
        ''' <returns>The description text</returns>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        '''     [erikvb]    5/29/2008   Added strDescription parameter, inorder to prevent displaying info when description is empty
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayInfo(ByVal strDescription As String) As String

            Dim result As String = "False"

            Try

                If (CType(Settings(SettingName.LinkDescriptionMode), String) = Consts.ShowLinkDescriptionYes) AndAlso (Not String.IsNullOrEmpty(strDescription)) Then
                    result = "True"
                Else
                    result = "False"
                End If

            Catch exc As Exception        'Module failed to load

                ProcessModuleLoadException(Me, exc)

            End Try

            Return result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayToolTip gets the tooltip to display
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strDescription">The description</param>
        ''' <returns>The tooltip text</returns>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' 	[bonosoft]	11/3/2004	Added default option, if option not set. (DNN-115)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayToolTip(ByVal strDescription As String) As String

            Dim result As String = strDescription

            If CType(Settings(SettingName.LinkDescriptionMode), String) <> Consts.ShowLinkDescriptionNo Then

                result = String.Empty

            End If

            Return result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatURL correctly formats the links url
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="Link">The link</param>
        ''' <returns>The correctly formatted url</returns>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FormatURL(ByVal Link As String, ByVal TrackClicks As Boolean) As String

            Return Common.Globals.LinkClick(Link, TabId, ModuleId, TrackClicks)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatIcon correctly formats the link icon
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <returns>The correctly formatted url</returns>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FormatIcon(ByVal ImageURL As String) As String

            Dim result As String = String.Empty

            Select Case Me.ModuleContentType

                Case Enums.ModuleContentTypes.Links

                    If CType(Settings(SettingName.Icon), String) <> String.Empty Then
                        result = Common.Globals.LinkClick(CType(Settings(SettingName.Icon), String), TabId, ModuleId, False)
                    End If

                Case Enums.ModuleContentTypes.Folder

                    If Not String.IsNullOrEmpty(ImageURL) Then
                        result = ResolveUrl(ImageURL)
                    End If

            End Select

            Return result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayIcon displays the icon
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <returns>true or false</returns>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayIcon() As String

            Dim result As String = "False"

            Try

                Select Case Me.ModuleContentType

                    Case Enums.ModuleContentTypes.Links

                        If CType(Settings(SettingName.Icon), String) <> String.Empty Then
                            result = "True"
                        Else
                            result = "False"
                        End If

                    Case Enums.ModuleContentTypes.Folder
                        result = "True"
                End Select

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

            Return result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HtmlDecode decodes the html string
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strValue">The string to decode</param>
        ''' <returns>The decoded html</returns>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function HtmlDecode(ByVal strValue As String) As String

            Dim result As String = String.Empty

            Try

                result = Server.HtmlDecode(strValue)

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

            Return result

        End Function

        Public Function NoWrap() As String

            If Not CType(Settings("nowrap"), String) = "W" Then
                Return "style=""white-space: nowrap"""
            End If

            Return String.Empty
        End Function

        Public ReadOnly Property Horizontal() As String
            Get

                Dim result As String = String.Empty

                If CType(Settings(SettingName.Direction), String) = Consts.DirectionHorizontal Then
                    result = "Horizontal"
                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property PopupTrigger() As String
            Get

                Dim result As String = String.Empty

                If Me.LinkDescriptionMode = Consts.ShowLinkDescriptionJQuery Then
                    result = " trigger"
                End If

                Return result

            End Get
        End Property

        Public ReadOnly Property ShowPopup() As String
            Get

                Dim result As String = "false"

                If Me.LinkDescriptionMode = Consts.ShowLinkDescriptionJQuery Then
                    result = "true"
                End If

                Return result

            End Get
        End Property

#End Region

#Region " Event Handlers "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                If CStr(Me.Settings(SettingName.LinkDescriptionMode)) = Consts.ShowLinkDescriptionYes Then

                    If Not Me.Page.ClientScript.IsClientScriptIncludeRegistered(Consts.ToogleJS) Then

                        Dim jspath As String = Page.ResolveUrl("~/DesktopModules/Links/js/toggle.js")
                        Me.Page.ClientScript.RegisterClientScriptInclude(Consts.PopupJS, jspath)

                    End If

                End If

                If Me.ModuleContentType.Equals(Enums.ModuleContentTypes.Friends) AndAlso Me.Settings(Consts.ModuleContentItem).Equals("BusinessCardMode") Then
                    pnlList.Visible = False
                    pnlDropdown.Visible = False
                Else
                    If CType(Settings(SettingName.DisplayMode), String) = Consts.DisplayModeDropdown Then
                        pnlList.Visible = False
                        pnlDropdown.Visible = True
                    Else
                        pnlList.Visible = True
                        pnlDropdown.Visible = False
                    End If
                End If

                If Not Page.IsPostBack Then

                    If Request.Params(Consts.FileId) IsNot Nothing Then

                        Me.FileId = CInt(Request.Params(Consts.FileId))

                    End If

                    If Me.FileId <> Null.NullInteger Then

                        Dim fileCont As New DotNetNuke.Services.FileSystem.FileController

                        Dim fileInfo As DotNetNuke.Services.FileSystem.FileInfo = fileCont.GetFileById(Me.FileId, Me.PortalId)

                        If fileInfo IsNot Nothing Then

                            Dim folderCont As New DotNetNuke.Services.FileSystem.FolderController

                            Dim folderInfo As DotNetNuke.Services.FileSystem.FolderInfo = folderCont.GetFolderInfo(Me.PortalId, fileInfo.FolderId)

                            If folderInfo IsNot Nothing Then

                                If DotNetNuke.Security.Permissions.FolderPermissionController.HasFolderPermission(Me.PortalId, folderInfo.FolderPath, "READ") Then

                                    FileSystemUtils.DownloadFile(PortalSettings, Me.FileId, False, True)

                                End If

                            End If

                        End If

                    End If

                    Dim roleController As New DotNetNuke.Security.Roles.RoleController
                    Dim objLinks As New LinkController
                    Dim roles = roleController.GetUserRoles(Me.PortalId, Me.UserId)
                    Dim isInARole As Boolean = False
                    Dim linksToShow As New ArrayList

                    Dim links As New List(Of LinkInfo)

                    Select Case Me.ModuleContentType

                        Case Enums.ModuleContentTypes.Links

                            links = objLinks.GetLinks(ModuleId).Cast(Of LinkInfo).ToList()

                            Dim imgUrl As String = DotNetNuke.Common.Globals.LinkClick(ImageFileId, TabId, ModuleId)
                            For Each link As LinkInfo In links
                                link.ImageURL = imgUrl
                                'pre alpha
                                'link.ImageURL = String.Format("~/DesktopModules/Links/WebThumbnail.ashx?url={0}&tw={1}&th={2}", Server.UrlEncode(link.Url), 300, 200)
                            Next

                        Case Enums.ModuleContentTypes.Menu

                            Dim tabsToShow As ArrayList = New ArrayList(TabController.GetTabsByParent(Int32.Parse(Settings(Consts.ModuleContentItem).ToString()), Me.PortalId))

                            For Each tabinfo As DotNetNuke.Entities.Tabs.TabInfo In tabsToShow

                                ' 1.0.2 - Workaround for dnn powered content localization - [alexander.zimmer] & [simon.meraner]
                                ' serialize tab object in order to be able to identify whether the content localization is active. 
                                Dim tabSerializer As New System.Xml.Serialization.XmlSerializer(GetType(DotNetNuke.Entities.Tabs.TabInfo))
                                Dim stream As New System.IO.StringWriter()
                                tabSerializer.Serialize(stream, tabinfo)

                                Dim sr As System.IO.StringReader = New System.IO.StringReader(stream.ToString)
                                Dim xmlDoc As New XmlDocument
                                xmlDoc.Load(sr)

                                stream.Close()

                                Dim tabCulture As XmlElement = xmlDoc.SelectSingleNode("/tab/cultureCode")

                                Dim showCulture As Boolean = True

                                If tabCulture IsNot Nothing Then
                                    Dim cultureNode As XmlNode = tabCulture.FirstChild

                                    ' dnn 5.5.x detected ... exclude tabs where the cultures doesn`t matcj the current culture
                                    If cultureNode IsNot Nothing AndAlso cultureNode.Value <> System.Threading.Thread.CurrentThread.CurrentCulture.Name Then
                                        showCulture = False
                                    End If
                                End If

                                ' -------------------------------------------------------------------------------------------------------

                                If showCulture Then
                                    Dim link As New LinkInfo

                                    link.NewWindow = False
                                    link.Title = tabinfo.TabName
                                    link.Url = NavigateURL(tabinfo.TabID)
                                    link.GrantRoles = ";"
                                    link.Description = tabinfo.Description
                                    link.ItemId = tabinfo.TabID
                                    ' 2014 TODO: Menu
                                    If MenuAllUser.Equals("No") Then
                                        For Each role As Permissions.TabPermissionInfo In tabinfo.TabPermissions
                                            link.GrantRoles += role.RoleID & ";"
                                        Next
                                    Else
                                        ' -2: All Users; -1: Unauthenticated Users
                                        link.GrantRoles += "-2;"
                                    End If

                                    links.Add(link)
                                End If

                            Next


                        Case Enums.ModuleContentTypes.Folder

                            Dim filesCont As New DotNetNuke.Services.FileSystem.FileController

                            Dim arrFiles As ArrayList = FileSystemUtils.GetFilesByFolder(Me.PortalId, Me.FolderId)

                            For Each file As DotNetNuke.Services.FileSystem.FileInfo In arrFiles

                                Dim link As New LinkInfo

                                link.NewWindow = False
                                link.Title = file.FileName
                                link.ItemId = file.FileId
                                link.Url = NavigateURL(Me.TabId, String.Empty, "FileId=" & file.FileId.ToString())
                                link.GrantRoles = ";"
                                link.Description = Utils.GetFileSizeString(file.Size)
                                link.ImageURL = Utils.GetImageURL(file.Extension)

                                For Each permission As Permissions.FolderPermissionInfo In Me.FolderPermissions

                                    If permission.AllowAccess = True AndAlso permission.PermissionKey = "READ" AndAlso permission.UserID = Null.NullInteger Then

                                        link.GrantRoles += permission.RoleID & ";"

                                    End If

                                Next

                                links.Add(link)

                            Next

                        Case Enums.ModuleContentTypes.Friends

                            If Me.Settings(Consts.ModuleContentItem).Equals("NormalMode") Then
                                pnlFriends.Visible = False
                            Else
                                pnlFriends.Visible = True
                                pnlDropdown.Visible = False
                                pnlList.Visible = False
                            End If

                            ' list friends
                            Dim currentUser As UserInfo = UserController.GetCurrentUserInfo()
                            If currentUser IsNot Nothing And currentUser.UserID <> -1 Then

                                For Each lFriend As LinksFriend In GetFriendsSource(currentUser)
                                    Dim link As New LinkInfo
                                    link.NewWindow = False
                                    Select Case Convert.ToInt16(Me.Settings(SettingName.DisplayAttribute))
                                        Case Enums.DisplayAttribute.Username
                                            link.Title = lFriend.UserName
                                        Case Enums.DisplayAttribute.DisplayName
                                            link.Title = lFriend.DisplayName
                                        Case Enums.DisplayAttribute.FirstName
                                            link.Title = lFriend.UserFirstName
                                        Case Enums.DisplayAttribute.LastName
                                            link.Title = lFriend.UserLastName
                                        Case Enums.DisplayAttribute.FullName
                                            link.Title = lFriend.UserFullName
                                    End Select
                                    'link.Title = lFriend.UserName
                                    link.Url = DotNetNuke.Common.Globals.LinkClick("UserID=" & lFriend.UserID, Me.TabId, Me.ModuleId)
                                    link.GrantRoles = ";"
                                    link.ItemId = lFriend.UserID
                                    link.Description = "Status: " & lFriend.Status & "<br />Username: " & lFriend.UserName & "<br />Displayname: " & lFriend.DisplayName & "<br />Full Name: " & lFriend.UserFullName
                                    links.Add(link)
                                Next
                                lvFriends.DataSource = GetFriendsSource(currentUser)
                                lvFriends.DataBind()
                            End If

                            If Convert.ToInt16(Me.Settings(SettingName.DisplayOrder)) = Enums.DisplayOrder.ASC Then
                                links = links.OrderBy(Function(l) l.Title).ToList()
                            Else
                                links = links.OrderByDescending(Function(l) l.Title).ToList()
                            End If

                    End Select

                    If Me.DisplayMode = Consts.DisplayModeDropdown Then

                        If IsEditable() Then
                            cmdEdit.Visible = True
                        Else
                            cmdEdit.Visible = False
                        End If

                        cmdGo.ToolTip = Localization.GetString("cmdGo")

                        If Me.LinkDescriptionMode = Consts.ShowLinkDescriptionYes Then
                            cmdInfo.Visible = True
                        Else
                            cmdInfo.Visible = False
                        End If

                        cmdInfo.ToolTip = Localization.GetString("cmdInfo", Me.LocalResourceFile)

                        For Each link As LinkInfo In links

                            If Me.ModuleContentType = Enums.ModuleContentTypes.Links AndAlso Me.UsePermissions Then
                                link.GrantRoles += "-2;"
                            End If

                            isInARole = False

                            If link.RefreshContent Then
                                objLinks.RefreshLinkContent(link)
                            End If

                            For Each role As RoleInfo In roles
                                If link.GrantRoles.Contains(";" & role.RoleID & ";") Then
                                    isInARole = True
                                End If
                            Next

                            '
                            If Me.ModuleContentType = Enums.ModuleContentTypes.Friends Then
                                isInARole = True
                            End If

                            If isInARole _
                            Or link.GrantRoles.Contains(";-2;") _
                            Or link.GrantRoles.Contains(";-1;") _
                            Or Me.UserInfo.IsSuperUser _
                            Or Me.UserInfo.IsInRole(Me.PortalSettings.AdministratorRoleName) Then

                                linksToShow.Add(link)

                            End If

                        Next

                        cboLinks.DataSource = linksToShow

                        Select Case Me.ModuleContentType

                            Case Enums.ModuleContentTypes.Menu

                                cboLinks.DataValueField = "ItemId"

                            Case Enums.ModuleContentTypes.Folder

                                cboLinks.DataValueField = "ItemId"
                                cboLinks.DataTextField = "Title"

                            Case Enums.ModuleContentTypes.Friends
                                cboLinks.DataValueField = "ItemId"
                        End Select

                        cboLinks.DataBind()

                    Else

                        For Each link As LinkInfo In links

                            If Me.ModuleContentType = Enums.ModuleContentTypes.Links Then

                                If Me.UsePermissions = False Then

                                    link.GrantRoles += "-2;"

                                End If

                            End If

                            isInARole = False

                            If link.RefreshContent Then
                                objLinks.RefreshLinkContent(link)
                            End If

                            For Each role As RoleInfo In roles
                                If link.GrantRoles.Contains(";" & role.RoleID & ";") Then
                                    isInARole = True
                                End If
                            Next
                            '
                            If Me.ModuleContentType = Enums.ModuleContentTypes.Friends Then
                                isInARole = True
                            End If

                            If isInARole _
                            Or link.GrantRoles.Contains("-2") _
                            Or link.GrantRoles.Contains("-1") _
                            Or Me.UserInfo.IsSuperUser _
                            Or Me.UserInfo.IsInRole(Me.PortalSettings.AdministratorRoleName) Then

                                linksToShow.Add(link)

                            End If

                        Next

                        lstLinks.DataSource = linksToShow
                        lstLinks.DataBind()

                    End If

                End If

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdEdit_Click runs when the edit button is clciked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdEdit.Click

            Try

                If Not cboLinks.SelectedItem Is Nothing Then

                    Response.Redirect(EditUrl("ItemID", cboLinks.SelectedItem.Value), True)

                End If

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdGo_Click runs when the Go Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdGo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGo.Click

            Try

                If Not cboLinks.SelectedItem Is Nothing Then

                    Dim strURL As String = String.Empty

                    Dim objLinks As New LinkController
                    Dim objLink As New LinkInfo

                    Select Case Me.ModuleContentType

                        Case Enums.ModuleContentTypes.Links

                            objLink = objLinks.GetLink(Integer.Parse(cboLinks.SelectedValue), ModuleId)
                            strURL = FormatURL(objLink.Url.ToString, objLink.TrackClicks)

                        Case Enums.ModuleContentTypes.Menu

                            objLink = New LinkInfo()

                            Dim tabCont As New DotNetNuke.Entities.Tabs.TabController
                            Dim tabInfo As DotNetNuke.Entities.Tabs.TabInfo = tabCont.GetTab(Integer.Parse(cboLinks.SelectedItem.Value), Me.PortalId, False)

                            strURL = NavigateURL(tabInfo.TabID)
                            objLink.TrackClicks = False

                        Case Enums.ModuleContentTypes.Folder

                            objLink = New LinkInfo()
                            strURL = NavigateURL(Me.TabId, String.Empty, "FileId", cboLinks.SelectedValue)

                        Case Enums.ModuleContentTypes.Friends
                            objLink = New LinkInfo()
                            strURL = DotNetNuke.Common.Globals.LinkClick("UserID=" & Integer.Parse(cboLinks.SelectedItem.Value), Me.TabId, Me.ModuleId)

                    End Select

                    If Not objLink Is Nothing Then

                        If objLink.NewWindow = True Then
                            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "OpenLink", "window.open('" + strURL + "','_blank')", True)
                        Else
                            Response.Redirect(strURL, True)
                        End If

                    End If

                End If

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdInfo_Click runs when the Info (...) button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdInfo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdInfo.Click

            Try

                If Not cboLinks.SelectedItem Is Nothing Then

                    Dim desc As String = String.Empty

                    Select Case Me.ModuleContentType

                        Case Enums.ModuleContentTypes.Links

                            Dim objLinks As New LinkController
                            Dim objLink As New LinkInfo

                            objLink = objLinks.GetLink(Integer.Parse(cboLinks.SelectedItem.Value), ModuleId)

                            If objLink Is Nothing Then

                                desc = HtmlDecode(objLink.Description.ToString)

                            End If

                        Case Enums.ModuleContentTypes.Menu

                            Dim tabCont As New DotNetNuke.Entities.Tabs.TabController
                            Dim tabInfo As DotNetNuke.Entities.Tabs.TabInfo = tabCont.GetTab(Integer.Parse(cboLinks.SelectedItem.Value), Me.PortalId, False)

                            If tabInfo IsNot Nothing Then
                                desc = tabInfo.Description
                            End If

                        Case Enums.ModuleContentTypes.Folder

                            Dim fileCont As New DotNetNuke.Services.FileSystem.FileController
                            Dim fileInfo As DotNetNuke.Services.FileSystem.FileInfo

                            fileInfo = fileCont.GetFileById(Integer.Parse(cboLinks.SelectedItem.Value), Me.PortalId)

                            If fileInfo IsNot Nothing Then
                                desc = Utils.GetFileSizeString(fileInfo.Size)
                            End If

                        Case Enums.ModuleContentTypes.Friends

                            Dim currentUser As UserInfo = UserController.GetCurrentUserInfo()
                            Dim friendUser As UserInfo = UserController.GetUserById(Me.PortalId, Integer.Parse(cboLinks.SelectedItem.Value))
                            Dim updatedRelation As UserRelationship = RelationshipController.Instance.GetFriendRelationship(currentUser, friendUser)
                            If updatedRelation.Status.ToString.Equals("Accepted") Then
                                desc = "Status: " & updatedRelation.Status.ToString()
                            Else
                                If currentUser.UserID <> updatedRelation.UserId Then
                                    ' current user not initialize
                                    desc = "Status: " & "Receive Request from " & friendUser.Username
                                Else
                                    desc = "Status: " & "Request " & updatedRelation.Status.ToString()
                                End If
                            End If
                            desc &= "<br />Username: " & friendUser.Username & "<br />Displayname: " & friendUser.DisplayName & "<br />Full Name: " & friendUser.FullName
                    End Select

                    If Not String.IsNullOrEmpty(desc) Then

                        If lblDescription.Text = String.Empty Then
                            lblDescription.Text = desc
                        Else
                            lblDescription.Text = String.Empty
                        End If

                    End If

                End If

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' lstLinks_Select runs when an item in the Links list is selected
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lstLinks_Select(ByVal Sender As Object, ByVal e As RepeaterCommandEventArgs) Handles lstLinks.ItemCommand

            Try

                lstLinks.Items(e.Item.ItemIndex).FindControl("pnlDescription").Visible = Not lstLinks.Items(e.Item.ItemIndex).FindControl("pnlDescription").Visible

            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        Protected Sub lstLinks_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles lstLinks.ItemDataBound

            Try

                If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

                    Dim linkHyp As HtmlAnchor = CType(e.Item.FindControl("linkHyp"), HtmlAnchor)
                    Dim lblMoreInfo As Label = CType(e.Item.FindControl("lblMoreInfo"), Label)
                    Dim pnlDescription As Panel = CType(e.Item.FindControl("pnlDescription"), Panel)

                    lblMoreInfo.Attributes.Add("onclick", "toggleVisibility('" & pnlDescription.ClientID & "')")
                    lblMoreInfo.Attributes.Add("style", "cursor: pointer;")

                End If


            Catch exc As Exception

                ProcessModuleLoadException(Me, exc)

            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Unload runs when the page is unloaded. It is used to solve caching problems with the core that cause certain items to not work.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[awhittington]	01/04/2007	Added Page_Unload
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload

            If DisplayMode = Consts.DisplayModeDropdown _
            Or DisplayMode = "Y" Then

                DataCache.RemoveCache(ModuleController.CacheKey(TabModuleId))

            End If

            If Not IsPostBack Then

                DataCache.RemoveCache(ModuleController.CacheKey(TabModuleId))

            End If

        End Sub


        Protected Sub btnRemoveFriend_OnCommand(ByVal sender As Object, ByVal e As CommandEventArgs)
            If Not String.IsNullOrEmpty(e.CommandName) Then
                Dim selectUser As UserInfo = UserController.GetUserById(Me.PortalId, Convert.ToInt16(e.CommandName))
                ' remove friend
                Dim currentUser As UserInfo = UserController.GetCurrentUserInfo()
                Try
                    ' beta api
                    'RelationshipController.Instance.DeleteFriend(currentUser, selectUser)
                    FriendsController.Instance.DeleteFriend(currentUser, selectUser)
                    ' update friends list
                    lvFriends.DataSource = GetFriendsSource(currentUser)
                    lvFriends.DataBind()
                Catch ex As Exception
                    ProcessModuleLoadException(Me, ex)
                End Try
            End If
        End Sub

        Protected Sub btnAcceptFriendRequest_OnCommand(ByVal sender As Object, ByVal e As CommandEventArgs)
            If Not String.IsNullOrEmpty(e.CommandName) Then
                Try
                    Dim currentUser As UserInfo = UserController.GetCurrentUserInfo()
                    ' accept friend request
                    RelationshipController.Instance.AcceptUserRelationship(Convert.ToInt32(e.CommandName))
                    ' update friends list
                    lvFriends.DataSource = GetFriendsSource(currentUser)
                    lvFriends.DataBind()
                    ' refresh
                    Page.Response.Redirect(Page.Request.Url.ToString(), True)
                Catch ex As Exception
                    ProcessModuleLoadException(Me, ex)
                End Try
            End If
        End Sub

        Protected Sub cmbPageSize_SelectedIndexChanged(ByVal o As Object, ByVal e As RadComboBoxSelectedIndexChangedEventArgs)
            lvFriends.PageSize = Integer.Parse(e.Value)
            lvFriends.CurrentPageIndex = 0
            lvFriends.Rebind()
        End Sub

        Public Function RedirectUserProfile(ByVal userId As Integer) As String
            Dim profileUrl As String = DotNetNuke.Common.Globals.LinkClick("UserID=" & userId, Me.TabId, Me.ModuleId)
            Return "window.location='" & profileUrl & "'"
        End Function

        Public Function MakeVisible(ByVal status As String) As String
            If status.ToString().Equals("Accepted") Then
                Return "True"
            Else
                Return "False"
            End If
        End Function

        Public Function MakeAcceptFriendRequestVisible(ByVal status As String, ByVal userId As Integer) As String
            If status.ToString().Equals("Accepted") Then
                Return "False"
            Else
                Dim currentUser As UserInfo = UserController.GetCurrentUserInfo()
                Dim relationUser As UserInfo = UserController.GetUserById(Me.PortalId, userId)
                Dim relationship As UserRelationship = RelationshipController.Instance.GetFriendRelationship(currentUser, relationUser)
                If currentUser.UserID <> relationship.UserId Then
                    ' current user not initialize
                    Return "True"
                Else
                    Return "False"
                End If
            End If
        End Function


        Public Function GetFriendsSource(ByVal currentUser As UserInfo) As ArrayList
            Dim friendSourceList As ArrayList = New ArrayList()
            ' get friends
            ' beta api
            'Dim relationshipList As List(Of UserRelationship) = RelationshipController.Instance.GetFriends(currentUser)
            Dim relationshipList As List(Of UserRelationship) = RelationshipController.Instance.GetUserRelationships(currentUser)
            If relationshipList IsNot Nothing Then

                For Each relation As UserRelationship In relationshipList
                    Dim friendInfo As UserInfo
                    ' check who makes request 
                    If currentUser.UserID = relation.UserId Then
                        friendInfo = UserController.GetUserById(Me.PortalId, relation.RelatedUserId)
                    Else
                        friendInfo = UserController.GetUserById(Me.PortalId, relation.UserId)
                    End If
                    ' get updated relationship
                    Dim updatedRelation As UserRelationship = RelationshipController.Instance.GetFriendRelationship(currentUser, friendInfo)
                    If updatedRelation IsNot Nothing Then
                        Dim friendSource As LinksFriend = New LinksFriend()
                        With friendSource
                            .UserID = friendInfo.UserID
                            .PhotoUrl = friendInfo.Profile.PhotoURL
                            .UserName = friendInfo.Username
                            .DisplayName = friendInfo.DisplayName
                            .UserFirstName = friendInfo.FirstName
                            .UserLastName = friendInfo.LastName
                            .UserFullName = friendInfo.FullName
                            .UserRelationshipID = relation.UserRelationshipId
                        End With
                        If updatedRelation.Status.ToString.Equals("Accepted") Then
                            friendSource.Status = updatedRelation.Status.ToString()
                        Else
                            If currentUser.UserID <> relation.UserId Then
                                ' current user not initialize
                                friendSource.Status = "Receive Request from " & friendInfo.Username
                            Else
                                friendSource.Status = "Request " & updatedRelation.Status.ToString()
                            End If
                        End If
                        friendSourceList.Add(friendSource)
                    End If
                Next
            End If
            Return friendSourceList
        End Function

#End Region

#Region " Optional Interfaces "

        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get

                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection

                Select Case Me.ModuleContentType

                    Case Enums.ModuleContentTypes.Links

                        Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "add.gif", EditUrl(), False, Security.SecurityAccessLevel.Edit, True, False)

                End Select

                Return Actions

            End Get
        End Property

#End Region

    End Class

End Namespace
