Public Class OrientationFinder
    Private _ImageWidth As Integer
    Private _ImageHeight As Integer
    Private _Image As Bitmap
    Private dblAmplifier As Double = 8
    Private bmsource As FastPixel
    Private _V1Size As Integer = 6
    Private _templates As New List(Of Bitmap)
    Private _CurrentResult As Integer = 0

    Public Property Image() As Bitmap
        Get
            Return _Image
        End Get
        Set(ByVal value As Bitmap)
            _Image = value
        End Set
    End Property

    Private Function CreateSectionBitmap(ByRef MyRect As Rectangle) As Bitmap
        Dim x As Integer = 0
        Dim y As Integer = 0
        Dim _bmap As New Bitmap(_V1Size, _V1Size, Imaging.PixelFormat.Format32bppRgb)
        For x = MyRect.Left To MyRect.Left + MyRect.Width - 1
            For y = MyRect.Top To MyRect.Top + MyRect.Height - 1
                Dim _col As Color = bmsource.GetPixel(x, y)
                _bmap.SetPixel(x - MyRect.Left, y - MyRect.Top, _col)
            Next y
        Next
        Return _bmap
    End Function

    Public Sub SplitImage(ByVal ImagePath As String)
        _CurrentResult += 1
        Dim y As Integer
        Dim x As Integer
        _Image = Bitmap.FromFile(ImagePath)
        _ImageWidth = _Image.Width
        _ImageHeight = _Image.Height

        bmsource = New FastPixel(_Image)
        bmsource.Lock()

        _templates.Clear()
        Dim _count As Integer = 0
        Dim intPixLeft As Integer = 0
        Dim intPixTop As Integer = 0

        Dim _elementsacross As Integer = 0

        For y = 0 To bmsource.Height - _V1Size Step 1
            For x = 0 To bmsource.Width - _V1Size Step 1
                _count += 1
                Dim myrect As New Rectangle(x, y, _V1Size, _V1Size)
                _templates.Add(bmsource.CloneRegion(myrect, False, True).Bitmap)

                If y = 0 Then _elementsacross += 1
            Next
        Next
errJumphere:
        MsgBox("All patches processed. Templates found:" & _templates.Count)
        'Now we output all our different templates

        DisplayAllPieces(_elementsacross)


        MsgBox("All template files generated")
    End Sub

    Private Sub DisplayAllPieces(PiecesPerRow As Integer)

        Dim newbm As New Bitmap(5000, 5000, Imaging.PixelFormat.Format32bppRgb)
        Dim G As Graphics = Graphics.FromImage(newbm)
        G.Clear(Color.FromArgb(192, 192, 192))

        Dim i As Integer
        For i = 0 To _templates.Count - 1
            Dim _srcbmap As Bitmap = _templates.Item(i)
            Dim xdest As Integer = i Mod PiecesPerRow
            Dim ydest As Integer = i \ PiecesPerRow
            CopyBMap(_templates.Item(i), newbm, xdest * (_V1Size + 2), ydest * (_V1Size + 2))
        Next i

        newbm.Save("D:\vbtest\samples\template" & _CurrentResult & ".png", Imaging.ImageFormat.Png)

    End Sub

    Private Sub CopyBlock(ByRef newBM As Bitmap, xstart As Integer, ystart As Integer)
        Dim x As Integer
        Dim y As Integer
        For y = ystart To ystart + _V1Size - 1
            For x = xstart To xstart + _V1Size - 1
                Dim _col As Color = bmsource.GetPixel(x, y)
                newBM.SetPixel(x, y, _col)
            Next x
        Next y
    End Sub

    Public Sub LoadImage(ByVal ImagePath As String)
        _CurrentResult += 1
        Dim y As Integer
        Dim x As Integer
        _Image = Bitmap.FromFile(ImagePath)
        _ImageWidth = _Image.Width
        _ImageHeight = _Image.Height

        bmsource = New FastPixel(_Image)
        bmsource.Lock()

        Dim _count As Integer = 0
        Dim intPixLeft As Integer = 0
        Dim intPixTop As Integer = 0
        For y = 0 To bmsource.Height - _V1Size
            For x = 0 To bmsource.Width - _V1Size
                _count += 1
                Dim myrect As New Rectangle(x, y, _V1Size, _V1Size)
                ProcessPatch(myrect, x, y)

                'System.IO.File.WriteAllText("d:\vbtest\patchprocessed.txt", _count & ":" & _templates.Count)


            Next
        Next
