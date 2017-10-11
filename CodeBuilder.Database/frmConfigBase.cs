using Fireasy.Data;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodeBuilder.Database
{
    public partial class frmConfigBase : Form, IConnectionConfig
    {
        public frmConfigBase()
        {
            InitializeComponent();
        }

        public string ConnectionString { get; set; }

        private void frmConfigBase_Load(object sender, System.EventArgs e)
        {
            if (ConnectionString != null)
            {
                ParseConnectionStr(new ConnectionString(ConnectionString).Properties);
            }
        }

        protected virtual void ParseConnectionStr(ConnectionProperties properties)
        {
        }

        protected virtual string BuildConnectionStr()
        {
            return (string)ConnectionString;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            ConnectionString = BuildConnectionStr();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
