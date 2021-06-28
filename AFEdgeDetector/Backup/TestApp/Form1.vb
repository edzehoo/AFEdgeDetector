Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox1.Text = OpenFileDialog1.FileName


        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim oriItem As New AFEdgeDetector.EdgeDetector
        oriItem.LoadImage(TextBox1.Text)

        PictureBox1.Image = oriItem.Image
        TextBox2.Text = oriItem.ImageWidth
        TextBox3.Text = oriItem.ImageHeight

        Dim oriBitmap As Bitmap = oriItem.ConvertToGrayscale()
        oriItem.Image = oriBitmap
        PictureBox2.Image = oriBitmap
        oriBitmap = oriItem.EdgeDetect()
        PictureBox3.Image = oriBitmap







    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim oriItem As New AFEdgeDetector.EdgeDetector
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
End Class
