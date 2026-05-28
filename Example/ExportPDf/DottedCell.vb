Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class DottedCell
    Implements IPdfPCellEvent

    Private ReadOnly _border As Integer = 0

    Public Sub New(ByVal border As Integer)
        _border = border
    End Sub

    Public Sub CellLayout(cell As PdfPCell, position As Rectangle, canvases() As PdfContentByte) Implements IPdfPCellEvent.CellLayout
        Dim canvas As PdfContentByte = canvases(PdfPTable.LINECANVAS)
        canvas.SaveState()
        canvas.SetLineDash(1)
        canvas.SetLineDash(0, 3, 2)

        cell.Border = Rectangle.NO_BORDER

        If _border = Rectangle.TOP_BORDER Then
            canvas.MoveTo(position.GetRight(1), position.GetTop(1))
            canvas.LineTo(position.GetLeft(1), position.GetTop(1))
        End If

        If _border = Rectangle.BOTTOM_BORDER Then
            canvas.MoveTo(position.GetRight(1), position.GetBottom(1))
            canvas.LineTo(position.GetLeft(1), position.GetBottom(1))
        End If

        If _border = Rectangle.RIGHT_BORDER Then
            canvas.MoveTo(position.GetRight(1), position.GetTop(1))
            canvas.LineTo(position.GetRight(1), position.GetBottom(1))
        End If

        If _border = Rectangle.LEFT_BORDER Then
            canvas.MoveTo(position.GetLeft(1), position.GetTop(1))
            canvas.LineTo(position.GetLeft(1), position.GetBottom(1))
        End If

        canvas.Stroke()
        canvas.RestoreState()
    End Sub
End Class
