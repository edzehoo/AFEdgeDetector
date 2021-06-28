Public Class Ensemble
    Private _Ensemble(,) As Integer
    Private _width As Integer
    Private _height As Integer


    Public Property EnsembleMatrix() As Integer(,)
        Get
            Return _Ensemble
        End Get
        Set(value As Integer(,))
            _Ensemble = value
        End Set
    End Property

    Public Sub New(Width As Integer, Height As Integer)
        _width = Width
        _height = Height
        ReDim _Ensemble(Width - 1, Height - 1)
    End Sub

    'Return a match amount after comparison
    Public Function ComparePattern(ByRef BMSrc As FastPixel, ByRef rect As Rectangle) As Double

    End Function

    Private Function CompareLumaDelta(Col1 As Integer, Col2 As Integer) As Boolean
        If Math.Abs(Col1 - Col2) > EdgeDetector.MATCHLUMA_DELTA Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function ComparePattern(ByRef DiffMatrix(,) As Integer, OriginX As Integer, OriginY As Integer) As Double
        'when we compare, we check how near we match the pattern.
        'the algorithm is that we compare only the relative diff matrix from origin
        'LUMA is ignored at this point, because we just wanna know if we match the pattern

        Dim x As Integer = 0
        Dim y As Integer = 0
        For y = 0 To UBound(DiffMatrix, 2)
            For x = 0 To UBound(DiffMatrix, 1)
                If x <> OriginX And y <> OriginY Then
                    'compare against the ensemble. 
                    If CompareLumaDelta(DiffMatrix(x, y), _Ensemble(x, y)) = False Then
                        Return 0
                    End If
                End If
            Next x
        Next y

        Return 1
    End Function



    'Private Function GetMatrix(originX As Integer, originY As Integer, Color As Drawing.Color, ByRef BMSrc As FastPixel, ByRef rect As Rectangle) As Byte(,)
    '    Dim i As Integer = 0
    '    Dim j As Integer = 0
    '    Dim myarray(,) As Byte
    '    For j = rect.Y To rect.Y + rect.Height - 1
    '        For i = rect.X To rect.X + rect.Width - 1
    '            Dim _col As System.Drawing.Color = BMSrc.GetPixel(i, j)
    '            If IsBlack(_col) = False And j <> originY And i <> originX Then
    '                'we need to translate each and every pixel to a matrix of 
    '                'relationships


    '                'check against the template stored to see if any one matches


    '            End If
    '        Next i
    '    Next j
    'End Function



    Private Sub CompareWhiteDots(ByRef BMSrc As FastPixel, x As Integer, y As Integer)
        Dim k As Integer
        Dim j As Integer
        Dim _rating As Double
        For j = 0 To _height - 1
            For k = 0 To _width - 1
                _rating = CompareEnsemble(k, j, BMSrc, x, y)


            Next k
        Next j
    End Sub

    Private Function CompareEnsemble(X As Integer, Y As Integer, ByRef BMSrc As FastPixel, x2 As Integer, y2 As Integer)

    End Function

    Private Function IsBlack(ByRef Col As System.Drawing.Color) As Boolean
        If Col.R = 0 And Col.G = 0 And Col.B = 0 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
