using CodeBuilder.Core;
using Fireasy.Common.Serialization;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace CodeBuilder.Tools
{
    public partial class frmRemoter : DockContent, IFontApplyer
    {
        private List<Remoter.Connection> connections = new List<Remoter.Connection>();

        public frmRemoter()
        {
            InitializeComponent();
        }

        private void frmRemote_Load(object sender, System.EventArgs e)
        {
            LoadData();
        }

        private void lstItems_ItemDoubleClick(object sender, TreeListItemEventArgs e)
        {
            var sysPath = System.Environment.GetFolderPath(Environment.SpecialFolder.System);
            var p = new ProcessStartInfo();
            p.WorkingDirectory = sysPath;
            p.FileName = "mstsc.exe";
            p.Arguments = "/v:" + e.Item.Cells[1].Text;
            Process.Start(p);
        }

        private void lstItems_ItemSelectionChanged(object sender, TreeListItemSelectionEventArgs e)
        {
            if (lstItems.SelectedItems.Count > 0)
            {
                var item = lstItems.SelectedItems[0];
                PropertyUnity.SetObject(new { Name = item.Text, Host = item.Cells[1].Text });
            }
            else
            {
                PropertyUnity.SetObject(null);
            }
        }

        private void tlbtnAdd_Click(object sender, EventArgs e)
        {
            var groups = connections.Select(s => (string)s.group).Distinct().ToList();

            using (var frm = new frmRemoteEdit { Groups = groups })
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    connections.Add(frm.Connection);
                    SaveData();

                    var item = new TreeListItem(frm.Connection.name);
                    item.Tag = frm.Connection;
                    item.ImageIndex = 0;
                    item.Group = frm.Connection.group;
                    lstItems.Items.Add(item);
                    item.Cells[1].Value = frm.Connection.host;
                    lstItems.Grouping(true);
                }
            }
        }

        private void tlbtnEdit_Click(object sender, EventArgs e)
        {
            if (lstItems.SelectedItems.Count == 0)
            {
                MessageBoxHelper.ShowExclamation("请选择一个远程连接。");
                return;
            }

            var item = lstItems.SelectedItems[0];
            var groups = connections.Select(s => s.group).Distinct().ToList();
            var conn = (Remoter.Connection)item.Tag;
            var group = conn.group;

            using (var frm = new frmRemoteEdit { Groups = groups, Connection = conn })
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SaveData();

                    if (group != frm.Connection.group)
                    {
                        item.Group = frm.Connection.group;
                        lstItems.Grouping(true);
                    }
                    else if (groups.IndexOf(frm.Connection.group) == -1)
                    {
                        LoadData();
                    }
                    else
                    {
                        item.Text = frm.Connection.name;
                        item.Cells[1].Value = frm.Connection.host;
                    }
                }
            }
        }

        private void tlbtnDelete_Click(object sender, EventArgs e)
        {
            if (lstItems.SelectedItems.Count == 0)
            {
                MessageBoxHelper.ShowExclamation("请选择一个远程连接。");
                return;
            }

            var item = lstItems.SelectedItems[0];
            if (MessageBoxHelper.ShowQuestion("是否真的要删除[" + item.Text + "]? ") == System.Windows.Forms.DialogResult.Yes)
            {
                connections.Remove((Remoter.Connection)item.Tag);
                lstItems.Items.Remove(item);
                lstItems.Grouping(true);
                SaveData();
            }
        }

        private void LoadData()
        {
            lstItems.Items.Clear();
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "remote.json");
            if (!File.Exists(file))
            {
                return;
            }

            var json = File.ReadAllText(file);
            connections = new JsonSerializer().Deserialize<List<Remoter.Connection>>(json);

            foreach (var s in connections)
            {
                var item = new TreeListItem(s.name);
                item.Tag = s;
                item.ImageIndex = 0;
                item.Group = s.group;
                lstItems.Items.Add(item);
                item.Cells[1].Value = s.host;
            }

            lstItems.Grouping(true);
        }

        private void SaveData()
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "remote.json");
            var json = new JsonSerializer().Serialize(connections);

            File.WriteAllText(file, json, Encoding.UTF8);
        }
    }
}
