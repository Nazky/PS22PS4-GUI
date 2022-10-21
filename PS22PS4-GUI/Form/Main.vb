Imports System.IO
Imports System
Imports System.IO.Compression
Imports DiscUtils.Iso9660
Imports System.Text
Imports System.Net
Imports HtmlAgilityPack
Imports System.Threading

Public Class Form1
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Sub systemcmd(ByVal cmd As String, arg As String)
        Dim pHelp As New ProcessStartInfo
        pHelp.FileName = cmd
        pHelp.Arguments = arg
        pHelp.UseShellExecute = True
        pHelp.WindowStyle = ProcessWindowStyle.Hidden

        Dim proc As Process = Process.Start(pHelp)
        proc.WaitForExit()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Directory.CreateDirectory("bin\mpkg")
        Directory.CreateDirectory("bin\lang")
        Directory.CreateDirectory("bin\configs")
        Directory.CreateDirectory("bin\LUA")
        checknet()
        My.Settings.Reset()
    End Sub

    Sub checknet()
        File.WriteAllText("s.bat", "dotnet --list-runtimes > r.txt")
        Shell("s.bat", AppWinStyle.Hide, True)
        Dim r As String = File.ReadAllText("r.txt")
        If r.Contains("Microsoft.NETCore.App 5") Then
            File.Delete("r.txt")
            File.Delete("s.bat")
        Else
            MsgBox(".net 5.0 Runtime not found please install it first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            Process.Start("https://download.visualstudio.microsoft.com/download/pr/a0832b5a-6900-442b-af79-6ffddddd6ba4/e2df0b25dd851ee0b38a86947dd0e42e/dotnet-runtime-5.0.17-win-x64.exe")
            File.Delete("r.txt")
            File.Delete("s.bat")
            Me.Close()
        End If
    End Sub

    Private Sub TextBox1_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox1_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            TextBox1.Text = files(0)
        Next
        ExtractPS2.info(TextBox1.Text)
    End Sub

    Private Sub TextBox2_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox2.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox2_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox2.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            TextBox2.Text = files(0)
        Next
        PictureBox2.Image = Image.FromFile(TextBox2.Text)
    End Sub

    Private Sub TextBox3_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox3.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            TextBox3.Text = files(0)
        Next
        PictureBox1.Image = Image.FromFile(TextBox3.Text)
    End Sub

    Private Sub TextBox3_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox3.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        Dim spkg As New FolderBrowserDialog
        spkg.Description = "Choose a folder to save the pkg"
        If spkg.ShowDialog = DialogResult.OK Then
            TextBox4.Text = spkg.SelectedPath
        End If
    End Sub

    Private Sub TextBox4_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox4.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox4_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox4.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each path In files
            If Directory.Exists(files(0)) Then
                TextBox4.Text = files(0)
            End If
        Next
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            If File.Exists(TextBox1.Text) And File.Exists(TextBox2.Text) And File.Exists(TextBox3.Text) And Directory.Exists(TextBox4.Text) Then

                'MsgBox(gn)
                PS22PS4.CreatePKG(TextBox1.Text, Label10.Text, Label9.Text, Label11.Text, Label12.Text, TextBox3.Text, TextBox2.Text, My.Settings.Config, My.Settings.LUA, RichTextBox1, TextBox4.Text)


            Else
                MsgBox("Please put valid path !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Dim oo As New OpenFileDialog
        oo.Title = "Choose a PS2 ISO"
        oo.Filter = "ISO file (*.iso) | *.iso"
        If oo.ShowDialog = DialogResult.OK Then
            ExtractPS2.info(oo.FileName)
            TextBox1.Text = oo.FileName
        End If
    End Sub

    Private Sub Label5_DoubleClick(sender As Object, e As EventArgs) Handles Label5.DoubleClick

    End Sub

    Private Sub CheckUpdateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckUpdateToolStripMenuItem.Click
        MsgBox("In the next update ;)", MsgBoxStyle.Exclamation, "PS22PS4-GUI")
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged
        RichTextBox1.SelectionStart = RichTextBox1.Text.Length
        RichTextBox1.ScrollToCaret()
    End Sub

    Private Sub Installnet50ToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub CompatibilityListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CompatibilityListToolStripMenuItem.Click
        CL.Show()
    End Sub

    Private Sub CreditsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreditsToolStripMenuItem.Click
        Credit.Show()
    End Sub

    Private Sub ConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigToolStripMenuItem.Click
        If Label10.Text = "N/A" Then
            MsgBox("Please import a ISO first", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            Dim ic As New OpenFileDialog
            ic.Title = "Choose a config for the current game"
            ic.Filter = "Text file (*.txt) | *.txt|Config file (*.conf) | *.config"
            If ic.ShowDialog = DialogResult.OK Then
                'MsgBox(ic.FileName)
                My.Settings.Config = ic.FileName
            End If
        End If

    End Sub

    Private Sub LUAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LUAToolStripMenuItem.Click
        If Label10.Text = "N/A" Then
            MsgBox("Please import a ISO first", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            Dim ic As New OpenFileDialog
            ic.Title = "Choose a config for the current game"
            ic.Filter = "LUA file (*.lua) | *.lua"
            If ic.ShowDialog = DialogResult.OK Then
                'MsgBox(ic.FileName)
                My.Settings.LUA = ic.FileName
            End If
        End If
    End Sub

    Private Sub Label10_DoubleClick(sender As Object, e As EventArgs) Handles Label10.DoubleClick
        Try
            If Label10.Text = "" Then
                MsgBox("Please choose a ISO first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            Else
                My.Computer.Clipboard.SetText(Label10.Text, TextDataFormat.Text)
                MsgBox(Label10.Text & " copy to clipboard", MsgBoxStyle.Information, "PS22PS4-GUI")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click

    End Sub

    Private Sub Label9_DoubleClick(sender As Object, e As EventArgs) Handles Label9.DoubleClick
        Try
            If Label9.Text = "N/A" Then
                MsgBox("Please choose a ISO first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            Else
                My.Computer.Clipboard.SetText(Label9.Text, TextDataFormat.Text)
                MsgBox(Label9.Text & " copy to clipboard", MsgBoxStyle.Information, "PS22PS4-GUI")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Private Sub LUAToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles LUAToolStripMenuItem1.Click
        CreateLua.Show()
    End Sub

    Private Sub ConfigToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ConfigToolStripMenuItem1.Click
        CreateConfig.Show()
    End Sub
End Class
