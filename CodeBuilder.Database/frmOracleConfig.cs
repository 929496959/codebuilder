
using System.Collections.Generic;
using Fireasy.Common.Extensions;
using Fireasy.Data;

namespace CodeBuilder.Database
{
    public partial class frmOracleConfig : frmConfigBase
    {
        public frmOracleConfig()
        {
            InitializeComponent();
        }

        protected override void ParseConnectionStr(ConnectionProperties properties)
        {
            txtSvr.Text = properties.TryGetValue("data source");
            txtUser.Text = properties.TryGetValue("user id");
            txtPwd.Text = properties.TryGetValue("password");
        }

        protected override string BuildConnectionStr()
        {
            return string.Format("data source={0};user id={1};password={2};", txtSvr.Text, txtUser.Text, txtPwd.Text);
        }
    }
}
