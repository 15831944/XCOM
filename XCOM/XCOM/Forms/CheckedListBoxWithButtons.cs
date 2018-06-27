using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public enum ButtonAlignment
    {
        Left,
        Right
    }

    public class CheckedListBoxWithButtons : CheckedListBox
    {
        public delegate void ButtonClickEventHandler(object sender, ButtonClickEventArgs e);

        [Category("Behavior"), Browsable(true)]
        public event ButtonClickEventHandler ButtonClick;

        Dictionary<int, List<string>> buttonMap = new Dictionary<int, List<string>>();
        private bool mouseDown = false;
        private int hoverItemIndex = -1;
        private int hoverButtonIndex = -1;
        private int buttonWidth = 20;
        private int buttonMargin = 1;
        private int dotSize = 4;
        private int buttonCols = 0;

        private ButtonAlignment buttonAlignment = ButtonAlignment.Left;
        [Category("Appearance"), Browsable(true), DefaultValue(typeof(ButtonAlignment), "Left")]
        public ButtonAlignment ButtonAlignment
        {
            get { return buttonAlignment; }
            set { buttonAlignment = value; Invalidate(); }
        }

        public CheckedListBoxWithButtons()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public void AddButton(int index)
        {
            AddButton(index, "");
        }

        public void AddButton(int index, string text)
        {
            if (!buttonMap.ContainsKey(index))
                buttonMap.Add(index, new List<string>());

            buttonMap[index].Add(text);

            buttonCols = Math.Max(buttonCols, buttonMap[index].Count);
        }

        public void RemoveButton(int index, int buttonIndex)
        {
            buttonMap[index].RemoveAt(buttonIndex);

            buttonCols = 0;
            foreach (var buttons in buttonMap.Values)
            {
                buttonCols = Math.Max(buttonCols, buttons.Count);
            }
        }

        public void RemoveButtons(int index)
        {
            buttonMap.Remove(index);

            buttonCols = 0;
            foreach (var buttons in buttonMap.Values)
            {
                buttonCols = Math.Max(buttonCols, buttons.Count);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (Items.Count == 0) return;

            if (buttonMap.TryGetValue(e.Index, out var buttons))
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    Rectangle bounds = GetButtonBounds(e.Index, i);
                    System.Windows.Forms.VisualStyles.PushButtonState state = System.Windows.Forms.VisualStyles.PushButtonState.Normal;
                    if (hoverItemIndex == e.Index && hoverButtonIndex == i)
                    {
                        if (mouseDown)
                            state = System.Windows.Forms.VisualStyles.PushButtonState.Pressed;
                        else
                            state = System.Windows.Forms.VisualStyles.PushButtonState.Hot;
                    }
                    ButtonRenderer.DrawButton(e.Graphics, bounds, state);
                    if (string.IsNullOrEmpty(buttons[i]))
                        e.Graphics.FillEllipse(SystemBrushes.WindowText, bounds.Left + (bounds.Width - dotSize) / 2, bounds.Top + (bounds.Height - dotSize) / 2, dotSize, dotSize);
                    else
                        TextRenderer.DrawText(e.Graphics, buttons[i], Font, bounds, SystemColors.WindowText, TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        private Rectangle GetButtonBounds(int itemIndex, int buttonIndex)
        {
            Rectangle bounds = this.GetItemRectangle(itemIndex);
            int buttonLeft = 0;
            if (ButtonAlignment == ButtonAlignment.Left)
            {
                buttonLeft = bounds.Right - buttonWidth * (buttonCols - buttonIndex) - buttonMargin * (buttonCols - buttonIndex - 1);
            }
            else
            {
                var buttons = buttonMap[itemIndex];
                buttonLeft = bounds.Right - buttonWidth * (buttons.Count - buttonIndex) - buttonMargin * (buttons.Count - buttonIndex - 1);
            }
            return new Rectangle(buttonLeft, bounds.Top, buttonWidth, bounds.Height);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int oldHoverItemIndex = hoverItemIndex;
            int oldHoverButtonIndex = hoverButtonIndex;
            hoverItemIndex = IndexFromPoint(e.Location);
            hoverButtonIndex = -1;
            if (hoverItemIndex != -1 && buttonMap.TryGetValue(hoverItemIndex, out var buttons))
            {
                if (buttons.Count != 0)
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        Rectangle bounds = GetButtonBounds(hoverItemIndex, i);

                        if (bounds.Contains(e.Location))
                        {
                            hoverButtonIndex = i;
                            break;
                        }
                    }
                }
            }

            if (hoverItemIndex != oldHoverItemIndex || hoverButtonIndex != oldHoverButtonIndex)
                Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (hoverItemIndex != -1 && hoverButtonIndex != -1 && (e.Button & MouseButtons.Left) != MouseButtons.None)
            {
                mouseDown = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if ((e.Button & MouseButtons.Left) != MouseButtons.None)
            {
                mouseDown = false;
                Invalidate();

                if (hoverItemIndex != -1 && hoverButtonIndex != -1)
                {
                    OnButtonClick(new ButtonClickEventArgs(hoverItemIndex, hoverButtonIndex, buttonMap[hoverItemIndex][hoverButtonIndex]));
                }
            }
        }

        protected void OnButtonClick(ButtonClickEventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs e)
        {
            if (hoverButtonIndex != -1)
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
                    Rectangle irect = this.GetItemRectangle(i);
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
            public int ItemIndex { get; private set; }
            public int ButtonIndex { get; private set; }
            public string ButtonText { get; private set; }

            public ButtonClickEventArgs(int itemIndex, int buttonIndex, string buttonText)
            {
                ItemIndex = itemIndex;
                ButtonIndex = buttonIndex;
                ButtonText = buttonText;
            }
        }
    }
}
