Imports System.IO

Public Class CreateLua

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            System.IO.File.WriteAllText("bin\LUA\" & My.Settings.GID & "_config.lua", RichTextBox1.Text)
            MsgBox("LUA saved to" & vbCrLf & Application.StartupPath & "\bin\LUA\" & My.Settings.GID & "_config.lua", MsgBoxStyle.Information, "PS22PS4-GUI")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Private Sub CreateLua_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.GID = "" Then
            MsgBox("Please import a ISO first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            Me.Close()
        Else
            Me.Text = Me.Text & My.Settings.GID
        End If
    End Sub

    Private Sub RichTextBox1_DragEnter(sender As Object, e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub RichTextBox1_DragDrop(sender As Object, e As DragEventArgs)
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            If Directory.Exists(files(0)) Then
                RichTextBox1.Text = System.IO.File.ReadAllText(files(0))
            End If
        Next
    End Sub
End Class