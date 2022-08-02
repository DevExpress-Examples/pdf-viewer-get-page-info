Imports DevExpress.Pdf
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Ribbon
Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace pdf_viewer_sticky_note

    Public Partial Class Form1
        Inherits RibbonForm

        Public Sub New()
            InitializeComponent()
            pdfViewer.LoadDocument("Demo.pdf")
        End Sub

        Private Shared Function CreateTransform(ByVal degree As Integer, ByVal cropBox As PdfRectangle) As Matrix
            Dim rotationMatrix As Matrix
            Select Case degree Mod 360
                Case 90
                    rotationMatrix = New Matrix(0, -1, 1, 0, 0, CSng(cropBox.Width))
                Case 180
                    rotationMatrix = New Matrix(-1, 0, 0, -1, CSng(cropBox.Width), CSng(cropBox.Height))
                Case 270
                    rotationMatrix = New Matrix(0, 1, -1, 0, CSng(cropBox.Height), 0)
                Case Else
                    rotationMatrix = New Matrix(1, 0, 0, 1, 0, 0)
            End Select

            Dim matrix = New Matrix(1, 0, 0, 1, CSng(-cropBox.Left), CSng(-cropBox.Bottom))
            Using rotationMatrix
                matrix.Multiply(rotationMatrix, MatrixOrder.Append)
            End Using

            matrix.Invert()
            Return matrix
        End Function

        Private Shared Function NormalizeRotate(ByVal rotate As Integer) As Integer
            Dim [step] As Integer = -360
            If rotate < 0 Then [step] = 360
            While rotate < 0 AndAlso [step] > 0 OrElse rotate > 270 AndAlso [step] < 0
                rotate += [step]
            End While

            Return rotate
        End Function

        Private Sub AddStickyNote()
            Dim pageInfo = pdfViewer.GetPageInfo(1)
            Dim normalizedAngle As Integer = NormalizeRotate(pdfViewer.RotationAngle + pageInfo.RotationAngle)
            Dim cropBox = pageInfo.CropBox
            Dim viewCropBox As PdfRectangle
            If normalizedAngle = 90 OrElse normalizedAngle = 270 Then
                viewCropBox = New PdfRectangle(0, 0, cropBox.Height, cropBox.Width)
            Else
                viewCropBox = New PdfRectangle(0, 0, cropBox.Width, cropBox.Height)
            End If

            Dim stickyNotePoint As PdfPoint = New PdfPoint(viewCropBox.Right - 20, viewCropBox.Top - 20)
            Dim matrix = CreateTransform(normalizedAngle, cropBox)
            Dim points = {New PointF(CSng(stickyNotePoint.X), CSng(stickyNotePoint.Y))}
            matrix.TransformPoints(points)
            Dim stickyNotePosition As PdfDocumentPosition = New PdfDocumentPosition(1, New PdfPoint(points(0).X, points(0).Y))
            pdfViewer.AddStickyNote(stickyNotePosition, "Note", Color.Yellow)
        End Sub

        Private Sub barButtonItem1_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
            AddStickyNote()
        End Sub
    End Class
End Namespace
