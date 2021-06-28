Public Class Form1
    Private _Mask As New Mask
    Dim oriItem As New OrientationFinder
    Private _edgedetectdone As Boolean = False
    Private oriED As New EdgeDetector

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox1.Text = OpenFileDialog1.FileName


        End If
    End Sub

    Private Function ValidateParams() As Boolean
        If IsNumeric(TextBox4.Text) = False Then
            MsgBox("Sorry, please key in a numeric value for the threshold %")
            Return False
        End If
        Return True
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If ValidateParams() = False Then Exit Sub
        Dim oriItem As New EdgeDetector
        oriItem.LoadImage(TextBox1.Text)


        PictureBox1.Image = oriItem.Image
        TextBox2.Text = oriItem.ImageWidth
        TextBox3.Text = oriItem.ImageHeight

        Dim oriBitmap As FastPixel = oriItem.ConvertToGrayscale()
        'PictureBox2.Image = oriBitmap.Bitmap
        Dim oriBitmap2 As FastPixel = oriItem.EdgeDetect(oriBitmap)
        PictureBox2.Image = oriBitmap2.Bitmap
        Dim oriBitmap3 As FastPixel = oriItem.DoPass(oriBitmap2, 9, _Mask, RadioButton2.Checked, TextBox4.Text)

        oriBitmap2.Bitmap.Save("d:\vbtest\edge.png", Imaging.ImageFormat.Png)
        oriBitmap3.Bitmap.Save("d:\vbtest\pass.png", Imaging.ImageFormat.Png)

        PictureBox3.Image = oriBitmap3.Bitmap







    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim oriItem As New EdgeDetector
        oriItem.LoadImage(TextBox1.Text)

        PictureBox1.Image = oriItem.Image
        TextBox2.Text = oriItem.ImageWidth
        TextBox3.Text = oriItem.ImageHeight

        Dim oriBitmap As Bitmap = oriItem.OffsetImage(350, 0, 0)
        PictureBox3.Image = oriBitmap
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        System.IO.File.WriteAllText("d:\vbtest\outputarray.txt", "")
        PictureBox1.SizeMode = PictureBoxSizeMode.AutoSize
        PictureBox2.SizeMode = PictureBoxSizeMode.AutoSize
        PictureBox3.SizeMode = PictureBoxSizeMode.AutoSize
        Me.WindowState = FormWindowState.Maximized
        SplitContainer1.SplitterDistance = Me.Width \ 2
        _Mask.LoadHypercolumns()
    End Sub


    Private Sub PictureBox2_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox2.Paint
        e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.Bicubic
        e.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.None
        MyBase.OnPaint(e)
    End Sub

    Private Sub PictureBox3_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox3.Paint
        e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.Bicubic
        e.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.None
        MyBase.OnPaint(e)
    End Sub

    Private Sub Button4_Click_2(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button4_Click_3(sender As Object, e As EventArgs) Handles Button4.Click
        If ValidateParams() = False Then Exit Sub
        Dim oriItem As New EdgeDetector
        oriItem.LoadImage(TextBox1.Text)


        'PictureBox1.Image = oriItem.Image
        TextBox2.Text = oriItem.ImageWidth
        TextBox3.Text = oriItem.ImageHeight


        PictureBox2.Image = oriItem.Image
        Dim oriBitmap2 As New FastPixel(oriItem.Image)
        Dim oriBitmap3 As FastPixel = oriItem.DoPass(oriBitmap2, 9, _Mask, RadioButton2.Checked, TextBox4.Text)

        oriBitmap2.Bitmap.Save("d:\vbtest\edge.png", Imaging.ImageFormat.Png)
        oriBitmap3.Bitmap.Save("d:\vbtest\pass.png", Imaging.ImageFormat.Png)

        PictureBox3.Image = oriBitmap3.Bitmap
    End Sub

    Private Sub PaintFullHC()
        If ValidateParams() = False Then Exit Sub
        Dim oriItem As New EdgeDetector
        oriItem.LoadImage(TextBox1.Text)

        Dim oriBitmap As FastPixel = New FastPixel(oriItem.Image)


        _Mask.HyperColumn.Lock()
        oriBitmap.Lock()
        oriBitmap.Clear(Color.Black)

        Dim ipx As Integer
        Dim jpy As Integer
        For jpy = 0 To _Mask.HyperColumn.Height - 1
            For ipx = 0 To _Mask.HyperColumn.Width - 1
                Dim mainx As Integer = ipx
                Dim mainy As Integer = jpy

                Dim pixelvalue2 As Color = _Mask.HyperColumn.GetPixel(mainx, mainy)


                oriBitmap.SetPixel(mainx, mainy, pixelvalue2)


            Next ipx
        Next jpy


        oriBitmap.Unlock(True)
        _Mask.HyperColumn.Unlock(True)
        PictureBox3.Image = oriBitmap.Bitmap
        oriBitmap.Bitmap.Save("d:\vbtest\outputa.png", Imaging.ImageFormat.Png)

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        PaintFullHC()
        Exit Sub

        If ValidateParams() = False Then Exit Sub
        Dim oriItem As New EdgeDetector
        oriItem.LoadImage(TextBox1.Text)

        Dim oriBitmap As FastPixel = oriItem.ConvertToGrayscale


        _Mask.HyperColumn.Lock()
        oriBitmap.Lock()
        oriBitmap.Clear(Color.Black)

        Dim _lom As List(Of MaskForm) = _Mask.MaskList
        Dim l As Integer = 0
        For l = 0 To _lom.Count - 1
            Dim _mf As MaskForm = _lom.Item(l)
            'for each maskform
            Dim m As Integer
            Dim ipx As Integer
            Dim jpx As Integer
            Dim topy As Integer = l * 18
            Dim convolsize As Integer = 9
            For m = 0 To _mf.MaskVariationsCount - 1
                Dim leftx As Integer = m * 18





                For ipx = topy To (topy + convolsize - 1)
                        For jpx = leftx To (leftx + convolsize - 1)
                            Dim y As Integer = ipx - topy
                            Dim x As Integer = jpx - leftx

                            Dim hcx As Integer = (m * 9) + x
                            Dim hcy As Integer = (l * 9) + y
                            Dim pixelvalue2 As Color = _Mask.HyperColumn.GetPixel(hcx, hcy)


                            oriBitmap.SetPixel(jpx, ipx, pixelvalue2)
                        Next jpx

                    Next ipx



            Next m
        Next l

        oriBitmap.Unlock(True)
        _Mask.HyperColumn.Unlock(True)
        PictureBox3.Image = oriBitmap.Bitmap


    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        'Load all images and run through it on 5x5 grid, and have it create
        'a population map of "most popular" orientations

        'go through each set of pixels. Each new 'set' would be formed by looking 
        'at the bright part and dark part, and categorizing based on this.

        'if it

        'Go through each patch.


        oriItem.SplitImage(TextBox1.Text)
        'oriItem.LoadImage(TextBox1.Text)



    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If ValidateParams() = False Then Exit Sub
        Dim oriItem As New EdgeDetector
        oriItem.LoadImage(TextBox1.Text)



        'PictureBox1.Image = oriItem.Image
        TextBox2.Text = oriItem.ImageWidth
        TextBox3.Text = oriItem.ImageHeight




        PictureBox2.Image = oriItem.Image
        Dim oriBitmap2 As New FastPixel(oriItem.Image)
        Dim oriBitmap3 As FastPixel = oriItem.EdgeDetect(oriBitmap2)

        'oriBitmap3.Bitmap.Save("d:\vbtest\pass.png", Imaging.ImageFormat.Png)

        PictureBox3.Image = oriBitmap3.Bitmap
        _edgedetectdone = True
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If ValidateParams() = False Then Exit Sub

        oriED.LoadImage(TextBox1.Text)


        'PictureBox1.Image = oriED.Image
        TextBox2.Text = oriED.ImageWidth
        TextBox3.Text = oriED.ImageHeight


        PictureBox2.Image = oriED.Image
        Dim oriBitmap2 As New FastPixel(oriED.Image)
        Dim oriBitmap3 As FastPixel = oriED.ColorDiff(oriBitmap2)


        'oriBitmap3.Bitmap.Save("d:\vbtest\pass.png", Imaging.ImageFormat.Png)

        PictureBox3.Image = oriBitmap3.Bitmap
        _edgedetectdone = True
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim test As New ColorMine.ColorSpaces.Comparisons.CieDe2000Comparison()
        Dim colspace As New ColorMine.ColorSpaces.Rgb(0, 0, 0)
        Dim colspace2 As New ColorMine.ColorSpaces.Rgb(255, 255, 255)
        MsgBox(test.Compare(colspace, colspace2))




    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        If _edgedetectdone = True Then

            'The input here is already LUMA, no further conversion needed

            oriED.LoadImage(PictureBox3.Image)

            TextBox2.Text = oriED.ImageWidth
            TextBox3.Text = oriED.ImageHeight


            PictureBox2.Image = oriED.Image
            Dim oriBitmap2 As New FastPixel(oriED.Image)
            Dim oriBitmap3 As FastPixel = oriED.TraceOutlines(oriBitmap2)
            PictureBox3.Image = oriBitmap3.Bitmap
            MsgBox("ALl done, globcount:" & oriED.globcount)
        Else
            MsgBox("Sorry, please do edgedetect/color diff first")
        End If

    End Sub
End Class
