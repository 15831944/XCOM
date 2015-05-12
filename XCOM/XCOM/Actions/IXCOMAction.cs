using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM
{
    [Flags]
    public enum ActionInterface
    {
        None = 0,
        Command = 1,
        Dialog = 2,
        Both = Command | Dialog
    }

    public interface IXCOMAction
    {
        string Name { get; }
        int Order { get; }
        bool Recommended { get; }
        ActionInterface Interface { get; }
        bool ShowDialog();

        string[] Run(string filename, Database db);
    }
}
