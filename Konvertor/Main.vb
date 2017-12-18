Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text

Public Class Main
    'Konvertor Variables
    Private filepath As String = ""
    Private fileselected As Boolean = False
    Private filename As String = ""
    Private err As Boolean = False
    Private filecontent As String = ""


    'Konvertor Events
    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        openfile()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        openfile()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        startprocess()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        startprocess()
    End Sub

    Private Sub ExitToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem2.Click
        Me.Close()
    End Sub

    Private Sub AboutToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem1.Click
        AboutBox1.ShowDialog()
    End Sub

    'Konvertor Methods
    Private Sub openfile()
        OpenFileDialog1.Filter = "Filtered HTML|*.htm;*.html"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            filepath = OpenFileDialog1.FileName
            TextBox2.Text = filepath
            fileselected = True
            filename = Path.GetFileName(filepath).Split(".")(0)
        Else
            TextBox2.Text = "No File Selected"
            fileselected = False
        End If
    End Sub

    Private Sub newStatus(ByVal str As String, ByVal p As Integer)
        Threading.Thread.Sleep(500)
        ProgressBar1.Value = p
        TextBox1.AppendText(str & Environment.NewLine)
    End Sub

    Private Sub startprocess()
        If (Not System.IO.Directory.Exists("Output")) Then
            System.IO.Directory.CreateDirectory("Output")
        End If
        err = False
        TextBox1.Clear()
        If fileselected Then
            newStatus("Loading File: " & filepath, 0)
            Try
                Using fsSource As FileStream = New FileStream(filepath, FileMode.Open, FileAccess.Read)
                    Dim bytes() As Byte = New Byte((fsSource.Length) - 1) {}
                    Dim numBytesToRead As Integer = CType(fsSource.Length, Integer)
                    Dim numBytesRead As Integer = 0
                    While (numBytesToRead > 0)
                        Dim n As Integer = fsSource.Read(bytes, numBytesRead, numBytesToRead)
                        If (n = 0) Then
                            Exit While
                        End If
                        numBytesRead = (numBytesRead + n)
                        numBytesToRead = (numBytesToRead - n)
                    End While
                    Dim filebyte() As Byte = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.UTF8, bytes)
                    filecontent = Encoding.UTF8.GetString(filebyte)
                    filebyte = New Byte() {0}
                End Using
            Catch ex As Exception
                err = True
                newStatus("File loading Failed.", 0)
                Return
            End Try
            newStatus("File loaded.", 10)
            newStatus("Removing Unknown Characters.", 15)
            filecontent = filecontent.Replace(vbCr, " ").Replace(vbLf, " ")
            Dim rgx As Regex = New Regex("<p.*?>")
            filecontent = rgx.Replace(filecontent, "")
            filecontent = filecontent.Replace("</p>", "").Replace("<br>", "").Replace("</a>", "")
            rgx = New Regex("<span.*?>")
            filecontent = rgx.Replace(filecontent, "")
            rgx = New Regex("<a.*?>")
            filecontent = rgx.Replace(filecontent, "")
            filecontent = filecontent.Replace("</span>", "")
            rgx = New Regex("<div[^>]*")
            filecontent = rgx.Replace(filecontent, "<div")
            rgx = New Regex("<div>(.*)</div>")
            filecontent = rgx.Match(filecontent).Value
            filecontent = filecontent.Replace("<div>", "").Replace("</div>", "")
            newStatus("Unknown Characters removed.", 20)
            rgx = New Regex("(&nbsp;)+")
            filecontent = rgx.Replace(filecontent, " ")
            filecontent = filecontent.Replace("""", "'").Replace(filename & "_files", "/content/exam/" & filename & "_files")
            rgx = New Regex("\s+")
            filecontent = rgx.Replace(filecontent, " ")
            newStatus("Setting Pattern for reading content.", 25)
            rgx = New Regex("[ ](\d+[.][ ])")
            filecontent = rgx.Replace(filecontent, Environment.NewLine & "$1")
            rgx = New Regex("[ ]([abcde][)][ ])")
            filecontent = rgx.Replace(filecontent, Environment.NewLine & "$1")
            newStatus("Pattern Set Successfully.", 30)
            newStatus("Reading Questions Started.", 35)
            newStatus("===========================================", 40)
            newStatus(" ", 45)
            rgx = New Regex("^(\d+)[.][ ]")
            Dim nrgx As New Regex("^([abcde])[)][ ]")
            Dim orgx As New Regex("([abcdABCD])[ ]?$")
            Dim line As String = ""
            Dim streader As New StringReader(filecontent)
            Dim qno As Integer = 1
            Try
                Dim match As Match
                While True
                    line = streader.ReadLine()
                    If line Is Nothing Then
                        Exit While
                    Else
                        match = rgx.Match(line)
                        If match.Success Then
                            If qno = match.Groups(1).Value Then
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                If match.Groups(1).Value <> "a" Then
                                    err = True
                                    newStatus("Option a) of Q." & qno & " not found.", 45)
                                End If
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                If match.Groups(1).Value <> "b" Then
                                    err = True
                                    newStatus("Option b) of Q." & qno & " not found.", 45)
                                End If
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                If match.Groups(1).Value <> "c" Then
                                    err = True
                                    newStatus("Option c) of Q." & qno & " not found.", 45)
                                End If
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                If match.Groups(1).Value <> "d" Then
                                    err = True
                                    newStatus("Option d) of Q." & qno & " not found.", 45)
                                End If
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                If match.Groups(1).Value = "e" Then
                                    match = orgx.Match(line)
                                    If match.Success = False Then
                                        err = True
                                        newStatus("Option e) of Q." & qno & " is invalid.", 45)
                                    End If
                                Else
                                    err = True
                                    newStatus("Option e) of Q." & qno & " not found.", 45)
                                End If
                                qno = qno + 1
                            Else
                                err = True
                                newStatus("Question number " & qno & " not found.", 45)
                                Exit While
                            End If
                        End If
                    End If
                End While
            Catch ex As Exception
                err = True
                newStatus("Error Reading... #" & qno, 100)
            End Try
            If err Then
                Try
                    newStatus("===========================================", 75)
                    newStatus(" ", 75)
                    newStatus("Creating Report file.", 75)
                    Using sr As New StreamWriter("Output\Report.txt", False, Encoding.UTF8)
                        sr.Write(filecontent)
                    End Using
                    newStatus("Report saved to Report.txt", 100)
                Catch ex As Exception
                    newStatus("Error saving... #", 100)
                End Try
            Else
                rgx = New Regex("^(\d+)[.][ ](.*)")
                nrgx = New Regex("^([abcde])[)][ ](.*)")
                streader = New StringReader(filecontent)
                qno = 0
                Try
                    Dim file As System.IO.StreamWriter
                    file = My.Computer.FileSystem.OpenTextFileWriter("Output\" & filename & ".php", False, Encoding.UTF8)
                    file.WriteLine("<?php")
                    file.WriteLine("$q = array();")
                    file.WriteLine("$q[] = array();")
                    file.WriteLine("$a = array();")
                    Dim match As Match
                    While True
                        line = streader.ReadLine()
                        If line Is Nothing Then
                            Exit While
                        Else
                            match = rgx.Match(line)
                            If match.Success Then
                                file.WriteLine("$q[" & qno & "][0] = """ & match.Groups(2).Value & """;")
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                file.WriteLine("$q[" & qno & "][1] = """ & match.Groups(2).Value & """;")
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                file.WriteLine("$q[" & qno & "][2] = """ & match.Groups(2).Value & """;")
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                file.WriteLine("$q[" & qno & "][3] = """ & match.Groups(2).Value & """;")
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                file.WriteLine("$q[" & qno & "][4] = """ & match.Groups(2).Value & """;")
                                line = streader.ReadLine()
                                match = nrgx.Match(line)
                                file.WriteLine("$a[" & qno & "] = """ & match.Groups(2).Value.ToLower.Replace(" ", "") & """;")
                                qno = qno + 1
                            End If
                        End If
                    End While
                    file.WriteLine("?>")
                    file.Close()
                    newStatus("Found " & qno & " Questions.", 80)
                    newStatus("Saving file..  " & filename & ".php", 90)
                    newStatus("Saved to Output Folder.", 100)
                Catch ex As Exception
                    err = True
                    newStatus("Unknown Error Occured.", 100)
                End Try
            End If
        Else
            MsgBox("No File Selected")
        End If
    End Sub
End Class
