Imports System.Net.Mime.MediaTypeNames
Imports System.Security.Cryptography
Imports System.Text

Public Class PS22PS4
    Shared Sub CreatePKG(ISO As String, gid As String, gn As String, gv As String, gr As String, icon As String, background As String, config As String, LUA As String, rcht As RichTextBox, pkgf As String)
        Try
            rcht.Text = "ISO path: " & ISO & vbCrLf & "Game ID: " & gid & vbCrLf & "Game Name: " & gn & vbCrLf & "Game version: " & gv & "Game region: " & gr & vbCrLf & "Icon path: " & icon & vbCrLf & "Background path: " & background & vbCrLf
            If config = "" Then
                rcht.Text = rcht.Text & "config file: N/A" & vbCrLf
            Else
                rcht.Text = rcht.Text & "config file: " & config & vbCrLf
            End If
            If LUA = "" Then
                rcht.Text = rcht.Text & "LUA file: N/A" & vbCrLf & "---------------------------------------------------------------------------------------------------" & vbCrLf
            Else
                rcht.Text = rcht.Text & "LUA file: " & LUA & vbCrLf & "---------------------------------------------------------------------------------------------------" & vbCrLf
            End If

            dpkg(rcht, pkgf)
            cps2(rcht, ISO, pkgf)
            ic(rcht, pkgf, gn)
            il(rcht, pkgf, gn)
            setid(rcht, gid, pkgf)
            setgn(rcht, gn, pkgf)
            setc(rcht, icon, pkgf)
            setb(rcht, background, pkgf)
            compilpkg(rcht, pkgf, gn)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub dpkg(rcht As RichTextBox, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Decrypting PKG..." & vbCrLf
            System.IO.Directory.CreateDirectory(pkgf & "\Temp")
            systemcmd("bin\tools\orbis-pub-cmd.exe", "img_extract --passcode 00000000000000000000000000000000 .\bin\tools\source.pkg " & pkgf & "\Temp")
            My.Computer.FileSystem.RenameDirectory(pkgf & "\Temp\Sc0", "sce_sys")
            System.IO.File.Move(pkgf & "\Temp\sce_sys\param.sfo", pkgf & "\Temp\image0\sce_sys\param.sfo")
            System.IO.Directory.Delete(pkgf & "\Temp\sce_sys", True)
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub cps2(rcht As RichTextBox, ISO As String, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Importing ISO..." & vbCrLf
            System.IO.File.Copy(ISO, pkgf & "\Temp\image0\image\disc01.iso")
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub ic(rcht As RichTextBox, pkgf As String, gn As String)
        Try
            If My.Settings.Config = "" Then
                rcht.Text = rcht.Text & "Importing default config..." & vbCrLf
                Dim confd As String = System.IO.File.ReadAllText(pkgf & "\Temp\image0\config-emu-ps4.txt")
                System.IO.File.WriteAllText(pkgf & "\Temp\image0\config-emu-ps4.txt", confd.Replace("#Markus95", "#" & gn.Replace("Game name: ", "").Replace("SLUS-20091", My.Settings.GID)))
                Form1.ProgressBar1.Value += 11
            ElseIf System.IO.File.Exists("bin\configs\" & My.Settings.GID & ".conf") Then
                rcht.Text = rcht.Text & "Importing custom config..." & vbCrLf
                System.IO.File.Copy("bin\configs\" & My.Settings.GID & ".conf", pkgf & "\Temp\image0\config-emu-ps4.txt", True)
                Form1.ProgressBar1.Value += 11
            Else
                rcht.Text = rcht.Text & "Importing custom config..." & vbCrLf
                System.IO.File.Copy(My.Settings.Config, "bin\configs\" & My.Settings.GID & ".conf", True)
                System.IO.File.Copy("bin\configs\" & My.Settings.GID & ".conf", pkgf & "\Temp\image0\config-emu-ps4.txt", True)
                Form1.ProgressBar1.Value += 11
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub il(rcht As RichTextBox, pkgf As String, gn As String)
        Try
            If My.Settings.LUA = "" Then
            ElseIf System.IO.File.Exists("bin\LUA\" & My.Settings.GID & "_config.lua") Then
                rcht.Text = rcht.Text & "Importing LUA config..." & vbCrLf
                System.IO.File.Copy("bin\LUA\" & My.Settings.GID & "_config.lua", pkgf & "\Temp\image0\patches\" & My.Settings.GID & "_config.lua", True)
                Form1.ProgressBar1.Value += 11
            Else
                rcht.Text = rcht.Text & "Importing LUA config..." & vbCrLf
                System.IO.File.Copy(My.Settings.LUA, "bin\LUA\" & My.Settings.GID & "_config.lua", True)
                System.IO.File.Copy("bin\LUA\" & My.Settings.GID & "_config.lua", pkgf & "\Temp\image0\patches\" & My.Settings.GID & "_config.lua", True)
                Form1.ProgressBar1.Value += 11
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setid(rcht As RichTextBox, gid As String, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Patching game id..." & vbCrLf
            System.IO.Directory.CreateDirectory(pkgf & "\Temp")
            'MsgBox("""" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x31F --text " & """" & My.Settings.GID.Replace("-", "") & """")
            systemcmd("bin\tools\cmdlhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x31F --text " & """" & My.Settings.GID.Replace("-", "") & """")
            systemcmd("bin\tools\cmdlhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x670 --text " & """" & My.Settings.GID.Replace("-", "") & """")
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setgn(rcht As RichTextBox, gn As String, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Patching game name..." & vbCrLf
            systemcmd("bin\tools\cmdlhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x5F0 --hex " & """00000000000000000000000000000000""")
            systemcmd("bin\tools\cmdlhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x600 --hex " & """00000000000000000000000000000000""")
            systemcmd("bin\tools\cmdlhex.exe", """" & pkgf & "\Temp\image0\sce_sys\param.sfo"" " & "--address 0x5F0 --text " & """" & gn.Replace("Game name: ", "") & """")
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setc(rcht As RichTextBox, icon As String, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Patching game cover..." & vbCrLf
            'System.IO.File.Copy(icon, pkgf & "\Temp\image0\sce_sys\icon0.png")
            systemcmd("bin\tools\magick.exe", "convert  " & icon & " " & pkgf & "\Temp\image0\sce_sys\icon0.jpg")
            systemcmd("bin\tools\magick.exe", "convert  " & pkgf & "\Temp\image0\sce_sys\icon0.jpg" & " " & pkgf & "\Temp\image0\sce_sys\icon0.png")
            systemcmd("bin\tools\magick.exe", "mogrify -resize 512x512!  " & pkgf & "\Temp\image0\sce_sys\icon0.png")
            System.IO.File.Copy(pkgf & "\Temp\image0\sce_sys\icon0.png", pkgf & "\Temp\image0\sce_sys\save_data.png")
            systemcmd("bin\tools\magick.exe", "mogrify -resize 228x128!  " & pkgf & "\Temp\image0\sce_sys\save_data.png")
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setb(rcht As RichTextBox, back As String, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Patching game background..." & vbCrLf
            'System.IO.File.Copy(icon, pkgf & "\Temp\image0\sce_sys\icon0.png")
            systemcmd("bin\tools\magick.exe", "convert  " & back & " " & pkgf & "\Temp\image0\sce_sys\pic1.jpg")
            systemcmd("bin\tools\magick.exe", "mogrify -resize 1920x1080! " & pkgf & "\Temp\image0\sce_sys\pic1.jpg")
            systemcmd("bin\tools\magick.exe", "convert  " & pkgf & "\Temp\image0\sce_sys\pic1.jpg" & " " & pkgf & "\Temp\image0\sce_sys\pic1.png")
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Sub

    Shared Sub setlua(rcht As RichTextBox, lua As String, pkgf As String)
        Try
            rcht.Text = rcht.Text & "Adding custom lua..." & vbCrLf
            System.IO.File.Copy(lua, pkgf & "\Temp\image0\patches\" & System.IO.Path.GetFileName(lua), True)
            Form1.ProgressBar1.Value += 11
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub compilpkg(rcht As RichTextBox, pkgf As String, gn As String)
        Try
            rcht.Text = rcht.Text & "Compiling PKG..." & vbCrLf
            systemcmd("bin\tools\gengp4.exe", pkgf & "\Temp\image0")
            'My.Computer.FileSystem.RenameFile(pkgf & "\Temp\image0.gp4", "image0.txt")
            Dim gp4 As String = System.IO.File.ReadAllText(pkgf & "\Temp\image0.gp4")
            System.IO.File.WriteAllText(pkgf & "\Temp\image0.gp4", gp4.Replace("<scenarios default_id=""1"">", "<scenarios default_id=""0"">"))
            systemcmd("bin\tools\orbis-pub-cmd.exe", "img_create " & pkgf & "\Temp\image0.gp4 " & pkgf & "\" & My.Settings.GID & "_PS2.pkg")
            'My.Computer.FileSystem.RenameFile(pkgf & "\Temp\image0.gp4 " & pkgf & "\" & "1.pkg", My.Settings.GID & "_PS2.pkg")
            'System.IO.Directory.Delete(pkgf & "\Temp", True)
            rcht.Text = rcht.Text & "PKG compiled." & vbCrLf
            System.IO.Directory.Delete(pkgf & "\Temp", True)
            Form1.ProgressBar1.Value = 100
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
