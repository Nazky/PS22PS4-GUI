Imports System.IO
Imports System.IO.Compression

Public Class ZipExtractor
    Public Sub ExtractZipFile(zipFilePath As String, extractPath As String)
        Using archive As ZipArchive = ZipFile.OpenRead(zipFilePath)
            For Each entry As ZipArchiveEntry In archive.Entries
                Dim entryPath As String = Path.Combine(extractPath, entry.FullName)

                If entry.FullName.EndsWith("/") OrElse entry.FullName.EndsWith("\") Then
                    Directory.CreateDirectory(entryPath)
                Else
                    Directory.CreateDirectory(Path.GetDirectoryName(entryPath))
                    entry.ExtractToFile(entryPath, True)
                End If
            Next
        End Using
    End Sub
End Class
