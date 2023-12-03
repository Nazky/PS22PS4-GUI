Imports System.Diagnostics

Public Class BatchRunner
    Public Sub RunBatchFile(batchFilePath As String)
        Dim processStartInfo As New ProcessStartInfo With {
            .FileName = "cmd.exe",
            .Arguments = "/k """ & batchFilePath & """"
        }

        Dim process As New Process()
        process.StartInfo = processStartInfo
        process.Start()
    End Sub
End Class
