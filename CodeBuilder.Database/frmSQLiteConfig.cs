
using Fireasy.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using Fireasy.Common.Extensions;

namespace CodeBuilder.Database
{
    public partial class frmSQLiteConfig : frmConfigBase
    {
        public frmSQLiteConfig()
        {
            InitializeComponent();
        }

        protected override void ParseConnectionStr(ConnectionProperties properties)
        {
            txtFile.Text = properties.TryGetValue("data source");
        }

        protected override string BuildConnectionStr()
        {
            return string.Format("data source={0};pooling=True", txtFile.Text);
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "SQLite DB|*.db3|所有文件|*.*";
                dialog.FileName = txtFile.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFile.Text = dialog.FileName;
                }
            }
        }
    }
}
