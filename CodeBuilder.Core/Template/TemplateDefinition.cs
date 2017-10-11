// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace CodeBuilder.Core.Template
{
    /// <summary>
    /// 模板定义。
    /// </summary>
    public class TemplateDefinition
    {
        public TemplateDefinition()
        {
            Partitions = new List<PartitionDefinition>();
            Groups = new List<GroupDefinition>();
            Resources = new List<string>();
        }

        /// <summary>
        /// 获取或设置标识。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置模板名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置配置文件名称。
        /// </summary>
        public string ConfigFileName { get; set; }

        /// <summary>
        /// 获取或设置作者。
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 获取或设置版本。
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 获取或设置模板备注。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取分部定义列表。
        /// </summary>
        public List<PartitionDefinition> Partitions { get; private set; }

        /// <summary>
        /// 获取组定义列表。
        /// </summary>
        public List<GroupDefinition> Groups { get; private set; }

        /// <summary>
        /// 获取资源列表。
        /// </summary>
        public List<string> Resources { get; private set; }
    }
}
