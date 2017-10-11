// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CodeBuilder.Core.Source;
using System.Collections.Generic;

namespace CodeBuilder.Core
{
    /// <summary>
    /// 数据源提供者。
    /// </summary>
    public interface ISourceProvider
    {
        string Name { get; }

        /// <summary>
        /// 连接数据源，获取预览表。
        /// </summary>
        /// <returns></returns>
        List<Table> Preview();

        /// <summary>
        /// 获取指定表的架构。
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="processHandler"></param>
        /// <returns></returns>
        List<Table> GetSchema(List<Table> tables, TableSchemaProcessHandler processHandler);

        List<string> GetHistory();
    }
}
