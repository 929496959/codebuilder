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

            FillGroups(lstPart.Items, Template.Groups);
            FillPartitions(lstPart.Items, Template.Partitions);

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
            GetPartitions(lstPart.Items);

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

        private void GetPartitions(TreeListItemCollection items)
        {
            foreach (var item in items)
            {
                if (item.Tag is PartitionDefinition && item.Checked)
                {
                    Partitions.Add(item.Tag as PartitionDefinition);
                }

                GetPartitions(item.Items);
            }
        }

        private void FillGroups(TreeListItemCollection items, List<GroupDefinition> groups)
        {
            foreach (var group in groups)
            {
                var item = new TreeListItem(group.Name);
                item.Checked = true;
                item.Image = Properties.Resources.category;
                items.Add(item);

                FillGroups(item.Items, group.Groups);
                FillPartitions(item.Items, group.Partitions);

                item.Expended = true;
            }
        }

        private void FillPartitions(TreeListItemCollection items, List<PartitionDefinition> partitions)
        {
            foreach (var part in partitions)
            {
                var item = new TreeListItem(part.Name);
                item.Checked = true;
                item.Image = Properties.Resources.file;
                item.Tag = part;
                items.Add(item);
                item.Cells[1].Value = part.Output;
            }
        }
    }
}
