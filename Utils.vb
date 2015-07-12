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

    Public Class Utils

        Public Shared Function GetImageURL(ByVal FileExtension As String) As String

            Dim result As String = String.Empty
            Dim baseURL As String = "~/images/FileManager/Icons/"

            Select Case FileExtension

                Case "bmp"
                    result = baseURL & "bmp.gif"
                Case "avi"
                    result = baseURL & "avi.gif"
                Case "doc", "docx"
                    result = baseURL & "doc.gif"
                Case "exe"
                    result = baseURL & "exe.gif"
                Case "gif"
                    result = baseURL & "gif.gif"
                Case "html", "htm"
                    result = baseURL & "htm.gif"
                Case "jpg"
                    result = baseURL & "jpg.gif"
                Case "js"
                    result = baseURL & "js.gif"
                Case "move"
                    result = baseURL & "move.gif"
                Case "mp3"
                    result = baseURL & "mp3.gif"
                Case "mpeg", "mpg"
                    result = baseURL & "mpeg.gif"
                Case "pdf"
                    result = baseURL & "pdf.gif"
                Case "ppt", "pptx"
                    result = baseURL & "ppt.gif"
                Case "tif"
                    result = baseURL & "tif.gif"
                Case "txt"
                    result = baseURL & "txt.gif"
                Case "vb", "vbs"
                    result = baseURL & "vb.gif"
                Case "wav"
                    result = baseURL & "wav.gif"
                Case "xls", "xlsx"
                    result = baseURL & "xls.gif"
                Case "xml"
                    result = baseURL & "xml.gif"
                Case "zip"
                    result = baseURL & "zip.gif"
                Case Else
                    result = baseURL & "txt.gif"
            End Select

            Return result

        End Function

        Public Shared Function GetFileSizeString(ByVal FileSize As Integer) As String

            Dim result As String = String.Empty

            If FileSize < 100 Then

                result = CStr(FileSize) & " B"

            ElseIf FileSize >= 100 And FileSize < 100000 Then

                result = (FileSize / 1000).ToString("0.0") & " KB"

            ElseIf FileSize >= 100000 Then

                result = (FileSize / 1000000).ToString("0.0") & " MB"

            End If

            Return result

        End Function

    End Class

End Namespace
