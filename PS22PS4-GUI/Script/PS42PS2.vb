Imports System.IO
Imports System.Text
Imports DiscUtils.Iso9660

Public Class PS42PS2
    <STAThread>
    Shared Sub ExtractPKG(pkg As String, out As String, rcht As RichTextBox, frm As Form1)
        If frm.InvokeRequired Then
            Try
                If Directory.Exists(out & "\Temp") Then
                    Directory.Delete(out & "\Temp", True)
                    System.IO.Directory.CreateDirectory(out & "\Temp")
                Else
                    System.IO.Directory.CreateDirectory(out & "\Temp")
                End If
                If pkg.Contains(".pkg") Or pkg.Contains(".PKG") Then
                    frm.Invoke(Sub() Form1.ProgressBar2.Value = 0)
                    frm.Invoke(Sub() Form1.UseWaitCursor = True)
                    frm.Invoke(Sub() Form1.Button1.Enabled = False)
                    frm.Invoke(Sub() Form1.MenuStrip1.Enabled = False)
                    frm.Invoke(Sub() Form1.TabControl1.Enabled = False)
                    frm.Invoke(Sub() rcht.Text = "PKG path: " & pkg & vbCrLf & "ISO/BIN path folder: " & out & vbCrLf & "---------------------------------------------------------------------------------------------------" & vbCrLf)
                    frm.Invoke(Sub() rcht.Text = rcht.Text & "Decrypting PKG..." & vbCrLf)
                    systemcmd("bin\tools\orbis-pub-cmd.exe", "img_extract --passcode 00000000000000000000000000000000 " & pkg & " " & out & "\Temp")
                    frm.Invoke(Sub() rcht.Text = rcht.Text & "PKG Decrypted" & vbCrLf)
                    frm.Invoke(Sub() Form1.ProgressBar2.Value += 25)
                    FindISOInfo(out, rcht, frm)
                    Directory.Delete(out & "\Temp", True)
                    frm.Invoke(Sub() Form1.ProgressBar2.Value = 100)
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

    Shared Sub FindISOInfo(out As String, rcht As RichTextBox, frm As Form1)
        Dim size As Long = FileLen(out & "\Temp\image0\image\disc01.iso")
        If size >= 1000000000 Then
            frm.Invoke(Sub() rcht.Text = rcht.Text & "File detect: ISO(?)" & vbCrLf)
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Extracting ISO..." & vbCrLf)
            File.Copy(out & "\Temp\image0\image\disc01.iso", out & "\" & ExtractISOInfo(out & "\Temp\image0\image\disc01.iso") & ".iso")
            frm.Invoke(Sub() Form1.ProgressBar2.Value += 25)
            frm.Invoke(Sub() rcht.Text = rcht.Text & "ISO extracted" & vbCrLf)

        Else
            frm.Invoke(Sub() rcht.Text = rcht.Text & "File detect: BIN(?)" & vbCrLf)
            frm.Invoke(Sub() rcht.Text = rcht.Text & "Extracting BIN..." & vbCrLf)
            File.Copy(out & "\Temp\image0\image\disc01.iso", out & "\" & getid(File.ReadAllText(out & "\Temp\image0\image\disc01.iso")) & ".bin")
            frm.Invoke(Sub() Form1.ProgressBar2.Value += 25)
            frm.Invoke(Sub() rcht.Text = rcht.Text & "BIN extracted" & vbCrLf)
        End If
    End Sub

    Shared Function ExtractISOInfo(isopath As String)
        Try
            If isopath.Contains(".iso") Or isopath.Contains(".ISO") Then
                Using psISO As FileStream = File.Open(isopath, FileMode.Open)
                    Dim dvd As New CDReader(psISO, True)
                    Dim info As Stream = dvd.OpenFile("SYSTEM.CNF", FileMode.Open)
                    Dim convinfo As New StreamReader(info, Encoding.UTF8)
                    File.WriteAllText("ps2.info", convinfo.ReadToEnd)
                    Dim id As String = getid(File.ReadAllText("ps2.info"))
                    File.Delete("ps2.info")
                    Return id
                End Using
            End If
        Catch ex As Exception

        End Try
    End Function

    Shared Function getid(iso As String)
        Try
            Dim sSource As String = iso 'String that is being searched
            Dim sDelimStart As String = "cdrom0:\" 'First delimiting word
            Dim sDelimEnd As String = ";" 'Second delimiting word
            Dim nIndexStart As Integer = sSource.IndexOf(sDelimStart) 'Find the first occurrence of f1
            Dim nIndexEnd As Integer = sSource.IndexOf(sDelimEnd) 'Find the first occurrence of f2

            If nIndexStart > -1 AndAlso nIndexEnd > -1 Then '-1 means the word was not found.
                Dim res As String = Strings.Mid(sSource, nIndexStart + sDelimStart.Length + 1, nIndexEnd - nIndexStart - sDelimStart.Length) 'Crop the text between
                'MessageBox.Show(res.Replace("_", "").Replace(".", "")) 'Display
                Return res.Replace("_", "-").Replace(".", "")
            Else
                MessageBox.Show("One or both of the delimiting words were not found!")
            End If
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try
    End Function

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
