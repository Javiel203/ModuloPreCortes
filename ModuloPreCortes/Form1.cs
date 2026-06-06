using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IVisionPrecutDocs;

namespace ModuloPreCortes
{
    
    public partial class Form1 : Form
    {
        private PrecutDocs pcd = new PrecutDocs();
        public Form1()
        {
            InitializeComponent();
           
            this.Controls.Add(pcd);
            pcd.Dock = DockStyle.Fill;
            this.WindowState = FormWindowState.Maximized;

        }
    }
}
