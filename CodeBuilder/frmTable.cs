using CodeBuilder.Core;
// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core.Source;
using CodeBuilder.Core.Template;
using Fireasy.Common.Extensions;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmTable : DockForm
    {
        public frmTable()
        {
            InitializeComponent();
        }

        public Action<object> SelectItemAct { get; set; }

        public void FillTables(IEnumerable<IObject> tables)
        {
            if (tables == null)
            {
                return;
            }

            lstObject.BeginUpdate();
            lstObject.Items.Clear();

            foreach (var t in tables)
            {
                var titem = new TreeListItem();
                lstObject.Items.Add(titem);
                titem.Image = Properties.Resources.table;
                titem.Checked = true;
                titem.Tag = t;
                titem.Cells[0].Value = t.Name;
                titem.Cells[1].Value = t.Description;

                t.As<INotifyPropertyChanged>(s => s.PropertyChanged += (o, e) =>
                    {
                        UpdateObject(o, e.PropertyName);
                    });

                foreach (var f in t.Fields)
                {
                    var citem = new TreeListItem();
                    titem.Items.Add(citem);

                    var c = f as Column;
                    if (c != null)
                    {
                        if (c.ForeignKey != null)
                        {
                            citem.Image = Properties.Resources.fk;
                        }
                        else if (c.IsPrimaryKey)
                        {
                            citem.Image = Properties.Resources.pk;
                        }
                        else
                        {
                            citem.Image = Properties.Resources.column;
                        }
                    }

                    citem.Checked = true;
                    citem.Tag = f;
                    citem.Cells[0].Value = f.Name;
                    citem.Cells[1].Value = f.Description;

                    f.As<INotifyPropertyChanged>(s => s.PropertyChanged += (o, e) =>
                        {
                            UpdateObject(o, e.PropertyName);
                        });
                }
            }

            lstObject.EndUpdate();
        }

        public object GetSelectedObject()
        {
            return lstObject.SelectedItems.Count == 0 ? null : lstObject.SelectedItems[0].Tag;
        }

        public List<Table> GetCheckedTables()
        {
            var list = new List<Table>();

            foreach (var titem in lstObject.Items)
            {
                if (titem.Checked)
                {
                    var clTable = (titem.Tag as Table).Clone();
                    clTable.Columns.Clear();

                    foreach (var citem in titem.Items)
                    {
                        if (citem.Checked)
                        {
                            clTable.Columns.Add(citem.Tag as Column);
                        }
                    }

                    list.Add(clTable);
                }
            }

            return list;
        }

        public void UpdateObject(object obj, string propertyName)
        {
            if (lstObject.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstObject.SelectedItems[0];

            if (obj is Table)
            {
                var table = obj as Table;
                switch (propertyName)
                {
                    case "Name":
                        item.Cells[0].Value = table.Name;
                        break;
                    case "Description":
                        item.Cells[1].Value = table.Description;
                        break;
                }
            }
            else if (obj is Column)
            {
                var column = obj as Column;
                switch (propertyName)
                {
                    case "Name":
                        item.Cells[0].Value = column.Name;
                        break;
                    case "Description":
                        item.Cells[1].Value = column.Description;
                        break;
                    case "IsPrimaryKey":
                    case "ForeignKey":
                        if (column.ForeignKey != null)
                        {
                            item.Image = Properties.Resources.fk;
                        }
                        else if (column.IsPrimaryKey)
                        {
                            item.Image = Properties.Resources.pk;
                        }
                        else
                        {
                            item.Image = Properties.Resources.column;
                        }

                        break;
                }
            }
        }

        private void lstObject_ItemSelectionChanged(object sender, TreeListItemSelectionEventArgs e)
        {
            if (SelectItemAct != null)
            {
                SelectItemAct(lstObject.SelectedItems.Count > 0 ? lstObject.SelectedItems[0].Tag : null);
            }
        }

        private void lstObject_AfterCellUpdated(object sender, TreeListAfterCellUpdatedEventArgs e)
        {
            var obj = e.Cell.Item.Tag;

            if (obj is Table)
            {
                var table = obj as Table;
                table.Description = e.NewValue.ToString();
            }
            else if (obj is Column)
            {
                var column = obj as Column;
                column.Description = e.NewValue.ToString();
            }

            if (SelectItemAct != null)
            {
                SelectItemAct(obj);
            }
        }

        private void mnuSelAllTable_Click(object sender, EventArgs e)
        {
            foreach (var item in lstObject.Items)
            {
                item.Checked = true;
            }
        }

        private void mnuSelInvTable_Click(object sender, EventArgs e)
        {
            foreach (var item in lstObject.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void mnuSelAllColumn_Click(object sender, EventArgs e)
        {
            if (lstObject.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstObject.SelectedItems[0];
            if (item.Tag is Column)
            {
                item = item.Parent;
            }

            foreach (var child in item.Items)
            {
                child.Checked = true;
            }
        }

        private void mnuSelInvColumn_Click(object sender, EventArgs e)
        {
            if (lstObject.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstObject.SelectedItems[0];
            if (item.Tag is Column)
            {
                item = item.Parent;
            }

            foreach (var child in item.Items)
            {
                child.Checked = !child.Checked;
            }
        }

        private void mnuBuild_Click(object sender, EventArgs e)
        {
            TemplateDefinition template = null;
            if (StaticUnity.TemplateProvider == null ||
                (template = StaticUnity.Template) == null)
            {
                MessageBoxHelper.ShowExclamation("你还没有选择生成模板，请从【模板】菜单中选择。");
                return;
            }
            
            if (lstObject.SelectedItems.Count == 0)
            {
                return;
            }

            var item = lstObject.SelectedItems[0];
            if (item.Tag is Column)
            {
                item = item.Parent;
            }

            var table = item.Tag as Table;

            var option = new TemplateOption();
            option.Template = template;
            option.Partitions = StaticUnity.Template.Partitions;
            option.DynamicAssemblies.AddRange(StaticUnity.DynamicAssemblies);
            option.Profile = StaticUnity.Profile;

            var tables = new List<Table> { table };

            Cursor = Cursors.WaitCursor;
            try
            {
                var result = StaticUnity.TemplateProvider.GenerateFiles(option, tables, null);
                if (result != null)
                {
                    foreach (var file in result)
                    {
                        var editor = new frmEditor { GenerateResult = file };
                        editor.Show(this.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
