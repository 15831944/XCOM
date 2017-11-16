using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XCOM.Commands.RoadDesign
{
    public partial class DrawCulvertForm : XCOM.Utility.VersionDisplayForm
    {
        private bool basePointSelected;
        private Point3d basePt;

        public Point3d BasePoint { get { return basePt; } set { basePointSelected = true; basePt = value; txtX.Text = basePt.X.ToString(); txtY.Text = basePt.Y.ToString(); txtZ.Text = basePt.Z.ToString(); } }

        public double BaseChainage { get { double v = 0; AcadUtility.AcadText.TryChainageFromString(txtBaseCH.Text, out v); return v; } set { txtBaseCH.Text = AcadUtility.AcadText.ChainageToString(value); } }
        public double BaseLevel { get { double v = 0; double.TryParse(txtBaseLevel.Text, out v); return v; } set { txtBaseLevel.Text = value.ToString(); } }
        public double ProfileScale { get { double v = 0; double.TryParse(txtScale.Text, out v); return v; } set { txtScale.Text = value.ToString(); } }

        public string LayerName { get { return txtLayer.Text; } set { txtLayer.Text = value; } }
        public double TextHeight { get { double v = 0; double.TryParse(txtTextHeight.Text, out v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public double HatchScale { get { double v = 0; double.TryParse(txtHatchScale.Text, out v); return v; } set { txtHatchScale.Text = value.ToString(); } }

        public bool DrawCulvertInfo { get { return cbDrawCulvertInfo.Checked; } }

        public List<CulvertInfo> GetData()
        {
            List<CulvertInfo> items = new List<CulvertInfo>();

            for (int i = culvertGrid.FixedRows; i < culvertGrid.RowsCount; i++)
            {
                bool emptyRow = true;
                for (int j = 0; j < culvertGrid.ColumnsCount; j++)
                {
                    if (culvertGrid[i, j].Value != null && !string.IsNullOrEmpty((string)culvertGrid[i, j].Value))
                    {
                        emptyRow = false;
                        break;
                    }
                }
                if (emptyRow) continue;

                bool emptyCell = false;
                for (int j = 0; j < culvertGrid.ColumnsCount; j++)
                {
                    double val;
                    if (j == 0)
                    {
                        if (!AcadUtility.AcadText.TryChainageFromString((string)culvertGrid[i, j].Value, out val))
                        {
                            emptyCell = true;
                            break;
                        }
                    }
                    else if (j == 1 || j == 5 || j == 6)
                    {
                        if (!double.TryParse((string)culvertGrid[i, j].Value, out val))
                        {
                            emptyCell = true;
                            break;
                        }
                    }
                }
                if (emptyCell)
                {
                    MessageBox.Show((i - culvertGrid.FixedRows + 1).ToString() + " nolu satırda KM, kot veya boyut bilgisi okunamadı.", "Menfez Çizimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                double ch = 0;
                double level = 0;
                double length = 0;
                double grade = 0;
                double skew = 0;
                double width = 0;
                double height = 0;
                double welllength = 0;
                AcadUtility.AcadText.TryChainageFromString((string)culvertGrid[i, 0].Value, out ch);
                double.TryParse((string)culvertGrid[i, 1].Value, out level);
                double.TryParse((string)culvertGrid[i, 2].Value, out length);
                double.TryParse((string)culvertGrid[i, 3].Value, out grade);
                double.TryParse((string)culvertGrid[i, 4].Value, out skew);
                double.TryParse((string)culvertGrid[i, 5].Value, out width);
                double.TryParse((string)culvertGrid[i, 6].Value, out height);
                double.TryParse((string)culvertGrid[i, 7].Value, out welllength);

                CulvertInfo culvert = new CulvertInfo();
                culvert.Chainage = ch;
                culvert.Level = level;
                culvert.Length = length;
                culvert.Grade = grade;
                culvert.Skew = skew;
                culvert.Width = width;
                culvert.Height = height;
                culvert.WellLength = welllength;
                items.Add(culvert);
            }

            items.Sort((p1, p2) => p1.Chainage.CompareTo(p2.Chainage));

            return items;
        }

        public DrawCulvertForm()
        {
            InitializeComponent();

            basePointSelected = false;
            basePt = Point3d.Origin;

            SourceGrid.Cells.Views.Cell cellView = new SourceGrid.Cells.Views.Cell();
            cellView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;
            cellView.BackColor = Color.White;

            SourceGrid.Cells.Views.Cell errorView = new SourceGrid.Cells.Views.Cell();
            errorView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;
            errorView.BackColor = Color.LightCoral;

            ChainageCellChangedEvent chainageEventController = new ChainageCellChangedEvent(cellView, errorView);
            DoubleCellChangedEvent doubleEventController = new DoubleCellChangedEvent(cellView, errorView);

            culvertGrid.BorderStyle = BorderStyle.FixedSingle;
            culvertGrid.EnableSort = false;

            culvertGrid.ColumnsCount = 8;
            culvertGrid.RowsCount = 1024;

            culvertGrid.FixedRows = 1;

            culvertGrid[0, 0] = new SourceGrid.Cells.ColumnHeader("KM");
            culvertGrid[0, 1] = new SourceGrid.Cells.ColumnHeader("Kot (m)");
            culvertGrid[0, 2] = new SourceGrid.Cells.ColumnHeader("Boyu (m)");
            culvertGrid[0, 3] = new SourceGrid.Cells.ColumnHeader("Eğim (%)");
            culvertGrid[0, 4] = new SourceGrid.Cells.ColumnHeader("Verevlilik");
            culvertGrid[0, 5] = new SourceGrid.Cells.ColumnHeader("Genişlik (m)");
            culvertGrid[0, 6] = new SourceGrid.Cells.ColumnHeader("Yükseklik (m)");
            culvertGrid[0, 7] = new SourceGrid.Cells.ColumnHeader("Kuyu Boyu (m)");

            culvertGrid.Columns[0].Tag = "CH";
            culvertGrid.Columns[1].Tag = "Level";
            culvertGrid.Columns[2].Tag = "Length";
            culvertGrid.Columns[3].Tag = "Grade";
            culvertGrid.Columns[4].Tag = "Skew";
            culvertGrid.Columns[5].Tag = "Width";
            culvertGrid.Columns[6].Tag = "Height";
            culvertGrid.Columns[7].Tag = "WellLength";

            for (int j = 0; j < culvertGrid.ColumnsCount; j++)
            {
                culvertGrid.Columns[j].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            }

            for (int i = 0; i < culvertGrid.RowsCount; i++)
            {
                culvertGrid.Rows[i].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            }

            for (int i = culvertGrid.FixedRows; i < culvertGrid.RowsCount; i++)
            {
                for (int j = 0; j < culvertGrid.ColumnsCount; j++)
                {
                    culvertGrid[i, j] = new SourceGrid.Cells.Cell("", typeof(string));
                    culvertGrid[i, j].View = cellView;
                    if (j == 0)
                        culvertGrid[i, j].AddController(chainageEventController);
                    else
                        culvertGrid[i, j].AddController(doubleEventController);
                }
            }

            culvertGrid.AutoSizeCells();

            int maxWidth = 50;
            for (int j = 0; j < culvertGrid.ColumnsCount; j++)
            {
                maxWidth = Math.Max(maxWidth, culvertGrid.Columns[j].Width);
            }
            for (int j = 0; j < culvertGrid.ColumnsCount; j++)
            {
                culvertGrid.Columns[j].MinimalWidth = maxWidth;
            }

            culvertGrid.AutoSizeCells();
        }

        private void btnPickBasePoint_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                PromptPointResult ptRes = ed.GetPoint("\nBaz noktası: ");
                if (ptRes.Status == PromptStatus.OK) BasePoint = ptRes.Value;
            }
        }

        public class ChainageCellChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            SourceGrid.Cells.Views.Cell cellView;
            SourceGrid.Cells.Views.Cell errorView;

            public ChainageCellChangedEvent(SourceGrid.Cells.Views.Cell sourceCellView, SourceGrid.Cells.Views.Cell sourceErrorView)
                : base()
            {
                cellView = sourceCellView;
                errorView = sourceErrorView;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                object val = sender.Value;
                double res = 0;
                if (val == null || string.IsNullOrEmpty((string)val))
                {
                    sender.Cell.View = cellView;
                }
                else if (!AcadUtility.AcadText.TryChainageFromString((string)val, out res))
                {
                    sender.Cell.View = errorView;
                }
                else
                {
                    sender.Value = AcadUtility.AcadText.ChainageToString(res);
                    sender.Cell.View = cellView;
                }
            }
        }

        public class DoubleCellChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            SourceGrid.Cells.Views.Cell cellView;
            SourceGrid.Cells.Views.Cell errorView;

            public DoubleCellChangedEvent(SourceGrid.Cells.Views.Cell sourceCellView, SourceGrid.Cells.Views.Cell sourceErrorView)
                : base()
            {
                cellView = sourceCellView;
                errorView = sourceErrorView;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                object val = sender.Value;
                double res = 0;
                if (val == null || string.IsNullOrEmpty((string)val))
                {
                    sender.Cell.View = cellView;
                }
                else if (!double.TryParse((string)val, out res))
                {
                    sender.Cell.View = errorView;
                }
                else
                {
                    sender.Cell.View = cellView;
                }
            }
        }

        public class CulvertInfo
        {
            public double Chainage { get; set; }
            public double Level { get; set; }
            public double Length { get; set; }
            public double Grade { get; set; }
            public double Skew { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Wall { get { return 0.3; } }
            public double TopSlab { get { return 0.4; } }
            public double BottomSlab { get { return 0.4; } }
            public double WellLength { get; set; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!basePointSelected)
            {
                MessageBox.Show("Baz noktasını seçin.", "Menfez Çizimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void txtBaseCH_Validating(object sender, CancelEventArgs e)
        {
            double val = 0;
            if (AcadUtility.AcadText.TryChainageFromString(txtBaseCH.Text, out val))
            {
                txtBaseCH.Text = AcadUtility.AcadText.ChainageToString(val);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void txtBaseLevel_Validating(object sender, CancelEventArgs e)
        {
            double val = 0;
            if (!double.TryParse(txtBaseLevel.Text, out val))
            {
                e.Cancel = true;
            }
        }

        private void txtScale_Validating(object sender, CancelEventArgs e)
        {
            double val = 0;
            if (!double.TryParse(txtScale.Text, out val))
            {
                e.Cancel = true;
            }
        }

        private void txtTextHeight_Validating(object sender, CancelEventArgs e)
        {
            double val = 0;
            if (!double.TryParse(txtTextHeight.Text, out val) || val < 0.00001)
            {
                e.Cancel = true;
            }
        }

        private void txtHatchScale_Validating(object sender, CancelEventArgs e)
        {
            double val = 0;
            if (!double.TryParse(txtHatchScale.Text, out val) || val < 0.00001)
            {
                e.Cancel = true;
            }
        }

        private void txtLayer_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLayer.Text))
            {
                e.Cancel = true;
            }
        }
    }
}
