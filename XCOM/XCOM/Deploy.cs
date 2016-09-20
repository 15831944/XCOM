using System;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace XCOM.Commands.XCommand
{
    public class FileEventArgs : EventArgs
    {
        public string Filename { get; private set; }

        public FileEventArgs(string filename)
        {
            Filename = filename;
        }
    }

    public class FileOpenedEventArgs : EventArgs
    {
        public string Filename { get; private set; }
        public string Error { get; private set; }

        public FileOpenedEventArgs(string filename, string error)
        {
            Filename = filename;
            Error = error;
        }
    }

    public class ActionEventArgs : EventArgs
    {
        public string ActionName { get; private set; }
        public string Filename { get; private set; }

        public ActionEventArgs(string filename, string actionName)
        {
            ActionName = actionName;
            Filename = filename;
        }
    }

    public class XCOMActionErrorEventArgs : EventArgs
    {
        public string ActionName { get; private set; }
        public string Filename { get; private set; }
        public System.Exception Error { get; private set; }

        public XCOMActionErrorEventArgs(string filename, string actionName, System.Exception error)
        {
            Filename = filename;
            ActionName = actionName;
            Error = error;
        }
    }

    public class XCOMActionProgressEventArgs : EventArgs
    {
        public string ActionName { get; private set; }
        public string Filename { get; private set; }
        public string Message { get; private set; }

        public XCOMActionProgressEventArgs(string filename, string actionName, string message)
        {
            Filename = filename;
            ActionName = actionName;
            Message = message;
        }
    }

    public class Deploy
    {
        public event EventHandler DeployStarted;

        public event EventHandler<FileEventArgs> FileStarted;
        public event EventHandler<FileOpenedEventArgs> FileOpened;

        public event EventHandler<ActionEventArgs> ActionStarted;

        public event EventHandler<XCOMActionErrorEventArgs> ActionError;
        public event EventHandler<XCOMActionProgressEventArgs> ActionProgress;

        public event EventHandler<ActionEventArgs> ActionCompleted;

        public event EventHandler<FileEventArgs> FileCompleted;

        public event EventHandler DeployCompleted;

        private List<string> sourceFiles;
        private List<IXCOMAction> deployActions;

        public Deploy()
        {
            sourceFiles = new List<string>();
            deployActions = new List<IXCOMAction>();
        }

        public void AddFile(string filepath)
        {
            sourceFiles.Add(filepath);
        }

        public void AddFiles(IEnumerable<string> filepaths)
        {
            sourceFiles.AddRange(filepaths);
        }

        public void AddAction(IXCOMAction action)
        {
            deployActions.Add(action);
        }

        public void AddActions(IEnumerable<IXCOMAction> actions)
        {
            deployActions.AddRange(actions);
        }

        public void Run(Database db)
        {
            // Apply each action
            foreach (IXCOMAction action in deployActions)
            {
                action.Run(string.Empty, db);
            }
        }

        public void RunBatch()
        {
            List<List<string>> errors = new List<List<string>>();

            OnDeployStarted();

            // Process documents
            foreach (string file in sourceFiles)
            {
                OnFileStarted(file);

                // Open
                using (Database db = new Database(false, true))
                {
                    // Open the file into the empty database
                    string openError = string.Empty;
                    try
                    {
                        db.ReadDwgFile(file, FileShare.ReadWrite, true, String.Empty);
                        db.RetainOriginalThumbnailBitmap = true;
                    }
                    catch (System.Exception ex)
                    {
                        openError = ex.Message;
                    }
                    OnFileOpened(file, openError);
                    if (!string.IsNullOrEmpty(openError))
                    {
                        OnFileCompleted(file);
                        continue;
                    }

                    // Apply each action
                    foreach (IXCOMAction action in deployActions)
                    {
                        OnActionStarted(file, action.Name);
                        action.Error += (sender, e) =>
                        {
                            OnActionError(file, action.Name, e.Error);
                        };
                        action.Progress += (sender, e) =>
                        {
                            OnActionProgress(file, action.Name, e.Message);
                        };
                        action.Run(file, db);
                        OnActionCompleted(file, action.Name);
                    }

                }

                OnFileCompleted(file);
            }

            OnDeployCompleted();
        }

        protected virtual void OnDeployStarted()
        {
            if (DeployStarted != null)
            {
                DeployStarted(this, new EventArgs());
            }
        }

        protected virtual void OnFileStarted(string filename)
        {
            if (FileStarted != null)
            {
                FileStarted(this, new FileEventArgs(filename));
            }
        }

        protected virtual void OnFileOpened(string filename, string error)
        {
            if (FileOpened != null)
            {
                FileOpened(this, new FileOpenedEventArgs(filename, error));
            }
        }

        protected virtual void OnActionStarted(string filename, string actionName)
        {
            if (ActionStarted != null)
            {
                ActionStarted(this, new ActionEventArgs(filename, actionName));
            }
        }

        protected virtual void OnActionError(string filename, string actionName, System.Exception error)
        {
            if (ActionError != null)
            {
                ActionError(this, new XCOMActionErrorEventArgs(filename, actionName, error));
            }
        }

        protected virtual void OnActionProgress(string filename, string actionName, string message)
        {
            if (ActionProgress != null)
            {
                ActionProgress(this, new XCOMActionProgressEventArgs(filename, actionName, message));
            }
        }

        protected virtual void OnActionCompleted(string filename, string actionName)
        {
            if (ActionCompleted != null)
            {
                ActionCompleted(this, new ActionEventArgs(filename, actionName));
            }
        }

        protected virtual void OnFileCompleted(string filename)
        {
            if (FileCompleted != null)
            {
                FileCompleted(this, new FileEventArgs(filename));
            }
        }

        protected virtual void OnDeployCompleted()
        {
            if (DeployCompleted != null)
            {
                DeployCompleted(this, new EventArgs());
            }
        }
    }
}