Public Class gen
    Public Shared Function TrimLeftRight(Main As String) As String
        Dim trimChars(2) As Char
        trimChars(0) = vbCr
        trimChars(1) = vbLf
        trimChars(2) = " "

        Return Main.TrimStart(trimChars).TrimEnd(trimChars)
    End Function

    Public Shared Function OutputArray(ByRef myArray(,) As Integer) As String
        Dim a As Integer
        Dim b As Integer
        Dim _calluci As String = ""
        For b = 0 To UBound(myArray, 2)
            For a = 0 To UBound(myArray, 1)
                _calluci += " " + myArray(a, b).ToString
            Next a
            _calluci += vbCrLf
        Next b
        _calluci += vbCrLf + vbCrLf

        System.IO.File.AppendAllText("d:\vbtest\outputarray.txt", _calluci)

        Return _calluci
    End Function

End Class


