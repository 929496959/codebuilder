// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using CodeBuilder.Core.Source;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeBuilder.Database
{
    public partial class frmTableSelector : Form
    {
        private List<Table> tables;

        public frmTableSelector()
        {
            InitializeComponent();
            Icon = Util.GetIcon();
        }

        public frmTableSelector(IEnumerable<Table> tables)
            : this()
        {
            this.tables = tables.ToList();
            FillTables(string.Empty);
        }

        public List<Table> Selected { get; private set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            tables.Clear();
            base.OnClosing(e);
        }

        private void FillTables(string keyword)
        {
            lstTable.BeginUpdate();
            lstTable.Items.Clear();

            IEnumerable<Table> ts = tables;

            if (!string.IsNullOrEmpty(keyword))
            {
                ts = ts.Where(s => Regex.IsMatch(s.Name, keyword, RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(s.Description, keyword, RegexOptions.IgnoreCase));
            }

            foreach (var t in ts)
            {
                var item = new TreeListItem();
                item.Tag = t;
                item.Image = (t as Table).IsView ? Properties.Resources.view : Properties.Resources.table;
                lstTable.Items.Add(item);

                item.Cells[0].Value = t.Name;
                item.Cells[1].Value = t.Description;
            }

            lstTable.EndUpdate();
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillTables(txtKeyword.Text);
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (var item in lstTable.Items)
            {
                item.Checked = true;
            }
        }

        private void btnInv_Click(object sender, EventArgs e)
        {
            foreach (var item in lstTable.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            FillTables(txtKeyword.Text);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Selected = new List<Table>();
            foreach (var item in lstTable.Items)
            {
                if (item.Checked)
                {
                    Selected.Add((Table)item.Tag);
                }
            }

            if (Selected.Count == 0)
            {
                MessageBoxHelper.ShowExclamation("至少选择一个以上的表。");
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
