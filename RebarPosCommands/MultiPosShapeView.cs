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

        private DoubleBufferedFlowLayoutPanel layoutPanel;

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
                mCellBackColor = AcadUtility.AcadGraphics.ModelBackgroundColor();
            mSelectedShape = string.Empty;
            mCellSize = new Size(300, 150);
            mSelectionColor = SystemColors.Highlight;
            mShowShapeNames = true;

            this.Name = "MultiPosShapeView";
            this.Size = new System.Drawing.Size(900, 450);
            this.SuspendLayout();

            this.layoutPanel = new DoubleBufferedFlowLayoutPanel();
            this.layoutPanel.Dock = DockStyle.Fill;
            this.layoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.layoutPanel.AutoScroll = true;
            this.Controls.Add(layoutPanel);

            this.ResumeLayout(false);

            mSelectedShape = string.Empty;
        }

        public void SetShapes(IEnumerable<string> shapes)
        {
            this.layoutPanel.Suspend();

            this.layoutPanel.Controls.Clear();

            foreach (string shape in shapes)
            {
                PosShapeView cell = new PosShapeView();
                cell.Size = mCellSize;
                cell.Shape = PosShape.Shapes[shape];
                cell.Click += new EventHandler(cell_Click);
                this.layoutPanel.Controls.Add(cell);
            }

            this.layoutPanel.Resume();
        }

        public void SetPieceLengths(int index, string a, string b, string c, string d, string e, string f)
        {
            if (index > 0 && index < layoutPanel.Controls.Count)
            {
                PosShapeView cell = layoutPanel.Controls[index] as PosShapeView;
                if (cell != null)
                {
                    PosShape shape = cell.Shape;
                    shape.SetShapeTexts(a, b, c, d, e, f);
                }
            }
        }

        public void SetPieceLengths(string a, string b, string c, string d, string e, string f)
        {
            for (int i = 0; i < layoutPanel.Controls.Count; i++)
            {
                PosShapeView cell = layoutPanel.Controls[i] as PosShapeView;
                if (cell != null)
                {
                    PosShape shape = cell.Shape;
                    shape.SetShapeTexts(a, b, c, d, e, f);
                }
            }
        }

        public void ClearPieceLengths(int index)
        {
            if (index > 0 && index < layoutPanel.Controls.Count)
            {
                PosShapeView cell = layoutPanel.Controls[index] as PosShapeView;
                if (cell != null)
                {
                    PosShape shape = cell.Shape;
                    shape.ClearShapeTexts();
                }
            }
        }

        public void ClearPieceLengths()
        {
            for (int i = 0; i < layoutPanel.Controls.Count; i++)
            {
                PosShapeView cell = layoutPanel.Controls[i] as PosShapeView;
                if (cell != null)
                {
                    PosShape shape = cell.Shape;
                    shape.ClearShapeTexts();
                }
            }
        }

        public void UpdateCells()
        {
            this.layoutPanel.Suspend();

            foreach (Control item in layoutPanel.Controls)
            {
                PosShapeView cell = (PosShapeView)item;
                cell.ShowShapeName = ShowShapeNames;
                cell.SelectionColor = SelectionColor;
                cell.Selected = (cell.Shape.Name == mSelectedShape);
                cell.Size = mCellSize;
                cell.Refresh();
            }

            this.layoutPanel.Resume();
        }

        void cell_Click(object sender, EventArgs e)
        {
            SelectedShape = ((PosShapeView)sender).Shape.Name;

            if (ShapeClick != null)
                ShapeClick(this, new MultiPosShapeViewClickEventArgs(mSelectedShape));
        }
    }
}
