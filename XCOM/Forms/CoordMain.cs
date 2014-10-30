using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCOM.Forms
{
    public partial class CoordMain : Form
    {
        public CoordMain()
        {
            Text = "Koordinat v" + typeof(MainForm).Assembly.GetName().Version.ToString(2);

            InitializeComponent();
        }
    }
}
