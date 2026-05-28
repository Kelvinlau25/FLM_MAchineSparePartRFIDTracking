Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class PageNumberFooter
    Inherits PdfPageEventHelper

    Private font As Font
    Private horizontalAllignment As Integer

    Public Sub New(ByVal font As Font, ByVal horizontalAllignment As Integer)
        Me.font = font
        Me.horizontalAllignment = horizontalAllignment
    End Sub

    Public Overrides Sub OnEndPage(ByVal writer As PdfWriter, ByVal document As Document)
        GeneratePageNumber(writer, document)
    End Sub

    Private Sub GeneratePageNumber(ByVal writer As PdfWriter, ByVal document As Document)
        Dim currPageNum As String = String.Format("Page {0}", writer.PageNumber.ToString)
        Dim footerTable As New PdfPTable(1)
        Dim pageNumCell As New PdfPCell(New Phrase(currPageNum, font))
        pageNumCell.HorizontalAlignment = horizontalAllignment
        pageNumCell.BorderWidth = 0

        With footerTable
            .AddCell(pageNumCell)
            .TotalWidth = 50
            .WriteSelectedRows(0, -1, document.Right - 80, document.Bottom + 10, writer.DirectContent)
        End With
    End Sub

End Class
