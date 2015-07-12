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
Imports System.Configuration
Imports System.Data

Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Links

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LinkInfo Class provides the Links Business Object
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/23/2004	Moved Links to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LinkInfo
        Implements IHydratable

#Region "Private Members"

        Private _ItemId As Integer
        Private _ModuleId As Integer
        Private _Title As String
        Private _Url As String
        Private _ViewOrder As Integer
        Private _Description As String
        Private _CreatedByUser As Integer
        Private _CreatedDate As Date
        Private _TrackClicks As Boolean
        Private _NewWindow As Boolean
        Private _RefreshInterval As Integer = 0
        Private _GrantRoles As String
        Private _ImageURL As String

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

#Region "Properties"

        Public Property ItemId() As Integer
            Get
                Return _ItemId
            End Get
            Set(ByVal Value As Integer)
                _ItemId = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal Value As String)
                _Title = Value
            End Set
        End Property

        Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal Value As String)
                _Url = Value
            End Set
        End Property

        Public Property ViewOrder() As Integer
            Get
                Return _ViewOrder
            End Get
            Set(ByVal Value As Integer)
                _ViewOrder = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        Public Property CreatedByUser() As Integer
            Get
                Return _CreatedByUser
            End Get
            Set(ByVal Value As Integer)
                _CreatedByUser = Value
            End Set
        End Property

        Public Property CreatedDate() As Date
            Get
                Return _CreatedDate
            End Get
            Set(ByVal Value As Date)
                _CreatedDate = Value
            End Set
        End Property

        Public Property TrackClicks() As Boolean
            Get
                Return _TrackClicks
            End Get
            Set(ByVal Value As Boolean)
                _TrackClicks = Value
            End Set
        End Property

        Public Property NewWindow() As Boolean
            Get
                Return _NewWindow
            End Get
            Set(ByVal Value As Boolean)
                _NewWindow = Value
            End Set
        End Property

        Public ReadOnly Property RefreshContent() As Boolean
            Get

                If RefreshInterval > 0 Then
                    Return True
                End If

                Return False

            End Get
        End Property

        Public Property RefreshInterval() As Integer
            Get
                Return _RefreshInterval
            End Get
            Set(ByVal value As Integer)
                _RefreshInterval = value
            End Set
        End Property

        Public Property GrantRoles() As String
            Get
                Return _GrantRoles
            End Get
            Set(ByVal value As String)
                _GrantRoles = value
            End Set
        End Property

        Public Property ImageURL() As String
            Get
                Return _ImageURL
            End Get
            Set(ByVal value As String)
                _ImageURL = value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fill hydrates the object from a Datareader
        ''' </summary>
        ''' <remarks>The Fill method is used by the CBO method to hydrate the object
        ''' rather than using the more expensive Refection  methods.</remarks>
        ''' <history>
        ''' 	[erikvb]	06/02/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill
            ItemId = Convert.ToInt32(Null.SetNull(dr("ItemId"), ItemId))
            ModuleId = Convert.ToInt32(Null.SetNull(dr("ModuleId"), ModuleId))
            Title = Convert.ToString(Null.SetNull(dr("Title"), Title))
            Url = Convert.ToString(Null.SetNull(dr("URL"), Url))
            ViewOrder = Convert.ToInt32(Null.SetNull(dr("ViewOrder"), ViewOrder))
            Description = Convert.ToString(Null.SetNull(dr("Description"), Description))
            CreatedByUser = Convert.ToInt32(Null.SetNull(dr("CreatedByUser"), CreatedByUser))
            RefreshInterval = Convert.ToInt32(Null.SetNull(dr("RefreshInterval"), RefreshInterval))
            CreatedDate = Convert.ToDateTime(Null.SetNull(dr("CreatedDate"), CreatedDate))
            TrackClicks = Convert.ToBoolean(Null.SetNull(dr("TrackClicks"), TrackClicks))
            NewWindow = Convert.ToBoolean(Null.SetNull(dr("NewWindow"), NewWindow))
            GrantRoles = Convert.ToString(Null.SetNull(dr("GrantRoles"), GrantRoles))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <remarks>The KeyID property is part of the IHydratble interface.  It is used
        ''' as the key property when creating a Dictionary</remarks>
        ''' <history>
        ''' 	[erikvb]	06/02/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return ItemId
            End Get
            Set(ByVal value As Integer)
                ItemId = value
            End Set
        End Property
#End Region

    End Class

    Public Class TargetInfo

#Region " Constructors "

        Public Sub New(ByVal Url As String)
            Me._Url = Url
        End Sub

#End Region

#Region " Private Members "



#End Region

#Region " Properties "

        Private _Url As String
        Public ReadOnly Property Url() As String
            Get
                Return _Url
            End Get
        End Property

        Private _Description As String
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        Private _Title As String
        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal value As String)
                _Title = value
            End Set
        End Property

#End Region

    End Class

End Namespace
