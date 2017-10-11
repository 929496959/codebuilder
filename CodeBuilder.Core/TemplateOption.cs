// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core.Template;

using System.Collections.Generic;
using System.IO;

namespace CodeBuilder.Core
{
    public class TemplateOption
    {
        public TemplateOption()
        {
            DynamicAssemblies = new List<string>();
        }

        public TemplateDefinition Template { get; set; }

        public List<PartitionDefinition> Partitions { get; set; }

        /// <summary>
        /// 获取或设置输出目录。
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// 获取动态程序集路径。
        /// </summary>
        public List<string> DynamicAssemblies { get; private set; }

        public Profile Profile { get; set; }

        public bool WriteToDisk { get; set; }
    }
}
