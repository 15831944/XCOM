using System;
using System.Collections.Generic;
using System.Drawing;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsSystem;
using Autodesk.AutoCAD.GraphicsInterface;

namespace RebarPosCommands
{
    public class DrawingPreview
    {
        private static DrawingPreview instance = null;

        private Autodesk.AutoCAD.GraphicsSystem.Manager gsm = null;
        private Autodesk.AutoCAD.GraphicsSystem.Device device = null;
        private Autodesk.AutoCAD.GraphicsSystem.View view = null;
        private Autodesk.AutoCAD.GraphicsSystem.Model model = null;

        public static DrawingPreview Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DrawingPreview();
                }
                return instance;
            }
        }

        private DrawingPreview()
        {
            gsm = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.GraphicsManager;

            KernelDescriptor descriptor = new KernelDescriptor();
            descriptor.addRequirement(Autodesk.AutoCAD.UniqueString.Intern("3D Drawing"));
            GraphicsKernel kernel = Manager.AcquireGraphicsKernel(descriptor);

            view = new Autodesk.AutoCAD.GraphicsSystem.View();

            device = gsm.CreateAutoCADOffScreenDevice(kernel);
            device.DeviceRenderType = RendererType.Default;
            device.BackgroundColor = Color.Black;
            device.Add(view);
            device.Update();

            model = gsm.CreateAutoCADModel(kernel);
        }

        public System.Drawing.Image SnapShot(IEnumerable<Drawable> items, Extents3d extents, Size sz, Color backColor)
        {
            device.BackgroundColor = backColor;
            device.OnSize(sz);

            view.EraseAll();
            foreach (Drawable item in items)
            {
                view.Add(item, model);
            }

            Point3d center = new Point3d((extents.MinPoint.X + extents.MaxPoint.X) / 2.0, (extents.MinPoint.Y + extents.MaxPoint.Y) / 2.0, 0.0);
            view.SetView(center + Vector3d.ZAxis,
                center,
                Vector3d.YAxis,
                1.04 * Math.Abs(extents.MaxPoint.X - extents.MinPoint.X),
                1.04 * Math.Abs(extents.MaxPoint.Y - extents.MinPoint.Y));
            
            device.Update();

            return view.GetSnapshot(new Rectangle(0, 0, sz.Width, sz.Height));
        }

        public System.Drawing.Image SnapShot(IEnumerable<Drawable> items, Size sz, Color backColor)
        {
            Extents3d extents = new Extents3d();
            foreach (Drawable item in items)
            {
                Extents3d? itemExtents = item.Bounds;
                if (itemExtents.HasValue)
                {
                    extents.AddExtents(itemExtents.Value);
                }
            }

            return SnapShot(items, extents, sz, backColor);
        }
    }
}
