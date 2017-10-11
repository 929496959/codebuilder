using Fireasy.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;

namespace CodeBuilder.PowerDesigner
{
    public class DbTypeManager
    {
        static Dictionary<string, Dictionary<string, string>> cache = new Dictionary<string, Dictionary<string, string>>();

        static DbTypeManager()
        {
            var configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "pd.cfg");
            var json = new JsonSerializer();
            cache = json.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(configFileName, Encoding.Default));
        }

        public static DbType? GetDbType(string databaseType, string dataType)
        {
            Dictionary<string, string> sub = cache.FirstOrDefault(s => databaseType.Contains(s.Key)).Value;
            string dbType;

            if (sub != null)
            {
                if (sub.TryGetValue(dataType.ToLower(), out dbType))
                {
                    DbType result;
                    if (Enum.TryParse<DbType>(dbType, out result))
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}
