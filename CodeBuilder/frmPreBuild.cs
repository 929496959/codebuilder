// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using CodeBuilder.Core.Template;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmPreBuild : Form
    {
        public frmPreBuild()
        {
            InitializeComponent();
            Icon = Util.GetIcon();
        }

        public TemplateDefinition Template { get; set; }

        public List<PartitionDefinition> Partitions { get; private set; }

        private void frmPreBuild_Load(object sender, EventArgs e)
        {
            lstPart.BeginUpdate();

            foreach (var part in Template.Partitions)
            {
                var item = new TreeListItem(part.Name);
                item.Checked = true;
                item.Image = Properties.Resources.file;
                item.Tag = part;
                lstPart.Items.Add(item);
                item.Cells[1].Value = part.Output;
            }

            lstPart.EndUpdate();

            txtPath.Text = Config.Instance.OutputDirectory;
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择代码生成后输出的目录:";
                dialog.SelectedPath = txtPath.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtPath.Text.Length == 0)
            {
                MessageBoxHelper.ShowExclamation("请选择代码输出的目录。");
                return;
            }

            Partitions = new List<PartitionDefinition>();
            foreach (var item in lstPart.Items)
            {
                if (item.Checked)
                {
                    Partitions.Add(item.Tag as PartitionDefinition);
                }
            }

            if (Partitions.Count == 0)
            {
                MessageBoxHelper.ShowExclamation("至少要选择一个分部。");
                return;
            }

            if (Config.Instance.OutputDirectory != txtPath.Text)
            {
                Config.Instance.OutputDirectory = txtPath.Text;
                Config.Save();
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
