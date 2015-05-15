using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace XCOM.Commands.XCommand
{
    public class Command_XCOM
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("XCOM")]
        public static void XCOMBatch()
        {
            MainForm mainForm = new MainForm();
            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(mainForm) == System.Windows.Forms.DialogResult.OK)
            {
                // Read settings
                var deploy = new Deploy();
                // Add source files
                string[] filenames = mainForm.Filenames;
                deploy.AddFiles(filenames);
                // Add actions
                IXCOMAction[] actions = mainForm.SelectedActions;
                deploy.AddActions(actions);

                ProgressForm progressForm = new ProgressForm();
                Thread thread = new Thread(() => { Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(progressForm); });
                thread.IsBackground = true;
                thread.Start();

                deploy.DeployStarted += (sender, e) =>
                {
                    if (progressForm.InvokeRequired)
                        progressForm.Invoke(new Action(() => { progressForm.Start(filenames, 1 + actions.Length); }));
                    else
                        progressForm.Start(filenames, 3 + actions.Length);
                };

                deploy.FileStarted += (sender, e) =>
                {
                    if (progressForm.InvokeRequired)
                        progressForm.Invoke(new Action(() => { progressForm.StartFile(e.Filename); }));
                    else
                        progressForm.StartFile(e.Filename);
                };
                deploy.FileOpened += (sender, e) =>
                {
                    if (progressForm.InvokeRequired)
                        progressForm.Invoke(new Action(() => { progressForm.ActionComplete(e.Filename, "Aç", e.Error); }));
                    else
                        progressForm.ActionComplete(e.Filename, "Aç", e.Error);
                };
                deploy.ActionCompleted += (sender, e) =>
                {
                    if (progressForm.InvokeRequired)
                        progressForm.Invoke(new Action(() => { progressForm.ActionComplete(e.Filename, e.ActionName, e.Errors); }));
                    else
                        progressForm.ActionComplete(e.Filename, e.ActionName, e.Errors);
                };
                deploy.FileCompleted += (sender, e) =>
                {
                    if (progressForm.InvokeRequired)
                        progressForm.Invoke(new Action(() => { progressForm.FileComplete(e.Filename); }));
                    else
                        progressForm.FileComplete(e.Filename);
                };
                deploy.DeployCompleted += (sender, e) =>
                {
                    if (progressForm.InvokeRequired)
                        progressForm.Invoke(new Action(() => { progressForm.Complete(); }));
                    else
                        progressForm.Complete();
                };

                // Start processing
                deploy.RunBatch();
            }
        }
    }
}
