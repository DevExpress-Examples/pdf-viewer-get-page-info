using DevExpress.Pdf;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;


namespace pdf_viewer_sticky_note
{
    public partial class Form1 : RibbonForm
    {
        public Form1()
        {
            InitializeComponent();
            pdfViewer.LoadDocument("Demo.pdf");
        }
        static Matrix CreateTransform(int degree, PdfRectangle cropBox)
        {
            Matrix rotationMatrix;
            switch (degree % 360)
            {
                case 90:
                    rotationMatrix = new Matrix(0, -1, 1, 0, 0, (float)cropBox.Width);
                    break;
                case 180:
                    rotationMatrix = new Matrix(-1, 0, 0, -1, (float)cropBox.Width, (float)cropBox.Height);
                    break;
                case 270:
                    rotationMatrix = new Matrix(0, 1, -1, 0, (float)cropBox.Height, 0);
                    break;
                default:
                    rotationMatrix = new Matrix(1, 0, 0, 1, 0, 0);
                    break;
            }
            var matrix = new Matrix(1, 0, 0, 1, (float)-cropBox.Left, (float)-cropBox.Bottom);
            using (rotationMatrix)
                matrix.Multiply(rotationMatrix, MatrixOrder.Append);
            matrix.Invert();
            return matrix;
        }
        static int NormalizeRotate(int rotate)
        {
            int step = -360;
            if (rotate < 0)
                step = 360;
            while ((rotate < 0 && step > 0) || (rotate > 270 && step < 0))
                rotate += step;
            return rotate;
        }
        void AddStickyNote()
        {
            var pageInfo = pdfViewer.GetPageInfo(1);
            int normalizedAngle = NormalizeRotate(pdfViewer.RotationAngle + pageInfo.RotationAngle);
            var cropBox = pageInfo.CropBox;
            PdfRectangle viewCropBox;
            if (normalizedAngle == 90 || normalizedAngle == 270)
                viewCropBox = new PdfRectangle(0, 0, cropBox.Height, cropBox.Width);
            else
                viewCropBox = new PdfRectangle(0, 0, cropBox.Width, cropBox.Height);
            PdfPoint stickyNotePoint = new PdfPoint(viewCropBox.Right - 20, viewCropBox.Top - 20);
            var matrix = CreateTransform(normalizedAngle, cropBox);
            var points = new[] { new PointF((float)stickyNotePoint.X, (float)stickyNotePoint.Y) };
            matrix.TransformPoints(points);
            PdfDocumentPosition stickyNotePosition = new PdfDocumentPosition(1, new PdfPoint(points[0].X, points[0].Y));
            pdfViewer.AddStickyNote(stickyNotePosition, "Note", Color.Yellow);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddStickyNote();
        }
    }
}