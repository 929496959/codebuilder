// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.IO;

namespace CodeBuilder.Core.Template
{
    public static class ResourceWriter
    {
        public static void Write(TemplateDefinition template, string output)
        {
            if (template.Resources.Count == 0)
            {
                return;
            }

            var path = Path.Combine(template.ConfigFileName.Substring(0, template.ConfigFileName.LastIndexOf(".")), "Resources");
            foreach (var res in template.Resources)
            {
                var source = Path.Combine(path, res);
                var desc = Path.Combine(output, res);
                var descDir = new FileInfo(desc).DirectoryName;
                if (!Directory.Exists(descDir))
                {
                    Directory.CreateDirectory(descDir);
                }

                File.Copy(source, desc, true);
            }
        }
    }
}
