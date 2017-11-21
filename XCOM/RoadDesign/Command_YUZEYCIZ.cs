using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;

namespace XCOM.Commands.RoadDesign
{
    public class Command_YUZEYCIZ
    {
        List<Autodesk.AutoCAD.GraphicsInterface.Drawable> temporaryGraphics;

        public Command_YUZEYCIZ()
        {
            temporaryGraphics = new List<Autodesk.AutoCAD.GraphicsInterface.Drawable>();

            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CommandWillStart += MdiActiveDocument_CommandWillStart;
        }

        void MdiActiveDocument_CommandWillStart(object sender, Autodesk.AutoCAD.ApplicationServices.CommandEventArgs e)
        {
            if (string.Compare(e.GlobalCommandName, "REGEN") == 0)
            {
                EraseTemporaryGraphics();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYCIZ")]
        public void DrawSurface()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography topo = Topography.Instance;
            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!Topography.EnsureSurfaceNotEmpty(surface)) return;
            TriangleNet.Mesh mesh = (surface == Topography.SurfaceType.Original ? topo.OriginalTIN : topo.ProposedTIN);

            // Object type
            PromptKeywordOptions opts = new PromptKeywordOptions("\nÇizim nesneleri [Geçici/3dFace/Point/polyfaceMesh] <Geçici>: ", "Temporary 3dFace Point polyfaceMesh");
            opts.Keywords.Default = "Temporary";
            opts.AllowNone = true;
            PromptResult res = doc.Editor.GetKeywords(opts);
            string outputType = res.StringResult;
            if (res.Status == PromptStatus.None)
            {
                outputType = "Temporary";
            }
            else if (res.Status != PromptStatus.OK)
            {
                return;
            }

            // Color option
            opts = new PromptKeywordOptions("\nRenkli çizim [E/H] <E>: ", "Yes No");
            opts.Keywords.Default = "Yes";
            opts.AllowNone = true;
            res = doc.Editor.GetKeywords(opts);
            bool useColor = true;
            if (res.Status == PromptStatus.None)
            {
                useColor = true;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return;
            }
            else
            {
                useColor = (string.Compare(res.StringResult, "Yes", StringComparison.OrdinalIgnoreCase) == 0);
            }
            double zmin = double.MaxValue; double zmax = double.MinValue;
            foreach (TriangleNet.Data.Vertex v in mesh.Vertices)
            {
                zmin = Math.Min(zmin, v.Attributes[0]);
                zmax = Math.Max(zmax, v.Attributes[0]);
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                try
                {
                    if (string.Compare(outputType, "Temporary", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        EraseTemporaryGraphics();
                        foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                        {
                            TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                            TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                            TriangleNet.Data.Vertex v3 = tri.GetVertex(2);
                            Face face = new Face(new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]), true, true, true, true);
                            if (useColor)
                            {
                                face.Color = ColorFromZ((v1.Attributes[0] + v2.Attributes[0] + v3.Attributes[0]) / 3, zmin, zmax);
                            }
                            temporaryGraphics.Add(face);
                        }
                        DrawTemporaryGraphics();
                    }
                    else if (string.Compare(outputType, "3dFace", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                        {
                            TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                            TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                            TriangleNet.Data.Vertex v3 = tri.GetVertex(2);
                            Face face = new Face(new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]), true, true, true, true);
                            if (useColor)
                            {
                                face.Color = ColorFromZ((v1.Attributes[0] + v2.Attributes[0] + v3.Attributes[0]) / 3, zmin, zmax);
                            }

                            btr.AppendEntity(face);
                            tr.AddNewlyCreatedDBObject(face, true);
                        }
                    }
                    else if (string.Compare(outputType, "Point", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        foreach (TriangleNet.Data.Vertex v in mesh.Vertices)
                        {
                            DBPoint point = new DBPoint(new Point3d(v.X, v.Y, v.Attributes[0]));
                            if (useColor)
                            {
                                point.Color = ColorFromZ(v.Attributes[0], zmin, zmax);
                            }

                            btr.AppendEntity(point);
                            tr.AddNewlyCreatedDBObject(point, true);
                        }
                    }
                    else if (string.Compare(outputType, "polyfaceMesh", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        PolyFaceMesh pfm = new PolyFaceMesh();
                        btr.AppendEntity(pfm);
                        tr.AddNewlyCreatedDBObject(pfm, true);

                        // Vertices
                        SortedDictionary<int, Point3d> vertices = new SortedDictionary<int, Point3d>();
                        foreach (TriangleNet.Data.Vertex v in mesh.Vertices)
                        {
                            vertices.Add(v.ID, new Point3d(v.X, v.Y, v.Attributes[0]));
                        }
                        foreach (KeyValuePair<int, Point3d> v in vertices)
                        {
                            PolyFaceMeshVertex point = new PolyFaceMeshVertex(v.Value);
                            pfm.AppendVertex(point);
                            tr.AddNewlyCreatedDBObject(point, true);
                        }

                        // Faces
                        foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                        {
                            FaceRecord face = new FaceRecord((short)(tri.P0 + 1), (short)(tri.P1 + 1), (short)(tri.P2 + 1), 0);
                            if (useColor)
                            {
                                face.Color = ColorFromZ((tri.GetVertex(0).Attributes[0] + tri.GetVertex(1).Attributes[0] + tri.GetVertex(2).Attributes[0]) / 3, zmin, zmax);
                            }
                            pfm.AppendFaceRecord(face);
                            tr.AddNewlyCreatedDBObject(face, true);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }

        private void DrawTemporaryGraphics()
        {
            Autodesk.AutoCAD.GraphicsInterface.TransientManager man = Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager;
            foreach (Autodesk.AutoCAD.GraphicsInterface.Drawable item in temporaryGraphics)
            {
                man.AddTransient(item, Autodesk.AutoCAD.GraphicsInterface.TransientDrawingMode.DirectShortTerm, 128, new IntegerCollection());
            }
        }

        private void EraseTemporaryGraphics()
        {
            Autodesk.AutoCAD.GraphicsInterface.TransientManager man = Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager;
            foreach (Autodesk.AutoCAD.GraphicsInterface.Drawable item in temporaryGraphics)
            {
                man.EraseTransient(item, new IntegerCollection());
                item.Dispose();
            }
            temporaryGraphics.Clear();
        }

        private Color ColorFromZ(double z, double zmin, double zmax)
        {
            Color maxColor = Color.FromRgb(224, 243, 219);
            Color midColor = Color.FromRgb(168, 221, 181);
            Color minColor = Color.FromRgb(67, 162, 202);

            if (zmax == zmin)
            {
                return minColor;
            }

            double ratio = (z - zmin) / (zmax - zmin);
            if (ratio > 0.5)
            {
                ratio = (ratio - 0.5) * 2;
                byte r = (byte)(ratio * (double)(maxColor.Red - midColor.Red) + (double)midColor.Red);
                byte g = (byte)(ratio * (double)(maxColor.Green - midColor.Green) + (double)midColor.Green);
                byte b = (byte)(ratio * (double)(maxColor.Blue - midColor.Blue) + (double)midColor.Blue);
                return Color.FromRgb(r, g, b);
            }
            else
            {
                ratio = ratio * 2;
                byte r = (byte)(ratio * (double)(midColor.Red - minColor.Red) + (double)minColor.Red);
                byte g = (byte)(ratio * (double)(midColor.Green - minColor.Green) + (double)minColor.Green);
                byte b = (byte)(ratio * (double)(midColor.Blue - minColor.Blue) + (double)minColor.Blue);
                return Color.FromRgb(r, g, b);
            }
        }
    }
}
