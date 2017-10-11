// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

namespace CodeBuilder.Core
{
    public class FileTypeHelper
    {
        private static Dictionary<string, string> filters = new Dictionary<string, string>();

        static FileTypeHelper()
        {
            filters.Add(".cs", "C#代码文件|*.cs");
            filters.Add(".vb", "VB代码文件|*.vb");
            filters.Add(".cfg", "配置文件|*.cfg");
            filters.Add(".sql", "SQL脚本文件|*.sql");
            filters.Add(".xml", "XML文档文件|*.xml");
            filters.Add(".txt", "文本文件|*.txt");
            filters.Add("-", "所有文件|*.*");
        }

        public static void Register(string extension, string descrition)
        {
            if (filters.ContainsKey(extension.ToLower()))
            {
                return;
            }

            filters.Remove("-");
            filters.Add(extension.ToLower(), descrition);
            filters.Add("-", "所有文件|*.*");
        }

        public static string GetAllFilters()
        {
            return string.Join("|", filters.Select(s => s.Value));
        }

        public static int GetFilterIndex(string extension)
        {
            var index = 1;
            foreach (var kvp in filters)
            {
                if (kvp.Key.Equals(extension, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}
