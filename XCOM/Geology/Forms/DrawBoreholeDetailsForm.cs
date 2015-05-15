using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XCOM.Commands.Geology
{
    public partial class DrawBoreholeDetailsForm : Form
    {
        private bool columnDrag = false;
        private int columnDragIndex = -1;
        private int columnOverIndex = -1;

        public bool HasGroundwater { get { return cbGroundwater.Checked; } }
        public double GroundwaterLevel { get { double val = -1; double.TryParse(txtGroundwater.Text, out val); return val; } }

        private int GetLastDataRow()
        {
            List<double> depths = new List<double>();
            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                string key = (string)boreholeGrid.Columns[j].Tag;
                if (string.Compare(key, "Depth", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    for (int i = boreholeGrid.FixedRows; i < boreholeGrid.RowsCount; i++)
                    {
                        string val = string.Empty;
                        object obj = boreholeGrid[i, j].Value;
                        if (obj == null) return i - 1;
                        val = (string)obj;
                        if (string.IsNullOrEmpty(val)) return i - 1;
                        double depth = 0;
                        val = val.Replace(',', '.');
                        if (!double.TryParse(val, out depth)) return i - 1;
                    }
                    return boreholeGrid.ColumnsCount - 1;
                }
            }
            return 0;
        }

        public Dictionary<string, List<string>> GetData()
        {
            int n = GetLastDataRow();
            Dictionary<string, List<string>> items = new Dictionary<string, List<string>>();
            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                bool hasData = false;
                string key = (string)boreholeGrid.Columns[j].Tag;
                for (int i = boreholeGrid.FixedRows; i <= n; i++)
                {
                    object obj = boreholeGrid[i, j].Value;
                    if (obj != null)
                    {
                        string val = (string)obj;
                        if (!string.IsNullOrEmpty(val))
                        {
                            hasData = true;
                            break;
                        }
                    }
                }

                if (hasData)
                {
                    items.Add(key, new List<string>());
                }
            }

            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                string key = (string)boreholeGrid.Columns[j].Tag;
                if (items.ContainsKey(key))
                {
                    for (int i = boreholeGrid.FixedRows; i <= n; i++)
                    {
                        string val = string.Empty;
                        object obj = boreholeGrid[i, j].Value;
                        if (obj != null) val = (string)obj;
                        items[key].Add(val);
                    }
                }
            }
            return items;
        }

        public Dictionary<string, string> GetHeaders()
        {
            int n = GetLastDataRow();
            Dictionary<string, string> items = new Dictionary<string, string>();
            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                bool hasData = false;
                string key = (string)boreholeGrid.Columns[j].Tag;
                for (int i = boreholeGrid.FixedRows; i <= n; i++)
                {
                    object obj = boreholeGrid[i, j].Value;
                    if (obj != null)
                    {
                        string val = (string)obj;
                        if (!string.IsNullOrEmpty(val))
                        {
                            hasData = true;
                            break;
                        }
                    }
                }

                if (hasData)
                {
                    string val = string.Empty;
                    object obj = boreholeGrid[0, j].Value;
                    if (obj != null) val = (string)obj;
                    items.Add(key, val);
                }
            }

            return items;
        }

        public List<double> GetDepths()
        {
            int n = GetLastDataRow();
            List<double> depths = new List<double>();
            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                string key = (string)boreholeGrid.Columns[j].Tag;
                if (string.Compare(key, "Depth", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    for (int i = boreholeGrid.FixedRows; i <= n; i++)
                    {
                        string val = string.Empty;
                        object obj = boreholeGrid[i, j].Value;
                        if (obj != null) val = (string)obj;

                        double depth = -1;
                        double.TryParse(val, out depth);
                        depths.Add(depth);
                    }
                }
            }
            return depths;
        }

        public DrawBoreholeDetailsForm()
        {
            InitializeComponent();

            Text = "Sondaj Detayları v" + typeof(DrawBoreholeDetailsForm).Assembly.GetName().Version.ToString(2);

            SourceGrid.Cells.Views.Cell cellView = new SourceGrid.Cells.Views.Cell();
            cellView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;
            cellView.BackColor = Color.White;
            cellView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;

            SourceGrid.Cells.Views.Cell errorView = new SourceGrid.Cells.Views.Cell();
            errorView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;
            errorView.BackColor = Color.LightCoral;
            errorView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;

            DoubleCellChangedEvent doubleEventController = new DoubleCellChangedEvent(cellView, errorView);
            DoubleOrStringCellChangedEvent doubleOrStringEventController = new DoubleOrStringCellChangedEvent(cellView, errorView);

            boreholeGrid.BorderStyle = BorderStyle.FixedSingle;
            boreholeGrid.EnableSort = false;

            boreholeGrid.ColumnsCount = 9;
            boreholeGrid.RowsCount = 1024;

            boreholeGrid.FixedRows = 1;

            boreholeGrid[0, 0] = new SourceGrid.Cells.ColumnHeader("Derinlik (m)");
            boreholeGrid[0, 1] = new SourceGrid.Cells.ColumnHeader("SPT");
            boreholeGrid[0, 2] = new SourceGrid.Cells.ColumnHeader("TCR");
            boreholeGrid[0, 3] = new SourceGrid.Cells.ColumnHeader("RQD");
            boreholeGrid[0, 4] = new SourceGrid.Cells.ColumnHeader("qu (MPa)");
            boreholeGrid[0, 5] = new SourceGrid.Cells.ColumnHeader("Is (MPa)");
            boreholeGrid[0, 6] = new SourceGrid.Cells.ColumnHeader("Es (MPa)");
            boreholeGrid[0, 7] = new SourceGrid.Cells.ColumnHeader("PL (kg/cm2)");
            boreholeGrid[0, 8] = new SourceGrid.Cells.ColumnHeader("E (kg/cm2)");

            boreholeGrid.Columns[0].Tag = "Depth";
            boreholeGrid.Columns[1].Tag = "SPT";
            boreholeGrid.Columns[2].Tag = "TCR";
            boreholeGrid.Columns[3].Tag = "RQD";
            boreholeGrid.Columns[4].Tag = "qu";
            boreholeGrid.Columns[5].Tag = "Is";
            boreholeGrid.Columns[6].Tag = "Es";
            boreholeGrid.Columns[7].Tag = "PL";
            boreholeGrid.Columns[8].Tag = "E";

            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                boreholeGrid.Columns[j].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            }

            for (int i = 0; i < boreholeGrid.RowsCount; i++)
            {
                boreholeGrid.Rows[i].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            }

            for (int i = boreholeGrid.FixedRows; i < boreholeGrid.RowsCount; i++)
            {
                for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
                {
                    boreholeGrid[i, j] = new SourceGrid.Cells.Cell("", typeof(string));
                    boreholeGrid[i, j].View = cellView;
                    if ((string)boreholeGrid.Columns[j].Tag == "Depth")
                    {
                        boreholeGrid[i, j].AddController(doubleEventController);
                    }
                    else
                    {
                        boreholeGrid[i, j].AddController(doubleOrStringEventController);
                    }
                }
            }

            boreholeGrid.AutoSizeCells();

            int maxWidth = 50;
            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                maxWidth = Math.Max(maxWidth, boreholeGrid.Columns[j].Width);
            }
            for (int j = 0; j < boreholeGrid.ColumnsCount; j++)
            {
                boreholeGrid.Columns[j].MinimalWidth = maxWidth;
            }

            boreholeGrid.AutoSizeCells();
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
                else if (!double.TryParse(((string)val).Replace(',', '.'), out res))
                {
                    sender.Cell.View = errorView;
                }
                else
                {
                    sender.Value = res.ToString();
                    sender.Cell.View = cellView;
                }
            }
        }

        public class DoubleOrStringCellChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            SourceGrid.Cells.Views.Cell cellView;
            SourceGrid.Cells.Views.Cell errorView;

            public DoubleOrStringCellChangedEvent(SourceGrid.Cells.Views.Cell sourceCellView, SourceGrid.Cells.Views.Cell sourceErrorView)
                : base()
            {
                cellView = sourceCellView;
                errorView = sourceErrorView;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                object val = sender.Value;
                if (val == null || string.IsNullOrEmpty((string)val))
                {
                    sender.Cell.View = cellView;
                }
                else
                {
                    string res = (string)val;
                    res = res.Replace(',', '.');
                    sender.Value = res;
                    sender.Cell.View = cellView;
                }
            }
        }

        private void boreholeGrid_MouseDown(object sender, MouseEventArgs e)
        {
            SourceGrid.Position pos = boreholeGrid.PositionAtPoint(e.Location);
            if (!pos.IsEmpty() && pos.Row == 0 && pos.Column >= 0 && pos.Column < boreholeGrid.ColumnsCount)
            {
                columnDrag = true;
                columnDragIndex = pos.Column;
                columnOverIndex = pos.Column;
                boreholeGrid.Selection.ResetSelection(true);
                boreholeGrid.Selection.SelectColumn(columnOverIndex, true);
                boreholeGrid.Selection.FocusColumn(columnOverIndex);
            }
        }

        private void boreholeGrid_MouseMove(object sender, MouseEventArgs e)
        {
            SourceGrid.Position pos = boreholeGrid.PositionAtPoint(e.Location);
            if (columnDrag && !pos.IsEmpty() && pos.Column >= 0 && pos.Column < boreholeGrid.ColumnsCount)
            {
                columnOverIndex = pos.Column;
                boreholeGrid.Columns.Move(columnDragIndex, columnOverIndex);
                columnDragIndex = pos.Column;
                boreholeGrid.Selection.ResetSelection(true);
                boreholeGrid.Selection.SelectColumn(columnOverIndex, true);
                boreholeGrid.Selection.FocusColumn(columnOverIndex);
            }
        }

        private void boreholeGrid_MouseUp(object sender, MouseEventArgs e)
        {
            columnDrag = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
