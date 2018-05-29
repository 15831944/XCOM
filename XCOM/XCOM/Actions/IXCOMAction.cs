using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    [Flags]
    public enum ActionInterface
    {
        None = 0,
        Command = 1,
        Dialog = 2,
        Both = Command | Dialog
    }

    public class ActionProgressEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public ActionProgressEventArgs(string message)
        {
            Message = message;
        }
    }

    public class ActionErrorEventArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public ActionErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }

    public interface IXCOMAction
    {
        string Name { get; }
        int Order { get; }
        bool Recommended { get; }
        ActionInterface Interface { get; }
        bool ShowDialog();

        void Run(string filename, Database db);

        event EventHandler<ActionProgressEventArgs> Progress;
        event EventHandler<ActionErrorEventArgs> Error;
    }

    public abstract class XCOMActionBase : IXCOMAction
    {
        public abstract string Name { get; }
        public abstract int Order { get; }
        public virtual bool Recommended { get { return false; } }
        public virtual ActionInterface Interface { get { return ActionInterface.Command; } }
        public virtual bool ShowDialog() { return true; }

        public override string ToString()
        {
            return Name;
        }

        public virtual void Run(string filename, Database db) { throw new NotImplementedException(); }

        public event EventHandler<ActionProgressEventArgs> Progress;
        public event EventHandler<ActionErrorEventArgs> Error;

        protected virtual void OnProgress(string message)
        {
            Progress?.Invoke(this, new ActionProgressEventArgs(message));
        }

        protected virtual void OnError(Exception error)
        {
            Error?.Invoke(this, new ActionErrorEventArgs(error));
        }
    }
}
