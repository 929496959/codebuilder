using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmTip : Form
    {
        public frmTip()
        {
            InitializeComponent();
        }

        public static void Show(Control parent, Control control, string title, string message)
        {
            var frm = new frmTip();

            var p = parent.PointToScreen(control.Location);
            if (p.Y + control.Height > Screen.PrimaryScreen.Bounds.Height)
            {
                p.Y -= frm.Height;
            }
            else
            {
                p.Y += control.Height;
            }

            frm.label1.Text = title;
            frm.textBox1.Text = message;
            frm.Location = p;
            frm.Show();
        }

        private void frmTip_Load(object sender, EventArgs e)
        {

        }

        private void frmTip_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void frmTip_Paint(object sender, PaintEventArgs e)
        {
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(Pens.Gray, rect);
        }
    }
}
