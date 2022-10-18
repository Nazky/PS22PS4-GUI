Imports System.IO
Imports System
Imports System.IO.Compression
Imports DiscUtils.Iso9660
Imports System.Text
Imports System.Net
Imports HtmlAgilityPack

Public Class Form1
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Directory.CreateDirectory("bin")
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

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
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
                Dim gn = InputBox("Please put the name of the game.", "PS22PS4-GUI", "Game name here")
                If gn = "" Or gn = "Game name here" Then
                    MsgBox("Not valid name found", MsgBoxStyle.Critical, "PS22PS4-GUI")
                Else
                    MsgBox(gn)
                End If
            Else
                MsgBox("Please put valid path !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Sub ISO2PKG()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim oo As New OpenFileDialog
        oo.Title = "Choose a PS2 ISO"
        oo.Filter = "ISO file (*.iso) | *.iso"
        If oo.ShowDialog = DialogResult.OK Then
            ExtractPS2.info(oo.FileName)
            TextBox1.Text = oo.FileName
        End If
    End Sub

    Private Sub Label5_DoubleClick(sender As Object, e As EventArgs) Handles Label5.DoubleClick
        If Label5.Text = "Game ID: N/A" Then
            MsgBox("Please choose a ISO first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            My.Computer.Clipboard.SetText(Label5.Text.Replace("Game ID: ", ""))
            MsgBox(Label5.Text.Replace("Game ID: ", "") & " copy to clipboard", MsgBoxStyle.Information, "PS22PS4-GUI")
        End If
    End Sub
End Class
