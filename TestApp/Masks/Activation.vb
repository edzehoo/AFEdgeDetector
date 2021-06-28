Public Class Activation
    Private _Ensemble As Ensemble
    Private _x As Integer
    Private _y As Integer

    Public Property Ensemble As Ensemble
        Get
            Return _Ensemble
        End Get
        Set(value As Ensemble)
            _Ensemble = value
        End Set
    End Property


    Public Property X As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            _x = value
        End Set
    End Property

    Public Property Y As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            _y = value
        End Set
    End Property

End Class
