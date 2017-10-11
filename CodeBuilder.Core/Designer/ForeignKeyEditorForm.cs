using CodeBuilder.Core.Source;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeBuilder.Core.Designer
{
    public partial class ForeignKeyEditorForm : Form
    {
        private Column column;

        public ForeignKeyEditorForm(Column column)
        {
            InitializeComponent();
            this.column = column;
        }

        public object Value
        {
            get
            {
                return column.ForeignKey;
            }
        }

        private void ForeignKeyEditorForm_Load(object sender, EventArgs e)
        {
            TreeListItem selected = null;
            listBox1.BeginUpdate();

            foreach (var t in column.Owner.Host.Tables)
            {
                if (t == column.Owner)
                {
                    continue;
                }

                var f = listBox1.Items.Add(t.Name);
                f.Image = Properties.Resources.table;
                f.Cells[1].Value = t.Description;

                foreach (var c in t.Columns)
                {
                    var s = f.Items.Add(c.Name);
                    s.Tag = c;
                    if (c.IsPrimaryKey)
                    {
                        s.Image = Properties.Resources.pk;
                    }
                    else
                    {
                        s.Image = Properties.Resources.column;
                    }

                    s.Cells[1].Value = c.Description;

                    if (column.ForeignKey != null)
                    {
                        if (column.ForeignKey.PkTable == t && column.ForeignKey.PkColumn == c)
                        {
                            s.Selected = true;
                            selected = s;
                        }
                    }
                }
            }

            listBox1.EndUpdate();

            if (selected != null)
            {
                selected.EnsureVisible();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            column.BindForeignKey(null);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnBind_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0)
            {
                MessageBoxHelper.ShowExclamation("请选择要绑定的列。");
                return;
            }

            var target = listBox1.SelectedItems[0].Tag as Column;
            if (target == null)
            {
                MessageBoxHelper.ShowExclamation("请选择要绑定的列。");
                return;
            }

            column.BindForeignKey(new Reference(target.Owner, target, column.Owner, column));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
