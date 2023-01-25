Imports System.IO
Imports System
Imports System.IO.Compression
Imports DiscUtils.Iso9660
Imports System.Text
Imports System.Net
Imports HtmlAgilityPack
Imports System.Threading
Imports System.Drawing.Imaging

Public Class Form1
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Dim msgb = MsgBox("Do you really want to close the software ?", MsgBoxStyle.Information + MsgBoxStyle.YesNo)
        If msgb = MsgBoxResult.Yes Then
            Me.Close()
        End If
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
        'Directory.CreateDirectory("bin\mpkg")
        Directory.CreateDirectory("bin\lang")
        Directory.CreateDirectory("bin\configs")
        Directory.CreateDirectory("bin\LUA")
        listemu()
        checknet()
        My.Settings.Reset()
    End Sub

    Sub listemu()
        For Each pkg As String In Directory.GetFiles("bin\emulators")
            If pkg.Contains(".pkg") Then
                ToolStripComboBox1.Items.Add(Path.GetFileNameWithoutExtension(pkg))
            End If
        Next
    End Sub

    Sub checknet()
        Try
            IO.File.WriteAllText("s.bat", "dotnet --list-runtimes > r.txt")
            Shell("s.bat", AppWinStyle.Hide, True)
            Dim r As String = IO.File.ReadAllText("r.txt")
            If r.Contains("Microsoft.NETCore.App 6") Then
                IO.File.Delete("r.txt")
                IO.File.Delete("s.bat")
            Else
                MsgBox(".net 6.0 Runtime not found please install it first !", MsgBoxStyle.Critical)
                Me.Invoke(Sub() Me.WindowState = FormWindowState.Minimized)
                Shell("winget install Microsoft.DotNet.DesktopRuntime.6 -h", AppWinStyle.NormalFocus, True)
                Me.Invoke(Sub() Me.WindowState = FormWindowState.Normal)

                IO.File.Delete("r.txt")
                IO.File.Delete("s.bat")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
            MsgBox("Can't install .NET 6 using winget" & vbCr & "Please intall the Desktop Runtime manually.", MsgBoxStyle.Critical)
            Process.Start("https://download.visualstudio.microsoft.com/download/pr/4c5e26cf-2512-4518-9480-aac8679b0d08/523f1967fd98b0cf4f9501855d1aa063/windowsdesktop-runtime-6.0.13-win-x64.exe")
            IO.File.Delete("r.txt")
            IO.File.Delete("s.bat")
            Me.Close()
        End Try


    End Sub

    Private Sub TextBox1_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox1_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim i = 0
        TextBox1.Text = ""
        Label15.Text = "Disc number: 0"
        For Each path In files
            If i = 5 Then
                MsgBox("5 ISO/BIN LIMIT !!!", MsgBoxStyle.Critical, "PS22PS4-GUI")
            Else
                TextBox1.Text = TextBox1.Text & files(i) & vbCrLf
                i += 1
            End If

        Next

        Dim fl = Split(TextBox1.Text, vbCrLf)
        Label15.Text = Label15.Text.Replace("Disc number: 0", "Disc number: " & fl.Length - 1)
        Dim b As Thread = New Thread(Sub() ExtractPS2.info(fl(0), Me))
        b.IsBackground = True
        b.SetApartmentState(ApartmentState.STA)
        b.Start()

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
            If TextBox2.Text = "Drag and drop here (optional)" Or File.Exists(TextBox2.Text) And File.Exists(TextBox3.Text) And Directory.Exists(TextBox4.Text) Then

                If TextBox2.Text = "Drag and drop here (optional)" Then
                    PictureBox2.Image.Save("back.jpg", ImageFormat.Jpeg)
                    Dim b As Thread = New Thread(Sub() PS22PS4.CreatePKG(TextBox1.Text, Label10.Text, Label9.Text, Label11.Text, Label12.Text, Chr(34) & TextBox3.Text & Chr(34), "back.jpg", My.Settings.Config, My.Settings.LUA, RichTextBox1, TextBox4.Text, Me))
                    b.IsBackground = True
                    b.SetApartmentState(ApartmentState.STA)
                    b.Start()

                Else
                    Dim b As Thread = New Thread(Sub() PS22PS4.CreatePKG(TextBox1.Text, Label10.Text, Label9.Text, Label11.Text, Label12.Text, TextBox3.Text, TextBox2.Text, My.Settings.Config, My.Settings.LUA, RichTextBox1, TextBox4.Text, Me))
                    b.IsBackground = True
                    b.SetApartmentState(ApartmentState.STA)
                    b.Start()
                End If
                'MsgBox(gn)
            Else
                MsgBox("Please put valid path !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

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

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs)

    End Sub

    Private Sub ToolStripComboBox1_Click(sender As Object, e As EventArgs) Handles ToolStripComboBox1.Click

    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles Label13.Click

    End Sub

    Private Sub TextBox5_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox5.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TextBox5_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox5.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        If files(0).Contains(".PKG") Or files(0).Contains(".pkg") Then
            For Each path In files
                TextBox5.Text = files(0)
                Button1.Enabled = True
            Next
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox5.Text = "Drag and drop here" Or TextBox6.Text = "Drag and drop here" Then
            MsgBox("Please complete all path !", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            Dim b As Thread = New Thread(Sub() PS42PS2.ExtractPKG(TextBox5.Text, TextBox6.Text, RichTextBox2, Me))
            b.IsBackground = True
            b.SetApartmentState(ApartmentState.STA)
            b.Start()
        End If

    End Sub

    Private Sub TextBox6_DragDrop(sender As Object, e As DragEventArgs) Handles TextBox6.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        If Directory.Exists(files(0)) Then
            For Each path In files
                TextBox6.Text = files(0)
                Button1.Enabled = True
            Next
        End If
    End Sub

    Private Sub TextBox6_DragEnter(sender As Object, e As DragEventArgs) Handles TextBox6.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub EmulatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EmulatorToolStripMenuItem.Click
        If Label10.Text = "N/A" Then
            MsgBox("Please import a ISO first", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            Dim ic As New OpenFileDialog
            ic.Title = "Choose a config for the current game"
            ic.Filter = "Text file (*.txt) | *.txt|Config file (*.conf) | *.conf"
            If ic.ShowDialog = DialogResult.OK Then
                'MsgBox(ic.FileName)
                Directory.CreateDirectory("bin/configs/emulator")
                If File.Exists("bin/configs/emulator/" & My.Settings.GID & ".conf") Then
                    Dim ae = MsgBox("Config aleardy exist do you want to replace ?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, "PS22PS4-GUI")
                    If ae = MsgBoxResult.Yes Then
                        File.Copy(ic.FileName, "bin/configs/emulator/" & My.Settings.GID & ".conf", True)
                    End If
                Else
                    File.Copy(ic.FileName, "bin/configs/emulator/" & My.Settings.GID & ".conf", True)
                End If
            End If
        End If
    End Sub

    Private Sub MultiDiscToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MultiDiscToolStripMenuItem.Click
        If Label10.Text = "N/A" Then
            MsgBox("Please import a ISO first", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            Dim ic As New OpenFileDialog
            ic.Title = "Choose a config for the current game"
            ic.Filter = "Config file (*.conf) | *.conf"
            If ic.ShowDialog = DialogResult.OK Then
                'MsgBox(ic.FileName)
                Directory.CreateDirectory("bin/configs/multi-disc")
                If File.Exists("bin/configs/multi-disc/" & My.Settings.GID & "_cli.conf") Then
                    Dim ae = MsgBox("Config aleardy exist do you want to replace ?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, "PS22PS4-GUI")
                    If ae = MsgBoxResult.Yes Then
                        File.Copy(ic.FileName, "bin/configs/multi-disc/" & My.Settings.GID & "_cli.conf", True)
                    End If
                Else
                    File.Copy(ic.FileName, "bin/configs/multi-disc/" & My.Settings.GID & "_cli.conf", True)
                End If
            End If
        End If
    End Sub

    Private Sub PS3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PS3ToolStripMenuItem.Click
        If Label10.Text = "N/A" Then
            MsgBox("Please import a ISO first", MsgBoxStyle.Critical, "PS22PS4-GUI")
        Else
            Dim ic As New OpenFileDialog
            ic.Title = "Choose a config for the current game"
            ic.Filter = "Config file (*.cfgbin) | *.cfgbin"
            If ic.ShowDialog = DialogResult.OK Then
                'MsgBox(ic.FileName)
                Directory.CreateDirectory("bin/configs/PS3")
                If File.Exists("bin/configs/PS3/" & My.Settings.GID & "_lopnor.cfgbin") Then
                    Dim ae = MsgBox("Config aleardy exist do you want to replace ?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, "PS22PS4-GUI")
                    If ae = MsgBoxResult.Yes Then
                        File.Copy(ic.FileName, "bin/configs/PS3/" & My.Settings.GID & "_lopnor.cfgbin", True)
                    End If
                Else
                    File.Copy(ic.FileName, "bin/configs/PS3/" & My.Settings.GID & "_lopnor.cfgbin", True)
                End If
            End If
        End If
    End Sub

    Private Sub EmulatorToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EmulatorToolStripMenuItem1.Click
        CreateConfig.Show()
    End Sub

    Private Sub MultiDiscToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles MultiDiscToolStripMenuItem1.Click
        MsgBox("In the next update :)", MsgBoxStyle.Exclamation, "PS22PS4-GUI")
    End Sub

    Private Sub ExitToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem1.Click
        Dim msgb = MsgBox("Do you really want to close the software ?", MsgBoxStyle.Information + MsgBoxStyle.YesNo)
        If msgb = MsgBoxResult.Yes Then
            Me.Close()
        End If

    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click

    End Sub

    Private Sub GetMoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetMoreToolStripMenuItem.Click
        Process.Start("https://github.com/Nazky/PS22PS4-GUI/tree/ps2emu")
    End Sub
End Class
