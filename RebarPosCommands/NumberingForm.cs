﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

namespace RebarPosCommands
{
    public partial class NumberingForm : VersionDisplayForm
    {
        public enum Ordering
        {
            None = 0,
            Location = 1,
            Shape = 2,
            Diameter = 3,
            TotalLength = 4
        }

        List<ObjectId> sourceItems;
        List<PosCopy> m_PosList;
        Dictionary<string, List<ObjectId>> m_DetachedPosList;

        public NumberingForm()
        {
            InitializeComponent();

            sourceItems = new List<ObjectId>();
            m_PosList = new List<PosCopy>();
            m_DetachedPosList = new Dictionary<string, List<ObjectId>>();
        }

        public bool Init(ObjectId[] items)
        {
            cbOrder1.SelectedIndex = 1;
            cbOrder2.SelectedIndex = 2;
            cbOrder3.SelectedIndex = 3;
            cbOrder4.SelectedIndex = 4;

            txtStartNum.Text = "1";
            rbKeepExisting.Checked = true;

            sourceItems = new List<ObjectId>(items);

            ReadPos();
            AddMissing();
            SortDisplayList();
            PopulateList();

            lbItems_SelectedIndexChanged(lbItems, new EventArgs());

            return true;
        }

        private void PopulateList()
        {
            if (m_PosList.Count == 0)
            {
                lbItems.Enabled = false;
                gbAutoNumber.Enabled = false;
                gbManualNumber.Enabled = false;
                btnOK.Enabled = false;
                return;
            }
            else
            {
                lbItems.Enabled = true;
                gbAutoNumber.Enabled = true;
                gbManualNumber.Enabled = true;
                btnOK.Enabled = true;
            }

            PosSettings group = PosSettings.Current;

            double lengthScale = 1.0;
            switch (group.DrawingUnit)
            {
                case PosSettings.DrawingUnits.Millimeter:
                    lengthScale = 1.0;
                    break;
                case PosSettings.DrawingUnits.Centimeter:
                    lengthScale = 10.0;
                    break;
                case PosSettings.DrawingUnits.Decimeter:
                    lengthScale = 100.0;
                    break;
                case PosSettings.DrawingUnits.Meter:
                    lengthScale = 1000.0;
                    break;
            }
            switch (group.DisplayUnit)
            {
                case PosSettings.DrawingUnits.Millimeter:
                    lengthScale /= 1.0;
                    break;
                case PosSettings.DrawingUnits.Centimeter:
                    lengthScale /= 10.0;
                    break;
                case PosSettings.DrawingUnits.Decimeter:
                    lengthScale /= 100.0;
                    break;
                case PosSettings.DrawingUnits.Meter:
                    lengthScale /= 1000.0;
                    break;
            }

            lbItems.Items.Clear();
            foreach (PosCopy copy in m_PosList)
            {
                ListViewItem item = new ListViewItem(copy.newpos);
                item.Tag = copy;
                if (copy.existing)
                {
                    item.SubItems.Add(copy.pos);
                    item.SubItems.Add(copy.priority.ToString());
                    item.SubItems.Add(copy.diameter);
                    item.SubItems.Add(copy.shapename);
                    item.SubItems.Add(copy.length);

                    string a = string.Empty;
                    string b = string.Empty;
                    string c = string.Empty;
                    string d = string.Empty;
                    string e = string.Empty;
                    string f = string.Empty;

                    if (copy.isVarA)
                        a = (copy.minA * lengthScale).ToString("F0") + "~" + (copy.maxA * lengthScale).ToString("F0");
                    else
                        a = (copy.minA * lengthScale).ToString("F0");
                    if (copy.isVarB)
                        b = (copy.minB * lengthScale).ToString("F0") + "~" + (copy.maxB * lengthScale).ToString("F0");
                    else
                        b = (copy.minB * lengthScale).ToString("F0");
                    if (copy.isVarC)
                        c = (copy.minC * lengthScale).ToString("F0") + "~" + (copy.maxC * lengthScale).ToString("F0");
                    else
                        c = (copy.minC * lengthScale).ToString("F0");
                    if (copy.isVarD)
                        d = (copy.minD * lengthScale).ToString("F0") + "~" + (copy.maxD * lengthScale).ToString("F0");
                    else
                        d = (copy.minD * lengthScale).ToString("F0");
                    if (copy.isVarE)
                        e = (copy.minE * lengthScale).ToString("F0") + "~" + (copy.maxE * lengthScale).ToString("F0");
                    else
                        e = (copy.minE * lengthScale).ToString("F0");
                    if (copy.isVarF)
                        f = (copy.minF * lengthScale).ToString("F0") + "~" + (copy.maxF * lengthScale).ToString("F0");
                    else
                        f = (copy.minF * lengthScale).ToString("F0");

                    item.SubItems.Add(a);
                    item.SubItems.Add(b);
                    item.SubItems.Add(c);
                    item.SubItems.Add(d);
                    item.SubItems.Add(e);
                    item.SubItems.Add(f);
                }
                else
                {
                    item.BackColor = Color.Silver;
                }
                lbItems.Items.Add(item);
            }
        }

