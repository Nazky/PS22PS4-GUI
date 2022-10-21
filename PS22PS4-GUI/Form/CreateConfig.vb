Public Class CreateConfig
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            System.IO.File.WriteAllText("bin\configs\" & My.Settings.GID & ".conf", RichTextBox1.Text)
            MsgBox("LUA saved to" & vbCrLf & Application.StartupPath & "bin\configs\" & My.Settings.GID & ".conf", MsgBoxStyle.Information, "PS22PS4-GUI")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Private Sub CreateConfig_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.GID = "" Then
            MsgBox("Please import a ISO first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            Me.Close()
        Else
            Me.Text = Me.Text & My.Settings.GID
        End If
    End Sub
End Class