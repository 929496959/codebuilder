using CodeBuilder.Core;
using Fireasy.Common.Composition;
using Fireasy.Common.Extensions;
using Fireasy.Windows.Forms;
using System;
using System.Text;
using System.Windows.Forms;

namespace CodeBuilder
{
    public partial class frmOption : Form
    {
        public frmOption()
        {
            InitializeComponent();
        }

        private void frmOption_Load(object sender, EventArgs e)
        {
            LoadEncodings();
            LoadPlugins();
        }

        private void LoadEncodings()
        {
            cboEncoding.DisplayMember = "DisplayName";
            cboEncoding.ValueMember = "Name";
            cboEncoding.Items.Add(new { DisplayName = "默认", Name = string.Empty });

            foreach (var en in Encoding.GetEncodings())
            {
                var index = cboEncoding.Items.Add(en);
                if (Config.Instance.Encoding == en.Name)
                {
                    cboEncoding.SelectedIndex = index;
                }
            }

            if (cboEncoding.SelectedIndex == -1)
            {
                cboEncoding.SelectedIndex = 0;
            }

            chkCheckUpdate.Checked = Config.Instance.CheckUpdate;
            chkView.Checked = Config.Instance.Source_View;
        }

        private void LoadPlugins()
        {
            var sources = Imports.GetServices<ISourceProvider>();
            var templates = Imports.GetServices<ITemplateProvider>();
            var tools = Imports.GetServices<IToolProvider>();

            var sourceGroup = new TreeListGroup("数据源");
            var templateGroup = new TreeListGroup("模板");
            var toolGroup = new TreeListGroup("工具");
            lstPlugin.Groups.Add(sourceGroup);
            lstPlugin.Groups.Add(templateGroup);
            lstPlugin.Groups.Add(toolGroup);

            Func<IPlugin, TreeListItem> func = (s) =>
               {
                   var assembly = s.GetType().Assembly.GetName();
                   var item = new TreeListItem(s.Name);
                   item.Image = Properties.Resources.plugin;
                   item.Cells.Add(assembly.Name);
                   item.Cells.Add(assembly.Version.ToString());
                   return item;
               };

            sources.ForEach(s => sourceGroup.Items.Add(func(s)));
            templates.ForEach(s => templateGroup.Items.Add(func(s)));
            tools.ForEach(s => toolGroup.Items.Add(func(s)));
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cboEncoding.SelectedIndex != -1)
            {
                if (cboEncoding.SelectedIndex == 0)
                {
                    Config.Instance.Encoding = string.Empty;
                    StaticUnity.Encoding = Encoding.Default;
                }
                else
                {
                    Config.Instance.Encoding = ((EncodingInfo)cboEncoding.SelectedItem).Name;
                    StaticUnity.Encoding = Encoding.GetEncoding(Config.Instance.Encoding);
                }
            }

            Config.Instance.CheckUpdate = chkCheckUpdate.Checked;
            Config.Instance.Source_View = chkView.Checked;
            Config.Save();


            DialogResult = DialogResult.OK;
        }
    }
}