errJumphere:
        MsgBox("All patches processed. Templates found:" & _templates.Count)
        'Now we output all our different templates

        Dim newbm As New Bitmap(500, 500)

        Dim i As Integer
        For i = 0 To _templates.Count - 1
            Dim _srcbmap As Bitmap = _templates.Item(i)
            Dim xdest As Integer = i Mod 12
            Dim ydest As Integer = i \ 12
            copybmap(_srcbmap, newbm, xdest * _V1Size, ydest & _V1Size)
        Next i
        newbm.Save("D:\vbtest\samples\template" & _CurrentResult & ".png", Imaging.ImageFormat.Png)
        MsgBox("All template files generated")
    End Sub

    Private Sub CopyBMap(ByRef SourceBM As Bitmap, ByRef TargetBM As Bitmap, xstart As Integer, ystart As Integer)
        Dim x As Integer
        Dim y As Integer

        For x = 0 To _V1Size - 1
            For y = 0 To _V1Size - 1
                Dim _C As Color = SourceBM.GetPixel(x, y)
                TargetBM.SetPixel(xstart + x, ystart + y, _C)
            Next y
        Next x

    End Sub

    Public Sub UnloadImage()
        bmsource.Unlock(True)
    End Sub

    Public Function IsEmptyPatch(ByRef Rect As Rectangle) As Boolean
        Dim x As Integer
        Dim y As Integer
        For x = Rect.Left To (Rect.Left + Rect.Width - 1)
            For y = Rect.Top To (Rect.Top + Rect.Height - 1)
                Dim c As Color = bmsource.GetPixel(x, y)
                If c.R <> 0 Or c.G <> 0 Or c.B <> 0 Then
                    Return False
                End If
            Next y
        Next x
        Return True
    End Function

    Public Sub ProcessPatch(ByRef Rect As Rectangle, xindex As Integer, yindex As Integer)
        Dim x As Integer = 0
        Dim y As Integer = 0

        If IsEmptyPatch(Rect) Then
            Exit Sub
        End If




        If _templates.Count > 0 Then

            Dim selectedtemplate As Bitmap = FindMatchingTemplate(Rect)

            'merge the template
            If selectedtemplate Is Nothing Then
                CreateNewTemplate(Rect, xindex, yindex)
            Else
                'MergeTemplates(selectedtemplate, Rect)
            End If
        Else
            CreateNewTemplate(Rect, xindex, yindex)

        End If



    End Sub

    Private Function FindMatchingTemplate(ByRef Rect As Rectangle) As Bitmap
        Dim j As Integer = 0
        Dim x As Integer = 0
        Dim y As Integer = 0


        Dim _smallestdiffpctg As Double = 101
        Dim _selectedbm As Bitmap = Nothing


        For j = 0 To _templates.Count - 1
            Dim templatebm As Bitmap = _templates.Item(j)
            'We compare against the bitmap in bm, we do pixel for pixel comparison

            Dim _totaldiff As Double = 0
                Dim _totaldiffpctg As Double = 0
                For x = Rect.Left To (Rect.Left + Rect.Width - 1)
                    For y = Rect.Top To (Rect.Top + Rect.Height - 1)
                        Dim c As Color = bmsource.GetPixel(x, y)
                        Dim luma As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)


                    Dim c2 As Color = templatebm.GetPixel(x - Rect.Left, y - Rect.Top)
                    Dim luma2 As Integer = CInt(c2.R * 0.3 + c2.G * 0.59 + c2.B * 0.11)

                    Dim diff As Integer = Math.Abs(luma2 - luma)
                        Dim diffpctg As Double = (CDbl(diff) / CDbl(256))
                        _totaldiff += diffpctg
                    Next y
                Next x

                _totaldiffpctg = ((_totaldiff) / CDbl(_V1Size * _V1Size)) * 100

            'System.IO.File.AppendAllText("d:\vbtest\siman.txt", "diffpctg:" & _totaldiffpctg & vbCrLf)


            If _totaldiffpctg < _smallestdiffpctg Then
                    _smallestdiffpctg = _totaldiffpctg
                    _selectedbm = templatebm
                End If


        Next j

        If _selectedbm Is Nothing Then
            Return Nothing
        Else
            'we check if the smallest diff is still exceeding our threshold
            If _smallestdiffpctg > 10 Then
                Return Nothing
            Else
                Return _selectedbm
            End If
        End If

    End Function

    Private Sub MergeTemplates(ByRef SelTemplate As Bitmap, ByRef Rect As Rectangle)
        Dim x As Integer
        Dim y As Integer
        For x = Rect.Left To (Rect.Left + Rect.Width - 1)
            For y = Rect.Top To (Rect.Top + Rect.Height - 1)
                Dim c As Color = bmsource.GetPixel(x, y)
                Dim luma As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)

                Dim d As System.Drawing.Color = SelTemplate.GetPixel(x - Rect.Left, y - Rect.Top)
                Dim sourceluma As Integer = CInt(d.R * 0.3 + d.G * 0.59 + d.B * 0.11)
                Dim _avg As Double = (CDbl(sourceluma) + CDbl(luma)) / 2
                Dim _finalluma As Integer = Math.Max(CInt(_avg), sourceluma)

                SelTemplate.SetPixel(x - Rect.Left, y - Rect.Top, Color.FromArgb(_finalluma, _finalluma, _finalluma))
            Next y
        Next x
    End Sub

    Private Sub CreateNewTemplate(ByRef Rect As Rectangle, XIndex As Integer, YIndex As Integer)
        'Else we add it in to templates 
        Dim x As Integer
        Dim y As Integer
        Dim _newimage As New Bitmap(_V1Size, _V1Size)
        Dim _save As Boolean = False
        For x = Rect.Left To (Rect.Left + Rect.Width - 1)
            For y = Rect.Top To (Rect.Top + Rect.Height - 1)
                Dim c As Color = bmsource.GetPixel(x, y)
                Dim luma As Integer = CInt(c.R * 0.3 + c.G * 0.59 + c.B * 0.11)
                Dim _col As System.Drawing.Color = Color.FromArgb(luma, luma, luma)

                _newimage.SetPixel(x - Rect.Left, y - Rect.Top, _col)
            Next y
        Next x
        '  _newimage.Save("D:\vbtest\samples\TEMPLATE" & XIndex & "-" & YIndex & ".png", Imaging.ImageFormat.Png)
        _templates.Add(_newimage)
    End Sub

End Class
