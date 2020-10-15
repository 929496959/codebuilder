
using CodeBuilder.Core;
using System;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;
namespace CodeBuilder
{
    public class ClosableDockContent : WeifenLuo.WinFormsUI.Docking.DockContent, IFontApplyer
    {
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseCurrent;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseOther;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseCurrent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseOther = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCloseCurrent,
            this.mnuCloseAll,
            this.mnuCloseOther});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 92);
            // 
            // mnuCloseAll
            // 
            this.mnuCloseAll.Name = "mnuCloseAll";
            this.mnuCloseAll.Size = new System.Drawing.Size(172, 22);
            this.mnuCloseAll.Text = "关闭所有";
            this.mnuCloseAll.Click += new System.EventHandler(this.mnuCloseAll_Click);
            // 
            // mnuCloseCurrent
            // 
            this.mnuCloseCurrent.Name = "mnuCloseCurrent";
            this.mnuCloseCurrent.Size = new System.Drawing.Size(172, 22);
            this.mnuCloseCurrent.Text = "关闭当前";
            this.mnuCloseCurrent.Click += new System.EventHandler(this.mnuCloseCurrent_Click);
            // 
            // mnuCloseOther
            // 
            this.mnuCloseOther.Name = "mnuCloseOther";
            this.mnuCloseOther.Size = new System.Drawing.Size(172, 22);
            this.mnuCloseOther.Text = "除此之外全部关闭";
            this.mnuCloseOther.Click += new System.EventHandler(this.mnuCloseOther_Click);
            // 
            // ClosableDockContent
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "ClosableDockContent";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public ClosableDockContent()
        {
            InitializeComponent();
            TabPageContextMenuStrip = contextMenuStrip1;
        }

        private void mnuCloseAll_Click(object sender, System.EventArgs e)
        {
            var documents = DockPanel.DocumentsToArray();

            foreach (IDockContent content in documents)
            {
                if (content is ClosableDockContent)
                {
                    content.DockHandler.Close();
                }
            }
        }

        private void mnuCloseCurrent_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void mnuCloseOther_Click(object sender, System.EventArgs e)
        {
            var documents = DockPanel.DocumentsToArray();

            foreach (IDockContent content in documents)
            {
                if (content is ClosableDockContent && !content.Equals(this))
                {
                    content.DockHandler.Close();
                }
            }
        }
    }
}
