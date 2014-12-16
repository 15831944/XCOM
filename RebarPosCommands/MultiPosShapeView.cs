using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Autodesk.AutoCAD.GraphicsSystem;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace RebarPosCommands
{
    public delegate void MultiPosShapeViewClickEventHandler(object sender, MultiPosShapeViewClickEventArgs e);

    public class MultiPosShapeViewClickEventArgs
    {
        public string Shape { get; private set; }

        public MultiPosShapeViewClickEventArgs(string shape)
        {
            Shape = shape;
        }
    }

    public class MultiPosShapeView : Panel
    {
        public event MultiPosShapeViewClickEventHandler ShapeClick;

        private FlowLayoutPanel layoutPanel;

        private string mSelectedShape;
        public string SelectedShape { get { return mSelectedShape; } set { mSelectedShape = value; UpdateCells(); } }

        private Size mCellSize;
        public Size CellSize { get { return mCellSize; } set { mCellSize = value; UpdateCells(); } }

        private Color mCellBackColor;
        public Color CellBackColor { get { return mCellBackColor; } set { mCellBackColor = value; UpdateCells(); } }

        private bool mShowShapeNames;
        public bool ShowShapeNames { get { return mShowShapeNames; } set { mShowShapeNames = value; UpdateCells(); } }

        private Color mSelectionColor;
        public Color SelectionColor { get { return mSelectionColor; } set { mSelectionColor = value; UpdateCells(); } }

        private Dictionary<int, List<string>> pieceLengths;

        protected bool IsDesigner
        {
            get
            {
                return (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv");
            }
        }

        public MultiPosShapeView()
        {
            if (IsDesigner)
                mCellBackColor = System.Drawing.SystemColors.Control;
            else
                mCellBackColor = DWGUtility.ModelBackgroundColor();
            mSelectedShape = string.Empty;
            mCellSize = new Size(300, 150);
            mSelectionColor = SystemColors.Highlight;
            mShowShapeNames = true;

            this.Name = "MultiPosShapeView";
            this.Size = new System.Drawing.Size(900, 450);
            this.SuspendLayout();

            this.layoutPanel = new FlowLayoutPanel();
            this.layoutPanel.Dock = DockStyle.Fill;
            this.layoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.layoutPanel.AutoScroll = true;
            this.Controls.Add(layoutPanel);

            this.ResumeLayout(false);

            mSelectedShape = string.Empty;

            pieceLengths = new Dictionary<int, List<string>>();
        }

        public void SetShapes(IEnumerable<string> shapes)
        {
            this.layoutPanel.SuspendLayout();

            this.layoutPanel.Controls.Clear();

            foreach (string shape in shapes)
            {
                PosShapeView cell = new PosShapeView();
                cell.Size = mCellSize;
                cell.Shape = PosShape.Shapes[shape];
                cell.Click += new EventHandler(cell_Click);
                cell.Paint += new PaintEventHandler(cell_Paint);
                this.layoutPanel.Controls.Add(cell);
            }

            this.layoutPanel.ResumeLayout();
        }

        public void SetPieceLengths(int index, string a, string b, string c, string d, string e, string f)
        {
            List<string> lengths = new List<string>() { a, b, c, d, e, f };
            if (pieceLengths.ContainsKey(index))
                pieceLengths[index] = lengths;
            else
                pieceLengths.Add(index, lengths);
        }

        public void UpdateCells()
        {
            this.layoutPanel.SuspendLayout();
            foreach (Control item in layoutPanel.Controls)
            {
                PosShapeView cell = (PosShapeView)item;
                cell.ShowShapeName = ShowShapeNames;
                cell.SelectionColor = SelectionColor;
                cell.Selected = (cell.Shape.Name == mSelectedShape);
                cell.Size = mCellSize;
                cell.Refresh();
            }
            this.layoutPanel.ResumeLayout();

            Refresh();
        }

        void cell_Click(object sender, EventArgs e)
        {
            SelectedShape = ((PosShapeView)sender).Shape.Name;

            if (ShapeClick != null)
                ShapeClick(this, new MultiPosShapeViewClickEventArgs(mSelectedShape));
        }

        void cell_Paint(object sender, PaintEventArgs e)
        {
            if (!Disposing && !IsDesigner)
            {
                PosShapeView cell = (PosShapeView)sender;
                int index = layoutPanel.Controls.IndexOf(cell);
                PosShape shape = cell.Shape;
                List<string> lengths = new List<string>();
                if (pieceLengths.TryGetValue(index, out lengths) && (lengths.Count == 6))
                {
                    shape.SetShapeTexts(lengths[0], lengths[1], lengths[2], lengths[3], lengths[4], lengths[5]);
                }
                shape.ClearShapeTexts();
            }
        }
    }
}