        private void AddMissing()
        {
            RemoveEmpty();

            int lastpos = 0;
            foreach (PosCopy copy in m_PosList)
            {
                int posno;
                if (int.TryParse(copy.newpos, out posno))
                {
                    lastpos = Math.Max(lastpos, posno);
                }
            }
            for (int i = 1; i <= lastpos; i++)
            {
                if (!m_PosList.Exists(p => p.newpos == i.ToString()))
                {
                    PosCopy copy = new PosCopy();
                    copy.newpos = i.ToString();
                    m_PosList.Add(copy);
                }
            }
        }

        private void RemoveEmpty()
        {
            m_PosList.RemoveAll(p => p.existing == false);
        }

        private void SortList()
        {
            List<Ordering> order = new List<Ordering>();
            order.Add((Ordering)cbOrder1.SelectedIndex);
            order.Add((Ordering)cbOrder2.SelectedIndex);
            order.Add((Ordering)cbOrder3.SelectedIndex);
            order.Add((Ordering)cbOrder4.SelectedIndex);

            m_PosList.Sort(new CompareForSort(order));
        }

        private class CompareForSort : IComparer<PosCopy>
        {
            List<Ordering> order;

            public CompareForSort(List<Ordering> ord)
            {
                order = ord;
            }

            public int Compare(PosCopy p1, PosCopy p2)
            {
                foreach (Ordering o in order)
                {
                    int res = 0;
                    switch (o)
                    {
                        case Ordering.None:
                            res = 0;
                            break;
                        case Ordering.Location:
                            if (p1.y - p2.y > (p1.scale + p2.scale) * 2.0)
                                res = -1;
                            else if (p2.y - p1.y > (p1.scale + p2.scale) * 2.0)
                                res = 1;
                            else if (p1.x < p2.x)
                                res = -1;
                            else if (p1.x > p2.x)
                                res = 1;
                            else
                                res = 0;
                            break;
                        case Ordering.Shape:
                            if (p1.priority > p2.priority)
                                res = -1;
                            else if (p1.priority < p2.priority)
                                res = 1;
                            else
                                res = string.CompareOrdinal(p1.shapename, p2.shapename);
                            break;
                        case Ordering.Diameter:
                            double d1 = 0.0, d2 = 0.0;
                            double.TryParse(p1.diameter, out d1);
                            double.TryParse(p2.diameter, out d2);
                            if (d1 < d2)
                                res = -1;
                            else if (d1 > d2)
                                res = 1;
                            else
                                res = 0;
                            break;
                        case Ordering.TotalLength:
                            double len1 = (p1.length1 + p1.length2) / 2.0;
                            double len2 = (p2.length1 + p2.length2) / 2.0;
                            if (len1 < len2)
                                res = -1;
                            else if (len1 > len2)
                                res = 1;
                            else
                                res = 0;
                            break;
                    }
                    if (res != 0)
                        return res;
                }

                return 0;
            }
        }

        private void SortDisplayList()
        {
            m_PosList.Sort(new CompareForDisplay());
        }

        private class CompareForDisplay : IComparer<PosCopy>
        {
            public int Compare(PosCopy e1, PosCopy e2)
            {
                int p1 = 0;
                int p2 = 0;
                int.TryParse(e1.newpos, out p1);
                int.TryParse(e2.newpos, out p2);
                return (p1 == p2 ? 0 : (p1 < p2 ? -1 : 1));
            }
        }

