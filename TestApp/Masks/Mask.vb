Public Class Mask
    Private _masklist As New List(Of MaskForm)
    Private _HyperColumn As FastPixel = Nothing


    Public Property HyperColumn As FastPixel
        Get
            Return _HyperColumn
        End Get
        Set(value As FastPixel)
            _HyperColumn = value
        End Set
    End Property


    Public Property MaskList As List(Of MaskForm)
        Get
            Return _masklist
        End Get
        Set(value As List(Of MaskForm))
            _masklist = value
        End Set
    End Property

    'Public Sub Load3x3Masks()
    '    Dim _3by3masks As String = My.Resources._3by3Masks
    '    Dim arrOrientations() As String = Split(_3by3masks, "<nextorientation>")
    '    Dim i As Integer
    '    For i = 0 To UBound(arrOrientations)
    '        Dim _masks As String = gen.TrimLeftRight(arrOrientations(i))

    '        Dim arrVariations() As String = Split(_masks, "<variation>")
    '        Dim j As Integer = 0

    '        Dim _mf As New MaskForm
    '        _mf.Orientation = i

    '        For j = 0 To UBound(arrVariations)
    '            Dim _var As String = gen.TrimLeftRight(arrVariations(j))


    '            Dim arrvarlines() As String = Split(_var, vbCrLf)

    '            Dim _mymask(2, 2) As Double

    '            If UBound(arrvarlines) = 2 Then
    '                Dim arrFirstRowVals() As String = Split(arrvarlines(0), ",")
    '                Dim arrSecondRowVals() As String = Split(arrvarlines(1), ",")
    '                Dim arrthirdRowVals() As String = Split(arrvarlines(2), ",")
    '                If UBound(arrFirstRowVals) = 2 And UBound(arrSecondRowVals) = 2 And UBound(arrthirdRowVals) = 2 Then
    '                    _mymask(0, 0) = arrFirstRowVals(0)
    '                    _mymask(1, 0) = arrFirstRowVals(1)
    '                    _mymask(2, 0) = arrFirstRowVals(2)
    '                    _mymask(0, 1) = arrSecondRowVals(0)
    '                    _mymask(1, 1) = arrSecondRowVals(1)
    '                    _mymask(2, 1) = arrSecondRowVals(2)
    '                    _mymask(0, 2) = arrthirdRowVals(0)
    '                    _mymask(1, 2) = arrthirdRowVals(1)
    '                    _mymask(2, 2) = arrthirdRowVals(2)
    '                End If
    '            End If

    '            _mf.MaskVariations.Add(_mymask)
    '        Next j
    '        _masklist.Add(_mf)
    '    Next i

    'End Sub

    Public Sub LoadHypercolumns()
        Dim _bmap As Bitmap = System.Drawing.Bitmap.FromFile("D:\Business Ventures Backup\Personal Hobbies\Implementation-12Jul\AFEdgeDetector\TestApp\images\maskmain.png")
        _HyperColumn = New FastPixel(_bmap)

        Dim _mf As New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_0
        _mf.MaskVariationsCount = 9
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_11
        _mf.MaskVariationsCount = 8
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_22
        _mf.MaskVariationsCount = 10
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_33
        _mf.MaskVariationsCount = 11
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_45
        _mf.MaskVariationsCount = 13
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_56
        _mf.MaskVariationsCount = 14
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_67
        _mf.MaskVariationsCount = 11
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_78
        _mf.MaskVariationsCount = 11
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_90
        _mf.MaskVariationsCount = 9
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_101
        _mf.MaskVariationsCount = 11
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_112
        _mf.MaskVariationsCount = 11
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_123
        _mf.MaskVariationsCount = 14
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_135
        _mf.MaskVariationsCount = 13
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_146
        _mf.MaskVariationsCount = 11
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_157
        _mf.MaskVariationsCount = 10
        _masklist.Add(_mf)

        _mf = New MaskForm
        _mf.Orientation = Constants.ORIENTATIONTYPES.OT_168
        _mf.MaskVariationsCount = 8
        _masklist.Add(_mf)
    End Sub


    'Public Sub LoadMasks()
    '    Dim _2by2masks As String = My.Resources._2by2Masks
    '    Dim arrOrientations() As String = Split(_2by2masks, "<nextorientation>")
    '    Dim i As Integer
    '    For i = 0 To UBound(arrOrientations)
    '        Dim _masks As String = gen.TrimLeftRight(arrOrientations(i))

    '        Dim arrVariations() As String = Split(_masks, "<variation>")
    '        Dim j As Integer = 0

    '        Dim _mf As New MaskForm
    '        _mf.Orientation = i

    '        For j = 0 To UBound(arrVariations)
    '            Dim _var As String = gen.TrimLeftRight(arrVariations(j))


    '            Dim arrvarlines() As String = Split(_var, vbCrLf)

    '            Dim _mymask(1, 1) As Double

    '            If UBound(arrvarlines) = 1 Then
    '                Dim arrFirstRowVals() As String = Split(arrvarlines(0), ",")
    '                Dim arrSecondRowVals() As String = Split(arrvarlines(1), ",")
    '                If UBound(arrFirstRowVals) = 1 And UBound(arrSecondRowVals) = 1 Then
    '                    _mymask(0, 0) = arrFirstRowVals(0)
    '                    _mymask(1, 0) = arrFirstRowVals(1)
    '                    _mymask(0, 1) = arrSecondRowVals(0)
    '                    _mymask(1, 1) = arrSecondRowVals(1)
    '                End If
    '            End If

    '            _mf.MaskVariations.Add(_mymask)
    '        Next j
    '        _masklist.Add(_mf)
    '    Next i

    'End Sub
End Class
