Imports Microsoft.VisualBasic

Namespace DotNetNuke.Modules.Links
    Public Class LinksFriend
        Private _photoUrl As String
        Public Property PhotoUrl() As String
            Get
                Return _photoUrl
            End Get
            Set(ByVal value As String)
                _photoUrl = value
            End Set
        End Property

        Private _userID As Integer
        Public Property UserID() As Integer
            Get
                Return _userID
            End Get
            Set(ByVal value As Integer)
                _userID = value
            End Set
        End Property

        Private _userName As String
        Public Property UserName() As String
            Get
                Return _userName
            End Get
            Set(ByVal value As String)
                _userName = value
            End Set
        End Property

        Private _displayName As String
        Public Property DisplayName() As String
            Get
                Return _displayName
            End Get
            Set(ByVal value As String)
                _displayName = value
            End Set
        End Property

        Private _userFullName As String
        Public Property UserFullName() As String
            Get
                Return _userFullName
            End Get
            Set(ByVal value As String)
                _userFullName = value
            End Set
        End Property

        Private _userFirstName As String
        Public Property UserFirstName() As String
            Get
                Return _userFirstName
            End Get
            Set(ByVal value As String)
                _userFirstName = value
            End Set
        End Property

        Private _userLastName As String
        Public Property UserLastName() As String
            Get
                Return _userLastName
            End Get
            Set(ByVal value As String)
                _userLastName = value
            End Set
        End Property


        Private _status As String
        Public Property Status() As String
            Get
                Return _status
            End Get
            Set(ByVal value As String)
                _status = value
            End Set
        End Property

        Private _userRelationshipID As Integer
        Public Property UserRelationshipID() As Integer
            Get
                Return _userRelationshipID
            End Get
            Set(ByVal value As Integer)
                _userRelationshipID = value
            End Set
        End Property

    End Class
End Namespace