Imports System.Drawing
Imports System.Math

Public Class EdgeDetector
    Private _ImageWidth As Integer
    Private _ImageHeight As Integer
    Private _Image As Bitmap
    Private dblAmplifier As Double = 8
    Private _globcount As Integer = 0
    Private dblAmplifierCD As Double = 4
    Private Colormineobject = New ColorMine.ColorSpaces.Comparisons.CieDe2000Comparison()
    Private Enum FILTERTYPE
        LAYOUT_LUMINANCE = 0
        COLOR = 1

    End Enum
    Private Const _WINDOWWIDTH As Integer = 3
    Private Const _WINDOWHEIGHT As Integer = 3
    Public Const MATCHLUMA_DELTA As Integer = 30    'Not sure if this is a linear function
    Private Const _SLIDEHORZ As Integer = 1
    Private Const _SLIDEVERT As Integer = 1
    Private _ensembles As New List(Of Ensemble)     'The stored memory of ensembles
    Private _currentactivation As New List(Of Activation)     'The current activation of ensembles
    Private _lumamap As Integer(,)
    Private _GlobImageWidth As Integer
    Private _GlobImageHeight As Integer

    Public Property GlobCount As Integer
        Get
            Return _globcount
        End Get
        Set(value As Integer)
            _globcount = value
        End Set
    End Property

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

    Public Sub LoadImage(ByRef Img As Image)
        _Image = Img
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


    Public Function ConvertToGrayscale() As FastPixel
        Dim bmsource As New FastPixel(_Image)
        Dim bmdest As New FastPixel(_Image.Clone)
        Dim x
        Dim y
        bmsource.Lock()
        bmdest.Lock()
        For y = 0 To bmsource.Height - 1
            For x = 0 To bmsource.Width - 1
                Dim c As Color = bmsource.GetPixel(x, y)
                Dim luma As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)
                bmdest.SetPixel(x, y, Color.FromArgb(luma, luma, luma))
            Next
        Next
        bmsource.Unlock(True)
        bmdest.Unlock(True)
        Return bmdest
    End Function

    Private Function PixelValid(ByRef FP As FastPixel, ByVal xloc As Integer, ByVal yloc As Integer) As Integer


        If xloc < 0 Then Return -1
        If xloc >= _ImageWidth Then Return -1
        If yloc < 0 Then Return -1
        If yloc >= _ImageHeight Then Return -1

        Dim c As Color = FP.GetPixel(xloc, yloc)
        Dim luma As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)
        Return luma
    End Function

    Private Function PixelValidColor(ByRef FP As FastPixel, ByVal xloc As Integer, ByVal yloc As Integer) As Color


        If xloc < 0 Then Return Nothing
        If xloc >= _ImageWidth Then Return Nothing
        If yloc < 0 Then Return Nothing
        If yloc >= _ImageHeight Then Return Nothing

        Dim c As Color = FP.GetPixel(xloc, yloc)
        Return c
    End Function

    Private Function EightCellCheck(ByRef FP As FastPixel, ByVal x As Integer, ByVal y As Integer, ByVal intValue As Integer) As Integer

        Dim arrValues(7) As Integer
        arrValues(0) = PixelValid(FP, x - 1, y - 1)
        arrValues(1) = PixelValid(FP, x, y - 1)
        arrValues(2) = PixelValid(FP, x + 1, y - 1)
        arrValues(3) = PixelValid(FP, x - 1, y)
        arrValues(4) = PixelValid(FP, x + 1, y)
        arrValues(5) = PixelValid(FP, x - 1, y + 1)
        arrValues(6) = PixelValid(FP, x, y + 1)
        arrValues(7) = PixelValid(FP, x + 1, y + 1)

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

    Private Function CompareColors(Col1 As Color, Col2 As Color) As Double
        Dim colspace As New ColorMine.ColorSpaces.Rgb(Col1.R, Col1.G, Col1.B)
        Dim colspace2 As New ColorMine.ColorSpaces.Rgb(Col2.R, Col2.G, Col2.B)
        Return Colormineobject.Compare(colspace, colspace2)

    End Function

    Private Function EightCellColorCheck(ByRef FP As FastPixel, ByVal x As Integer, ByVal y As Integer, ByRef originalColor As Color) As Integer

        Dim arrValues(7) As Object
        arrValues(0) = PixelValidColor(FP, x - 1, y - 1)
        arrValues(1) = PixelValidColor(FP, x, y - 1)
        arrValues(2) = PixelValidColor(FP, x + 1, y - 1)
        arrValues(3) = PixelValidColor(FP, x - 1, y)
        arrValues(4) = PixelValidColor(FP, x + 1, y)
        arrValues(5) = PixelValidColor(FP, x - 1, y + 1)
        arrValues(6) = PixelValidcolor(FP, x, y + 1)
        arrValues(7) = PixelValidColor(FP, x + 1, y + 1)

        Dim intValidItems As Integer = 0
        Dim lngCounter As Integer
        For lngCounter = 0 To 7
            If arrValues(lngCounter) Is Nothing = False Then
                intValidItems += 1
            End If
        Next lngCounter

        'Here we attempt to compare colors
        'Dim dblTotal As Double = CDbl(intValue)
        Dim dblTotal As Double = 0
        For lngCounter = 0 To 7
            If arrValues(lngCounter) Is Nothing = False Then
                Dim dblValue As Double = CompareColors(originalColor, arrValues(lngCounter))
                dblTotal += dblValue
            End If
        Next lngCounter
        dblTotal = CDbl(dblTotal) / CDbl(intValidItems)

        'dbltotal is out of a 100 at this point, so we convert it to 255 scale
        dblTotal = dblTotal * 2.55

        Dim dblAbsval As Double = System.Math.Round(dblTotal * dblAmplifierCD, 0)

        If dblAbsval < 0 Then dblAbsval = 0
        Return Min(dblAbsval, 255)
    End Function

    Public Function DoPass(ByRef BMSource As FastPixel, ConvolutionSize As Integer, ByRef MainMasks As Mask, ShowColorMaps As Boolean, Threshold As Double) As FastPixel

        MainMasks.HyperColumn.Lock()



        Dim _ImageWidth As Integer = BMSource.Width
        Dim _ImageHeight As Integer = BMSource.Height

        Dim bmdest As New Bitmap(BMSource.Width, BMSource.Height, BMSource.Bitmap.PixelFormat)
        Dim _bmdest As New FastPixel(bmdest)

        Dim xconvols As Integer = _ImageWidth \ ConvolutionSize
        Dim yconvols As Integer = _ImageHeight \ ConvolutionSize

        _bmdest.Lock()
        BMSource.Lock()

        Dim xc As Integer
        Dim yc As Integer
        For yc = 0 To yconvols - 1
            For xc = 0 To xconvols - 1
                Dim matchedmaskx As Integer = -1
                Dim matchedmasky As Integer = -1
                Dim _orientation As Constants.ORIENTATIONTYPES = CheckOrientation(BMSource, _bmdest, xc, yc, ConvolutionSize, MainMasks, matchedmaskx, matchedmasky, Threshold)

                If ShowColorMaps = True Then
                    MapPixels(BMSource, _bmdest, xc, yc, ConvolutionSize, _orientation)
                Else
                    MapOrientationReconstruction(MainMasks, BMSource, _bmdest, xc, yc, ConvolutionSize, _orientation, matchedmaskx, matchedmasky)
                End If



            Next
        Next

        'For yc = 0 To yconvols - 1
        '    For xc = 0 To _ImageWidth - 1
        '        _bmdest.SetPixel(xc, yc * ConvolutionSize, Color.White)
        '    Next xc
        'Next yc

        'For xc = 0 To xconvols - 1
        '    For yc = 0 To _ImageHeight - 1
        '        _bmdest.SetPixel(xc * ConvolutionSize, yc, Color.White)
        '    Next yc
        'Next xc


        BMSource.Unlock(True)
        _bmdest.Unlock(True)
        MainMasks.HyperColumn.Unlock(True)
        Return _bmdest
    End Function

    Private Function CheckOrientation(ByRef BMSource As FastPixel, ByRef BMDest As FastPixel, xc As Integer, yc As Integer, convolsize As Integer, ByRef MainMasks As Mask, Optional ByRef MatchedMaskX As Integer = -1, Optional ByRef MatchedMaskY As Integer = -1, Optional Threshold As Double = 20) As Constants.ORIENTATIONTYPES
        Dim leftx As Integer = xc * convolsize
        Dim topy As Integer = yc * convolsize

        Dim ipx As Integer
        Dim jpx As Integer

        Dim _lom As List(Of MaskForm) = MainMasks.MaskList
        Dim l As Integer
        Dim m As Integer

        Dim _closestorientation As Constants.ORIENTATIONTYPES = -1
        Dim _highestpctg As Double = -1
        Dim _highestscore As Integer = -1
        MatchedMaskX = -1
        MatchedMaskY = -1

        For l = 0 To _lom.Count - 1
            Dim _mf As MaskForm = _lom.Item(l)
            'for each maskform
            For m = 0 To _mf.MaskVariationsCount - 1
                Dim _score As Integer = 0
                For ipx = topy To (topy + convolsize - 1)
                    For jpx = leftx To (leftx + convolsize - 1)
                        Dim y As Integer = ipx - topy
                        Dim x As Integer = jpx - leftx
                        Dim pixelvalue As Color = BMSource.GetPixel(jpx, ipx)

                        Dim hcx As Integer = (m * 9) + x
                        Dim hcy As Integer = (l * 9) + y
                        Dim pixelvalue2 As Color = MainMasks.HyperColumn.GetPixel(hcx, hcy)
                        _score += ComparePixel(pixelvalue, pixelvalue2)
                    Next jpx
                Next ipx
                Dim _totalvalue As Double = convolsize * 256    '2 pixels white
                Dim _similaritypctg As Double = (CDbl(_score) * 100) / _totalvalue

                If _similaritypctg > _highestpctg Then
                    MatchedMaskX = (m * 9)
                    MatchedMaskY = (l * 9)
                    _highestscore = _score
                    _highestpctg = _similaritypctg
                    _closestorientation = _mf.Orientation
                End If

                '        Dim _matrix As Double(,) = _mf.MaskVariations.Item(m)

                '        For ipx = topy To (topy + convolsize - 1)
                '            For jpx = leftx To (leftx + convolsize - 1)
                '                Dim y As Integer = ipx - topy
                '                Dim x As Integer = jpx - leftx

                '                Dim pixelvalue As Color = BMSource.GetPixel(jpx, ipx)
                '                _score += ComparePixel(pixelvalue, _matrix(x, y))
                '            Next jpx
                '        Next ipx
                '        Dim _totalvalue As Double = convolsize * 256    '2 pixels white
                '        _similaritypctg = (CDbl(_score) * 100) / _totalvalue

                '        If _similaritypctg > _highestpctg Then
                '            _highestpctg = _similaritypctg
                '            _closestorientation = _mf.Orientation
                '        End If
            Next m
        Next l


        If _highestpctg >= Threshold Then
            Return _closestorientation
        Else
            MatchedMaskX = -1
            MatchedMaskY = -1
            Return Constants.ORIENTATIONTYPES.OT_NOMATCH
        End If


    End Function

    Private Function ComparePixel(ByRef Pix1 As Color, Pix2 As Color) As Integer
        Dim _value As Integer = Pix1.R
        Dim _value2 As Integer = Pix2.R
        If _value2 = 0 Then
            'for black area, any value of pix1 would be negative
            Return 0
            'Return -_value
        Else
            'for any white area, any value of pix1 would be positive
            Return _value
        End If
    End Function

    Private Function MapPixels(ByRef bmSource As FastPixel, ByRef bmDest As FastPixel, xc As Integer, yc As Integer, ConvolSize As Integer, Orientation As Constants.ORIENTATIONTYPES)
        Dim leftx As Integer = xc * ConvolSize
        Dim topy As Integer = yc * ConvolSize
        Dim _ocol As Color = GetorientationColor(Orientation)
        Dim x As Integer
        Dim y As Integer
        For x = leftx To (leftx + ConvolSize - 1)
            For y = topy To (topy + ConvolSize - 1)
                bmDest.SetPixel(x, y, _ocol)
            Next y
        Next x
    End Function

    Private Function GetMaskPixel(ByRef mainmask As Mask, Orientation As Constants.ORIENTATIONTYPES, x As Integer, y As Integer) As Color
        Select Case Orientation
            Case Constants.ORIENTATIONTYPES.OT_0
                Dim xfin As Integer = (3 * 9) + x
                Dim yfin As Integer = (0 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_11
                Dim xfin As Integer = (3 * 9) + x
                Dim yfin As Integer = (1 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_22
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (2 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_33
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (3 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_45
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (4 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_56
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (5 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_67
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (6 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_78
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (7 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_90
                Dim xfin As Integer = (4 * 9) + x
                Dim yfin As Integer = (8 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_101
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (9 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_112
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (10 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_123
                Dim xfin As Integer = (6 * 9) + x
                Dim yfin As Integer = (11 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_135
                Dim xfin As Integer = (6 * 9) + x
                Dim yfin As Integer = (12 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_146
                Dim xfin As Integer = (4 * 9) + x
                Dim yfin As Integer = (13 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_157
                Dim xfin As Integer = (5 * 9) + x
                Dim yfin As Integer = (14 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Constants.ORIENTATIONTYPES.OT_168
                Dim xfin As Integer = (3 * 9) + x
                Dim yfin As Integer = (15 * 9) + y
                Return mainmask.HyperColumn.GetPixel(xfin, yfin)
            Case Else
                Return Color.Black
        End Select
    End Function

    Private Function MapOrientationReconstruction(ByRef mainmask As Mask, ByRef bmSource As FastPixel, ByRef bmDest As FastPixel, xc As Integer, yc As Integer, ConvolSize As Integer, Orientation As Constants.ORIENTATIONTYPES, MatchedMaskX As Integer, MatchedMaskY As Integer)
        Dim leftx As Integer = xc * ConvolSize
        Dim topy As Integer = yc * ConvolSize
        'Dim _ocol As Color = GetorientationColor(Orientation)
        Dim x As Integer
        Dim y As Integer
        For x = leftx To (leftx + ConvolSize - 1)
            For y = topy To (topy + ConvolSize - 1)
                Dim _offsetleft = x - leftx
                Dim _offsettop = y - topy

                If MatchedMaskX = -1 Or MatchedMaskY = -1 Then
                    bmDest.SetPixel(x, y, Color.Black)
                Else
                    bmDest.SetPixel(x, y, mainmask.HyperColumn.GetPixel(MatchedMaskX + _offsetleft, MatchedMaskY + _offsettop))
                End If
            Next y
        Next x
    End Function

    Private Function GetorientationColor(Orientation As Constants.ORIENTATIONTYPES) As Color
        'If Orientation = Constants.ORIENTATIONTYPES.OT_NOMATCH Then
        '    Return Color.Black
        'Else
        '    Return Color.White
        'End If


        Select Case Orientation
            Case Constants.ORIENTATIONTYPES.OT_0
                Return Color.FromArgb(216, 171, 14)
            Case Constants.ORIENTATIONTYPES.OT_11
                Return Color.FromArgb(216, 128, 14)
            Case Constants.ORIENTATIONTYPES.OT_22
                Return Color.FromArgb(216, 81, 14)
            Case Constants.ORIENTATIONTYPES.OT_33
                Return Color.FromArgb(216, 28, 14)
            Case Constants.ORIENTATIONTYPES.OT_45
                Return Color.FromArgb(162, 6, 102)
            Case Constants.ORIENTATIONTYPES.OT_56
                Return Color.FromArgb(212, 34, 189)
            Case Constants.ORIENTATIONTYPES.OT_67
                Return Color.FromArgb(176, 34, 212)
            Case Constants.ORIENTATIONTYPES.OT_78
                Return Color.FromArgb(139, 34, 212)
            Case Constants.ORIENTATIONTYPES.OT_90
                Return Color.FromArgb(109, 34, 212)
            Case Constants.ORIENTATIONTYPES.OT_101
                Return Color.FromArgb(76, 34, 212)
            Case Constants.ORIENTATIONTYPES.OT_112
                Return Color.FromArgb(34, 59, 212)
            Case Constants.ORIENTATIONTYPES.OT_123
                Return Color.FromArgb(34, 105, 212)
            Case Constants.ORIENTATIONTYPES.OT_135
                Return Color.FromArgb(34, 168, 212)
            Case Constants.ORIENTATIONTYPES.OT_146
                Return Color.FromArgb(34, 212, 185)
            Case Constants.ORIENTATIONTYPES.OT_157
                Return Color.FromArgb(63, 212, 34)
            Case Constants.ORIENTATIONTYPES.OT_168
                Return Color.FromArgb(189, 212, 34)
            Case Else
                Return Color.White
        End Select
    End Function

    Public Function EdgeDetect(ByRef BMSource As FastPixel) As FastPixel
        Dim _ImageWidth As Integer = BMSource.Width
        Dim _ImageHeight As Integer = BMSource.Height

        _GlobImageWidth = _ImageWidth
        _GlobImageHeight = _ImageHeight

        Dim bm As New Bitmap(_ImageWidth, _ImageHeight, BMSource.Bitmap.PixelFormat)

        Dim bmdest As New FastPixel(bm)
        Dim x
        Dim y
        BMSource.Lock()
        bmdest.Lock()

        'Redimension lumamap
        ReDim _lumamap(_ImageWidth - 1, _ImageHeight - 1)

        For y = 0 To BMSource.Height - 1
            For x = 0 To BMSource.Width - 1
                Dim c As Color = BMSource.GetPixel(x, y)
                Dim intValue As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)

                Dim intFinalValue As Integer = EightCellCheck(BMSource, x, y, intValue)
                bmdest.SetPixel(x, y, Color.FromArgb(intFinalValue, intFinalValue, intFinalValue))
                _lumamap(x, y) = intFinalValue

            Next
        Next
        bmdest.Unlock(True)
        BMSource.Unlock(True)
        Return bmdest
    End Function

    Private Function IsBlack(ByRef col As System.Drawing.Color) As Boolean
        If col.R = 0 And col.G = 0 And col.B = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function IsBlack(ByRef Luma As Integer) As Boolean
        If Luma = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetDiffMatrix(ByRef BMSource As FastPixel, x As Integer, y As Integer, ByRef OriginX As Integer, ByRef OriginY As Integer) As Integer(,)
        Dim _origin As Integer = _lumamap(x, y)
        OriginX = -1
        OriginY = -1

        Dim diffmatrix(,) As Integer
        ReDim diffmatrix(_WINDOWWIDTH - 1, _WINDOWHEIGHT - 1)

        Dim xstart As Integer = x - ((_WINDOWWIDTH - 1) \ 2)
        Dim ystart As Integer = y - ((_WINDOWHEIGHT - 1) \ 2)
        Dim xend As Integer = xstart + _WINDOWWIDTH - 1
        Dim yend As Integer = ystart + _WINDOWHEIGHT - 1

        'get the differences
        Dim a As Integer
        Dim b As Integer
        For a = ystart To yend
            For b = xstart To xend
                If a < 0 Or a >= _GlobImageHeight Or b < 0 Or b >= _GlobImageWidth Then
                    diffmatrix(b - xstart, a - ystart) = 0 - _origin
                ElseIf a = y And b = x Then
                    'its the origin
                    diffmatrix(b - xstart, a - ystart) = _origin

                    'we set the origin within the diffmatrix
                    OriginX = b - xstart
                    OriginY = a - ystart
                Else
                    diffmatrix(b - xstart, a - ystart) = _lumamap(b, a) - _origin
                End If

            Next b
        Next a

        Return diffmatrix
    End Function

    Private Sub FindTemplate(ByRef BMSource As FastPixel, x As Integer, y As Integer)
        'get the area of the 8 squares around the main square

        Dim _originX As Integer = -1
        Dim _originY As Integer = -1

        'we get the difference quotient between middle cell and all its surrounding cells
        Dim arrDiffMatrix(,) As Integer = GetDiffMatrix(BMSource, x, y, _originx, _originy)

        If _originX <> -1 And _originY <> -1 Then
            _globcount += 1

            'Save output
            'gen.OutputArray(arrDiffMatrix)

            'we try to compare this patch with what's in the ensembles. 
            'Any fastest way to compare?

            Dim _ensemblefound As Ensemble = Nothing
            If _ensembles.Count > 0 Then
                Dim i As Integer
                For i = 0 To _ensembles.Count - 1
                    If _ensembles.Item(i).ComparePattern(arrDiffMatrix, _originX, _originY) = 1 Then
                        _ensemblefound = _ensembles.Item(i)
                        Exit For
                    End If
                Next i
            End If
            If _ensemblefound Is Nothing Then
                Dim _ensem As New Ensemble(_WINDOWWIDTH, _WINDOWHEIGHT)
                _ensem.EnsembleMatrix = arrDiffMatrix
                _ensembles.Add(_ensem)
                _ensemblefound = _ensem
            End If

            'ENSEMBLEFOUND now represents that ensemble. Since we found this, we fire next level
            Dim _activation As New Activation
            _activation.Ensemble = _ensemblefound
            _activation.X = x
            _activation.Y = y
            _currentactivation.Add(_activation)
        End If



    End Sub

    Public Function TraceOutlines(ByRef BMSource As FastPixel) As FastPixel
        Dim _ImageWidth As Integer = BMSource.Width
        Dim _ImageHeight As Integer = BMSource.Height

        Dim bm As New Bitmap(_ImageWidth, _ImageHeight, BMSource.Bitmap.PixelFormat)

        Dim bmdest As New FastPixel(bm)
        Dim x As Integer
        Dim y As Integer
        BMSource.Lock()
        bmdest.Lock()

        _currentactivation.Clear()

        Dim _scount As Integer = 0
        For y = 0 To BMSource.Height - 1
            For x = 0 To BMSource.Width - 1
                'if its non black, we try to match here, and we emplant the 3x3 to grab snapshot
                'around the pixel, compare with templates... 

                Dim _luma As Integer = _lumamap(x, y)
                If IsBlack(_luma) = False Then
                    _scount += 1
                    FindTemplate(BMSource, x, y)
                End If
            Next x
        Next y



        'now we go through all ensembles
        FireNextLevel(BMSource)

        bmdest.Unlock(True)
        BMSource.Unlock(True)

        MsgBox("Total pixel processed:" & _scount)
        MsgBox(_currentactivation.Count)

        MsgBox("Ensembles generated:" & _ensembles.Count)

        Return bmdest
    End Function

    Private Function FireNextLevel(ByRef BMSource As FastPixel)
        Dim i As Integer
        System.IO.File.WriteAllText("d:\vbtest\activations.txt", "")
        For i = 0 To _currentactivation.Count - 1
            System.IO.File.AppendAllText("d:\vbtest\activations.txt", _currentactivation.Item(i).X & "," & _currentactivation.Item(i).Y & vbCrLf)
        Next i
    End Function

    Private Function GetLuminanceMap(ByRef BMSource As FastPixel) As Integer(,)
        Dim i As Integer = 0

    End Function

    Private Function ProcessWindow(ByRef BMSource As FastPixel, x As Integer, y As Integer, imgwidth As Integer, imgheight As Integer, ft As FILTERTYPE)
        'here we try to process the window
        For y = 0 To BMSource.Height - 1
            For x = 0 To BMSource.Width - 1
                Dim _col As System.Drawing.Color = BMSource.GetPixel(x, y)

            Next x
        Next y

    End Function

    Private Sub SaveExistingTemplate(ByRef BMSource As FastPixel, ByRef Rect As Rectangle)
        Dim i As Integer
        Dim _highestamt As Double = -1
        Dim _highestensemble As Ensemble = Nothing
        For i = 0 To _ensembles.Count - 1
            Dim _amt As Double = _ensembles.Item(i).ComparePattern(BMSource, Rect)
            If _amt > _highestamt Then
                _highestamt = _amt
                _highestensemble = _ensembles.Item(i)
            End If
        Next i

        If _highestensemble Is Nothing = False Then
            'we check the amount and see if we should declare it as new pattern 
            'or phase it into an existing pattern, based on threshold

        End If




    End Sub



    Public Function ColorDiff(ByRef BMSource As FastPixel) As FastPixel
        Dim _ImageWidth As Integer = BMSource.Width
        Dim _ImageHeight As Integer = BMSource.Height

        _GlobImageWidth = _ImageWidth
        _GlobImageHeight = _ImageHeight

        'Redimension lumamap
        ReDim _lumamap(_ImageWidth - 1, _ImageHeight - 1)

        Dim bm As New Bitmap(_ImageWidth, _ImageHeight, BMSource.Bitmap.PixelFormat)

        Dim bmdest As New FastPixel(bm)
        Dim x
        Dim y
        BMSource.Lock()
        bmdest.Lock()



        For y = 0 To BMSource.Height - 1
            For x = 0 To BMSource.Width - 1
                Dim c As Color = BMSource.GetPixel(x, y)

                Dim intFinalValue As Integer = EightCellColorCheck(BMSource, x, y, c)
                bmdest.SetPixel(x, y, Color.FromArgb(intFinalValue, intFinalValue, intFinalValue))
                _lumamap(x, y) = intFinalValue

            Next
        Next
        bmdest.Unlock(True)
        BMSource.Unlock(True)
        Return bmdest
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



