Imports System.Drawing
Imports System.Math

Public Class EdgeDetector
    Private _ImageWidth As Integer
    Private _ImageHeight As Integer
    Private _Image As Bitmap
    Private dblAmplifier As Double = 4

    Public Property Image() As Bitmap
        Get
            Return _Image
        End Get
        Set(ByVal value As Bitmap)
            _Image = value
        End Set
    End Property


    Public ReadOnly Property ImageWidth()
        Get
            Return _ImageWidth
        End Get
    End Property

    Public ReadOnly Property ImageHeight()
        Get
            Return _ImageHeight
        End Get
    End Property

    Public Sub LoadImage(ByVal ImagePath As String)
        _Image = Bitmap.FromFile(ImagePath)
        _ImageWidth = _Image.Width
        _ImageHeight = _Image.Height


    End Sub



    Private Function ValidColor(ByVal ColorValue As Integer) As Integer
        Dim intColor As Integer = IIf(ColorValue < 0, 0, ColorValue)
        Return IIf(intColor > 255, 255, intColor)
    End Function

    'compares 2 colors and return a difference shading
    Private Function ColorDiff(ByVal Color1 As Color, ByVal Color2 As Color) As Integer

    End Function


    Public Function ConvertToGrayscale() As Bitmap
        Dim bm As New Bitmap(_Image.Width, _Image.Height)
        Dim x
        Dim y
        For y = 0 To bm.Height - 1
            For x = 0 To bm.Width - 1
                Dim c As Color = _Image.GetPixel(x, y)
                Dim luma As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)
                bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma))
            Next
        Next
        Return bm
    End Function

    Private Function PixelValid(ByVal xloc As Integer, ByVal yloc As Integer) As Integer
 

        If xloc < 0 Then Return -1
        If xloc >= _ImageWidth Then Return -1
        If yloc < 0 Then Return -1
        If yloc >= _ImageHeight Then Return -1

        Dim c As Color = _Image.GetPixel(xloc, yloc)
        Return c.R
    End Function

    Private Function EightCellCheck(ByVal x As Integer, ByVal y As Integer, ByVal intValue As Integer) As Integer

        Dim arrValues(7) As Integer
        arrValues(0) = PixelValid(x - 1, y - 1)
        arrValues(1) = PixelValid(x, y - 1)
        arrValues(2) = PixelValid(x + 1, y - 1)
        arrValues(3) = PixelValid(x - 1, y)
        arrValues(4) = PixelValid(x + 1, y)
        arrValues(5) = PixelValid(x - 1, y + 1)
        arrValues(6) = PixelValid(x, y + 1)
        arrValues(7) = PixelValid(x + 1, y + 1)

        Dim intValidItems As Integer = 0
        Dim lngCounter As Integer
        For lngCounter = 0 To 7
            If arrValues(lngCounter) <> -1 Then
                intValidItems += 1
            End If
        Next lngCounter

        Dim dblTotal As Double = CDbl(intValue)
        For lngCounter = 0 To 7
            If arrValues(lngCounter) <> -1 Then
                Dim dblValue As Double = CDbl(arrValues(lngCounter)) / CDbl(intValidItems)
                dblTotal = dblTotal - dblValue
            End If
        Next lngCounter

        Dim dblAbsval As Double = System.Math.Round(dblTotal * dblAmplifier, 0)

        'ON-CENTER is inhibitory if light falls on the surround, so if the total of the surrounding
        '8 pixels is bigger than the center, then it will be fully inhibitive. Therefore firing activity=0

        If dblAbsval < 0 Then dblAbsval = 0

        Return Min(dblAbsval, 255)
    End Function

    Public Function DoPass(ConvolutionSize As Integer, ByRef MainMasks As mask) As Bitmap
        Dim _ImageWidth As Integer = _Image.Width
        Dim _ImageHeight As Integer = _Image.Height

        Dim bm As New Bitmap(_Image.Width, _Image.Height)
        Dim xconvols As Integer = _ImageWidth \ ConvolutionSize
        Dim yconvols As Integer = _ImageHeight \ ConvolutionSize

        Dim xc As Integer
        Dim yc As Integer
        For yc = 0 To yconvols - 1
            For xc = 0 To xconvols - 1
                Dim _orientation As Integer = CheckOrientation(xc, yc, ConvolutionSize)


                MapPixels(bm, xc, yc, ConvolutionSize, _orientation)
            Next
        Next
        Return bm
    End Function

    Private Function CheckOrientation(xc As Integer, yc As Integer, convolsize As Integer) As Integer
        Dim leftx As Integer = xc * convolsize
        Dim topy As Integer = yc * convolsize

        Dim ipx As Integer
        Dim jpx As Integer
        For ipx = topy To (topy + convolsize - 1)
            For jpx = leftx To (leftx + convolsize - 1)

            Next jpx
        Next ipx
    End Function

    Private Function MapPixels(ByRef bm As Bitmap, xc As Integer, yc As Integer, ConvolSize As Integer, Orientation As Integer)

    End Function

    Public Function EdgeDetect() As Bitmap
        Dim _ImageWidth As Integer = _Image.Width
        Dim _ImageHeight As Integer = _Image.Height

        Dim bm As New Bitmap(_Image.Width, _Image.Height)
        Dim x
        Dim y
        For y = 0 To bm.Height - 1
            For x = 0 To bm.Width - 1
                Dim c As Color = _Image.GetPixel(x, y)
                Dim intValue As Integer = c.R

                Dim intFinalValue As Integer = EightCellCheck(x, y, intValue)
                bm.SetPixel(x, y, Color.FromArgb(intFinalValue, intFinalValue, intFinalValue))
            Next
        Next
        Return bm
    End Function

    Public Function OffsetImage(ByVal OffsetR As Integer, ByVal OffsetG As Integer, ByVal OffsetB As Integer) As Bitmap
        Dim lngCounter As Integer
        Dim lngCounter2 As Integer

        Dim bm As New Bitmap(_Image.Width, _Image.Height)
        For lngCounter = 0 To _ImageWidth - 1
            For lngCounter2 = 0 To _ImageHeight - 1
                Dim oriCol As Color = _Image.GetPixel(lngCounter, lngCounter2)
                bm.SetPixel(lngCounter, lngCounter2, Color.FromArgb(100, ValidColor(oriCol.R + OffsetR), ValidColor(oriCol.G + OffsetG), ValidColor(oriCol.B + OffsetB)))
            Next lngCounter2
        Next
        Return bm
    End Function
End Class



