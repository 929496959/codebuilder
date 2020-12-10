// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.IO;
using System.Text;

namespace CodeBuilder.Core.Template
{
    public class PartitionWriter
    {
        private static Parser paser = new Parser();

        internal static void ClearCache()
        {
            paser = new Parser();
        }

        public static void Write(GenerateResult result, object schema, object profile, string output)
        {
            var fileName = result.Partition.Output;
            fileName = paser.Parse(schema, profile, fileName);

            var path = Path.Combine(output, fileName);
            var dir = path.Substring(0, path.LastIndexOf("\\"));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, result.Content, StaticUnity.Encoding);
        }
    }
}
