'ED's implementation of Fastpixel
Public Class FastPixel
    Private rgbValues() As Byte
    Private bmpData As Imaging.BitmapData
    Private bmpPtr As IntPtr
    Private locked As Boolean = False
    Private _stride As Integer
    Private _isAlpha As Boolean = False
    Private _bitmap As Bitmap
    Private _width As Integer
    Private _height As Integer
    Private _bytespp As Integer

    Public ReadOnly Property RGBValuesArray As Byte()
        Get
            Return rgbValues
        End Get
    End Property

    Public ReadOnly Property BytesPP() As Integer
        Get
            Return _bytespp
        End Get
    End Property
    Public ReadOnly Property Width() As Integer
        Get
            Return Me._width
        End Get
    End Property
    Public ReadOnly Property Height() As Integer
        Get
            Return Me._height
        End Get
    End Property
    Public ReadOnly Property IsAlphaBitmap() As Boolean
        Get
            Return Me._isAlpha
        End Get
    End Property
    Public ReadOnly Property Bitmap() As Bitmap
        Get
            Return Me._bitmap
        End Get
    End Property



    Public Sub New(ByVal bitmap As Bitmap)


        If (bitmap.PixelFormat = (bitmap.PixelFormat Or Imaging.PixelFormat.Indexed)) Then
            Throw New Exception("Cannot lock an Indexed image.")
            Return
        End If
        Me._bitmap = bitmap
        Me._isAlpha = (Me.Bitmap.PixelFormat = (Me.Bitmap.PixelFormat Or Imaging.PixelFormat.Alpha))

        Me._width = bitmap.Width
        Me._height = bitmap.Height
    End Sub

    Public Sub Lock()
        If Me.locked Then
            Throw New Exception("Bitmap already locked.")
            Return
        End If

        Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)
        Me.bmpData = Me.Bitmap.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, Me.Bitmap.PixelFormat)
        Me.bmpPtr = Me.bmpData.Scan0
        Me._bytespp = Image.GetPixelFormatSize(Me.Bitmap.PixelFormat) \ 8

        _stride = Me.bmpData.Stride

        Dim bytes As Integer = (_stride * Me.Height)
        ReDim Me.rgbValues(bytes - 1)
        System.Runtime.InteropServices.Marshal.Copy(Me.bmpPtr, rgbValues, 0, Me.rgbValues.Length)


        Me.locked = True
    End Sub
    Public Sub Unlock(ByVal setPixels As Boolean)
        If Not Me.locked Then
            Throw New Exception("Bitmap not locked.")
            Return
        End If
        ' Copy the RGB values back to the bitmap
        If setPixels Then System.Runtime.InteropServices.Marshal.Copy(Me.rgbValues, 0, Me.bmpPtr, Me.rgbValues.Length)
        ' Unlock the bits.
        Me.Bitmap.UnlockBits(bmpData)
        Me.locked = False
    End Sub

    Public Sub Clear(ByVal colour As Color)
        If Not Me.locked Then
            Throw New Exception("Bitmap not locked.")
            Return
        End If

        'If Me.IsAlphaBitmap Then
        '    For index As Integer = 0 To Me.rgbValues.Length - 1 Step 4
        '        Me.rgbValues(index) = colour.B
        '        Me.rgbValues(index + 1) = colour.G
        '        Me.rgbValues(index + 2) = colour.R
        '        Me.rgbValues(index + 3) = colour.A
        '    Next index
        'Else
        '    For index As Integer = 0 To Me.rgbValues.Length - 1 Step 3
        '        Me.rgbValues(index) = colour.B
        '        Me.rgbValues(index + 1) = colour.G
        '        Me.rgbValues(index + 2) = colour.R
        '    Next index
        'End If

        If Me.IsAlphaBitmap Then
            Dim i As Integer = 0
            For i = 0 To _height - 1
                Dim yindex As Integer = (i * _stride)
                For xindex As Integer = 0 To _stride - 1 Step Me._bytespp
                    Me.rgbValues(yindex + xindex) = colour.B
                    Me.rgbValues(yindex + xindex + 1) = colour.G
                    Me.rgbValues(yindex + xindex + 2) = colour.R
                    Me.rgbValues(yindex + xindex + 3) = colour.A
                Next xindex
            Next i
        Else
            Dim i As Integer = 0
            For i = 0 To _height - 1
                Dim yindex As Integer = (i * _stride)
                For xindex As Integer = 0 To _stride - 1 Step Me._bytespp
                    Me.rgbValues(yindex + xindex) = colour.B
                    Me.rgbValues(yindex + xindex + 1) = colour.G
                    Me.rgbValues(yindex + xindex + 2) = colour.R
                Next xindex
            Next i
        End If
    End Sub
    Public Sub SetPixel(ByVal location As Point, ByVal colour As Color)
        Me.SetPixel(location.X, location.Y, colour)
    End Sub
    Public Sub SetPixel(ByVal x As Integer, ByVal y As Integer, ByVal colour As Color)
        If Not Me.locked Then
            Throw New Exception("Bitmap not locked.")
            Return
        End If

        Dim index As Integer = (y * _stride) + (x * Me._bytespp)

        If Me.IsAlphaBitmap Then
            Me.rgbValues(index) = colour.B
            Me.rgbValues(index + 1) = colour.G
            Me.rgbValues(index + 2) = colour.R
            Me.rgbValues(index + 3) = colour.A
        Else
            Me.rgbValues(index) = colour.B
            Me.rgbValues(index + 1) = colour.G
            Me.rgbValues(index + 2) = colour.R
        End If
    End Sub
    Public Function GetPixel(ByVal location As Point) As Color
        Return Me.GetPixel(location.X, location.Y)
    End Function


    Public Function CloneRegion(ByRef Region As Rectangle, Optional UseFastArrayType As Boolean = False, Optional CommitImmediately As Boolean = True) As FastPixel
        Dim _bmap As New Bitmap(Region.Width, Region.Height, Me.bmpData.PixelFormat)
        Dim _fp As New FastPixel(_bmap)
        _fp.Lock()

        'we do a copy of raw byte data from one to another
        If UseFastArrayType = True Then
            Dim bytecount As Integer = Region.Width * Me._bytespp
            Dim y As Integer = 0
            For y = Region.Top To Region.Top + Region.Height - 1
                Dim yoffset As Integer = y * _stride
                Dim xoffset As Integer = Region.Left * Me._bytespp
                Array.Copy(Me.RGBValuesArray, yoffset + xoffset, _fp.RGBValuesArray, 0, bytecount)
            Next y
        Else
            Dim x As Integer = 0
            Dim y As Integer = 0
            For y = Region.Top To Region.Top + Region.Height - 1
                For x = Region.Left To Region.Left + Region.Width - 1
                    Dim c As System.Drawing.Color = Me.GetPixel(x, y)
                    _fp.SetPixel(x - Region.Left, y - Region.Top, c)
                Next x
            Next y
        End If


        _fp.Unlock(CommitImmediately)
        Return _fp
    End Function

    Public Function GetPixel(ByVal x As Integer, ByVal y As Integer) As Color
        If Not Me.locked Then
            Throw New Exception("Bitmap not locked.")
            Return Nothing
        End If

        Dim index As Integer = (y * _stride) + (x * Me._bytespp)

        If Me.IsAlphaBitmap Then
            Dim b As Integer = Me.rgbValues(index)
            Dim g As Integer = Me.rgbValues(index + 1)
            Dim r As Integer = Me.rgbValues(index + 2)
            Dim a As Integer = Me.rgbValues(index + 3)
            Return Color.FromArgb(a, r, g, b)
        Else
            Dim b As Integer = Me.rgbValues(index)
            Dim g As Integer = Me.rgbValues(index + 1)
            Dim r As Integer = Me.rgbValues(index + 2)
            Return Color.FromArgb(r, g, b)
        End If
    End Function
End Class