Imports System.IO
Imports System.Net.Mime.MediaTypeNames
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class PS22PS4
    <STAThread>
    Shared Sub CreatePKG(ISO As String, gid As String, gn As String, gv As String, gr As String, icon As String, background As String, config As String, LUA As String, rcht As RichTextBox, pkgf As String, frm As Form1)
        If frm.InvokeRequired Then
            'MsgBox("test")
            Try
                If Form1.ToolStripComboBox1.Text = "" Then
                    MsgBox("Please choose a emulator first !", MsgBoxStyle.Critical, "PS22PS4-GUI")
                Else
                    frm.Invoke(Sub() frm.NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info)
                    frm.Invoke(Sub() frm.NotifyIcon1.BalloonTipTitle = "Creating PS4 PKG")
                    frm.Invoke(Sub() frm.NotifyIcon1.BalloonTipText = "Sit back and relax, i inform you when it's done ;)")
                    frm.Invoke(Sub() frm.NotifyIcon1.ShowBalloonTip(1000))
                    frm.Invoke(Sub() Form1.Button5.Enabled = False)
                    frm.Invoke(Sub() Form1.ProgressBar1.Value = 0)
                    frm.Invoke(Sub() Form1.UseWaitCursor = True)
                    frm.Invoke(Sub() Form1.MenuStrip1.Enabled = False)
                    frm.Invoke(Sub() Form1.TabControl1.Enabled = False)
                    If pkgf = "Drag and drop here (optional)" Then
                        pkgf = Environment.CurrentDirectory & "\PS2-FPKG"
                    End If
                    If Directory.Exists(pkgf & "\Temp") Then
                        Directory.Delete(pkgf & "\Temp", True)
                    End If
                    frm.Invoke(Sub() rcht.Text = "ISO/BIN list: " & vbCrLf & ISO & vbCrLf & "Game ID: " & gid & vbCrLf & "Game Name: " & gn & vbCrLf & "Game version: " & gv.Replace(vbCrLf, "") & vbCrLf & "Game region: " & gr.Replace(vbCrLf, "") & vbCrLf & "Icon path: " & icon & vbCrLf & "Background path: " & background & vbCrLf & "Emulator: " & Form1.ToolStripComboBox1.Text & vbCrLf)
                    If config = "" Then
                        frm.Invoke(Sub() rcht.Text = rcht.Text & "config file: N/A" & vbCrLf)
                    Else
                        frm.Invoke(Sub() rcht.Text = rcht.Text & "config file: " & config & vbCrLf)
                    End If
                    If LUA = "" Then
                        frm.Invoke(Sub() rcht.Text = rcht.Text & "LUA file: N/A" & vbCrLf & "---------------------------------------------------------------------------------------------------" & vbCrLf)
                    Else
                        frm.Invoke(Sub() rcht.Text = rcht.Text & "LUA file: " & LUA & vbCrLf & "---------------------------------------------------------------------------------------------------" & vbCrLf)
                    End If
                End If

                dpkg(rcht, pkgf, frm)
                cps2(rcht, ISO, pkgf, frm)
                ic(rcht, pkgf, gn, frm)
                il(rcht, pkgf, gn, frm)
                setid(rcht, gid, pkgf, frm)
                setgn(rcht, gn, pkgf, frm)
                setc(rcht, icon, pkgf, frm)
                setb(rcht, background, pkgf, frm)
                compilpkg(rcht, pkgf, gn, frm)

                frm.Invoke(Sub() Form1.Button5.Enabled = True)
                frm.Invoke(Sub() Form1.ProgressBar1.Value = 100)
                frm.Invoke(Sub() Form1.UseWaitCursor = False)
                frm.Invoke(Sub() Form1.MenuStrip1.Enabled = True)
                frm.Invoke(Sub() Form1.TabControl1.Enabled = True)


            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
            End Try
        End If


    End Sub

    Shared Sub dpkg(rcht As RichTextBox, pkgf As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Decrypting emulator..." & vbCrLf)
            System.IO.Directory.CreateDirectory(pkgf & "\Temp")
            systemcmd("bin\tools\orbis-pub-cmd.exe", "img_extract --passcode 00000000000000000000000000000000 .\bin\emulators\" & Form1.ToolStripComboBox1.Text & ".pkg " & Chr(34) & pkgf & "\Temp" & Chr(34))
            My.Computer.FileSystem.RenameDirectory(pkgf & "\Temp\Sc0", "sce_sys")
            System.IO.File.Move(pkgf & "\Temp\sce_sys\param.sfo", pkgf & "\Temp\image0\sce_sys\param.sfo")
            System.IO.Directory.Delete(pkgf & "\Temp\sce_sys", True)
            frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try


    End Sub

    Shared Sub cps2(rcht As RichTextBox, ISO As String, pkgf As String, frm As Form1)
        Try
            If ISO.Contains(".iso") Or ISO.Contains(".ISO") Then
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing ISO..." & vbCrLf)
                Dim fl = Split(ISO, vbCrLf)
                Dim i = 0
                Dim D = 1
                For Each str As String In fl(i)
                    If fl(i) = "" Then
                    Else
                        System.IO.File.Copy(fl(i), pkgf & "\Temp\image0\image\disc0" & D & ".iso")
                        i += 1
                        D += 1
                    End If
                Next
                Dim confd As String = System.IO.File.ReadAllText(pkgf & "\Temp\image0\config-emu-ps4.txt")
                If confd.Contains("--max-disc-num=") Then
                    File.WriteAllText(pkgf & "\Temp\image0\config-emu-ps4.txt", confd.Replace("--max-disc-num=1", "--max-disc-num=" & D - 1))
                Else
                    File.AppendAllText(pkgf & "\Temp\image0\config-emu-ps4.txt", vbCrLf & "--max-disc-num=" & D - 1)
                End If
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
            ElseIf ISO.Contains(".bin") Or ISO.Contains(".BIN") Then
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing and converting BIN..." & vbCrLf)
                System.IO.File.Copy(ISO, pkgf & "\Temp\image0\image\disc01.iso")
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Patching BIN..." & vbCrLf)
                Dim crc = GetCRC32(pkgf & "\Temp\image0\image\disc01.iso")
                Dim LIMG = "4C494D4700000002FFFFFFFF00000930"
                Dim crcb As Byte() = stringToByteArray(LIMG.Replace("FFFFFFFF", crc))
                AppendByteToBIN(pkgf & "\Temp\image0\image\disc01.iso", crcb)
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
                'MsgBox("done")
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Public Shared Function stringToByteArray(text As String) As Byte()
        Dim bytes As Byte() = New Byte(text.Length \ 2 - 1) {}

        For i As Integer = 0 To text.Length - 1 Step 2
            bytes(i \ 2) = Byte.Parse(text(i).ToString() & text(i + 1).ToString(), System.Globalization.NumberStyles.HexNumber)
        Next

        Return bytes
    End Function

    Shared Sub AppendByteToBIN(ByVal FilepathToAppendTo As String, ByRef Content() As Byte)
        Dim s As New System.IO.FileStream(FilepathToAppendTo, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite)
        s.Write(Content, 0, Content.Length)
        s.Close()
    End Sub

    Shared Function GetCRC32(ByVal sFileName As String) As String
        Try
            Dim FS As FileStream = New FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
            Dim CRC32Result As Integer = &HFFFFFFFF
            Dim Buffer(4096) As Byte
            Dim ReadSize As Integer = 4096
            Dim Count As Integer = FS.Read(Buffer, 0, ReadSize)
            Dim CRC32Table(256) As Integer
            Dim DWPolynomial As Integer = &HEDB88320
            Dim DWCRC As Integer
            Dim i As Integer, j As Integer, n As Integer
            'Create CRC32 Table
            For i = 0 To 255
                DWCRC = i
                For j = 8 To 1 Step -1
                    If (DWCRC And 1) Then
                        DWCRC = ((DWCRC And &HFFFFFFFE) \ 2&) And &H7FFFFFFF
                        DWCRC = DWCRC Xor DWPolynomial
                    Else
                        DWCRC = ((DWCRC And &HFFFFFFFE) \ 2&) And &H7FFFFFFF
                    End If
                Next j
                CRC32Table(i) = DWCRC
            Next i
            'Calcualting CRC32 Hash
            Do While (Count > 0)
                For i = 0 To Count - 1
                    n = (CRC32Result And &HFF) Xor Buffer(i)
                    CRC32Result = ((CRC32Result And &HFFFFFF00) \ &H100) And &HFFFFFF
                    CRC32Result = CRC32Result Xor CRC32Table(n)
                Next i
                Count = FS.Read(Buffer, 0, ReadSize)
            Loop
            FS.Close()
            Return Hex(CRC32Result)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Shared Sub ic(rcht As RichTextBox, pkgf As String, gn As String, frm As Form1)
        Try
            If My.Settings.Config = "" Then
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing default config..." & vbCrLf)
                Dim confd As String = System.IO.File.ReadAllText(pkgf & "\Temp\image0\config-emu-ps4.txt")
                System.IO.File.WriteAllText(pkgf & "\Temp\image0\config-emu-ps4.txt", confd.Replace("#Markus95", "#" & gn.Replace("Game name: ", "").Replace("SLUS-20091", My.Settings.GID)))
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
            ElseIf System.IO.File.Exists("bin\configs\" & My.Settings.GID & ".conf") Then
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing custom config..." & vbCrLf)
                System.IO.File.Copy("bin\configs\" & My.Settings.GID & ".conf", pkgf & "\Temp\image0\config-emu-ps4.txt", True)
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
            Else
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing custom config..." & vbCrLf)
                System.IO.File.Copy(My.Settings.Config, "bin\configs\" & My.Settings.GID & ".conf", True)
                System.IO.File.Copy("bin\configs\" & My.Settings.GID & ".conf", pkgf & "\Temp\image0\config-emu-ps4.txt", True)
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub il(rcht As RichTextBox, pkgf As String, gn As String, frm As Form1)
        Try
            If My.Settings.LUA = "" Then
            ElseIf System.IO.File.Exists("bin\LUA\" & My.Settings.GID & "_config.lua") Then
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing LUA config..." & vbCrLf)
                System.IO.File.Copy("bin\LUA\" & My.Settings.GID & "_config.lua", pkgf & "\Temp\image0\patches\" & My.Settings.GID & "_config.lua", True)
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
            Else
                frm.Invoke(Sub() rcht.Text = rcht.Text & "Importing LUA config..." & vbCrLf)
                System.IO.File.Copy(My.Settings.LUA, "bin\LUA\" & My.Settings.GID & "_config.lua", True)
                System.IO.File.Copy("bin\LUA\" & My.Settings.GID & "_config.lua", pkgf & "\Temp\image0\patches\" & My.Settings.GID & "_config.lua", True)
                frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setid(rcht As RichTextBox, gid As String, pkgf As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Patching game id..." & vbCrLf)
            System.IO.Directory.CreateDirectory(pkgf & "\Temp")
            'MsgBox("""" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x31F --text " & """" & My.Settings.GID.Replace("-", "") & """")
            systemcmd("bin\tools\replhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x31F --text " & """" & My.Settings.GID.Replace("-", "") & """")
            systemcmd("bin\tools\replhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x670 --text " & """" & My.Settings.GID.Replace("-", "") & """")
            frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setgn(rcht As RichTextBox, gn As String, pkgf As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Patching game name..." & vbCrLf)
            systemcmd("bin\tools\replhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x5F0 --hex " & """00000000000000000000000000000000""")
            systemcmd("bin\tools\replhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x600 --hex " & """00000000000000000000000000000000""")
            systemcmd("bin\tools\replhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x5F0 --text " & """" & gn.Replace("Game name: ", "") & """")
            frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setc(rcht As RichTextBox, icon As String, pkgf As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Patching game cover..." & vbCrLf)
            'System.IO.File.Copy(icon, pkgf & "\Temp\image0\sce_sys\icon0.png")
            systemcmd("bin\tools\magick.exe", "convert  " & Chr(34) & icon & Chr(34) & " " & Chr(34) & pkgf & "\Temp\image0\sce_sys\icon0.jpg" & Chr(34))
            systemcmd("bin\tools\magick.exe", "convert  " & Chr(34) & pkgf & "\Temp\image0\sce_sys\icon0.jpg" & Chr(34) & " " & Chr(34) & pkgf & "\Temp\image0\sce_sys\icon0.png" & Chr(34))
            systemcmd("bin\tools\magick.exe", "mogrify -resize 512x512!  " & Chr(34) & pkgf & "\Temp\image0\sce_sys\icon0.png" & Chr(34))
            System.IO.File.Copy(pkgf & "\Temp\image0\sce_sys\icon0.png", pkgf & "\Temp\image0\sce_sys\save_data.png")
            systemcmd("bin\tools\magick.exe", "mogrify -resize 228x128!  " & pkgf & "\Temp\image0\sce_sys\save_data.png")
            frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setb(rcht As RichTextBox, back As String, pkgf As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Patching game background..." & vbCrLf)
            'System.IO.File.Copy(icon, pkgf & "\Temp\image0\sce_sys\icon0.png")
            systemcmd("bin\tools\magick.exe", "convert  " & Chr(34) & back & Chr(34) & " " & Chr(34) & pkgf & "\Temp\image0\sce_sys\pic1.jpg" & Chr(34))
            systemcmd("bin\tools\magick.exe", "mogrify -resize 1920x1080! " & Chr(34) & pkgf & "\Temp\image0\sce_sys\pic1.jpg" & Chr(34))
            systemcmd("bin\tools\magick.exe", "convert  " & Chr(34) & pkgf & "\Temp\image0\sce_sys\pic1.jpg" & Chr(34) & " " & Chr(34) & pkgf & "\Temp\image0\sce_sys\pic1.png" & Chr(34))
            frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setlua(rcht As RichTextBox, lua As String, pkgf As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Adding custom lua..." & vbCrLf)
            System.IO.File.Copy(lua, pkgf & "\Temp\image0\patches\" & System.IO.Path.GetFileName(lua), True)
            frm.Invoke(Sub() Form1.ProgressBar1.Value += 11)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub compilpkg(rcht As RichTextBox, pkgf As String, gn As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Compiling PKG..." & vbCrLf)
            systemcmd("bin\tools\gengp4.exe", Chr(34) & pkgf & "\Temp\image0" & Chr(34))
            'My.Computer.FileSystem.RenameFile(pkgf & "\Temp\image0.gp4", "image0.txt")
            Dim gp4 As String = System.IO.File.ReadAllText(pkgf & "\Temp\image0.gp4")
            System.IO.File.WriteAllText(pkgf & "\Temp\image0.gp4", gp4.Replace("<scenarios default_id=""1"">", "<scenarios default_id=""0"">"))
            systemcmd("bin\tools\orbis-pub-cmd.exe", "img_create " & Chr(34) & pkgf & "\Temp\image0.gp4" & Chr(34) & " " & Chr(34) & pkgf & "\" & My.Settings.GN & "_" & My.Settings.GID & ".pkg" & Chr(34))
            'My.Computer.FileSystem.RenameFile(pkgf & "\Temp\image0.gp4 " & pkgf & "\" & "1.pkg", My.Settings.GID & "_PS2.pkg")
            'System.IO.Directory.Delete(pkgf & "\Temp", True)
            frm.Invoke(Sub() rcht.Text = rcht.Text & "PKG compiled." & vbCrLf)
            frm.Invoke(Sub() frm.NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info)
            frm.Invoke(Sub() frm.NotifyIcon1.BalloonTipTitle = "PKG compiled")
            frm.Invoke(Sub() frm.NotifyIcon1.BalloonTipText = "Game : " & My.Settings.GN & vbCrLf & "PKG : " & pkgf & "\" & My.Settings.GN & "_" & My.Settings.GID & ".pkg")
            frm.Invoke(Sub() frm.NotifyIcon1.ShowBalloonTip(1000))
            System.IO.Directory.Delete(pkgf & "\Temp", True)
            System.IO.File.Delete("back.jpg")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub systemcmd(ByVal cmd As String, arg As String)
        Dim pHelp As New ProcessStartInfo
        pHelp.FileName = cmd
        pHelp.Arguments = arg
        pHelp.UseShellExecute = True
        pHelp.WindowStyle = ProcessWindowStyle.Hidden

        Dim proc As Process = Process.Start(pHelp)
        proc.WaitForExit()
    End Sub
End Class
