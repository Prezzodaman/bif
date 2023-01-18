Public Class Form1

    Public mode As Integer
    Public file As String = ""
    Public data As String = ""
    Public previewed As Boolean
    Public opened As Boolean = False

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        OpenFileDialog1.Filter = "Image files|*.png;*.bmp;*.jpg"
        If OpenFileDialog1.FileName <> "" Then OpenFileDialog1.FileName = My.Computer.FileSystem.GetName(OpenFileDialog1.FileName)
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Label4.Text = My.Computer.FileSystem.GetName(OpenFileDialog1.FileName)
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Label4.Text <> "" Then
            If My.Computer.FileSystem.FileExists(OpenFileDialog1.FileName) Then
                Cursor = System.Windows.Forms.Cursors.WaitCursor
                SaveFileDialog1.Filter = "BIF files|*.bif"
                SaveFileDialog1.FileName = My.Computer.FileSystem.GetName(OpenFileDialog1.FileName).Substring(0, My.Computer.FileSystem.GetName(OpenFileDialog1.FileName).Length - 4)
                If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    If RadioButton1.Checked Then
                        mode = 1
                    ElseIf RadioButton2.Checked Then
                        mode = 2
                    ElseIf RadioButton3.Checked Then
                        mode = 3
                    End If
                    If Not previewed Then
                        preview(False)
                    End If
                    preview(True)
                    If CheckBox1.Checked Then
                        Dim data_comp As String = compress(data)
                        file += data_comp
                    Else
                        file += data
                    End If
                    My.Computer.FileSystem.WriteAllText(SaveFileDialog1.FileName, file, False, System.Text.Encoding.ASCII)
                    file = ""
                End If
                Cursor = System.Windows.Forms.Cursors.Arrow
            Else
                MsgBox("Image file can't be found! The file may have been moved or deleted.", MsgBoxStyle.Exclamation)
            End If
            Else
                MsgBox("File not selected!", MsgBoxStyle.Exclamation)
            End If
    End Sub

    Private Function compress(ByVal input As String)
        Dim data_comp As String = ""
        Dim data_prev As String = input.Substring(0, 1)
        Dim counter As Integer = 0
        For a = 0 To input.Length - 1
            If a > 0 Then data_prev = input.Substring(a - 1, 1)
            If input.Substring(a, 1) = data_prev Then
                counter += 1
            Else
                If counter > 1 Then
                    data_comp += data_prev & "-" & counter & ":"
                    counter = 1
                Else
                    data_comp += data_prev & "-1:"
                End If
            End If
        Next
        If data_prev = input.Substring(input.Length - 1, 1) Then
            data_comp += data_prev & "-" & counter - 1 & ":"
        End If
        If data_comp.Length = 0 Then data_comp = "0-" & input.Length
        Dim reduction = (0 - (Math.Round((data_comp.Length * 100) / input.Length, 2))) + 100
        Dim message As String = ""
        If reduction < 0 Then
            reduction = Math.Round((input.Length * 100) / data_comp.Length, 2)
            message = "The compression algorithm has bloated the file by " & reduction & "%!"
        Else
            message = "This is a " & reduction & "% reduction!"
        End If
        MsgBox("Original size: " & input.Length & " bytes" & vbNewLine & "Compressed size: " & data_comp.Length & " bytes" & vbNewLine & vbNewLine & message)
        Return data_comp
    End Function

    Private Function quantify(ByVal input_value As Integer, ByVal max_value As Integer, ByVal division As Integer)
        Dim result = Int(input_value / division) * division
        If result > max_value Then
            Return max_value
        Else
            Return result
        End If
    End Function

    Private Function onoff(ByVal input_value, ByVal output_if, ByVal threshold)
        If input_value > threshold Then
            Return output_if
        Else
            Return 0
        End If
    End Function

    Private Sub TrackBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.Scroll
        Label10.Text = sender.Value
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Label10.Text = TrackBar1.Value
        PreviewBox.Show()
        PreviewBox.Location = New Point(Me.Location.X + Me.Width + 60, Me.Location.Y + ((Me.Height - PreviewBox.Height) / 2))
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If My.Computer.FileSystem.FileExists(OpenFileDialog1.FileName) Then
            preview(False)
        Else
            MsgBox("File not selected!", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub preview(ByVal add_to_data As Boolean)
        Cursor = System.Windows.Forms.Cursors.WaitCursor
        If add_to_data Then
            data = ""
            file = ""
        End If
        Dim img_orig As New Bitmap(OpenFileDialog1.FileName)
        If CheckBox3.Checked Then
            Dim img_orig_actually As New Bitmap(img_orig, img_orig.Width / 2, img_orig.Height / 2)
            img_orig = img_orig_actually
        End If
        Dim fresh As New Bitmap(img_orig.Width, img_orig.Height)
        If CheckBox1.Checked Then
            file += "BIFC:"
        Else
            file += "BIF:"
        End If
        file += "W" & fresh.Width & ":H" & fresh.Height
        file += ":M" & mode
        file += ":D:"
        For y = 0 To fresh.Height - 1
            For x = 0 To fresh.Width - 1
                If RadioButton1.Checked Then
                    If img_orig.GetPixel(x, y).R - TrackBar1.Value > img_orig.GetPixel(x, y).G And img_orig.GetPixel(x, y).R - TrackBar1.Value > img_orig.GetPixel(x, y).B Or img_orig.GetPixel(x, y).R - TrackBar1.Value < TrackBar1.Value Or img_orig.GetPixel(x, y).G - TrackBar1.Value < TrackBar1.Value Or img_orig.GetPixel(x, y).B - TrackBar1.Value < TrackBar1.Value Then
                        If add_to_data Then
                            data += "1"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0))
                        End If
                    ElseIf img_orig.GetPixel(x, y).G - TrackBar1.Value > img_orig.GetPixel(x, y).R And img_orig.GetPixel(x, y).G - TrackBar1.Value > img_orig.GetPixel(x, y).B Then
                        If add_to_data Then
                            data += "2"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 0, 255, 0))
                        End If
                    ElseIf img_orig.GetPixel(x, y).B - TrackBar1.Value > img_orig.GetPixel(x, y).G And img_orig.GetPixel(x, y).B - TrackBar1.Value > img_orig.GetPixel(x, y).R Then
                        If add_to_data Then
                            data += "3"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 0, 0, 255))
                        End If
                    Else
                        If add_to_data Then
                            data += "0"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255))
                        End If
                    End If
                ElseIf RadioButton2.Checked Then
                    If img_orig.GetPixel(x, y).R - TrackBar1.Value > img_orig.GetPixel(x, y).G And img_orig.GetPixel(x, y).R - TrackBar1.Value > img_orig.GetPixel(x, y).B Then
                        If add_to_data Then
                            data += "1"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 255, 0, 255))
                        End If
                    ElseIf img_orig.GetPixel(x, y).G - TrackBar1.Value > img_orig.GetPixel(x, y).R And img_orig.GetPixel(x, y).G - TrackBar1.Value > img_orig.GetPixel(x, y).B Then
                        If add_to_data Then
                            data += "2"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 255, 255, 0))
                        End If
                    ElseIf img_orig.GetPixel(x, y).B - TrackBar1.Value > img_orig.GetPixel(x, y).G And img_orig.GetPixel(x, y).B - TrackBar1.Value > img_orig.GetPixel(x, y).R Then
                        If add_to_data Then
                            data += "3"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 0, 255, 255))
                        End If
                    ElseIf img_orig.GetPixel(x, y).R - TrackBar1.Value < TrackBar1.Value Or img_orig.GetPixel(x, y).G - TrackBar1.Value < TrackBar1.Value Or img_orig.GetPixel(x, y).B - TrackBar1.Value < TrackBar1.Value Then
                        If add_to_data Then
                            data += "4"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0))
                        End If
                    Else
                        If add_to_data Then
                            data += "0"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255))
                        End If
                    End If
                ElseIf RadioButton3.Checked Then
                    If img_orig.GetPixel(x, y).R > TrackBar1.Value Or img_orig.GetPixel(x, y).G > TrackBar1.Value Or img_orig.GetPixel(x, y).B > TrackBar1.Value Then
                        If add_to_data Then
                            data += "1"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255))
                        End If
                    Else
                        If add_to_data Then
                            data += "0"
                        Else
                            fresh.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0))
                        End If
                    End If
                End If
            Next
        Next
        Cursor = System.Windows.Forms.Cursors.Arrow
        If Not add_to_data Then
            update_preview(fresh)
            previewed = True
        End If
    End Sub

    Private Sub update_preview(ByVal image As Image)
        PreviewBox.PictureBox1.Image = image
        PreviewBox.Width = PreviewBox.PictureBox1.Image.Width
        PreviewBox.Height = PreviewBox.PictureBox1.Image.Height + 29
    End Sub

    Public compressed As Boolean = False
    Public header_length As Integer = 0

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        OpenFileDialog1.Filter = "BIF files|*.bif"
        OpenFileDialog1.FileName = ""
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Cursor = System.Windows.Forms.Cursors.WaitCursor
            file = My.Computer.FileSystem.ReadAllText(OpenFileDialog1.FileName)
            Dim stage As String = ""
            Dim width As String = ""
            Dim height As String = ""
            Dim mode_str As String = ""
            Dim width_val As Integer
            Dim height_val As Integer
            Dim mode_val As Integer
            Dim data As String = ""
            Dim data_char As String = ""
            Dim data_amount As String = ""
            Dim compressed_string As String = "Uncompressed"
            compressed = False
            'BIFC:W50:H50:M2:D:
            If file.Substring(0, 3) <> "BIF" Then
                MsgBox("Invalid header!", MsgBoxStyle.Exclamation)
                Exit Sub
            End If
            For a = 0 To file.Length - 1
                If file.Substring(a, 1) = "C" Then
                    compressed_string = "Compressed"
                    compressed = True
                End If
                If file.Substring(a, 1) = "W" Then
                    stage = "W"
                ElseIf file.Substring(a, 1) = "H" Then
                    stage = "H"
                ElseIf file.Substring(a, 1) = "M" Then
                    stage = "M"
                ElseIf file.Substring(a, 1) = "D" Then
                    header_length = a + 2
                    Exit For
                End If
                If stage = "W" Then
                    width += file.Substring(a, 1)
                ElseIf stage = "H" Then
                    height += file.Substring(a, 1)
                ElseIf stage = "M" Then
                    mode_str += file.Substring(a, 1)
                End If
            Next
            width = width.Substring(1, width.Length - 2)
            height = height.Substring(1, height.Length - 2)
            mode_str = mode_str.Substring(1, mode_str.Length - 2)
            width_val = Val(width)
            height_val = Val(height)
            mode_val = Val(mode_str)
            mode = mode_val
            Dim image = New Bitmap(width_val, height_val)
            Dim checker As Integer = 0
            Dim x_pos As Integer = 0
            Dim y_pos As Integer = 0
            For a = header_length To file.Length - 1
                If compressed Then
                    data += file.Substring(a, 1)
                    If file.Substring(a, 1) = "-" Then
                        data_char = data
                        data_char = data_char.Substring(0, data_char.Length - 1)
                        data = ""
                        checker = 1
                    End If
                    If file.Substring(a, 1) = ":" Then
                        data_amount = data
                        data_amount = data_amount.Substring(0, data_amount.Length - 1)
                        data = ""
                        checker = 2
                    End If
                    If checker = 2 Then
                        checker = 0
                        For b = 0 To Val(data_amount) - 1
                            image.SetPixel(x_pos, y_pos, color_set(mode_val, data_char))
                            x_pos += 1
                            If x_pos > width_val - 1 Then
                                x_pos = 0
                                y_pos += 1
                            End If
                            If y_pos > image.Height - 1 Then
                                MsgBox("Width and height are incorrect!", MsgBoxStyle.Exclamation)
                                Cursor = System.Windows.Forms.Cursors.Arrow
                                Exit Sub
                            End If
                        Next b
                    End If
                Else
                    image.SetPixel(x_pos, y_pos, color_set(mode_val, file.Substring(a, 1)))
                    x_pos += 1
                    If x_pos > width_val - 1 Then
                        x_pos = 0
                        y_pos += 1
                    End If
                End If
            Next

            Dim reduction = (0 - (Math.Round(((file.Length - header_length) * 100) / (image.Width * image.Height), 2))) + 100
            Dim bloated As Boolean = False
            If reduction < 0 Then
                reduction = Math.Round(((image.Width * image.Height) * 100) / (file.Length - header_length), 2)
                compressed_string = "Bloated"
                bloated = True
            End If

            If compressed Then
                If bloated Then
                    compressed_string += " (amount: " & reduction & "%)"
                Else
                    compressed_string += " (ratio: " & reduction & "%)"
                End If
            End If
            Label12.Text = My.Computer.FileSystem.GetName(OpenFileDialog1.FileName)
            update_preview(image)
            Label4.Text = ""
            opened = True
            Dim mode_sub As String = ""
            If mode_val = 1 Then
                mode_sub = " (red, green and blue)"
            ElseIf mode_val = 2 Then
                mode_sub = " (cyan, magenta, yellow and black)"
            ElseIf mode_val = 3 Then
                mode_sub = " (black and white)"
            End If
            Label14.Text = "Width: " & width & ", Height: " & height & ", Mode: " & mode & mode_sub & ", " & compressed_string
            Cursor = System.Windows.Forms.Cursors.Arrow
        End If
    End Sub

    Private Function color_set(ByVal mode As Integer, ByVal data As Integer) As Color
        If mode = 1 Then
            If data = "0" Then
                Return Color.FromArgb(255, 255, 255, 255)
            ElseIf data = "1" Then
                Return Color.FromArgb(255, 255, 0, 0)
            ElseIf data = "2" Then
                Return Color.FromArgb(255, 0, 255, 0)
            ElseIf data = "3" Then
                Return Color.FromArgb(255, 0, 0, 255)
            End If
        ElseIf mode = 2 Then
            If data = "0" Then
                Return Color.FromArgb(255, 255, 255, 255)
            ElseIf data = "1" Then
                Return Color.FromArgb(255, 255, 0, 255)
            ElseIf data = "2" Then
                Return Color.FromArgb(255, 255, 255, 0)
            ElseIf data = "3" Then
                Return Color.FromArgb(255, 0, 255, 255)
            ElseIf data = "4" Then
                Return Color.FromArgb(255, 0, 0, 0)
            End If
        ElseIf mode = 3 Then
            If data = "1" Then
                Return Color.FromArgb(255, 255, 255, 255)
            Else
                Return Color.FromArgb(255, 0, 0, 0)
            End If
        Else
            Return Color.Black
        End If
    End Function

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged
        PreviewBox.TopMost = sender.checked
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        If Not opened Then
            MsgBox("File not opened!", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        SaveFileDialog1.Filter = "PNG image|*.png"
        SaveFileDialog1.FileName = My.Computer.FileSystem.GetName(OpenFileDialog1.FileName).Substring(0, My.Computer.FileSystem.GetName(OpenFileDialog1.FileName).Length - 4)
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            PreviewBox.PictureBox1.Image.Save(SaveFileDialog1.FileName)
        End If
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        If Not opened Then
            MsgBox("File not opened!", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        SaveFileDialog1.Filter = "BIF files|*.bif"
        SaveFileDialog1.FileName = Label12.Text.Substring(0, Label12.Text.Length - 4)
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim image As New Bitmap(PreviewBox.PictureBox1.Image)
            Dim data = ""
            Cursor = System.Windows.Forms.Cursors.WaitCursor
            For y = 0 To image.Height - 1
                For x = 0 To image.Width - 1
                    If mode = 1 Then
                        If image.GetPixel(x, y) = Color.FromArgb(255, 255, 255, 255) Then
                            data += "0"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 255, 0, 0) Then
                            data += "1"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 0, 255, 0) Then
                            data += "2"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 0, 0, 255) Then
                            data += "3"
                        Else
                            data += "0"
                        End If
                    ElseIf mode = 2 Then
                        If image.GetPixel(x, y) = Color.FromArgb(255, 255, 255, 255) Then
                            data += "0"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 255, 0, 255) Then
                            data += "1"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 255, 255, 0) Then
                            data += "2"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 0, 255, 255) Then
                            data += "3"
                        ElseIf image.GetPixel(x, y) = Color.FromArgb(255, 0, 0, 0) Then
                            data += "4"
                        Else
                            data += "0"
                        End If
                    ElseIf mode = 3 Then
                        If image.GetPixel(x, y) = Color.FromArgb(255, 255, 255, 255) Then
                            data += "0"
                        Else
                            data += "1"
                        End If
                    End If
                Next
            Next
            Dim header As String
            If Not compressed Then
                data = compress(data)
                header = file.Substring(0, header_length).Replace("BIF", "BIFC")
            Else
                header = file.Substring(0, header_length).Replace("BIFC", "BIF")
            End If
            My.Computer.FileSystem.WriteAllText(SaveFileDialog1.FileName, header & data, False, System.Text.Encoding.ASCII)
            Cursor = System.Windows.Forms.Cursors.Arrow
        End If
    End Sub
End Class
