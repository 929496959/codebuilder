using Fireasy.Common.Extensions;
using Fireasy.Data;
using System.Collections.Generic;

namespace CodeBuilder.Database
{
    public partial class frmMySqlConfig : frmConfigBase
    {
        public frmMySqlConfig()
        {
            InitializeComponent();
        }

        protected override void ParseConnectionStr(ConnectionProperties properties)
        {
            txtSvr.Text = properties.TryGetValue("data source");
            txtDb.Text = properties.TryGetValue("database");
            txtUser.Text = properties.TryGetValue("user id");
            txtPwd.Text = properties.TryGetValue("password");
        }

        protected override string BuildConnectionStr()
        {
            return string.Format("data source={0};database={1};user id={2};password={3};pooling=true;charset=utf8", txtSvr.Text, txtDb.Text, txtUser.Text, txtPwd.Text);
        }
    }
}
