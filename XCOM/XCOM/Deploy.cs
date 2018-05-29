using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;

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
        public System.Exception Error { get; private set; }

        public FileOpenedEventArgs(string filename, System.Exception error)
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
                    System.Exception openError = null;
                    try
                    {
                        db.ReadDwgFile(file, FileShare.ReadWrite, true, String.Empty);
                        db.RetainOriginalThumbnailBitmap = true;
                    }
                    catch (System.Exception ex)
                    {
                        openError = ex;
                    }
                    OnFileOpened(file, openError);
                    // Close without any action if there was an error while opening the file
                    if (openError != null)
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
            DeployStarted?.Invoke(this, new EventArgs());
        }

        protected virtual void OnFileStarted(string filename)
        {
            FileStarted?.Invoke(this, new FileEventArgs(filename));
        }

        protected virtual void OnFileOpened(string filename, System.Exception error)
        {
            FileOpened?.Invoke(this, new FileOpenedEventArgs(filename, error));
        }

        protected virtual void OnActionStarted(string filename, string actionName)
        {
            ActionStarted?.Invoke(this, new ActionEventArgs(filename, actionName));
        }

        protected virtual void OnActionError(string filename, string actionName, System.Exception error)
        {
            ActionError?.Invoke(this, new XCOMActionErrorEventArgs(filename, actionName, error));
        }

        protected virtual void OnActionProgress(string filename, string actionName, string message)
        {
            ActionProgress?.Invoke(this, new XCOMActionProgressEventArgs(filename, actionName, message));
        }

        protected virtual void OnActionCompleted(string filename, string actionName)
        {
            ActionCompleted?.Invoke(this, new ActionEventArgs(filename, actionName));
        }

        protected virtual void OnFileCompleted(string filename)
        {
            FileCompleted?.Invoke(this, new FileEventArgs(filename));
        }

        protected virtual void OnDeployCompleted()
        {
            DeployCompleted?.Invoke(this, new EventArgs());
        }
    }
}