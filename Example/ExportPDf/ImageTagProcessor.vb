Imports iTextSharp.text
Imports iTextSharp.tool.xml
Imports iTextSharp.tool.xml.html
Imports iTextSharp.tool.xml.pipeline.html
Imports Image = iTextSharp.tool.xml.html.Image

Public Class ImageTagProcessor
    Inherits Image

    Public Overrides Function [End](ctx As IWorkerContext, tag As Tag, currentContent As IList(Of IElement)) As IList(Of IElement)
        Dim elements As IList(Of IElement) = MyBase.End(ctx, tag, currentContent)
        Dim attributeMap As IDictionary = tag.Attributes
        Dim src As String = attributeMap(HTML.Attribute.SRC)

        If Not String.IsNullOrEmpty(src) Then
            Dim img As iTextSharp.text.Image
            If src.StartsWith("data:image/") Then
                Dim base64Data As String = src.Split(",")(1)
                img = iTextSharp.text.Image.GetInstance(Convert.FromBase64String(base64Data))

                If img IsNot Nothing Then
                    Dim htmlPipelineContext As HtmlPipelineContext = GetHtmlPipelineContext(ctx)
                    elements.Add(GetCssAppliers().Apply(New Chunk(GetCssAppliers().Apply(img, tag, htmlPipelineContext), 0, 0, True), tag, htmlPipelineContext))
                End If
            End If
        End If

        Return elements
    End Function

End Class
