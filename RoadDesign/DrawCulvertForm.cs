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

namespace RoadDesign
{
    public partial class DrawCulvertForm : Form
    {
        private Point3d basePt;

        public Point3d BasePoint { get { return basePt; } set { basePt = value; txtX.Text = basePt.X.ToString(); txtY.Text = basePt.Y.ToString(); txtZ.Text = basePt.Z.ToString(); } }

        public double BaseChainage { get { double v = 0; CulvertInfo.TryChainageFromString(txtBaseCH.Text, out v); return v; } set { txtBaseCH.Text = CulvertInfo.ChainageToString(value); } }
        public double BaseLevel { get { double v = 0; double.TryParse(txtBaseLevel.Text, out v); return v; } set { txtBaseLevel.Text = value.ToString(); } }
        public double ProfileScale { get { double v = 0; double.TryParse(txtScale.Text, out v); return v; } set { txtScale.Text = value.ToString(); } }

        public bool DrawCulvertInfo { get { return cbDrawCulvertInfo.Checked; } }

        public List<CulvertInfo> GetData()
        {
            List<CulvertInfo> items = new List<CulvertInfo>();

            for (int i = culvertGrid.FixedRows; i < culvertGrid.RowsCount; i++)
            {
                bool emptyRow = false;
                for (int j = 0; j < culvertGrid.ColumnsCount; j++)
                {
                    if (culvertGrid[i, j].Value == null || string.IsNullOrEmpty((string)culvertGrid[i, j].Value))
                    {
                        emptyRow = true;
                        break;
                    }
                }
                if (emptyRow) break;

                bool emptyCell = false;
                for (int j = 0; j < culvertGrid.ColumnsCount; j++)
                {
                    double val;
                    if (j == 0)
                    {
                        if (!CulvertInfo.TryChainageFromString((string)culvertGrid[i, j].Value, out val))
                        {
                            emptyCell = true;
                            break;
                        }
                    }
                    else
                    {
                        if (!double.TryParse((string)culvertGrid[i, j].Value, out val))
                        {
                            emptyCell = true;
                            break;
                        }
                    }
                }
                if (emptyCell) continue;

                CulvertInfo culvert = new CulvertInfo();
                culvert.Chainage = CulvertInfo.ChainageFromString((string)culvertGrid[i, 0].Value);
                culvert.Level = double.Parse((string)culvertGrid[i, 1].Value);
                culvert.Length = double.Parse((string)culvertGrid[i, 2].Value);
                culvert.Grade = double.Parse((string)culvertGrid[i, 3].Value);
                culvert.Skew = double.Parse((string)culvertGrid[i, 4].Value);
                culvert.Width = double.Parse((string)culvertGrid[i, 5].Value);
                culvert.Height = double.Parse((string)culvertGrid[i, 6].Value);
                items.Add(culvert);
            }

            items.Sort((p1, p2) => p1.Chainage.CompareTo(p2.Chainage));

            return items;
        }

        public DrawCulvertForm()
        {
            InitializeComponent();

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

            culvertGrid.ColumnsCount = 7;
            culvertGrid.RowsCount = 1024;

            culvertGrid.FixedRows = 1;

            culvertGrid[0, 0] = new SourceGrid.Cells.ColumnHeader("KM");
            culvertGrid[0, 1] = new SourceGrid.Cells.ColumnHeader("Kot (m)");
            culvertGrid[0, 2] = new SourceGrid.Cells.ColumnHeader("Boyu (m)");
            culvertGrid[0, 3] = new SourceGrid.Cells.ColumnHeader("Eğim (%)");
            culvertGrid[0, 4] = new SourceGrid.Cells.ColumnHeader("Verevlilik");
            culvertGrid[0, 5] = new SourceGrid.Cells.ColumnHeader("Genişlik (m)");
            culvertGrid[0, 6] = new SourceGrid.Cells.ColumnHeader("Yükseklik (m)");

            culvertGrid.Columns[0].Tag = "CH";
            culvertGrid.Columns[1].Tag = "Level";
            culvertGrid.Columns[2].Tag = "Length";
            culvertGrid.Columns[3].Tag = "Grade";
            culvertGrid.Columns[4].Tag = "Skew";
            culvertGrid.Columns[5].Tag = "Width";
            culvertGrid.Columns[6].Tag = "Height";

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
                else if (!CulvertInfo.TryChainageFromString((string)val, out res))
                {
                    sender.Cell.View = errorView;
                }
                else
                {
                    sender.Value = CulvertInfo.ChainageToString(res);
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

            public static double ChainageFromString(string text)
            {
                double result = 0;
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Replace(',', '.');
                    text = text.Replace("+", "");
                    double.TryParse(text, out result);
                }
                return result;
            }

            public static bool TryChainageFromString(string text, out double value)
            {
                value = 0;
                if (string.IsNullOrEmpty(text)) return false;

                text = text.Replace(',', '.');
                text = text.Replace("+", "");

                return double.TryParse(text, out value);
            }

            public static string ChainageToString(double value)
            {
                double km = Math.Floor(value / 1000);
                double m = value - km * 1000;
                return string.Format("{0:0}+{1:000.00}", km, m);
            }
        }
    }
}
