﻿using Fireasy.Data;
using Fireasy.Data.Provider;

namespace CodeBuilder.Database
{
    public partial class frmMySqlConfig : frmConfigBase
    {
        public frmMySqlConfig()
        {
            InitializeComponent();
        }
        protected override IProvider Provider
        {
            get { return MySqlProvider.Instance; }
        }

        protected override void ParseConnectionStr(ConnectionProperties properties)
        {
            if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                var parameter = Provider.GetConnectionParameter(ConnectionString);
                txtSvr.Text = parameter.Server;
                txtDb.Text = parameter.Database;
                txtUser.Text = parameter.UserId;
                txtPwd.Text = parameter.Password;
                txtPort.Text = properties.TryGetValue("port");
            }
        }

        protected override string BuildConnectionStr()
        {
            var str = string.Format("data source={0};database={1};user id={2};password={3};pooling=false;charset=utf8", txtSvr.Text, txtDb.Text, txtUser.Text, txtPwd.Text);
            if (!string.IsNullOrWhiteSpace(txtPort.Text))
            {
                str += ";port=" + txtPort.Text;
            }

            return str;
        }
    }
}
