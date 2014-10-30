using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace XCOM
{
    public class CheckedListBoxWithButtons : CheckedListBox
    {
        public delegate void ButtonClickEventHandler(object sender, ButtonClickEventArgs e);

        [Category("Behavior"), Browsable(true)]
        public event ButtonClickEventHandler ButtonClick;

        Dictionary<object, bool> buttons = new Dictionary<object, bool>();
        private bool mouseDown = false;
        private int hoverButton = -1;
        private int buttonWidth = 20;

        public CheckedListBoxWithButtons()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public void ShowButton(object item)
        {
            if (buttons.ContainsKey(item))
                buttons[item] = true;
            else
                buttons.Add(item, true);
        }

        public void HideButton(object item)
        {
            buttons.Remove(item);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (Items.Count == 0) return;

            if (buttons.ContainsKey(Items[e.Index]))
            {
                Rectangle bounds = new Rectangle(e.Bounds.Right - buttonWidth, e.Bounds.Top, buttonWidth, e.Bounds.Height);
                System.Windows.Forms.VisualStyles.PushButtonState state = System.Windows.Forms.VisualStyles.PushButtonState.Normal;
                if (hoverButton == e.Index)
                {
                    if (mouseDown)
                        state = System.Windows.Forms.VisualStyles.PushButtonState.Pressed;
                    else
                        state = System.Windows.Forms.VisualStyles.PushButtonState.Hot;
                }
                ButtonRenderer.DrawButton(e.Graphics, bounds, state);
                int dotSize = 4;
                Rectangle dot = new Rectangle(bounds.Left + (bounds.Width - dotSize) / 2, bounds.Top + (bounds.Height - dotSize) / 2, dotSize, dotSize);
                e.Graphics.FillEllipse(SystemBrushes.WindowText, dot);
            }
        }

        private Rectangle GetButtonBounds(int index)
        {
            Rectangle bounds = this.GetItemRectangle(index);
            return new Rectangle(bounds.Right - buttonWidth, bounds.Top, buttonWidth, bounds.Height);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int oldHoverButton = hoverButton;
            hoverButton = -1;
            int i = 0;
            foreach (object item in Items)
            {
                if (buttons.ContainsKey(item))
                {
                    Rectangle bounds = GetButtonBounds(i);
                    if (bounds.Contains(e.Location))
                    {
                        hoverButton = i;
                        Invalidate();
                        break;
                    }
                }
                i++;
            }

            if (hoverButton != oldHoverButton) Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (hoverButton != -1 && (e.Button & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.None)
            {
                mouseDown = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if ((e.Button & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.None)
            {
                mouseDown = false;
                Invalidate();

                if (hoverButton != -1)
                {
                    OnButtonClick(new ButtonClickEventArgs(hoverButton));
                }
            }
        }

        protected void OnButtonClick(ButtonClickEventArgs e)
        {
            if (ButtonClick != null)
                ButtonClick(this, e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs e)
        {
            if (hoverButton != -1)
                e.NewValue = e.CurrentValue;
            else
                base.OnItemCheck(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i)
                        || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                        || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Selected, this.ForeColor,
                                this.BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Default, this.ForeColor,
                                this.BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }

        public class ButtonClickEventArgs : EventArgs
        {
            public int Index { get; private set; }

            public ButtonClickEventArgs(int index)
            {
                Index = index;
            }
        }
    }
}
