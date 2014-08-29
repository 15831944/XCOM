using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

namespace XCOM
{
    public class CommandImplementation
    {
        public static void RunBatch()
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
                // Version
                deploy.Version = mainForm.DwgVersion;
                // Suffix
                deploy.Suffix = mainForm.FilenameSuffix;

                try
                {
                    ProgressForm progressForm = new ProgressForm();
                    Thread thread = new Thread(() => { Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(progressForm); });
                    thread.IsBackground = true;
                    thread.Start();

                    deploy.DeployStarted += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.Start(filenames, 2 + actions.Length);
                            }
                        ));
                    };

                    deploy.FileStarted += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.SetFile(e.Filename);
                            }
                        ));
                    };
                    deploy.FileOpened += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.SetAction(e.Filename);
                            }
                        ));
                    };
                    deploy.ActionCompleted += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.ActionComplete(e.Filename, e.ActionName, e.Errors);
                            }
                        ));
                    };
                    deploy.FileSaved += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.SetAction(e.Filename);
                            }
                        ));
                    };
                    deploy.FileCompleted += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.FileComplete(e.Filename);
                            }
                        ));
                    };
                    deploy.DeployCompleted += (sender, e) =>
                    {
                        progressForm.Invoke(new Action(
                            () =>
                            {
                                progressForm.Complete();
                            }
                        ));
                    };

                    // Start processing
                    deploy.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
