using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RebarPosCommands
{
    public class PosShapeView : Panel
    {
        private static string CadFontResource = "RebarPosCommands.1CamBam_Stick_2.ttf";

        private PosShape mShape;
        public PosShape Shape { get { return mShape; } set { SetShape(value); } }
        public bool ShowShapeName { get; set; }
        public bool Selected { get; set; }
        public Color SelectionColor { get; set; }

        private PointF minExtents;
        private PointF maxExtents;
        System.Drawing.Text.PrivateFontCollection fontCollection;

        public PosShapeView()
        {
            if (IsDesigner)
                this.BackColor = System.Drawing.SystemColors.Control;
            else
                this.BackColor = DWGUtility.ModelBackgroundColor();

            this.Name = "PosShapeView";
            this.Size = new System.Drawing.Size(300, 150);
            this.SelectionColor = SystemColors.Highlight;

            fontCollection = new System.Drawing.Text.PrivateFontCollection();
            using (System.IO.Stream fontStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(CadFontResource))
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                if (fontStream != null)
                {
                    int count = 0;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = fontStream.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    } while (fontStream.CanRead && count > 0);
                    byte[] buffer = ms.ToArray();

                    var handle = System.Runtime.InteropServices.GCHandle.Alloc(buffer, System.Runtime.InteropServices.GCHandleType.Pinned);

                    try
                    {
                        IntPtr ptr = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                        fontCollection.AddMemoryFont(ptr, buffer.Length);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
            }
        }

        protected bool IsDesigner
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv");
            }
        }

        private void SetShape(PosShape shape)
        {
            mShape = shape;
            if (mShape != null)
            {
                SetExtents(mShape.Bounds);
            }
            Refresh();
        }

        private void SetExtents(Autodesk.AutoCAD.DatabaseServices.Extents3d ext)
        {
            minExtents = new PointF((float)ext.MinPoint.X, (float)ext.MinPoint.Y);
            maxExtents = new PointF((float)ext.MaxPoint.X, (float)ext.MaxPoint.Y);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (Enabled)
                g.Clear(BackColor);
            else
                g.Clear(SystemColors.Control);

            if (mShape == null) return;

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Coordinate transformation
            if (Math.Abs(maxExtents.X - minExtents.X) > 0 && Math.Abs(maxExtents.Y - minExtents.Y) > 0)
            {
                g.ResetTransform();
                RectangleF rectWorld = new RectangleF(minExtents.X, maxExtents.Y, (maxExtents.X - minExtents.X), -(maxExtents.Y - minExtents.Y));

                float xScale = (float)ClientRectangle.Width / Math.Abs(maxExtents.X - minExtents.X);
                float yScale = (float)ClientRectangle.Height / Math.Abs(maxExtents.Y - minExtents.Y);
                float scale = Math.Min(xScale, yScale);
                float xOffset = ((float)ClientRectangle.Width - scale * Math.Abs(maxExtents.X - minExtents.X)) / 2;
                float yOffset = ((float)ClientRectangle.Height - scale * Math.Abs(maxExtents.Y - minExtents.Y)) / 2;

                PointF[] ptPixel = new PointF[3];
                ptPixel[0] = new PointF(xOffset, yOffset);// Origin
                ptPixel[1] = new PointF(xOffset + scale * Math.Abs(maxExtents.X - minExtents.X), yOffset); // X Axis
                ptPixel[2] = new PointF(xOffset, scale * Math.Abs(maxExtents.Y - minExtents.Y) + yOffset); // Y Axis
                g.Transform = new Matrix(rectWorld, ptPixel);
            }

            // Draw each sub shape
            foreach (RebarPosCommands.PosShape.Shape item in mShape.Items)
            {
                if (!item.Visible) continue;
                using (Pen pen = new Pen(item.Color.ColorValue))
                {
                    if (item is RebarPosCommands.PosShape.ShapeLine)
                    {
                        RebarPosCommands.PosShape.ShapeLine obj = (RebarPosCommands.PosShape.ShapeLine)item;
                        g.DrawLine(pen, (float)obj.X1, (float)obj.Y1, (float)obj.X2, (float)obj.Y2);
                    }
                    else if (item is RebarPosCommands.PosShape.ShapeArc)
                    {
                        RebarPosCommands.PosShape.ShapeArc obj = (RebarPosCommands.PosShape.ShapeArc)item;
                        g.DrawArc(pen, (float)(obj.X - obj.R), (float)(obj.Y - obj.R), (float)(2 * obj.R), (float)(2 * obj.R), (float)obj.StartAngle, (float)obj.EndAngle);
                    }
                    else if (item is RebarPosCommands.PosShape.ShapeCircle)
                    {
                        RebarPosCommands.PosShape.ShapeCircle obj = (RebarPosCommands.PosShape.ShapeCircle)item;
                        g.DrawEllipse(pen, (float)(obj.X - obj.R), (float)(obj.Y - obj.R), (float)(2 * obj.R), (float)(2 * obj.R));
                    }
                    else if (item is RebarPosCommands.PosShape.ShapeText)
                    {
                        RebarPosCommands.PosShape.ShapeText obj = (RebarPosCommands.PosShape.ShapeText)item;
                        StringAlignment hAlign = StringAlignment.Near;
                        if (obj.HorizontalAlignment == Autodesk.AutoCAD.DatabaseServices.TextHorizontalMode.TextCenter || obj.HorizontalAlignment == Autodesk.AutoCAD.DatabaseServices.TextHorizontalMode.TextMid) hAlign = StringAlignment.Center;
                        if (obj.HorizontalAlignment == Autodesk.AutoCAD.DatabaseServices.TextHorizontalMode.TextRight) hAlign = StringAlignment.Far;
                        StringAlignment vAlign = StringAlignment.Near;
                        if (obj.VerticalAlignment == Autodesk.AutoCAD.DatabaseServices.TextVerticalMode.TextVerticalMid) vAlign = StringAlignment.Center;
                        if (obj.VerticalAlignment == Autodesk.AutoCAD.DatabaseServices.TextVerticalMode.TextTop) vAlign = StringAlignment.Far;
                        DrawCadString(g, pen, obj.Text, new PointF((float)obj.X, (float)obj.Y), (float)obj.Height, hAlign, vAlign);
                    }
                }
            }

            g.ResetTransform();
            // Show shape name
            if (ShowShapeName)
            {
                using (Brush brush = new SolidBrush(IsDark(BackColor) ? Color.White : Color.Black))
                {
                    e.Graphics.DrawString(mShape.Name, Font, brush, 4, 6);
                }
            }
            // Show selection box
            if (Selected)
            {
                using (Pen pen = new Pen(SelectionColor, 2.0f))
                {
                    Rectangle rec = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
                    rec.Inflate(-2, -2);
                    e.Graphics.DrawRectangle(pen, rec);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Refresh();
            base.OnSizeChanged(e);
        }

        private void DrawCadString(Graphics g, Pen pen, string str, PointF pt, float textHeight, StringAlignment hAlign, StringAlignment vAlign)
        {
            // Convert the text alignment point to pixel coordinates
            PointF[] ptPixel = new PointF[] { pt, new PointF(pt.X, pt.Y + textHeight) };
            g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, ptPixel);
            float heightPixel = Math.Abs(ptPixel[1].Y - ptPixel[0].Y) * 1.5f;
            PointF ptBase = new PointF(ptPixel[0].X, ptPixel[0].Y - heightPixel);

            // Revert transformation to identity while drawing text
            Matrix oldMatrix = g.Transform;
            g.ResetTransform();

            // Draw in pixel coordinates
            using (Font cadFont = new Font((fontCollection.Families.Length == 0 ? Font.FontFamily : fontCollection.Families[0]), heightPixel, GraphicsUnit.Pixel))
            using (StringFormat stringFormat = new StringFormat())
            using (Brush brush = new SolidBrush(pen.Color))
            {
                stringFormat.Alignment = hAlign;
                stringFormat.LineAlignment = vAlign;

                g.DrawString(str, cadFont, brush, ptBase, stringFormat);
            }

            // Restore old transformation
            g.Transform = oldMatrix;
        }

        private bool IsDark(Color c)
        {
            return Math.Sqrt(c.R * c.R * .241 + c.G * c.G * .691 + c.B * c.B * .068) < 130;
        }
    }
}
