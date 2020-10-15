using CodeBuilder.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeBuilder
{
    public class DockForm : WeifenLuo.WinFormsUI.Docking.DockContent, IFontApplyer
    {
        public Action CloseAct { get; set; }

        protected override void OnClosed(EventArgs e)
        {
            if (CloseAct != null)
            {
                CloseAct();
            }

            base.OnClosed(e);
        }
    }
}
