Imports System.Net
Imports System.IO
Imports System.Diagnostics
Imports Newtonsoft.Json.Linq
Imports System.Net.Http

Public Class Updater
    Private ReadOnly owner As String
    Private ReadOnly repo As String
    Private ReadOnly currentVersion As Version
    Private ReadOnly httpClient As New HttpClient()

    Public Sub New(owner As String, repo As String, currentVersion As Version)
        Me.owner = owner
        Me.repo = repo
        Me.currentVersion = currentVersion

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PS22PS4-GUI")
    End Sub

    Public Function CheckForUpdates() As Boolean
        Dim apiUrl As String = $"https://api.github.com/repos/{owner}/{repo}/releases/latest"

        Try
            Dim response = httpClient.GetAsync(apiUrl).Result
            response.EnsureSuccessStatusCode()

            Dim responseContent = response.Content.ReadAsStringAsync().Result
            Dim releaseInfo = JObject.Parse(responseContent)

            Dim tagName As String = releaseInfo("tag_name").ToString()
            Dim latestVersion As Version = New Version(tagName)

            If latestVersion > currentVersion Then
                MsgBox("Update found please wait." & vbCrLf & "New version : " & latestVersion.ToString & ".", MsgBoxStyle.Information)
                Form1.Text = Form1.Text & " | No it's not stuck the update is currently being download please wait..."
                DownloadUpdate("https://github.com/Nazky/PS22PS4-GUI/releases/latest/download/PS22PS4-GUI.zip")
                Return True
            Else
                MsgBox("No update found." & vbCrLf & "Latest version : " & latestVersion.ToString & ".", MsgBoxStyle.Information)
            End If

            Return False
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

    Private Sub DownloadUpdate(downloadUrl As String)

        If Directory.Exists("update") Then
            Directory.Delete("update", True)
        End If

        Directory.CreateDirectory("update")

        Using webClient As New WebClient()
            webClient.DownloadFile(downloadUrl, "Update.zip")
        End Using

        Dim ze As New ZipExtractor
        ze.ExtractZipFile("Update.zip", "update")

        RestartApplication()
    End Sub

    Private Sub RestartApplication()

        Dim fileCreator As New TextFileCreator()
        fileCreator.CreateTextFile("updater.bat", "@echo off

set ""processName=PS22PS4-GUI.exe""

REM Terminate the process forcefully
taskkill /F /IM ""%processName%""

cd /d ""%~dp0""

set ""updateFolder=%cd%\update""
set ""currentFolder=%cd%""
set ""executable=PS22PS4-GUI.exe""

ren ""%currentFolder%\%executable%"" ""%executable%.bak""

ren ""%currentFolder%\%executable%.config"" ""%executable%.config.bak""

xcopy ""%updateFolder%\*"" ""%currentFolder%\"" /E /I /Y

start """" ""%currentFolder%\%executable%""

exit")
        Dim batchRunner As New BatchRunner()
        batchRunner.RunBatchFile("updater.bat")
    End Sub
End Class
