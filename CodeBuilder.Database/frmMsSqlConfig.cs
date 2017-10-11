using Fireasy.Common.Extensions;
using Fireasy.Data;
using System.Collections.Generic;

namespace CodeBuilder.Database
{
    public partial class frmMsSqlConfig : frmConfigBase
    {
        public frmMsSqlConfig()
        {
            InitializeComponent();
        }

        protected override void ParseConnectionStr(ConnectionProperties properties)
        {
            txtSvr.Text = properties.TryGetValue("data source");
            txtDb.Text = properties.TryGetValue("initial catalog");
            txtUser.Text = properties.TryGetValue("user id");
            txtPwd.Text = properties.TryGetValue("password");
        }

        protected override string BuildConnectionStr()
        {
            return string.Format("data source={0};initial catalog={1};user id={2};password={3}", txtSvr.Text, txtDb.Text, txtUser.Text, txtPwd.Text);
        }
    }
}
