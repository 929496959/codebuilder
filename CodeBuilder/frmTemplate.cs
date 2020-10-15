using CodeBuilder.Core;
using CodeBuilder.Core.Template;
// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;

namespace CodeBuilder
{
    public partial class frmTemplate : DockForm
    {
        public frmTemplate()
        {
            InitializeComponent();
            Icon = Properties.Resources.template;
        }

        public Action<TemplateFile> OpenAct { get; set; }

        public Action TemplateAct { get; set; }

        public void Reload()
        {
            if (StaticUnity.Template == null)
            {
                return;
            }

            var storage = StaticUnity.TemplateProvider.GetStorage(StaticUnity.Template);

            lstPart.Items.Clear();
            lstPart.BeginUpdate();

            FillItems(lstPart.Items, storage.Directories, storage.Files);

            lstPart.EndUpdate();
        }

        private void FillItems(TreeListItemCollection items, List<TemplateDirectory> directories, List<TemplateFile> files)
        {
            foreach (var dir in directories)
            {
                var item = new TreeListItem(dir.Name);
                items.Add(item);

                item.Image = Properties.Resources.category;

                FillItems(item.Items, dir.Directories, dir.Files);

                item.Expended = true;
            }

            foreach (var file in files)
            {
                var item = new TreeListItem(file.Name);
                items.Add(item);
                item.Tag = file;
                item.Image = Properties.Resources.file;
            }
        }

        private void frmTemplate_Load(object sender, EventArgs e)
        {
            Reload();
        }

        private void lstPart_ItemDoubleClick(object sender, TreeListItemEventArgs e)
        {
            var file = e.Item.Tag as TemplateFile;
            if (file == null)
            {
                return;
            }

            if (OpenAct != null)
            {
                OpenAct(file);
            }
        }

        private void tlbNew_Click(object sender, EventArgs e)
        {
            using (var frm = new frmTemplateEditor())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TemplateAct();
                }
            }
        }

        private void tlbEdit_Click(object sender, EventArgs e)
        {
            if (StaticUnity.TemplateProvider == null || StaticUnity.Template == null)
            {
                MessageBoxHelper.ShowInformation("当前没有选择模板。");
            }

            using (var frm = new frmTemplateEditor { Template = StaticUnity.Template })
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TemplateAct();
                    Reload();
                }
            }
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (var frm = new frmTemplateCopy { Template = StaticUnity.Template })
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBoxHelper.ShowInformation("模板复制成功，请从主窗口【模板】菜单中选择。");
                    TemplateAct();
                }
            }
        }
    }
}
