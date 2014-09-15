using System;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace XCOMCore
{
    public delegate void DeployStartedEventHandler(object sender, EventArgs e);
    public delegate void DeployCompletedEventHandler(object sender, EventArgs e);

    public delegate void FileOpenedEventHandler(object sender, FileOpenedEventArgs e);
    public delegate void FileEventHandler(object sender, FileEventArgs e);

    public delegate void ActionStartedEventHandler(object sender, ActionStartedEventArgs e);
    public delegate void ActionCompletedEventHandler(object sender, ActionCompletedEventArgs e);

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

    public class ActionStartedEventArgs : EventArgs
    {
        public string ActionName { get; private set; }
        public string Filename { get; private set; }

        public ActionStartedEventArgs(string filename, string actionName)
        {
            ActionName = actionName;
            Filename = filename;
        }
    }

    public class ActionCompletedEventArgs : EventArgs
    {
        public string ActionName { get; private set; }
        public string Filename { get; private set; }
        public string[] Errors { get; private set; }

        public ActionCompletedEventArgs(string filename, string actionName, string[] errors)
        {
            ActionName = actionName;
            Filename = filename;
            Errors = errors;
        }
    }

    public class Deploy
    {
        public event DeployStartedEventHandler DeployStarted;

        public event FileEventHandler FileStarted;
        public event FileOpenedEventHandler FileOpened;

        public event ActionStartedEventHandler ActionStarted;
        public event ActionCompletedEventHandler ActionCompleted;

        public event FileEventHandler FileCompleted;

        public event DeployCompletedEventHandler DeployCompleted;

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
                        string[] actionErrors = action.Run(file, db);
                        OnActionCompleted(file, action.Name, actionErrors);

                        // Skip if there were errors
                        if (actionErrors.Length > 0)
                        {
                            OnFileCompleted(file);
                            continue;
                        }
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
                ActionStarted(this, new ActionStartedEventArgs(filename, actionName));
            }
        }
        protected virtual void OnActionCompleted(string filename, string actionName, string[] errors)
        {
            if (ActionCompleted != null)
            {
                ActionCompleted(this, new ActionCompletedEventArgs(filename, actionName, errors));
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