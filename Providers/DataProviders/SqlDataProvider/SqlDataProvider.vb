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
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Links

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The SqlDataProvider Class is an SQL Server implementation of the DataProvider Abstract
	''' class that provides the DataLayer for the Links Module.
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
	''' </history>
	''' -----------------------------------------------------------------------------
	Public Class SqlDataProvider

		Inherits DataProvider

#Region "Private Members"

		Private Const ProviderType As String = "data"
        Private Const ModuleQualifier As String = "dnnLinks_"

		Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
		Private _connectionString As String
		Private _providerPath As String
		Private _objectQualifier As String
		Private _databaseOwner As String

#End Region

#Region "Constructors"

		Public Sub New()

			' Read the configuration specific information for this provider
			Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            'Get Connection string from web.config
            _connectionString = Config.GetConnectionString()
            If _connectionString = "" Then
                ' Use connection string specified in provider
                _connectionString = objProvider.Attributes("connectionString")
            End If

            ' Read the attributes for this provider
            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

		End Sub

#End Region

#Region "Properties"

		Public ReadOnly Property ConnectionString() As String
			Get
				Return _connectionString
			End Get
		End Property

		Public ReadOnly Property ProviderPath() As String
			Get
				Return _providerPath
			End Get
		End Property

		Public ReadOnly Property ObjectQualifier() As String
			Get
				Return _objectQualifier
			End Get
		End Property

		Public ReadOnly Property DatabaseOwner() As String
			Get
				Return _databaseOwner
			End Get
		End Property

#End Region

#Region "Private Methods"

        Private Function GetFullyQualifiedName(ByVal name As String) As String
            Return DatabaseOwner & ObjectQualifier & ModuleQualifier & name
        End Function

        Private Function GetNull(ByVal Field As Object) As Object
            Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

#End Region

#Region "Public Methods"

        Public Overrides Function AddLink(ByVal ModuleId As Integer, ByVal UserId As Integer, ByVal CreatedDate As DateTime, ByVal Title As String, ByVal Url As String, ByVal ViewOrder As Integer, ByVal Description As String, ByVal RefreshInterval As Integer, ByVal GrantRoles As String) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("AddLink"), ModuleId, UserId, CreatedDate, Title, Url, ViewOrder, Description, RefreshInterval, GrantRoles), Integer)
        End Function

        Public Overrides Sub DeleteLink(ByVal ItemId As Integer, ByVal ModuleId As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteLink"), ItemId, ModuleId)
        End Sub

        Public Overrides Function GetLink(ByVal ItemId As Integer, ByVal ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetLink"), ItemId, ModuleId), IDataReader)
        End Function

        Public Overrides Function GetLinks(ByVal ModuleId As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetLinks"), ModuleId), IDataReader)
        End Function

        Public Overrides Sub UpdateLink(ByVal ItemId As Integer, ByVal UserId As Integer, ByVal CreatedDate As DateTime, ByVal Title As String, ByVal Url As String, ByVal ViewOrder As Integer, ByVal Description As String, ByVal RefreshInterval As Integer, ByVal GrantRoles As String)
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateLink"), ItemId, UserId, CreatedDate, Title, Url, ViewOrder, Description, RefreshInterval, GrantRoles)
        End Sub

#End Region

	End Class

End Namespace