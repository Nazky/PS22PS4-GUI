Imports System.IO
Imports System.Text
Public Class PS2EmuCreator
    <STAThread>
    Shared Sub ExtractPKG(pkg As String, out As String, pkgn As String, rcht As RichTextBox, frm As Form1)
        If frm.InvokeRequired Then
            Try
                If Directory.Exists(out & "\Temp") Then
                    Directory.Delete(out & "\Temp", True)
                    System.IO.Directory.CreateDirectory(out & "\Temp")
                Else
                    System.IO.Directory.CreateDirectory(out & "\Temp")
                End If
                If pkg.Contains(".pkg") Or pkg.Contains(".PKG") Then
                    frm.Invoke(Sub() Form1.ProgressBar1.Value = 0)
                    frm.Invoke(Sub() Form1.UseWaitCursor = True)
                    frm.Invoke(Sub() Form1.Button1.Enabled = False)
                    frm.Invoke(Sub() Form1.MenuStrip1.Enabled = False)
                    frm.Invoke(Sub() Form1.TabControl1.Enabled = False)
                    frm.Invoke(Sub() rcht.Text = "PKG path: " & pkg & vbCrLf & "Emu pkg path: " & out & "\" & pkgn & ".pkg" & vbCrLf & "---------------------------------------------------------------------------------------------------" & vbCrLf)
                    frm.Invoke(Sub() rcht.Text = rcht.Text & "Decrypting PKG..." & vbCrLf)
                    systemcmd("bin\tools\orbis-pub-cmd.exe", "img_extract --passcode 00000000000000000000000000000000 " & Chr(34) & pkg & Chr(34) & " " & Chr(34) & out & "\Temp" & Chr(34))
                    frm.Invoke(Sub() rcht.Text = rcht.Text & "PKG Decrypted" & vbCrLf)
                    frm.Invoke(Sub() Form1.ProgressBar1.Value += 25)
                    frm.Invoke(Sub() rcht.Text = rcht.Text & "Extracting emu..." & vbCrLf)
                    ExtractEmuFiles(out, pkgn)
                    frm.Invoke(Sub() Form1.ProgressBar1.Value += 25)
                    frm.Invoke(Sub() rcht.Text = rcht.Text & "Emulator extracted" & vbCrLf)
                    frm.Invoke(Sub() Form1.ProgressBar1.Value += 25)
                    compilpkg(rcht, out, pkgn, frm)
                    'Directory.Delete(out & "\Temp", True)
                    frm.Invoke(Sub() Form1.ProgressBar1.Value = 100)
                    frm.Invoke(Sub() Form1.UseWaitCursor = False)
                    frm.Invoke(Sub() Form1.Button1.Enabled = True)
                    frm.Invoke(Sub() Form1.MenuStrip1.Enabled = True)
                    frm.Invoke(Sub() Form1.TabControl1.Enabled = True)
                Else
                    MsgBox("Not supported file found !", MsgBoxStyle.Critical, "PS22PS4-GUI")
                End If
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
            End Try

        End If
    End Sub

    Shared Sub ExtractEmuFiles(fb As String, pkgn As String)
        Try
            System.IO.File.Move(fb & "\Temp\Sc0\param.sfo", fb & "\Temp\image0\sce_sys\param.sfo")
            System.IO.Directory.Delete(fb & "\Temp\Sc0", True)
            System.IO.Directory.Delete(fb & "\Temp\Image0\image", True)
            System.IO.Directory.CreateDirectory(fb & "\Temp\Image0\image")

            If System.IO.File.Exists(fb & "\Temp\Image0\config-emu-ps4.txt") Then
                Dim kcq = MsgBox("Do you want to backup the ps4 config ?" & vbCrLf & "The ps4 config backup can be find in : bin\configs\extracted\" & pkgn & ".txt", MsgBoxStyle.Information + MsgBoxStyle.YesNo)

                If kcq = MsgBoxResult.Yes Then
                    Directory.CreateDirectory("bin\configs\extracted")
                    If File.Exists(Application.StartupPath & "\bin\configs\extracted\" & pkgn & ".txt") Then
                        Dim rc = MsgBox("Config aleardy found to you want to replace it ?", MsgBoxStyle.Question + MsgBoxStyle.YesNo)
                        If rc = MsgBoxResult.Yes Then
                            System.IO.File.Copy(fb & "\Temp\Image0\config-emu-ps4.txt", Application.StartupPath & "\bin\configs\extracted\" & pkgn & ".txt", True)
                        End If
                    Else
                        System.IO.File.Copy(fb & "\Temp\Image0\config-emu-ps4.txt", Application.StartupPath & "\bin\configs\extracted\" & pkgn & ".txt")
                    End If

                End If
            End If

            If IsFolderEmpty(fb & "\Temp\Image0\patches") Then
                Console.WriteLine("Le dossier est vide.")

            Else
                Console.WriteLine("Le dossier n'est pas vide.")
                Dim kpq = MsgBox("Do you want to backup patches ?" & vbCrLf & "Backup can be find in : bin\configs\extracted\patches\" & pkgn, MsgBoxStyle.Information + MsgBoxStyle.YesNo)
                If kpq = MsgBoxResult.Yes Then
                    Directory.CreateDirectory(Application.StartupPath & "\bin\configs\extracted\patches\" & pkgn)
                    CopyFolder(fb & "\Temp\Image0\patches", Application.StartupPath & "\bin\configs\extracted\patches\" & pkgn)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    Public Shared Function IsFolderEmpty(folderPath As String) As Boolean
        Dim files As String() = Directory.GetFiles(folderPath)
        Return files.Length = 0
    End Function

    Shared Sub systemcmd(cmd As String, arg As String)
        Dim pHelp As New ProcessStartInfo
        pHelp.FileName = cmd
        pHelp.Arguments = arg
        pHelp.UseShellExecute = True
        pHelp.WindowStyle = ProcessWindowStyle.Hidden

        Dim proc As Process = Process.Start(pHelp)
        proc.WaitForExit()
    End Sub

    Public Shared Sub CopyFolder(sourceFolder As String, destinationFolder As String)
        If Not Directory.Exists(destinationFolder) Then
            Directory.CreateDirectory(destinationFolder)
        End If

        For Each file As String In Directory.GetFiles(sourceFolder)
            Dim fileName As String = Path.GetFileName(file)
            Dim destinationPath As String = Path.Combine(destinationFolder, fileName)
            System.IO.File.Copy(file, destinationPath, True)
        Next

        For Each subdirectory As String In Directory.GetDirectories(sourceFolder)
            Dim subdirectoryName As String = Path.GetFileName(subdirectory)
            Dim destinationSubdirectory As String = Path.Combine(destinationFolder, subdirectoryName)
            CopyFolder(subdirectory, destinationSubdirectory)
        Next
    End Sub

    Shared Sub compilpkg(rcht As RichTextBox, pkgf As String, pkgn As String, frm As Form1)
        Try
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Compiling PKG..." & vbCrLf)
            systemcmd("bin\tools\gengp4.exe", Chr(34) & pkgf & "\Temp\Image0" & Chr(34))
            Dim gp4 As String = System.IO.File.ReadAllText(pkgf & "\Temp\Image0.gp4")
            System.IO.File.WriteAllText(pkgf & "\Temp\Image0.gp4", gp4.Replace("<scenarios default_id=""1"">", "<scenarios default_id=""0"">"))
            systemcmd("bin\tools\orbis-pub-cmd.exe", "img_create " & Chr(34) & pkgf & "\Temp\Image0.gp4" & Chr(34) & " " & Chr(34) & Application.StartupPath & "\bin\emulators" & "\" & pkgn & ".pkg" & Chr(34))
            frm.Invoke(Sub() rcht.Text = rcht.Text & "PKG compiled." & vbCrLf)
            System.IO.Directory.Delete(pkgf & "\Temp", True)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub
End Class