        private void ApplyNumbers()
        {
            int count = 0;
            int poscount = 0;
            int detachedcount = 0;

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (PosCopy copy in m_PosList)
                    {
                        string num = copy.newpos;
                        if (copy.existing && !string.IsNullOrEmpty(num))
                        {
                            foreach (ObjectId id in copy.list)
                            {
                                RebarPos pos = RebarPos.FromObjectId(tr, id);
                                if (pos != null)
                                {
                                    pos.Pos = num;
                                    pos.Save(tr);
                                }
                                poscount++;
                            }
                            if (m_DetachedPosList.ContainsKey(copy.pos))
                            {
                                foreach (ObjectId id in m_DetachedPosList[copy.pos])
                                {
                                    RebarPos pos = RebarPos.FromObjectId(tr, id);
                                    if (pos != null)
                                    {
                                        pos.Pos = num;
                                        pos.Save(tr);
                                    }
                                    detachedcount++;
                                }
                            }

                            count++;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }

            MessageBox.Show(
                count.ToString() + " adet farklı numara verildi.\n" +
                (poscount + detachedcount).ToString() + " adet poz nesnesi numaralandırıldı.",
                "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReadPos()
        {
            try
            {
                if(rbGroupVarLength.Checked)
                    m_PosList = PosCopy.ReadAllInSelection(sourceItems, false, PosCopy.PosGrouping.PosKeyDifferentMarker);
                else
                    m_PosList = PosCopy.ReadAllInSelection(sourceItems, false, PosCopy.PosGrouping.PosKeyDifferentMarkerVarLength);

                m_DetachedPosList = new Dictionary<string, List<ObjectId>>();
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in sourceItems)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, id);
                        if (pos == null) continue;
                        if (!pos.Detached) continue;
                        if (string.IsNullOrEmpty(pos.Pos)) continue;

                        if (m_DetachedPosList.ContainsKey(pos.Pos))
                            m_DetachedPosList[pos.Pos].Add(id);
                        else
                            m_DetachedPosList.Add(pos.Pos, new List<ObjectId>() { id });
                    }
                    tr.Commit();
                }

                AddMissing();
                SortDisplayList();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = -1;
            int n = lbItems.Items.Count;
            if (lbItems.SelectedIndices.Count != 0)
                i = lbItems.SelectedIndices[0];
            btnDecrementNumber.Enabled = (i != -1) && (i > 0);
            btnIncrementNumber.Enabled = (i != -1) && (i < n - 1);
            btnAddPos.Enabled = (i != -1) && (i < n - 1);
            btnDeletePos.Enabled = (i != -1) && (n > 0) && (((PosCopy)lbItems.Items[i].Tag).existing == false);
            lblNumber.Enabled = (i != -1);
            txtNumber.Enabled = (i != -1);
            btnApplyNumber.Enabled = (i != -1);

            if (i != -1 && n > 0)
                txtNumber.Text = ((PosCopy)lbItems.Items[i].Tag).newpos;
            else
                txtNumber.Text = "";
        }

