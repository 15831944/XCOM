using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Topography
{
    public partial class DrawCulvertForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public Point3d BasePoint { get { return pickBasePoint.PickPoint; } set { pickBasePoint.PickPoint = value; } }

        public double BaseChainage { get { AcadUtility.AcadText.TryChainageFromString(txtBaseCH.Text, out double v); return v; } set { txtBaseCH.Text = AcadUtility.AcadText.ChainageToString(value); } }
        public double BaseLevel { get { double.TryParse(txtBaseLevel.Text, out double v); return v; } set { txtBaseLevel.Text = value.ToString(); } }
        public double ProfileScale { get { double.TryParse(txtScale.Text, out double v); return v; } set { txtScale.Text = value.ToString(); } }
        public bool DrawCulvertInfo { get { return cbDrawCulvertInfo.Checked; } set { cbDrawCulvertInfo.Checked = value; } }

        public double TextHeight { get { double.TryParse(txtTextHeight.Text, out double v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public double HatchScale { get { double.TryParse(txtHatchScale.Text, out double v); return v; } set { txtHatchScale.Text = value.ToString(); } }

        public string LayerName
        {
            get => cbLayer.Text;
            set => cbLayer.Text = value;
        }

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
                AcadUtility.AcadText.TryChainageFromString((string)culvertGrid[i, 0].Value, out double ch);
                double.TryParse((string)culvertGrid[i, 1].Value, out double level);
                double.TryParse((string)culvertGrid[i, 2].Value, out double length);
                double.TryParse((string)culvertGrid[i, 3].Value, out double grade);
                double.TryParse((string)culvertGrid[i, 4].Value, out double skew);
                double.TryParse((string)culvertGrid[i, 5].Value, out double width);
                double.TryParse((string)culvertGrid[i, 6].Value, out double height);
                double.TryParse((string)culvertGrid[i, 7].Value, out double welllength);

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

        public class ChainageCellChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private readonly SourceGrid.Cells.Views.Cell cellView;
            private readonly SourceGrid.Cells.Views.Cell errorView;

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
            private readonly SourceGrid.Cells.Views.Cell cellView;
            private readonly SourceGrid.Cells.Views.Cell errorView;

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
            if (!pickBasePoint.IsPointSet)
            {
                MessageBox.Show("Baz noktasını seçin.", "Menfez Çizimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void txtBaseCH_Validating(object sender, CancelEventArgs e)
        {
            if (AcadUtility.AcadText.TryChainageFromString(txtBaseCH.Text, out double val))
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
            if (!double.TryParse(txtBaseLevel.Text, out double val))
            {
                e.Cancel = true;
            }
        }

        private void txtScale_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtScale.Text, out double val))
            {
                e.Cancel = true;
            }
        }

        private void txtTextHeight_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtTextHeight.Text, out double val) || val < 0.00001)
            {
                e.Cancel = true;
            }
        }

        private void txtHatchScale_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtHatchScale.Text, out double val) || val < 0.00001)
            {
                e.Cancel = true;
            }
        }
    }
}
