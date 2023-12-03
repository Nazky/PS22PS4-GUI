Imports System.IO

Public Class TextFileCreator
    Public Sub CreateTextFile(filePath As String, content As String)
        Using writer As New StreamWriter(filePath)
            writer.Write(content)
        End Using
    End Sub
End Class
