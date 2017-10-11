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
            dic.Add("MsSql", () => new frmMsSqlConfig());
            dic.Add("Oracle", () => new frmOracleConfig());
            dic.Add("MySql", () => new frmMySqlConfig());
            dic.Add("SQLite", () => new frmSQLiteConfig());
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
