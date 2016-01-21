using System;
using System.Windows.Forms;

namespace RebarPosCommands
{
    public class DoubleBufferedFlowLayoutPanel : FlowLayoutPanel
    {
        private const int WM_SETREDRAW = 0x000B;

        public DoubleBufferedFlowLayoutPanel()
        {
            DoubleBuffered = true;
            this.MouseEnter += DoubleBufferedFlowLayoutPanel_MouseEnter;
        }

        void DoubleBufferedFlowLayoutPanel_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        public void Suspend()
        {
            this.SuspendLayout();

            SuspendControl(this);

            foreach (Control control in this.Controls)
            {
                SuspendControl(control);
            }
        }

        public void Resume()
        {
            foreach (Control control in this.Controls)
            {
                ResumeControl(control);
            }

            ResumeControl(this);
            this.ResumeLayout();
        }

        public static void SuspendControl(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void ResumeControl(Control control)
        {
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }
    }
}
