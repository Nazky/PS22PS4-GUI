Imports DiscUtils.Iso9660
Imports System.IO
Imports System.Net
Imports System.Reflection.Emit
Imports System.Text
Imports HtmlAgilityPack
Imports NUnit.Framework.Interfaces
Imports Crc

Public Class ExtractPS2
    Shared Sub info(isopath As String, frm As Form1)
        Try
            frm.Invoke(Sub() frm.UseWaitCursor = True)
            frm.Invoke(Sub() frm.Button5.Enabled = False)
            If isopath.Contains(".iso") Or isopath.Contains(".ISO") Then
                Using psISO As FileStream = File.Open(isopath, FileMode.Open)
                    Dim dvd As New CDReader(psISO, True)
                    Dim info As Stream = dvd.OpenFile("SYSTEM.CNF", FileMode.Open)
                    Dim convinfo As New StreamReader(info, Encoding.UTF8)
                    File.WriteAllText("ps2.info", convinfo.ReadToEnd)
                    getid(File.ReadAllText("ps2.info"), frm)
                    getv(File.ReadAllText("ps2.info"), frm)
                    getr(File.ReadAllText("ps2.info"), frm)
                    frm.Invoke(Sub() frm.UseWaitCursor = False)
                    frm.Invoke(Sub() frm.Button5.Enabled = True)
                    'MsgBox(Application.StartupPath & "\bin\info")
                    If File.Exists(Application.StartupPath & "\bin\info\" & Form1.Label10.Text & ".info") Then
                        File.Delete("ps2.info")

                    Else
                        File.Move("ps2.info", Application.StartupPath & "\bin\info\" & Form1.Label10.Text.Replace("Game ID: ", "") & ".info")
                    End If
                End Using
            ElseIf isopath.Contains(".bin") Or isopath.Contains(".BIN") Then
                Dim info As String = File.ReadAllText(isopath)
                'Dim gid As String = Strings.Mid(info, info.IndexOf("BOOT2"))
                'MsgBox(gid)
                'crcheck(isopath)
                getid(info.Substring(info.IndexOf("BOOT2 =") + 1), frm)
                getv(info, frm)
                getr(info, frm)
                frm.Invoke(Sub() frm.UseWaitCursor = False)
                frm.Invoke(Sub() frm.Button5.Enabled = True)
            Else
                MsgBox("Not supported file found !", MsgBoxStyle.Critical, "PS22PS4-GUI")
            End If



        Catch ex As Exception
            ' MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub




    Shared Sub getid(str As String, frm As Form1)
        Try
            Dim sSource As String = str 'String that is being searched
            Dim sDelimStart As String = "cdrom0:\" 'First delimiting word
            Dim sDelimEnd As String = ";" 'Second delimiting word
            Dim nIndexStart As Integer = sSource.IndexOf(sDelimStart) 'Find the first occurrence of f1
            Dim nIndexEnd As Integer = sSource.IndexOf(sDelimEnd) 'Find the first occurrence of f2

            If nIndexStart > -1 AndAlso nIndexEnd > -1 Then '-1 means the word was not found.
                Dim res As String = Strings.Mid(sSource, nIndexStart + sDelimStart.Length + 1, nIndexEnd - nIndexStart - sDelimStart.Length) 'Crop the text between
                'MessageBox.Show(res.Replace("_", "").Replace(".", "")) 'Display
                frm.Invoke(Sub() Form1.Label10.Text = res.Replace("_", "").Replace(".", ""))
                getn(res.Replace("_", "-").Replace(".", ""), frm)
                My.Settings.GID = res.Replace("_", "-").Replace(".", "")
            Else
                MessageBox.Show("One or both of the delimiting words were not found!")
            End If
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub getv(str As String, frm As Form1)
        Try
            Dim sSource As String = str 'String that is being searched
            Dim sDelimStart As String = "VER = " 'First delimiting word
            Dim sDelimEnd As String = "VMODE" 'Second delimiting word
            Dim nIndexStart As Integer = sSource.IndexOf(sDelimStart) 'Find the first occurrence of f1
            Dim nIndexEnd As Integer = sSource.IndexOf(sDelimEnd) 'Find the first occurrence of f2

            If nIndexStart > -1 AndAlso nIndexEnd > -1 Then '-1 means the word was not found.
                Dim res As String = Strings.Mid(sSource, nIndexStart + sDelimStart.Length + 1, nIndexEnd - nIndexStart - sDelimStart.Length) 'Crop the text between
                'MessageBox.Show(res) 'Display
                frm.Invoke(Sub() Form1.Label11.Text = res)
            Else
                'MessageBox.Show("One or both of the delimiting words were not found!")
                sDelimStart = "VER="
                sDelimEnd = "VMODE"
                nIndexStart = sSource.IndexOf(sDelimStart)
                nIndexEnd = sSource.IndexOf(sDelimEnd)
                Dim res As String = Strings.Mid(sSource, nIndexStart + sDelimStart.Length + 1, nIndexEnd - nIndexStart - sDelimStart.Length) 'Crop the text between
                frm.Invoke(Sub() Form1.Label11.Text = res)
            End If
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub getr(str As String, frm As Form1)
        Try
            Dim sSource As String = str 'String that is being searched
            Dim res As String = sSource.Substring(sSource.LastIndexOf("VMODE") + 6) 'Crop the text between
            'MessageBox.Show(res) 'Display
            frm.Invoke(Sub() Form1.Label12.Text = res.Replace("=", ""))
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try

    End Sub

    Shared Sub getn(id As String, frm As Form1)
        Try
            Dim wc As New WebClient()
            Dim html = wc.DownloadString("http://redump.org/discs/quicksearch/" & id)
            wc.Dispose()
            Dim htmlDoc As New HtmlDocument()
            htmlDoc.LoadHtml(html)
            If htmlDoc.DocumentNode.InnerHtml.Contains("h1") Then
                For Each h1Node In htmlDoc.DocumentNode.SelectNodes("//h1")
                    ' Do Something...
                    frm.Invoke(Sub() Form1.Label9.Text = h1Node.InnerHtml.Replace(":", " -").Replace("/", "").Replace("\\", " ").Replace("?", "").Replace("*", "").Replace("|", "").Replace(">", "").Replace("<", ""))
                    My.Settings.GN = h1Node.InnerHtml.Replace(":", " -").Replace("/", "").Replace("\\", " ").Replace("?", "").Replace("*", "").Replace("|", "").Replace(">", "").Replace("<", "")
                Next
            Else
                For Each h1Node In htmlDoc.DocumentNode.SelectNodes("//td/a")
                    ' Do Something...
                    frm.Invoke(Sub() Form1.Label9.Text = h1Node.InnerHtml.Replace(":", " -").Replace("/", "").Replace("\\", " ").Replace("?", "").Replace("*", "").Replace("|", "").Replace(">", "").Replace("<", ""))
                    My.Settings.GN = h1Node.InnerHtml.Replace(":", " -").Replace("/", "").Replace("\\", " ").Replace("?", "").Replace("*", "").Replace("|", "").Replace(">", "").Replace("<", "")
                Next
            End If

        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical, "PS22PS4-GUI")
        End Try


    End Sub
End Class
