Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports iTextSharp.tool.xml.html.table

Public Class TableDataProcessor
    Inherits TableData

    Private Function HasBorderStyle(ByVal attributeMap As IDictionary, ByVal borderPosition As String, ByVal borderStyle As String) As Boolean
        If Not attributeMap.Contains("style") Then
            Return False
        End If

        Dim val As String = attributeMap("style").ToString.Split(";").FirstOrDefault(Function(o) o.Trim().StartsWith("border-style-" & borderPosition & ":"))
        If Not IsNothing(val) Then
            Return val.Split(":").Any(Function(o) o.Trim.ToLower = borderStyle)
        End If

        Return False
    End Function

    Private Function HasCustomClassBorderStyle(ByVal attributeMap As IDictionary, ByVal borderPosition As String, ByVal borderStyle As String) As Boolean
        If Not attributeMap.Contains("class") Then
            Return False
        End If

        Dim val As String = attributeMap("class").ToString.Split(" ").FirstOrDefault(Function(o) o.Trim().StartsWith("custom-border-" & borderPosition & "-" & borderStyle))
        If Not IsNothing(val) Then
            Return val.Split("-").Any(Function(o) o.Trim.ToLower = borderStyle)
        End If

        Return False
    End Function

    Public Overrides Function [End](ctx As IWorkerContext, tag As Tag, currentContent As IList(Of IElement)) As IList(Of IElement)
        Dim cells As IList(Of IElement) = MyBase.End(ctx, tag, currentContent)
        Dim attributeMap As IDictionary = tag.Attributes
        Dim pdfCell As PdfPCell = cells(0)

        If HasBorderStyle(attributeMap, "left", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.LEFT_BORDER)
        End If
        If HasBorderStyle(attributeMap, "left", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.LEFT_BORDER)
        End If
        If HasBorderStyle(attributeMap, "right", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.RIGHT_BORDER)
        End If
        If HasBorderStyle(attributeMap, "right", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.RIGHT_BORDER)
        End If
        If HasBorderStyle(attributeMap, "top", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.TOP_BORDER)
        End If
        If HasBorderStyle(attributeMap, "top", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.TOP_BORDER)
        End If
        If HasBorderStyle(attributeMap, "bottom", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.BOTTOM_BORDER)
        End If
        If HasBorderStyle(attributeMap, "bottom", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.BOTTOM_BORDER)
        End If

        If HasCustomClassBorderStyle(attributeMap, "left", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.LEFT_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "left", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.LEFT_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "right", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.RIGHT_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "right", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.RIGHT_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "top", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.TOP_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "top", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.TOP_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "bottom", "dotted") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DottedCell(Rectangle.BOTTOM_BORDER)
        End If
        If HasCustomClassBorderStyle(attributeMap, "bottom", "dashed") Then
            'pdfCell.CellEvent = Nothing
            pdfCell.CellEvent = New DashedCell(Rectangle.BOTTOM_BORDER)
        End If

        Return cells
    End Function
End Class
