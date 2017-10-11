using CodeBuilder.Core.Source;
using CodeBuilder.Core.Variable;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodeBuilder
{
    public delegate void FieldInsertEventHandler(string field);

    public class FieldCacheMenuItem : ToolStripMenuItem
    {
        private ToolStripMenuItem mnuTableFields = new ToolStripMenuItem("Table");
        private ToolStripMenuItem mnuColumnFields = new ToolStripMenuItem("Column");
        private ToolStripMenuItem mnuReferFields = new ToolStripMenuItem("Reference");
        private ToolStripMenuItem mnuProfileFields = new ToolStripMenuItem("Profile");

        public event FieldInsertEventHandler OnFieldInsert;

        public FieldCacheMenuItem()
        {
            DropDownItems.Add(mnuTableFields);
            DropDownItems.Add(mnuColumnFields);
            DropDownItems.Add(mnuReferFields);
            DropDownItems.Add(mnuProfileFields);
            InitInsertMenus();
        }

        private void InitInsertMenus()
        {
            InitMenuFields(mnuTableFields, SchemaExtensionManager.GetPropertyMaps<Table>());
            InitMenuFields(mnuColumnFields, SchemaExtensionManager.GetPropertyMaps<Column>());
            InitMenuFields(mnuReferFields, SchemaExtensionManager.GetPropertyMaps<Reference>());
            InitMenuFields(mnuProfileFields, ProfileExtensionManager.GetPropertyMaps());
        }

        private void InitMenuFields(ToolStripMenuItem menu, List<PropertyMap> properties)
        {
            foreach (var p in properties)
            {
                var item = new ToolStripMenuItem(string.Format("{0} ({1})", p.Name, p.TypeName));
                item.Tag = p.Name;
                item.Click += (o, e) =>
                    {
                        if (OnFieldInsert != null)
                        {
                            OnFieldInsert(item.Tag as string);
                        }
                    };

                menu.DropDownItems.Add(item);
            }
        }
    }
}
