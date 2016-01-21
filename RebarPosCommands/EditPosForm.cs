using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace RebarPosCommands
{
    public partial class EditPosForm : VersionDisplayForm
    {
        private enum UpdateLengthResult
        {
            OK = 1,
            SystemError = 2,
            ExceedsMaximum = 3,
            InvalidLength = 4
        }

        RebarPos m_Pos;
        string m_Shape;
        List<int> m_StandardDiameters;
        RebarPos.HitTestResult hit;
        string m_Formula;
        bool m_Bending;
        int m_Fields;
        int m_Precision;
        PosGroup.DrawingUnits m_DisplayUnits;
        PosGroup.DrawingUnits m_DrawingUnits;
        double m_MaxLength;
        List<Autodesk.AutoCAD.GraphicsInterface.Drawable> transients;

        public EditPosForm()
        {
            InitializeComponent();

            m_Pos = null;
            m_Shape = string.Empty;
            m_StandardDiameters = new List<int>();

            posShapeView.BackColor = AcadUtility.AcadGraphics.ModelBackgroundColor();
            transients = new List<Autodesk.AutoCAD.GraphicsInterface.Drawable>();

            this.Disposed += EditPosForm_Disposed;
            this.FormClosed += EditPosForm_FormClosed;
        }

        void EditPosForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            EraseTransients();
        }

        void EditPosForm_Disposed(object sender, EventArgs e)
        {
            EraseTransients();
        }

        public bool Init(RebarPos pos, Point3d pt)
        {
            m_Pos = pos;

            m_Shape = pos.ShapeName;

            txtPosMarker.Text = pos.Pos;
            txtPosCount.Text = pos.Count;
            cbPosDiameter.Text = pos.Diameter;
            txtPosSpacing.Text = pos.Spacing;
            txtPosMultiplier.Text = pos.Multiplier.ToString();
            chkIncludePos.Checked = (pos.Multiplier > 0);
            txtPosMultiplier.Enabled = (pos.Multiplier > 0);
            txtPosNote.Text = pos.Note;

            txtA.Text = pos.A;
            txtB.Text = pos.B;
            txtC.Text = pos.C;
            txtD.Text = pos.D;
            txtE.Text = pos.E;
            txtF.Text = pos.F;

            chkShowLength.Checked = pos.ShowLength;

            if (!SetGroup())
            {
                return false;
            }
            if (!SetShape())
            {
                return false;
            }

            UpdateLength();

            pnlAlignLength.Enabled = pos.HasLengthAttribute;
            pnlAlignNote.Enabled = pos.HasNoteAttribute;

            hit = pos.HitTest(pt);

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool haserror = false;
            if (!CheckPosMarker()) haserror = true;
            if (!CheckPosCount()) haserror = true;
            if (!CheckPosDiameter()) haserror = true;
            if (!CheckPosSpacing()) haserror = true;
            if (!CheckPosMultiplier()) haserror = true;
            if (m_Fields >= 1)
                if (!CheckPosLength(txtA)) haserror = true;
            if (m_Fields >= 2)
                if (!CheckPosLength(txtB)) haserror = true;
            if (m_Fields >= 3)
                if (!CheckPosLength(txtC)) haserror = true;
            if (m_Fields >= 4)
                if (!CheckPosLength(txtD)) haserror = true;
            if (m_Fields >= 5)
                if (!CheckPosLength(txtE)) haserror = true;
            if (m_Fields >= 6)
                if (!CheckPosLength(txtF)) haserror = true;

            if (haserror)
            {
                MessageBox.Show("Lütfen hatalı değerleri düzeltin.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            m_Pos.Pos = txtPosMarker.Text;
            m_Pos.Count = txtPosCount.Text;
            m_Pos.Diameter = cbPosDiameter.Text;
            m_Pos.Spacing = txtPosSpacing.Text;
            m_Pos.Multiplier = int.Parse(txtPosMultiplier.Text);
            m_Pos.Note = txtPosNote.Text;
            m_Pos.ShapeName = m_Shape;
            m_Pos.ShowLength = chkShowLength.Checked;

            m_Pos.A = txtA.Text;
            m_Pos.B = txtB.Text;
            m_Pos.C = txtC.Text;
            m_Pos.D = txtD.Text;
            m_Pos.E = txtE.Text;
            m_Pos.F = txtF.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GetLengthFromEntity(TextBox txt)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                PromptEntityOptions opts = new PromptEntityOptions("\nSelect entity: ");
                opts.SetRejectMessage("\nSelect a LINE, ARC, TEXT, MTEXT, POLYLINE or DIMENSION entity.");
                opts.AddAllowedClass(typeof(Line), false);
                opts.AddAllowedClass(typeof(Arc), false);
                opts.AddAllowedClass(typeof(DBText), false);
                opts.AddAllowedClass(typeof(MText), false);
                opts.AddAllowedClass(typeof(Dimension), false);
                opts.AddAllowedClass(typeof(Polyline), false);
                PromptEntityResult per = ed.GetEntity(opts);

                if (per.Status == PromptStatus.OK)
                {
                    ObjectId id = per.ObjectId;
                    Database db = HostApplicationServices.WorkingDatabase;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            DBObject obj = tr.GetObject(per.ObjectId, OpenMode.ForRead);
                            if (obj is Line)
                            {
                                Line dobj = obj as Line;
                                txt.Text = dobj.Length.ToString();
                            }
                            else if (obj is Arc)
                            {
                                Arc dobj = obj as Arc;
                                txt.Text = dobj.Length.ToString();
                            }
                            else if (obj is DBText)
                            {
                                DBText dobj = obj as DBText;
                                txt.Text = dobj.TextString;
                            }
                            else if (obj is MText)
                            {
                                MText dobj = obj as MText;
                                txt.Text = dobj.Text;
                            }
                            else if (obj is Dimension)
                            {
                                Dimension dobj = obj as Dimension;
                                txt.Text = dobj.Measurement.ToString();
                            }
                            else if (obj is Polyline)
                            {
                                Polyline dobj = obj as Polyline;
                                txt.Text = dobj.Length.ToString();
                            }
                            CheckPosLength(txt);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        tr.Commit();
                    }
                }
            }
        }

        private void GetLengthFromMeasurement(TextBox txt)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                string length = "";
                double length1 = 0;
                if (GetDistance("\nFirst point: ", "\nSecond point: ", out length1) == PromptStatus.OK)
                {
                    length = length1.ToString("F0");
                    double length2 = 0;
                    if (GetDistance("\n" + length + "-... First point: ", "\n" + length + "-... Second point: ", out length2, true) == PromptStatus.OK)
                    {
                        length += "~" + length2.ToString("F0");
                    }
                    txt.Text = length;
                    CheckPosLength(txt);
                }
            }
        }

        private PromptStatus GetDistance(string message1, string message2, out double length, bool allowEmpty)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointOptions opt1 = new PromptPointOptions(message1);
            opt1.AllowNone = allowEmpty;
            PromptPointResult res1 = ed.GetPoint(opt1);
            if (res1.Status == PromptStatus.OK)
            {
                PromptDistanceOptions opt2 = new PromptDistanceOptions(message2);
                opt2.AllowNone = allowEmpty;
                opt2.BasePoint = res1.Value;
                opt2.UseBasePoint = true;
                PromptDoubleResult res2 = ed.GetDistance(opt2);
                if (res2.Status == PromptStatus.OK)
                {
                    length = res2.Value;
                    return PromptStatus.OK;
                }
            }

            length = 0;
            return PromptStatus.Cancel;
        }

        private PromptStatus GetDistance(string message1, string message2, out double length)
        {
            return GetDistance(message1, message2, out length, false);
        }

        private void posShapeView_Click(object sender, EventArgs e)
        {
            using (SelectShapeForm form = new SelectShapeForm())
            {
                form.SetShapes(m_Shape);
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    m_Shape = form.Current;
                    SetShape();
                    UpdateLength();
                }
            }
        }

        private bool SetGroup()
        {
            PosGroup group = PosGroup.Current;
            if (group == null) return false;

            m_StandardDiameters = group.StandardDiameters;
            m_DisplayUnits = group.DisplayUnit;
            m_DrawingUnits = group.DrawingUnit;
            m_Bending = group.Bending;
            m_Precision = group.Precision;
            m_MaxLength = group.MaxBarLength;

            cbPosDiameter.Items.Clear();
            foreach (int d in m_StandardDiameters)
            {
                cbPosDiameter.Items.Add(d.ToString());
            }

            return true;
        }

        private bool SetShape()
        {
            PosShape shape = null;
            PosShape.Shapes.TryGetValue(m_Shape, out shape);
            posShapeView.Shape = shape;
            if (shape == null)
                return false;

            if (m_Bending)
                m_Formula = shape.FormulaBending;
            else
                m_Formula = shape.Formula;

            m_Fields = shape.Fields;
            txtA.Enabled = btnSelectA.Enabled = btnMeasureA.Enabled = (m_Fields >= 1);
            txtB.Enabled = btnSelectB.Enabled = btnMeasureB.Enabled = (m_Fields >= 2);
            txtC.Enabled = btnSelectC.Enabled = btnMeasureC.Enabled = (m_Fields >= 3);
            txtD.Enabled = btnSelectD.Enabled = btnMeasureD.Enabled = (m_Fields >= 4);
            txtE.Enabled = btnSelectE.Enabled = btnMeasureE.Enabled = (m_Fields >= 5);
            txtF.Enabled = btnSelectF.Enabled = btnMeasureF.Enabled = (m_Fields >= 6);

            if (!txtA.Enabled) txtA.Text = "";
            if (!txtB.Enabled) txtB.Text = "";
            if (!txtC.Enabled) txtC.Text = "";
            if (!txtD.Enabled) txtD.Text = "";
            if (!txtE.Enabled) txtE.Text = "";
            if (!txtF.Enabled) txtF.Text = "";

            lblPosShape.Text = m_Shape;

            return true;
        }

        private UpdateLengthResult UpdateLength()
        {
            // Get lengths
            double minLengthMM = 0;
            double maxLengthMM = 0;
            bool isVarLength = false;
            bool check = false;
            try
            {
                check = RebarPos.GetTotalLengths(m_Formula, m_Fields, txtA.Text, txtB.Text, txtC.Text, txtD.Text, txtE.Text, txtF.Text, cbPosDiameter.Text, out minLengthMM, out maxLengthMM, out isVarLength);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return UpdateLengthResult.SystemError;
            }

            if (check && (minLengthMM > double.Epsilon) && (maxLengthMM > double.Epsilon))
            {
                string unitPrefix = "mm";
                switch (m_DrawingUnits)
                {
                    case PosGroup.DrawingUnits.Millimeter:
                        unitPrefix = "mm";
                        break;
                    case PosGroup.DrawingUnits.Centimeter:
                        unitPrefix = "cm";
                        break;
                }

                // Scale from MM to drawing units
                double minLength = PosGroup.ConvertLength(minLengthMM, PosGroup.DrawingUnits.Millimeter, m_DrawingUnits);
                double maxLength = PosGroup.ConvertLength(maxLengthMM, PosGroup.DrawingUnits.Millimeter, m_DrawingUnits);

                if (isVarLength)
                {
                    lblTotalLength.Text = minLength.ToString("F" + m_Precision.ToString()) + "~" + maxLength.ToString("F" + m_Precision.ToString()) + " " + unitPrefix +
                         " (" + (minLengthMM / 1000.0).ToString("F2") + " m ~ " + (maxLengthMM / 1000.0).ToString("F2") + " m)";
                    lblAverageLength.Text = ((minLength + maxLength) / 2.0).ToString("F" + m_Precision.ToString()) + " " + unitPrefix +
                        " (" + ((minLengthMM + maxLengthMM) / 2.0 / 1000.0).ToString("F2") + " m)";

                    lblAverageLengthCaption.Visible = true;
                    lblAverageLength.Visible = true;
                }
                else
                {
                    lblTotalLength.Text = minLength.ToString("F" + m_Precision.ToString()) + " " + unitPrefix +
                        " (" + (minLengthMM / 1000.0).ToString("F2") + " m)";

                    lblAverageLengthCaption.Visible = false;
                    lblAverageLength.Visible = false;
                }

                if (minLengthMM > m_MaxLength * 1000.0 || maxLengthMM > m_MaxLength * 1000.0)
                {
                    lblAverageLength.ForeColor = Color.Red;
                    lblTotalLength.ForeColor = Color.Red;

                    return UpdateLengthResult.ExceedsMaximum;
                }
                else
                {
                    lblAverageLength.ForeColor = SystemColors.ControlText;
                    lblTotalLength.ForeColor = SystemColors.ControlText;

                    return UpdateLengthResult.OK;
                }
            }
            else
            {
                lblTotalLength.Text = "Hatalı Boy!";
                lblTotalLength.ForeColor = Color.Red;

                lblAverageLengthCaption.Visible = false;
                lblAverageLength.Visible = false;

                return UpdateLengthResult.InvalidLength;
            }
        }

        private void chkIncludePos_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkIncludePos.Checked)
            {
                txtPosMultiplier.Text = "0";
                txtPosMultiplier.Enabled = false;
            }
            else
            {
                int mult = 0;
                if (!int.TryParse(txtPosMultiplier.Text, out mult))
                    mult = 0;
                if (mult == 0)
                    txtPosMultiplier.Text = "1";
                txtPosMultiplier.Enabled = true;
            }
        }

        private void EditPosForm_Shown(object sender, EventArgs e)
        {
            switch (hit)
            {
                case RebarPos.HitTestResult.Count:
                    txtPosCount.Select();
                    txtPosCount.SelectAll();
                    break;
                case RebarPos.HitTestResult.Diameter:
                    cbPosDiameter.Select();
                    cbPosDiameter.SelectAll();
                    break;
                case RebarPos.HitTestResult.Length:
                    txtA.Select();
                    txtA.SelectAll();
                    break;
                case RebarPos.HitTestResult.Multiplier:
                    txtPosMultiplier.Select();
                    txtPosMultiplier.SelectAll();
                    break;
                case RebarPos.HitTestResult.Note:
                    txtPosNote.Select();
                    txtPosNote.SelectAll();
                    break;
                case RebarPos.HitTestResult.Pos:
                    txtPosMarker.Select();
                    txtPosMarker.SelectAll();
                    break;
                case RebarPos.HitTestResult.Spacing:
                    txtPosSpacing.Select();
                    txtPosSpacing.SelectAll();
                    break;
                default:
                    txtPosMarker.Select();
                    txtPosMarker.SelectAll();
                    break;
            }
        }

        private void txtPosMarker_Validating(object sender, CancelEventArgs e)
        {
            CheckPosMarker();
        }

        private void txtPosCount_Validating(object sender, CancelEventArgs e)
        {
            CheckPosCount();
        }

        private void cbPosDiameter_Validating(object sender, CancelEventArgs e)
        {
            CheckPosDiameter();
        }

        private void txtPosSpacing_Validating(object sender, CancelEventArgs e)
        {
            CheckPosSpacing();
        }

        private void txtPosMultiplier_Validating(object sender, CancelEventArgs e)
        {
            if (CheckPosMultiplier())
            {
                int mult = 0;
                int.TryParse(txtPosMultiplier.Text, out mult);

                if (mult == 0)
                {
                    chkIncludePos.Checked = false;
                    txtPosMultiplier.Enabled = false;
                }
            }
        }

        private void txtLength_Validating(object sender, CancelEventArgs e)
        {
            CheckPosLength((TextBox)sender);
        }

        private bool CheckPosMarker()
        {
            int val = 0;
            if (string.IsNullOrEmpty(txtPosMarker.Text) || int.TryParse(txtPosMarker.Text, out val))
            {
                errorProvider.SetError(txtPosMarker, "");
                return true;
            }
            else
            {
                errorProvider.SetError(txtPosMarker, "Poz numarası tam sayı olmalıdır.");
                errorProvider.SetIconAlignment(txtPosMarker, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }

        private bool CheckPosCount()
        {
            string str = txtPosCount.Text;
            str = str.Replace('x', '*');
            str = str.Replace('X', '*');

            if (string.IsNullOrEmpty(str) || Calculator.IsValid(str))
            {
                errorProvider.SetError(txtPosCount, "");
                return true;
            }
            else
            {
                errorProvider.SetError(txtPosCount, "Poz adedi yalnız rakam ve aritmetik işlemler içerebilir.");
                errorProvider.SetIconAlignment(txtPosCount, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }

        private bool CheckPosDiameter()
        {
            if (string.IsNullOrEmpty(cbPosDiameter.Text) || cbPosDiameter.Items.Contains(cbPosDiameter.Text))
            {
                errorProvider.SetError(cbPosDiameter, "");
                UpdateLength();
                return true;
            }
            else
            {
                errorProvider.SetError(cbPosDiameter, "Poz adedi standart çap listesi içinden seçilmelidir.");
                errorProvider.SetIconAlignment(cbPosDiameter, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }

        private bool CheckPosSpacing()
        {
            if (txtPosSpacing.IsValid)
            {
                errorProvider.SetError(txtPosSpacing, "");
                return true;
            }
            else
            {
                errorProvider.SetError(txtPosSpacing, "Poz aralığı yalnız rakam ve aralık işareti (~ veya -) içerebilir.");
                errorProvider.SetIconAlignment(txtPosSpacing, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }

        private bool CheckPosMultiplier()
        {
            int mult = 0;
            if (string.IsNullOrEmpty(txtPosMultiplier.Text) || int.TryParse(txtPosMultiplier.Text, out mult))
            {
                errorProvider.SetError(txtPosMultiplier, "");
                return true;
            }
            else
            {
                errorProvider.SetError(txtPosMultiplier, "Poz çarpanı tam sayı olmalıdır.");
                errorProvider.SetIconAlignment(txtPosMultiplier, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }

        private bool CheckPosLength(TextBox source)
        {
            bool isempty = false;
            bool haserror = false;

            // Split var lengths
            if (string.IsNullOrEmpty(source.Text))
            {
                isempty = true;
            }
            else
            {
                source.Text = source.Text.Replace('-', '~');
                string[] strparts = source.Text.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string str in strparts)
                {
                    string oldstr = str;
                    oldstr = oldstr.Replace('d', '0');
                    oldstr = oldstr.Replace('r', '0');
                    oldstr = oldstr.Replace('D', '0');
                    oldstr = oldstr.Replace('R', '0');
                    oldstr = oldstr.Replace('x', '*');
                    oldstr = oldstr.Replace('X', '*');

                    if (string.IsNullOrEmpty(oldstr))
                    {
                        isempty = true;
                        break;
                    }
                    else if (!Calculator.IsValid(oldstr))
                    {
                        haserror = true;
                        break;
                    }
                }
            }

            if (isempty)
            {
                errorProvider.SetError(source, "Lütfen parça boyunu girin.");
                errorProvider.SetIconAlignment(source, ErrorIconAlignment.MiddleLeft);
                return false;
            }
            else if (haserror)
            {
                errorProvider.SetError(source, "Parça boyu yalnız rakam ve aritmetik işlemler içerebilir.");
                errorProvider.SetIconAlignment(source, ErrorIconAlignment.MiddleLeft);
                return false;
            }

            UpdateLengthResult check = UpdateLength();
            if (check == UpdateLengthResult.OK)
            {
                errorProvider.SetError(source, "");
                return true;
            }
            else
            {
                if (check == UpdateLengthResult.ExceedsMaximum)
                    errorProvider.SetError(source, "Toplam boy maximum demir boyundan büyük olamaz.");
                else if (check == UpdateLengthResult.InvalidLength)
                    errorProvider.SetError(source, "Geçersiz parça boyu.");
                else
                    errorProvider.SetError(source, "Geçersiz boy.");

                errorProvider.SetIconAlignment(source, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }

        private void btnSelectA_Click(object sender, EventArgs e)
        {
            GetLengthFromEntity(txtA);
        }

        private void btnSelectB_Click(object sender, EventArgs e)
        {
            GetLengthFromEntity(txtB);
        }

        private void btnSelectC_Click(object sender, EventArgs e)
        {
            GetLengthFromEntity(txtC);
        }

        private void btnSelectD_Click(object sender, EventArgs e)
        {
            GetLengthFromEntity(txtD);
        }

        private void btnSelectE_Click(object sender, EventArgs e)
        {
            GetLengthFromEntity(txtE);
        }

        private void btnSelectF_Click(object sender, EventArgs e)
        {
            GetLengthFromEntity(txtF);
        }

        private void btnMeasureA_Click(object sender, EventArgs e)
        {
            GetLengthFromMeasurement(txtA);
        }

        private void btnMeasureB_Click(object sender, EventArgs e)
        {
            GetLengthFromMeasurement(txtB);
        }

        private void btnMeasureC_Click(object sender, EventArgs e)
        {
            GetLengthFromMeasurement(txtC);
        }

        private void btnMeasureD_Click(object sender, EventArgs e)
        {
            GetLengthFromMeasurement(txtD);
        }

        private void btnMeasureE_Click(object sender, EventArgs e)
        {
            GetLengthFromMeasurement(txtE);
        }

        private void btnMeasureF_Click(object sender, EventArgs e)
        {
            GetLengthFromMeasurement(txtF);
        }

        private void btnLastPos_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                DWGUtility.PromptRebarSelectionResult sel = DWGUtility.SelectAllPosUser();
                if (sel.Status != PromptStatus.OK) return;
                ObjectId[] items = sel.Value.GetObjectIds();
                int lastNum = DWGUtility.GetMaximumPosNumber(items);

                if (lastNum != -1)
                {
                    txtPosMarker.Text = (lastNum + 1).ToString();
                }
            }
        }

        private void btnPickNumber_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Operator, "<OR"),
                    new TypedValue((int)DxfCode.Start, "TEXT"),
                    new TypedValue((int)DxfCode.Start, "MTEXT"),
                    new TypedValue((int)DxfCode.Operator, "OR>")
                };
                PromptSelectionResult res = ed.GetSelection(new SelectionFilter(tvs));
                if (res.Status == PromptStatus.OK)
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            int total = 0;
                            foreach (SelectedObject sobj in res.Value)
                            {
                                string text = "";
                                DBObject obj = tr.GetObject(sobj.ObjectId, OpenMode.ForRead);
                                if (obj is DBText)
                                {
                                    DBText dobj = obj as DBText;
                                    text = dobj.TextString;
                                }
                                else if (obj is MText)
                                {
                                    MText dobj = obj as MText;
                                    text = dobj.Text;
                                }
                                if (!string.IsNullOrEmpty(text))
                                {
                                    text = text.TrimStart('(').TrimEnd(')');
                                    int num = 0;
                                    if (int.TryParse(text, out num))
                                    {
                                        total += num;
                                    }
                                }
                            }
                            if (total != 0)
                            {
                                txtPosCount.Text = total.ToString();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        tr.Commit();
                    }
                }
            }
        }

        private void btnPickSpacing_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                double length;
                if (GetDistance("\nStart point: ", "\nEnd point: ", out length) == PromptStatus.OK)
                {
                    double spa = 0;
                    if (double.TryParse(txtPosSpacing.Text, out spa) && spa > double.Epsilon)
                    {
                        int num = 1 + (int)Math.Ceiling(length / spa);
                        txtPosCount.Text = num.ToString();
                    }
                }
            }
        }

        private void btnDetach_Click(object sender, EventArgs e)
        {
            bool haserror = false;
            if (!CheckPosMarker()) haserror = true;

            if (haserror)
            {
                MessageBox.Show("Lütfen hatalı değerleri düzeltin.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            m_Pos.Pos = txtPosMarker.Text;
            m_Pos.Note = txtPosNote.Text;

            m_Pos.Detached = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAlign_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                PromptEntityOptions opts = new PromptEntityOptions("\nSelect entity: ");
                opts.AllowNone = false;
                opts.SetRejectMessage("\nSelect a LINE, ARC or POLYLINE entity.");
                opts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult per = ed.GetEntity(opts);
                if (per.Status == PromptStatus.OK)
                {
                    Point3d pt = per.PickedPoint;
                    ObjectId id = per.ObjectId;

                    Vector3d dir = new Vector3d();
                    Database db = HostApplicationServices.WorkingDatabase;
                    Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                    BlockReference bref = null;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        bref = (BlockReference)tr.GetObject(m_Pos.ID, OpenMode.ForRead);
                        Curve curve = (Curve)tr.GetObject(id, OpenMode.ForRead);
                        pt = curve.GetClosestPointTo(pt.TransformBy(ucs2wcs), curve.GetPlane().Normal, false);
                        dir = curve.GetFirstDerivative(pt);
                        tr.Commit();
                    }

                    Vector3d up = dir.CrossProduct(bref.Normal);
                    dir = dir / dir.Length;
                    up = up / up.Length;

                    Vector3d posDir = Vector3d.XAxis.RotateBy(bref.Rotation, bref.Normal);
                    Vector3d posUp = posDir.CrossProduct(bref.Normal);

                    bool isPosUp = (dir.DotProduct(posUp) > 0);

                    double offset = 0.0;
                    double offset1 = -0.0;
                    double offset2 = Math.Abs(57 * bref.ScaleFactors[0]);
                    if (isPosUp)
                    {
                        AlignPos(pt + offset1 * up, dir);
                        offset = offset2;
                    }
                    else
                    {
                        AlignPos(pt + offset2 * up, dir);
                        offset = offset1;
                    }

                    AddTransient(bref);

                    ed.UpdateScreen();

                    PromptKeywordOptions kopts = new PromptKeywordOptions("\nDiğer tarafa yerleştirilsin mi? [Evet/Hayır] <Hayir>: ", "Yes No");
                    kopts.AllowNone = true;
                    kopts.Keywords.Default = "No";
                    PromptResult kres = ed.GetKeywords(kopts);
                    if (kres.Status == PromptStatus.OK && kres.StringResult == "Yes")
                    {
                        AlignPos(pt + offset * up, dir);
                        UpdateTransient(bref);
                        ed.UpdateScreen();
                    }
                }
            }
        }

        private void AlignPos(Point3d pt, Vector3d direction)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference block = (BlockReference)tr.GetObject(m_Pos.ID, OpenMode.ForWrite);

                block.TransformBy(Matrix3d.Displacement(new Vector3d(-block.Position.X, -block.Position.Y, -block.Position.Z)));
                block.TransformBy(Matrix3d.Rotation(-block.Rotation, block.Normal, Point3d.Origin));

                block.TransformBy(Matrix3d.Rotation(Vector3d.XAxis.GetAngleTo(direction, Vector3d.ZAxis), block.Normal, Point3d.Origin));
                block.TransformBy(Matrix3d.Displacement(new Vector3d(pt.X, pt.Y, pt.Z)));

                tr.Commit();
            }
        }

        private void rbAlignLengthTop_Click(object sender, EventArgs e)
        {
            AlignAttribute(m_Pos.LengthID, 0, 57);
        }

        private void rbAlignLengthBottom_Click(object sender, EventArgs e)
        {
            AlignAttribute(m_Pos.LengthID, 0, -25);
        }

        private void rbAlignLengthRight_Click(object sender, EventArgs e)
        {
            AlignAttribute(m_Pos.LengthID, 190, 16);
        }

        private void rbAlignNoteTop_Click(object sender, EventArgs e)
        {
            AlignAttribute(m_Pos.NoteID, 0, 57);
        }

        private void rbAlignNoteBottom_Click(object sender, EventArgs e)
        {
            AlignAttribute(m_Pos.NoteID, 0, -25);
        }

        private void rbAlignNoteRight_Click(object sender, EventArgs e)
        {
            AlignAttribute(m_Pos.NoteID, 190, 16);
        }

        private void AlignAttribute(ObjectId attID, double xOffset, double yOffset)
        {
            if (attID.IsNull) return;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference block = (BlockReference)tr.GetObject(m_Pos.ID, OpenMode.ForRead);

                Vector3d posDir = Vector3d.XAxis.RotateBy(block.Rotation, block.Normal);
                Vector3d posUp = block.Normal.CrossProduct(posDir);

                AttributeReference attRef = (AttributeReference)tr.GetObject(attID, OpenMode.ForWrite);

                xOffset = xOffset * Math.Abs(block.ScaleFactors[0]);
                yOffset = yOffset * Math.Abs(block.ScaleFactors[0]);
                Point3d pt = block.Position + xOffset * posDir + yOffset * posUp;

                attRef.TransformBy(Matrix3d.Displacement(new Vector3d(-attRef.Position.X, -attRef.Position.Y, -attRef.Position.Z)));
                attRef.TransformBy(Matrix3d.Displacement(new Vector3d(pt.X, pt.Y, pt.Z)));

                AddTransient(block);
                ed.UpdateScreen();
                tr.Commit();
            }
        }

        private void AddTransient(Autodesk.AutoCAD.GraphicsInterface.Drawable item)
        {
            if (!transients.Contains(item))
            {
                transients.Add(item);
                Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager.AddTransient(item, Autodesk.AutoCAD.GraphicsInterface.TransientDrawingMode.DirectShortTerm, 0, new IntegerCollection());
            }
            UpdateTransient(item);
        }

        private void EraseTransient(Autodesk.AutoCAD.GraphicsInterface.Drawable item)
        {
            Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
            transients.Remove(item);
        }

        private void UpdateTransient(Autodesk.AutoCAD.GraphicsInterface.Drawable item)
        {
            Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
        }

        private void EraseTransients()
        {
            foreach (Autodesk.AutoCAD.GraphicsInterface.Drawable item in transients)
            {
                Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
            }
            transients.Clear();
        }
    }
}
