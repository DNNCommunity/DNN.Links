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

Imports DotNetNuke.Services.Search
Imports DotNetNuke.Common.Utilities.XmlUtils
Imports DotNetNuke.Entities.Tabs

Imports System
Imports System.Configuration
Imports System.Data
Imports System.Text
Imports System.xml

Namespace DotNetNuke.Modules.Links
 

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.Links
    ''' Project:    DotNetNuke
    ''' Class:      LinkController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LinkController Class represents the Links Business Layer
    ''' Methods in this class call methods in the Data Layer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LinkController
        Implements Entities.Modules.ISearchable
        Implements Entities.Modules.IPortable


#Region "Public Methods"


        Public Sub AddLink(ByVal objLink As LinkInfo)

            DataProvider.Instance().AddLink(objLink.ModuleId, objLink.CreatedByUser, objLink.CreatedDate, objLink.Title, objLink.Url, objLink.ViewOrder, objLink.Description, objLink.RefreshInterval, objLink.GrantRoles)

        End Sub

        Public Sub DeleteLink(ByVal ItemID As Integer, ByVal ModuleId As Integer)

            DataProvider.Instance().DeleteLink(ItemID, ModuleId)

        End Sub

        Public Sub DeleteLinkIfItExistsForModule(ByVal ModuleId As Integer, ByVal objLink as LinkInfo)
            
            For Each oldLink In GetLinks(ModuleId)
                If oldLink.Title = objLink.Title And oldLink.Url = objLink.Url
                    DeleteLink(oldLink.ItemID, ModuleId)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Function evaluates the targets content and tries to build a summary about the targets title and description.
        ''' </summary>
        ''' <param name="Url">Targets URL</param>
        ''' <returns>A short summary of the content</returns>
        ''' <remarks>finished</remarks>
        ''' <history>
        ''' 	[alex]	9/1/2009	Implementing functionality
        ''' </history>
        Public Function GetTargetContent(ByVal Url As String) As TargetInfo
            ' verify passed arguments
            If String.IsNullOrEmpty(Url) Then Throw New ArgumentNullException("URL", "Necessary parameter for reading content from a target is missing. Execution aborted.")

            Using webClient As New System.Net.WebClient
                ' use proxy if defined in host settings

                If Not String.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.ProxyServer) Then
                    Dim proxyPort As Integer
                    If Not Integer.TryParse(DotNetNuke.Entities.Host.Host.ProxyPort, proxyPort) Then Throw New InvalidCastException("ProxyPort", "Invalid proxy port in dotnetnuke host settings. Port must be an integer!")

                    Dim proxy As New System.Net.WebProxy(DotNetNuke.Entities.Host.Host.ProxyServer, proxyPort)
                    webClient.Proxy = proxy
                End If

                Dim contentStream As IO.Stream = webClient.OpenRead(Url)
                Dim contentReader As New IO.StreamReader(contentStream, True)

                Dim contentInformation As String = String.Empty
                Dim endTagFound As Boolean
                Dim startTagFound As Boolean

                Dim targetInfo As New TargetInfo(Url)

                While Not contentReader.EndOfStream
                    Dim currentContent = contentReader.ReadLine()
                    Dim addedStartTag As Boolean = False

                    If currentContent.Contains("<head") Then
                        ' start recording information
                        startTagFound = True
                    End If

                    If startTagFound Then
                        contentInformation &= currentContent
                    End If

                    If contentInformation.Contains("</head") Then
                        ' stop recording information and abort streaming after content evaluation
                        endTagFound = True
                    End If

                    If startTagFound AndAlso endTagFound Then
                        ' evaluate targets content
                        If contentInformation.ToLower.Contains("<meta") AndAlso contentInformation.ToLower.Contains("name=""description""") Then
                            ' iterate through meta tags and try to find the description meta element
                            Dim tmpContentInformation As String = contentInformation
                            While tmpContentInformation.ToLower.Contains("<meta")
                                Dim meta As String = tmpContentInformation.Remove(0, tmpContentInformation.IndexOf("<meta"))
                                meta = meta.Remove(meta.IndexOf(">"))

                                If meta.ToLower.Contains("name=""description""") Then
                                    targetInfo.Description = meta.Remove(0, meta.ToLower.IndexOf("content=""") + 9)
                                    targetInfo.Description = targetInfo.Description.Remove(targetInfo.Description.IndexOf(""""))
                                    targetInfo.Description = HttpUtility.HtmlDecode(targetInfo.Description.Trim)
                                End If

                                tmpContentInformation = tmpContentInformation.Remove(tmpContentInformation.IndexOf("<meta"), +5)
                            End While

                        End If

                        If contentInformation.ToLower.Contains("<title>") AndAlso contentInformation.ToLower.Contains("</title>") Then

                            ' try to get targets title
                            targetInfo.Title = contentInformation.Substring(contentInformation.IndexOf("<title>") + 7)
                            targetInfo.Title = targetInfo.Title.Remove(targetInfo.Title.IndexOf("</title>"))
                            targetInfo.Title = HttpUtility.HtmlDecode(targetInfo.Title).Trim

                        End If

                        Exit While
                    End If

                End While

                ' clean up memory
                contentReader.Close()
                contentReader.Dispose()
                contentStream.Close()
                contentStream.Dispose()

                Return targetInfo
            End Using

            Return Nothing
        End Function

        Public Function RefreshLinkContent(ByVal objLink As LinkInfo) As LinkInfo
            If Now.Subtract(New TimeSpan(0, objLink.RefreshInterval, 0)) >= objLink.CreatedDate Then
                Dim linkController As New LinkController
                Dim targetInfo As TargetInfo = linkController.GetTargetContent(objLink.Url)

                objLink.Title = targetInfo.Title
                objLink.Description = targetInfo.Description
                objLink.CreatedDate = Now

                linkController.UpdateLink(objLink)
            End If

            Return objLink
        End Function

        Public Function GetLinkByHigherViewOrder(ByVal ViewOrder As Integer, ByVal ModuleId As Integer) As String

            Dim prevOrder As Integer = Int32.MinValue

            For Each link As LinkInfo In GetLinks(ModuleId)
                If link.ViewOrder > prevOrder And link.ViewOrder < ViewOrder Then
                    prevOrder = link.ViewOrder
                End If
            Next
            If Not prevOrder = Int32.MinValue Then
                Return prevOrder
            Else
                Return ViewOrder
            End If
        End Function

        Public Sub UpdateViewOrder(ByVal objLink As LinkInfo, ByVal IncreaseValue As Integer, ByVal ModuleId As Integer)

            For Each link As LinkInfo In GetLinks(ModuleId)
                If IncreaseValue = -1 And link.ViewOrder <= objLink.ViewOrder And Not link.ItemId = objLink.ItemId Then
                    link.ViewOrder += IncreaseValue
                ElseIf IncreaseValue = 1 And link.ViewOrder >= objLink.ViewOrder And Not link.ItemId = objLink.ItemId Then
                    link.ViewOrder += IncreaseValue
                End If
                UpdateLink(link)
            Next
        End Sub

        Public Function GetLink(ByVal ItemID As Integer, ByVal ModuleId As Integer) As LinkInfo

            Return CType(CBO.FillObject(DataProvider.Instance().GetLink(ItemID, ModuleId), GetType(LinkInfo)), LinkInfo)

        End Function

        Public Function GetLinks(ByVal ModuleId As Integer) As ArrayList

            Return CBO.FillCollection(DataProvider.Instance().GetLinks(ModuleId), GetType(LinkInfo))

        End Function

        Public Sub UpdateLink(ByVal objLink As LinkInfo)

            DataProvider.Instance().UpdateLink(objLink.ItemId, objLink.CreatedByUser, objLink.CreatedDate, objLink.Title, objLink.Url, objLink.ViewOrder, objLink.Description, objLink.RefreshInterval, objLink.GrantRoles)

        End Sub

        Public Shared Function convertURLType(ByVal tabType As TabType) As String
            Select Case tabType
                Case Entities.Tabs.TabType.File
                    Return "F"
                Case Entities.Tabs.TabType.Member
                    Return "M"
                Case Entities.Tabs.TabType.Normal, Entities.Tabs.TabType.Tab
                    Return "T"
                Case Else
                    Return "U"
            End Select
        End Function



#End Region

#Region " Custom Hydrator "
        Private Function FillLink(ByVal dr As System.Data.IDataReader) As LinkInfo

            Dim linkInfo As LinkInfo = New LinkInfo

            While dr.Read

                linkInfo.ItemId = Convert.ToInt32(Null.SetNull(dr("ItemId"), linkInfo.ItemId))
                linkInfo.ModuleId = Convert.ToInt32(Null.SetNull(dr("ModuleId"), linkInfo.ModuleId))
                linkInfo.Title = Convert.ToString(Null.SetNull(dr("Title"), linkInfo.Title))
                linkInfo.Url = Convert.ToString(Null.SetNull(dr("URL"), linkInfo.Url))
                linkInfo.ViewOrder = Convert.ToInt32(Null.SetNull(dr("ViewOrder"), linkInfo.ViewOrder))
                linkInfo.Description = Convert.ToString(Null.SetNull(dr("Description"), linkInfo.Description))
                linkInfo.CreatedByUser = Convert.ToInt32(Null.SetNull(dr("CreatedByUser"), linkInfo.CreatedByUser))
                linkInfo.CreatedDate = Convert.ToDateTime(Null.SetNull(dr("CreatedDate"), linkInfo.CreatedDate))
                linkInfo.TrackClicks = Convert.ToBoolean(Null.SetNull(dr("TrackClicks"), linkInfo.TrackClicks))
                linkInfo.NewWindow = Convert.ToBoolean(Null.SetNull(dr("NewWindow"), linkInfo.NewWindow))
                linkInfo.RefreshInterval = Convert.ToInt32(Null.SetNull(dr("RefreshInterval"), linkInfo.RefreshInterval))

            End While

            Return linkInfo

        End Function

        Private Function FillLinks(ByVal dr As System.Data.IDataReader) As System.Collections.ArrayList

            Dim linkInfoList As New ArrayList

            While dr.Read

                Dim linkInfo As LinkInfo = New LinkInfo
                linkInfo.ItemId = Convert.ToInt32(Null.SetNull(dr("ItemId"), linkInfo.ItemId))
                linkInfo.ModuleId = Convert.ToInt32(Null.SetNull(dr("ModuleId"), linkInfo.ModuleId))
                linkInfo.Title = Convert.ToString(Null.SetNull(dr("Title"), linkInfo.Title))
                linkInfo.Url = Convert.ToString(Null.SetNull(dr("URL"), linkInfo.Url))
                linkInfo.ViewOrder = Convert.ToInt32(Null.SetNull(dr("ViewOrder"), linkInfo.ViewOrder))
                linkInfo.Description = Convert.ToString(Null.SetNull(dr("Description"), linkInfo.Description))
                linkInfo.CreatedByUser = Convert.ToInt32(Null.SetNull(dr("CreatedByUser"), linkInfo.CreatedByUser))
                linkInfo.CreatedDate = Convert.ToDateTime(Null.SetNull(dr("CreatedDate"), linkInfo.CreatedDate))
                linkInfo.TrackClicks = Convert.ToBoolean(Null.SetNull(dr("TrackClicks"), linkInfo.TrackClicks))
                linkInfo.NewWindow = Convert.ToBoolean(Null.SetNull(dr("NewWindow"), linkInfo.NewWindow))
                linkInfo.RefreshInterval = Convert.ToInt32(Null.SetNull(dr("RefreshInterval"), linkInfo.RefreshInterval))
                linkInfoList.Add(linkInfo)

            End While

            Return linkInfoList

        End Function

#End Region

#Region " Optional Interfaces "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSearchItems implements the ISearchable Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        ''' <history>
        '''		[cnurse]	11/17/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems
            Dim SearchItemCollection As New SearchItemInfoCollection

            Dim Links As ArrayList = GetLinks(ModInfo.ModuleID)

            Dim objLink As Object
            For Each objLink In Links
                Dim SearchItem As SearchItemInfo
                With CType(objLink, LinkInfo)
                    SearchItem = New SearchItemInfo(ModInfo.ModuleTitle & " - " & .Title, .Description, .CreatedByUser, .CreatedDate, ModInfo.ModuleID, .ItemId.ToString, .Description, "ItemId=" & .ItemId.ToString, Null.NullInteger)
                    SearchItemCollection.Add(SearchItem)
                End With
            Next

            Return SearchItemCollection
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ExportModule implements the IPortable ExportModule Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The Id of the module to be exported</param>
        ''' <history>
        '''		[cnurse]	11/17/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
            Dim strXML As String = ""
            Dim sbXML As New StringBuilder

            Dim arrLinks As ArrayList = GetLinks(ModuleID)
            If arrLinks.Count <> 0 Then
                sbXML.Append("<links>")
                Dim objLink As LinkInfo
                For Each objLink In arrLinks
                    sbXML.Append("<link>")
                    sbXML.AppendFormat("<title>{0}</title>", XMLEncode(objLink.Title))
                    sbXML.AppendFormat("<url>{0}</url>", XMLEncode(objLink.Url))
                    sbXML.AppendFormat("<vieworder>{0}</vieworder>", XMLEncode(objLink.ViewOrder.ToString))
                    sbXML.AppendFormat("<description>{0}</description>", XMLEncode(objLink.Description))
                    sbXML.AppendFormat("<newwindow>{0}</newwindow>", XMLEncode(objLink.NewWindow.ToString))
                    sbXML.AppendFormat("<trackclicks>{0}</trackclicks>", XMLEncode(objLink.TrackClicks.ToString))
                    sbXML.Append("</link>")
                Next
                sbXML.Append("</links>")
            End If
            Return sbXML.ToString()
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ImportModule implements the IPortable ImportModule Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The Id of the module to be imported</param>
        ''' <history>
        '''		[cnurse]	11/17/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule
            Dim xmlLink As XmlNode
            Dim xmlLinks As XmlNode = GetContent(Content, "links")
            For Each xmlLink In xmlLinks.SelectNodes("link")
                Dim objLink As New LinkInfo
                objLink.ModuleId = ModuleID
                objLink.Title = xmlLink.Item("title").InnerText
                objLink.Url = ImportUrl(ModuleID, xmlLink.Item("url").InnerText)
                objLink.ViewOrder = Integer.Parse(xmlLink.Item("vieworder").InnerText)
                objLink.Description = xmlLink.Item("description").InnerText
                objLink.NewWindow = Boolean.Parse(xmlLink.Item("newwindow").InnerText)
                Try
                    objLink.TrackClicks = Boolean.Parse(xmlLink.Item("trackclicks").InnerText)
                Catch
                    objLink.TrackClicks = False
                End Try
                objLink.CreatedDate = DateTime.Now()
                objLink.CreatedByUser = UserId
                DeleteLinkIfItExistsForModule(ModuleID, objLink)
                AddLink(objLink)

                ' url tracking
                Dim objUrls As New UrlController
                objUrls.UpdateUrl(GetPortalSettings.PortalId, objLink.Url, convertURLType(GetURLType(objLink.Url)), False, objLink.TrackClicks, ModuleID, objLink.NewWindow)
            Next

        End Sub

#End Region

    End Class

End Namespace
