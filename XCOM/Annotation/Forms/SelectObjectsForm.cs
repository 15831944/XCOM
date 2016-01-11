using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using System.Text.RegularExpressions;

namespace XCOM.Commands.Annotation
{
    public partial class SelectObjectsForm : XCOM.Utility.VersionDisplayForm
    {
        public enum SelectCoordinateObjects
        {
            Polyline,
            Circle,
            Block,
            Point,
            Line
        }

        public enum CoordinateOrdering
        {
            IncreasingX,
            DecreasingX,
            IncreasingY,
            DecreasingY,
        }

        public CoordinateOrdering Ordering
        {
            get
            {
                if (rbOrderXInc.Checked)
                    return CoordinateOrdering.IncreasingX;
                else if (rbOrderXDec.Checked)
                    return CoordinateOrdering.DecreasingX;
                else if (rbOrderYInc.Checked)
                    return CoordinateOrdering.IncreasingY;
                else // if (rbOrderYDec.Checked)
                    return CoordinateOrdering.DecreasingY;
            }
            set
            {
                switch (value)
                {
                    case CoordinateOrdering.IncreasingX:
                        rbOrderXInc.Checked = true;
                        break;
                    case CoordinateOrdering.DecreasingX:
                        rbOrderXDec.Checked = true;
                        break;
                    case CoordinateOrdering.IncreasingY:
                        rbOrderYInc.Checked = true;
                        break;
                    default: // CoordinateOrdering.DecreasingY
                        rbOrderYDec.Checked = true;
                        break;
                }
            }
        }

        public SelectCoordinateObjects SelectObjects
        {
            get
            {
                if (rbSelectPolyline.Checked)
                    return SelectObjectsForm.SelectCoordinateObjects.Polyline;
                else if (rbSelectCircle.Checked)
                    return SelectObjectsForm.SelectCoordinateObjects.Circle;
                else if (rbSelectBlock.Checked)
                    return SelectObjectsForm.SelectCoordinateObjects.Block;
                else if (rbSelectPoint.Checked)
                    return SelectObjectsForm.SelectCoordinateObjects.Point;
                else // if (rbSelectLine.Checked)
                    return SelectObjectsForm.SelectCoordinateObjects.Line;
            }
            set
            {
                switch (value)
                {
                    case SelectCoordinateObjects.Polyline:
                        rbSelectPolyline.Checked = true;
                        break;
                    case SelectCoordinateObjects.Circle:
                        rbSelectCircle.Checked = true;
                        break;
                    case SelectCoordinateObjects.Block:
                        rbSelectBlock.Checked = true;
                        break;
                    case SelectCoordinateObjects.Point:
                        rbSelectPoint.Checked = true;
                        break;
                    default: // SelectCoordinateObjects.Line
                        rbSelectLine.Checked = true;
                        break;
                }
            }
        }

        public SelectObjectsForm()
        {
            InitializeComponent();
        }
    }
}
