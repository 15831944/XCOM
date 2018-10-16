using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace AcadUtility.WinForms
{
    internal class PickCoordinateControlDesigner : ControlDesigner
    {
        private Adorner Adorner { get; set; }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            Adorner = new Adorner();
            BehaviorService.Adorners.Add(Adorner);
        }

        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            var coll = base.GetGlyphs(selectionType);
            if (selectionType != GlyphSelectionType.NotSelected)
                coll.Add(new VerticalDividerGlyph(BehaviorService, Adorner, (PickCoordinateControl)Control));
            return coll;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Adorner != null)
            {
                BehaviorService b = BehaviorService;
                if (b != null)
                {
                    b.Adorners.Remove(Adorner);
                }
            }

            base.Dispose(disposing);
        }

        public override SelectionRules SelectionRules => SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible;

        public override IList SnapLines
        {
            get
            {
                var snapLines = base.SnapLines;
                snapLines.Add(new SnapLine(SnapLineType.Left, ((PickCoordinateControl)Control).DividerLocation));
                return snapLines;
            }
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            string[] propertiesToHide = { "Text" };

            foreach (string propname in propertiesToHide)
            {
                var prop = (PropertyDescriptor)properties[propname];
                if (prop != null)
                {
                    // Add Browsable(false) to attributes
                    Attribute[] attributes = new Attribute[prop.Attributes.Count + 1];
                    prop.Attributes.CopyTo(attributes, 0);
                    attributes[attributes.Length - 1] = new BrowsableAttribute(false);

                    prop = TypeDescriptor.CreateProperty(this.GetType(), propname, prop.PropertyType, attributes);
                    properties[propname] = prop;
                }
            }
        }
    }

    internal class VerticalDividerGlyph : Glyph
    {
        private int location;
        private bool visible;

        public PickCoordinateControl Control { get; set; }
        public BehaviorService BehaviorService { get; set; }
        public Adorner Adorner { get; set; }
        private int Width { get; set; }

        public int Location
        {
            get => location;
            set
            {
                Adorner.Invalidate(Bounds);
                location = Math.Min(Control.ClientRectangle.Width - 20, Math.Max(20, value));
                Adorner.Invalidate(Bounds);
            }
        }

        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                Adorner.Invalidate(Bounds);
            }
        }

        public VerticalDividerGlyph(BehaviorService behaviorService, Adorner adorner, PickCoordinateControl control) : base(new VerticalDividerBehavior())
        {
            this.Control = control;
            this.BehaviorService = behaviorService;
            this.Adorner = adorner;
            this.Width = 5;

            this.Location = control.DividerLocation;
            this.Visible = false;
        }

        public override Cursor GetHitTest(Point p)
        {
            if (Bounds.Contains(p) || ((VerticalDividerBehavior)Behavior).DraggingGlyph)
            {
                return Cursors.VSplit;
            }

            return null;
        }

        public override void Paint(PaintEventArgs pe)
        {
            if (Visible)
            {
                using (Brush brush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent50, Color.Black, SystemColors.Control))
                {
                    pe.Graphics.FillRectangle(brush, Bounds);
                }
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                Point point = BehaviorService.ControlToAdornerWindow(Control);
                int height = Control.ClientRectangle.Height;

                return new Rectangle(point.X + Location - Width / 2, point.Y, Width, height);
            }
        }

        public void UpdateControl()
        {
            Control.DividerLocation = Location;
        }
    }

    internal class VerticalDividerBehavior : Behavior
    {
        public bool DraggingGlyph { get; set; } = false;
        private Point lastLocation;

        public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
        {
            if (button == MouseButtons.Left)
            {
                VerticalDividerGlyph vg = (VerticalDividerGlyph)g;
                vg.Visible = true;
                DraggingGlyph = true;
                lastLocation = mouseLoc;
                return true;
            }

            return false;
        }

        public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
        {
            if (DraggingGlyph)
            {
                VerticalDividerGlyph vg = (VerticalDividerGlyph)g;
                vg.Location = vg.Location + mouseLoc.X - lastLocation.X;
                lastLocation = mouseLoc;
                return true;
            }

            return false;
        }

        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            if (DraggingGlyph)
            {
                VerticalDividerGlyph vg = (VerticalDividerGlyph)g;
                vg.Visible = false;
                vg.UpdateControl();
                DraggingGlyph = false;
                return true;
            }

            return false;
        }
    }
}