        private void btnAutoNumber_Click(object sender, EventArgs e)
        {
            bool keepcurrent = rbKeepExisting.Checked;
            bool numberVarLength = rbNumberVarLength.Checked;

            int startnum = 0;
            if (keepcurrent)
            {
                foreach (PosCopy copy in m_PosList)
                {
                    int num = 0;
                    if (int.TryParse(copy.pos.Trim(), out num))
                    {
                        startnum = Math.Max(startnum, num);
                    }
                }
                startnum++;
            }
            else
            {
                startnum = int.Parse(txtStartNum.Text);
            }

            if (!keepcurrent && (startnum <= 0))
            {
                MessageBox.Show("Lütfen başlangıç numarasını girin.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RemoveEmpty();
            SortList();

            foreach (PosCopy copy in m_PosList)
            {
                if (!keepcurrent || string.IsNullOrEmpty(copy.pos.Trim()))
                {
                    PosCopy original = m_PosList.Find(p => !string.IsNullOrEmpty(p.pos) && p.key == copy.key);
                    if (original == null || (numberVarLength && copy.isVarLength))
                    {
                        // Add a new number
                        copy.newpos = startnum.ToString();
                        startnum++;
                    }
                    else
                    {
                        // Use existing number
                        copy.newpos = original.pos;
                    }
                }
            }

            AddMissing();
            SortDisplayList();
            PopulateList();
        }

        private void btnDecrementNumber_Click(object sender, EventArgs e)
        {
            int i = -1;
            int n = lbItems.Items.Count;
            if (lbItems.SelectedIndices.Count != 0)
                i = lbItems.SelectedIndices[0];
            if (i == -1) return;

            PosCopy old = m_PosList[i];
            string num = (int.Parse(old.newpos) - 1).ToString();
            old.newpos = num;
            PosCopy swold = m_PosList[i - 1];
            string swnum = (int.Parse(swold.newpos) + 1).ToString();
            swold.newpos = swnum;

            AddMissing();
            SortDisplayList();
            PopulateList();

            i = m_PosList.FindIndex(p => p.newpos == num);
            if (i != -1)
                lbItems.SelectedIndices.Add(i);
        }

        private void btnIncrementNumber_Click(object sender, EventArgs e)
        {
            int i = -1;
            int n = lbItems.Items.Count;
            if (lbItems.SelectedIndices.Count != 0)
                i = lbItems.SelectedIndices[0];
            if (i == -1) return;

            PosCopy old = m_PosList[i];
            string num = (int.Parse(old.newpos) + 1).ToString();
            old.newpos = num;
            PosCopy swold = m_PosList[i + 1];
            string swnum = (int.Parse(swold.newpos) - 1).ToString();
            swold.newpos = swnum;

            AddMissing();
            SortDisplayList();
            PopulateList();

            i = m_PosList.FindIndex(p => p.newpos == num);
            if (i != -1)
                lbItems.SelectedIndices.Add(i);
        }

        private void btnAddPos_Click(object sender, EventArgs e)
        {
            int i = -1;
            int n = lbItems.Items.Count;
            if (lbItems.SelectedIndices.Count != 0)
                i = lbItems.SelectedIndices[0];
            if (i == -1) return;

            int num = int.Parse(m_PosList[i].newpos);

            foreach (PosCopy copy in m_PosList)
            {
                int oldnum = int.Parse(copy.newpos);
                if (oldnum > num)
                {
                    copy.newpos = (oldnum + 1).ToString();
                }
            }

            AddMissing();
            SortDisplayList();
            PopulateList();

            i = m_PosList.FindIndex(p => p.newpos == (1 + num).ToString());
            if (i != -1)
                lbItems.SelectedIndices.Add(i);
        }

        private void btnDeletePos_Click(object sender, EventArgs e)
        {
            int i = -1;
            int n = lbItems.Items.Count;
            if (lbItems.SelectedIndices.Count != 0)
                i = lbItems.SelectedIndices[0];
            if (i == -1) return;

            int num = int.Parse(m_PosList[i].newpos);

            foreach (PosCopy copy in m_PosList)
            {
                int oldnum = int.Parse(copy.newpos);
                if (oldnum > num)
                {
                    copy.newpos = (oldnum - 1).ToString();
                }
            }

            AddMissing();
            SortDisplayList();
            PopulateList();

            i = m_PosList.FindIndex(p => p.newpos == (num).ToString());
            if (i != -1)
                lbItems.SelectedIndices.Add(i);
        }

        private void btnApplyNumber_Click(object sender, EventArgs e)
        {
            int i = -1;
            int n = lbItems.Items.Count;
            if (lbItems.SelectedIndices.Count != 0)
                i = lbItems.SelectedIndices[0];
            if (i == -1) return;

            string num = txtNumber.Text;
            int numi = 0;
            if (!int.TryParse(num, out numi) || numi <= 0)
            {
                MessageBox.Show("Lütfen yeni poz numarasını girin.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PosCopy old = m_PosList[i];
            int swsel = m_PosList.FindIndex(p => p.newpos == num);
            string swnum = old.newpos;
            old.newpos = num;

            if (swsel != -1)
            {
                PosCopy swold = m_PosList[swsel];
                swold.newpos = swnum;
            }

            AddMissing();
            SortDisplayList();
            PopulateList();

            i = m_PosList.FindIndex(p => p.newpos == num);
            if (i != -1)
                lbItems.SelectedIndices.Add(i);
        }

        private void rbKeepExisting_CheckedChanged(object sender, EventArgs e)
        {
            txtStartNum.Enabled = !rbKeepExisting.Checked;
        }

        private void rbNumberAll_CheckedChanged(object sender, EventArgs e)
        {
            txtStartNum.Enabled = !rbKeepExisting.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplyNumbers();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbNumberVarLength_CheckedChanged(object sender, EventArgs e)
        {
            ReadPos();
            AddMissing();
            SortDisplayList();
            PopulateList();

            lbItems_SelectedIndexChanged(lbItems, new EventArgs());
        }

        private void rbGroupVarLength_CheckedChanged(object sender, EventArgs e)
        {
            ReadPos();
            AddMissing();
            SortDisplayList();
            PopulateList();

            lbItems_SelectedIndexChanged(lbItems, new EventArgs());
        }
    }
}
