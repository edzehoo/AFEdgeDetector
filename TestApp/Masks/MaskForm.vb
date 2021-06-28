
Public Class MaskForm
    Private _orientation As Constants.ORIENTATIONTYPES
    Private _MaskVariationsCount As Integer = 0

    Public Property MaskVariationsCount As Integer
        Get
            Return _MaskVariationsCount
        End Get
        Set(value As Integer)
            _MaskVariationsCount = value
        End Set
    End Property

    Public Property Orientation As Constants.ORIENTATIONTYPES
        Get
            Return _orientation
        End Get
        Set(value As Constants.ORIENTATIONTYPES)
            _orientation = value
        End Set
    End Property
End Class
