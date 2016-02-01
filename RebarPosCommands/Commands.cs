using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.Linq;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(RebarPosCommands.MyCommands))]

namespace RebarPosCommands
{
    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public partial class MyCommands
    {
        public static string DeveloperSymbol { get { return "OZOZ"; } }
        public static string LicensedAppName { get { return "XCOM Bundle"; } }

        public static string RegAppName
        {
            get
            {
                return DeveloperSymbol + "_" + System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string LicenseRegistryKey
        {
            get
            {
                return "SOFTWARE\\" + DeveloperSymbol + "\\" + LicensedAppName;
            }
        }

        // The CommandMethod attribute can be applied to any public  member 
        // function of any public class.
        // The function should take no arguments and return nothing.
        // If the method is an instance member then the enclosing class is 
        // instantiated for each document. If the member is a static member then
        // the enclosing class is NOT instantiated.
        //
        // NOTE: CommandMethod has overloads where you can provide helpid and
        // context menu.
        public MyCommands()
        {
            // Monitor mouse cursor
            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ed.PointMonitor += new PointMonitorEventHandler(ed_PointMonitor);
            MonitoredPoint = Point3d.Origin;

            // Ensure that the pos block exists
            if (!RebarPos.EnsureBlockExists())
            {
                MessageBox.Show("Poz bloğu '" + RebarPos.BlockName + "' bulunamadı.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Shape overrule
            try
            {
                Overrule.AddOverrule(RXObject.GetClass(typeof(BlockReference)), ShowShapesOverrule.Instance, false);
                Overrule.Overruling = true;
            }
            catch
            {
                ;
            }

            // Load prompt
            string heading = "Donatı Pozlandırma ve Metraj Programı v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2) + " yüklendi.";
            ed.WriteMessage("\n");
            ed.WriteMessage(heading);
            ed.WriteMessage("\n");
            ed.WriteMessage(new string('=', heading.Length));
            PosCategories();

            // License information
            LicenseInformation();
        }

        public Point3d MonitoredPoint { get; private set; }

        [Category("Pozlandırma komutları")]
        [Description("Donatı pozlandırma ve metraj komutları.")]
        [CommandMethod("RebarPos", "POS", "POS_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_Pos()
        {
            if (!CheckLicense()) return;

            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            // Edit entity if there is a pickset
            PromptSelectionResult selectionRes = ed.SelectImplied();
            if (selectionRes.Status != PromptStatus.Error && selectionRes.Value.Count > 0)
            {
                ObjectId id = selectionRes.Value[0].ObjectId;
                ed.SetImpliedSelection(new ObjectId[0]);
                PosEdit(id, MonitoredPoint);
                return;
            }

            bool cont = true;
            while (cont)
            {
                PromptEntityOptions opts = new PromptEntityOptions("Poz secin veya [Yeni/Numaralandir/Kopyala/kOntrol/Metraj/bul Degistir/numara Sil/Acilimlar/ayaRlar]: ",
                    "New Numbering Copy Check BOQ Find Empty Shapes Preferences");
                opts.AllowNone = false;
                PromptEntityResult result = ed.GetEntity(opts);

                if (result.Status == PromptStatus.Keyword)
                {
                    switch (result.StringResult)
                    {
                        case "New":
                            NewPos();
                            break;
                        case "Numbering":
                            NumberPos();
                            break;
                        case "Empty":
                            EmptyBalloons();
                            break;
                        case "Copy":
                            CopyPos();
                            break;
                        case "Check":
                            PosCheck();
                            break;
                        case "BOQ":
                            DrawBOQ();
                            break;
                        case "Find":
                            FindReplace(false);
                            break;
                        case "Shapes":
                            PosShapes();
                            break;
                        case "Preferences":
                            ChangePosSettings();
                            break;
                    }
                    cont = false;
                }
                else if (result.Status == PromptStatus.OK)
                {
                    PosEdit(result.ObjectId, result.PickedPoint);
                    cont = true;
                }
                else
                {
                    cont = false;
                }
            }
        }

        void ed_PointMonitor(object sender, PointMonitorEventArgs e)
        {
            if (!e.Context.PointComputed)
                return;

            MonitoredPoint = e.Context.ComputedPoint;
        }

        [Category("Pozlandırma komutları")]
        [Description("Seçilen pozu ekranda düzenler.")]
        [CommandMethod("RebarPos", "POSEDIT", "POSEDIT_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_PosEdit()
        {
            if (!CheckLicense()) return;

            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            // Edit entity if there is a pickset
            PromptSelectionResult selectionRes = ed.SelectImplied();
            if (selectionRes.Status != PromptStatus.Error && selectionRes.Value.Count > 0)
            {
                ObjectId id = selectionRes.Value[0].ObjectId;
                ed.SetImpliedSelection(new ObjectId[0]);
                PosEdit(id, MonitoredPoint);
            }
            else
            {
                PromptEntityOptions opts = new PromptEntityOptions("Select entity: ");
                opts.AllowNone = false;
                PromptEntityResult result = ed.GetEntity(opts);
                if (result.Status == PromptStatus.OK)
                {
                    PosEdit(result.ObjectId, result.PickedPoint);
                }
            }
        }

        [Category("Pozlandırma komutları")]
        [Description("Çizime bir poz bloğu ekler.")]
        [CommandMethod("RebarPos", "NEWPOS", "NEWPOS_Local", CommandFlags.Modal)]
        public void CMD_NewPos()
        {
            if (!CheckLicense()) return;

            NewPos();
        }

        [Category("Pozlandırma komutları")]
        [Description("Tum pozlara otomatik numara verir. Pozlar çap, şekil, ve boylarına gore gruplandırılır. Ayrıca seçilen kriterlere göre sıralandırılır.")]
        [CommandMethod("RebarPos", "NUMBERPOS", "NUMBERPOS_Local", CommandFlags.Modal)]
        public void CMD_NumberPos()
        {
            if (!CheckLicense()) return;

            NumberPos();
        }

        [Category("Pozlandırma komutları")]
        [Description("Poz balonlarını siler.")]
        [CommandMethod("RebarPos", "EMPTYPOS", "EMPTYPOS_Local", CommandFlags.Modal)]
        public void CMD_EmptyBalloons()
        {
            if (!CheckLicense()) return;

            EmptyBalloons();
        }

        [Category("Pozlandırma komutları")]
        [Description("Pozları kontrol eder.")]
        [CommandMethod("RebarPos", "POSCHECK", "POSCHECK_Local", CommandFlags.Modal)]
        public void CMD_PosCheck()
        {
            if (!CheckLicense()) return;

            PosCheck();
        }

        [Category("Pozlandırma komutları")]
        [Description("Poz içeriğini diger pozlara kopyalar.")]
        [CommandMethod("RebarPos", "COPYPOS", "COPYPOS_Local", CommandFlags.Modal)]
        public void CMD_CopyPos()
        {
            if (!CheckLicense()) return;

            CopyPos();
        }

        [Category("Pozlandırma komutları")]
        [Description("Poz içeriğini diger pozlara kopyalar. Değiştirilen pozlar metraja dahil edilmez.")]
        [CommandMethod("RebarPos", "COPYPOSDETAIL", "COPYPOSDETAIL_Local", CommandFlags.Modal)]
        public void CMD_CopyPosDetail()
        {
            if (!CheckLicense()) return;

            CopyPosDetail();
        }

        [Category("Pozlandırma komutları")]
        [Description("Seçilen textlere poz numarası yazar.")]
        [CommandMethod("RebarPos", "COPYPOSNUMBER", "COPYPOSNUMBER_Local", CommandFlags.Modal)]
        public void CMD_CopyPosNumber()
        {
            if (!CheckLicense()) return;

            CopyPosNumber();
        }

        [Category("Pozlandırma komutları")]
        [Description("Verilen özelliklere sahip pozları seçer. Seçilen poz bloklarının özelliklerini bir seferde değiştirir.")]
        [CommandMethod("RebarPos", "POSFIND", "POSFIND_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_FindReplace()
        {
            if (!CheckLicense()) return;

            FindReplace(true);
        }

        [Category("Pozlandırma komutları")]
        [Description("Donatı açılımlarını düzenler.")]
        [CommandMethod("RebarPos", "POSSHAPES", "POSSHAPES_Local", CommandFlags.Modal)]
        public void CMD_PosShapes()
        {
            if (!CheckLicense()) return;

            PosShapes();
        }

        [Category("Pozlandırma komutları")]
        [Description("Donatı açılımlarını pozların üzerinde gösterir.")]
        [CommandMethod("RebarPos", "SHOWSHAPES", "SHOWSHAPES_Local", CommandFlags.Modal)]
        public void CMD_ShowShapes()
        {
            if (!CheckLicense()) return;

            ShowShapes(true);
        }

        [Category("Pozlandırma komutları")]
        [Description("Pozların üzerinde gösterilen tüm açılımları siler.")]
        [CommandMethod("RebarPos", "HIDESHAPES", "HIDESHAPES_Local", CommandFlags.Modal)]
        public void CMD_HideShapes()
        {
            if (!CheckLicense()) return;

            ShowShapes(false);
        }

        [Category("Pozlandırma komutları")]
        [Description("Poz boylarını gösterir veya saklar.")]
        [CommandMethod("RebarPos", "POSLENGTH", "POSLENGTH_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_PosLength()
        {
            if (!CheckLicense()) return;

            RebarPos.PromptRebarSelectionResult selresult = RebarPos.SelectAllPosUser();
            if (selresult.Status != PromptStatus.OK) return;

            PromptKeywordOptions opts = new PromptKeywordOptions("L boyunu [Göster/giZle]: ", "Show Hide");
            opts.AllowNone = false;
            PromptResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetKeywords(opts);

            if (result.Status == PromptStatus.OK)
            {
                switch (result.StringResult)
                {
                    case "Show":
                        ShowPosLength(selresult.Value.GetObjectIds(), true);
                        break;
                    case "Hide":
                        ShowPosLength(selresult.Value.GetObjectIds(), false);
                        break;
                }
            }
        }

        [Category("Pozlandırma komutları")]
        [Description("Pozların metraja dahil edilip edilmemesini düzenler.")]
        [CommandMethod("RebarPos", "INCLUDEPOS", "INCLUDEPOS_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_IncludePos()
        {
            if (!CheckLicense()) return;

            RebarPos.PromptRebarSelectionResult selresult = RebarPos.SelectAllPosUser();
            if (selresult.Status != PromptStatus.OK) return;

            PromptKeywordOptions opts = new PromptKeywordOptions("Metraja [Dahil et/metrajdan Cikar]: ", "Add Remove");
            opts.AllowNone = false;
            PromptResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetKeywords(opts);

            if (result.Status == PromptStatus.OK)
            {
                switch (result.StringResult)
                {
                    case "Add":
                        IncludePosInBOQ(selresult.Value.GetObjectIds(), true);
                        break;
                    case "Remove":
                        IncludePosInBOQ(selresult.Value.GetObjectIds(), false);
                        break;
                }
            }
        }

        [Category("Pozlandırma komutları")]
        [Description("Son poz numarasını gösterir.")]
        [CommandMethod("RebarPos", "LASTPOSNUMBER", "LASTPOSNUMBER_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_LastPosNumber()
        {
            if (!CheckLicense()) return;

            RebarPos.PromptRebarSelectionResult sel = RebarPos.SelectAllPosUser();
            if (sel.Status != PromptStatus.OK) return;
            ObjectId[] items = sel.Value.GetObjectIds();

            int lastNum = GetLastPosNumber(items);

            if (lastNum != -1)
            {
                MessageBox.Show("Son poz numarası: " + lastNum.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        [Category("Pozlandırma komutları")]
        [Description("Poz blokları için mirror komutu.")]
        [CommandMethod("RebarPos", "POSMIRROR", "POSMIRROR_Local", CommandFlags.Modal)]
        public void CMD_PosMirror()
        {
            if (!CheckLicense()) return;

            ;
        }

        [Category("Metraj komutları")]
        [Description("Seçilen pozların metrajını yapar.")]
        [CommandMethod("RebarPos", "BOQ", "BOQ_Local", CommandFlags.Modal | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void CMD_DrawBOQ()
        {
            if (!CheckLicense()) return;

            DrawBOQ();
        }

        [Category("Metraj komutları")]
        [Description("Metraj tablolarını siler.")]
        [CommandMethod("RebarPos", "DELETEBOQ", "DELETEBOQ_Local", CommandFlags.Modal)]
        public void CMD_DeleteBOQ()
        {
            if (!CheckLicense()) return;

            DeleteBOQ();
        }

        [Category("Diğer komutlar")]
        [Description("Pozlandırma ayarlarını değiştirir.")]
        [CommandMethod("RebarPos", "POSSETTINGS", "POSSETTINGS_Local", CommandFlags.Modal)]
        public void CMD_PosSettings()
        {
            if (!CheckLicense()) return;

            ChangePosSettings();
        }

        [Category("Diğer komutlar")]
        [Description("Poz bloğu değişikliklerini çizime uygular.")]
        [CommandMethod("RebarPos", "UPGRADEPOSBLOCKS", "UPGRADEPOSBLOCKS_Local", CommandFlags.Modal)]
        public void CMD_UpgradePosBlocks()
        {
            if (!CheckLicense()) return;

            ;
        }

        [Category("Diğer komutlar")]
        [Description("Lisans bilgilerini gosterir.")]
        [CommandMethod("RebarPos", "POSLICENSE", "POSLICENSE_Local", CommandFlags.Modal)]
        public void CMD_PosLicense()
        {
            LicenseInformation();
        }

        [Category("Diğer komutlar")]
        [Description("Donatı pozlandırma ve metraj komutlarının açıklamalarını gösterir.")]
        [CommandMethod("RebarPos", "POSHELP", "POSHELP_Local", CommandFlags.Modal)]
        public void CMD_PosHelp()
        {
            PosHelp();
        }
    }
}
