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

Imports System
Imports DotNetNuke

Namespace DotNetNuke.Modules.Links

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DataProvider Class Is an abstract class that provides the DataLayer
    ''' for the Links Module.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            ' [alex - 09/01/09] removed  object assembly name (required for WPA projects)
            objProvider = CType(Framework.Reflection.CreateObject("data", "DotNetNuke.Modules.Links", ""), DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

#End Region

#Region "Abstract methods"

        Public MustOverride Function AddLink(ByVal ModuleId As Integer, ByVal UserId As Integer, ByVal CreatedDate As DateTime, ByVal Title As String, ByVal Url As String, ByVal ViewOrder As Integer, ByVal Description As String, ByVal RefreshInterval As Integer, ByVal GrantRoles As String) As Integer
        Public MustOverride Sub DeleteLink(ByVal ItemID As Integer, ByVal ModuleId As Integer)
        Public MustOverride Function GetLink(ByVal ItemID As Integer, ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Function GetLinks(ByVal ModuleId As Integer) As IDataReader
        Public MustOverride Sub UpdateLink(ByVal ItemId As Integer, ByVal UserId As Integer, ByVal CreatedDate As DateTime, ByVal Title As String, ByVal Url As String, ByVal ViewOrder As Integer, ByVal Description As String, ByVal RefreshInterval As Integer, ByVal GrantRoles As String)

#End Region

    End Class
End Namespace

