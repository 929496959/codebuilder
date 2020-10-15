using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBuilder.Database
{
    public class ConfigForms
    {
        private static Dictionary<string, Func<IConnectionConfig>> dic = new Dictionary<string, Func<IConnectionConfig>>();

        static ConfigForms()
        {
            dic.Add("SqlServer", () => new frmMsSqlConfig());
            dic.Add("Oracle", () => new frmOracleConfig());
            dic.Add("MySql", () => new frmMySqlConfig());
            dic.Add("SQLite", () => new frmSQLiteConfig());
            dic.Add("PostgreSql", () => new frmPostgresqlConfig());
            dic.Add("Firebird", () => new frmFirebirdConfig());
            dic.Add("OleDb", () => new DataLinkerDialog());
        }

        public static IConnectionConfig GetConfigForm(string providerName)
        {
            Func<IConnectionConfig> r;
            if (dic.TryGetValue(providerName, out r))
            {
                return r();
            }

            return null;
        }
    }
}
