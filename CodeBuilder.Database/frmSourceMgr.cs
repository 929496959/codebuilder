// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using Fireasy.Common.Serialization;
using Fireasy.Data.Provider;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CodeBuilder.Database
{
    public partial class frmSourceMgr : Form
    {
        private string sourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "datasources.cfg");
        private List<DbSourceStruct> sources = null;
        
        public frmSourceMgr()
        {
            InitializeComponent();
            Icon = Util.GetIcon();
        }

        public DbSourceStruct DbConStr { get; private set; }

        private void frmSourceMgr_Load(object sender, EventArgs e)
        {
            LoadProviderTypes();
            LoadDataSources();
        }

        private void LoadProviderTypes()
        {
            foreach (var p in ProviderHelper.GetSupportedProviders())
            {
                tbtnAdd.DropDownItems.Add(new ToolStripMenuItem(p));
            }
        }

        private void LoadDataSources()
        {
            var json = new JsonSerializer();
            var content = File.ReadAllText(sourceFileName, Encoding.Default);
            sources = json.Deserialize<List<DbSourceStruct>>(content);

            lstProvider.Items.Clear();

            foreach (var s in sources)
            {
                var t = new TreeListItem();
                lstProvider.Items.Add(t);
                t.Group = s.Type;
                t.ImageIndex = 0;
                t.Tag = s;
                t.Cells[0].Value = s.Name;
                t.Cells[1].Value = s.ConnectionString;
            }

            lstProvider.Grouping(true);
        }

        private void SaveDataSources()
        {
            var option = new JsonSerializeOption { Indent = true };
            var json = new JsonSerializer(option);
            var content = json.Serialize(sources);
            File.WriteAllText(sourceFileName, content, Encoding.Default);
        }

        private void tbtnTest_Click(object sender, EventArgs e)
        {
            var item = lstProvider.SelectedItems[0];
            var provider = ProviderHelper.GetDefinedProviderInstance(item.Group);
            if (provider == null)
            {
                MessageBoxHelper.ShowError(string.Format("没有发现 {0} 的数据库适配器。", item.Group));
            }

            using (var db = new Fireasy.Data.Database(item.Cells[1].Value.ToString(), provider))
            {
                try
                {
                    var exp = db.TryConnect();
                    if (exp == null)
                    {
                        MessageBoxHelper.ShowInformation(string.Format("{0} 连接成功。", item.Text));
                    }
                    else
                    {
                        throw exp;
                    }
                }
                catch (Exception exp)
                {
                    MessageBoxHelper.ShowError(string.Format("{0} 连接失败。详细信息如下：\n\n{1}", item.Text, exp.Message));
                }
            }
        }

        private void tbtnAdd_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var source = new DbSourceStruct { Type = e.ClickedItem.Text };
            using (var frm = new frmSourceEdit(source))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var item = new TreeListItem();
                    lstProvider.Items.Add(item);
                    item.Group = source.Type;
                    item.ImageIndex = 0;
                    item.Tag = source;
                    item.Cells[0].Value = source.Name;
                    item.Cells[1].Value = source.ConnectionString;

                    sources.Add(source);
                    SaveDataSources();

                    lstProvider.Grouping(true);
                }
            }
        }

        private void tbtnEdit_Click(object sender, EventArgs e)
        {
            if (lstProvider.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstProvider.SelectedItems[0];
            var source = item.Tag as DbSourceStruct;

            using (var frm = new frmSourceEdit(source))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    item.Cells[0].Value = source.Name;
                    item.Cells[1].Value = source.ConnectionString;

                    SaveDataSources();
                }
            }
        }

        private void tbtnDelete_Click(object sender, EventArgs e)
        {
            if (lstProvider.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstProvider.SelectedItems[0];

            if (MessageBoxHelper.ShowQuestion("确认删除 " + item.Text + "吗?") == DialogResult.No)
            {
                return;
            }

            var source = item.Tag as DbSourceStruct;

            lstProvider.Items.Remove(item);
            sources.Remove(source);

            SaveDataSources();
            lstProvider.Grouping(true);
        }

        private void tbtnSelect_Click(object sender, EventArgs e)
        {
            if (lstProvider.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstProvider.SelectedItems[0];
            DbConStr = new DbSourceStruct { Type = item.Group, ConnectionString = item.Cells[1].Value.ToString() };
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void lstProvider_DoubleClick(object sender, EventArgs e)
        {
            tbtnSelect_Click(null, null);
        }

        private void lstProvider_ItemSelectionChanged(object sender, TreeListItemSelectionEventArgs e)
        {
            var isSelected = lstProvider.SelectedItems.Count > 0;
            tbtnEdit.Enabled = tbtnSelect.Enabled = tbtnDelete.Enabled = tbtnTest.Enabled = isSelected;
        }
    }
}
