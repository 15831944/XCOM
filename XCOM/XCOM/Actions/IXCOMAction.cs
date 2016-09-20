using System;
using Autodesk.AutoCAD.DatabaseServices;

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
}
