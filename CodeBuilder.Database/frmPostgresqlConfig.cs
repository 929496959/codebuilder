﻿using Fireasy.Data;
using Fireasy.Data.Provider;

namespace CodeBuilder.Database
{
    public partial class frmPostgresqlConfig : frmConfigBase
    {
        public frmPostgresqlConfig()
        {
            InitializeComponent();
        }

        protected override IProvider Provider
        {
            get { return PostgreSqlProvider.Instance; }
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
            var str = string.Format("Server={0};Database={1};UserId={2};Password={3}", txtSvr.Text, txtDb.Text, txtUser.Text, txtPwd.Text);
            if (!string.IsNullOrWhiteSpace(txtPort.Text))
            {
                str += ";port=" + txtPort.Text;
            }

            return str;
        }

    }
}
