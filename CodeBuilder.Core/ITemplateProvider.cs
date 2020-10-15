// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CodeBuilder.Core.Source;
using CodeBuilder.Core.Template;
using System.Collections.Generic;

namespace CodeBuilder.Core
{
    public interface ITemplateProvider : IPlugin
    {
        string Name { get; }

        string WorkDir { get; }

        List<GenerateResult> GenerateFiles(TemplateOption option, List<Table> tables, CodeGenerateHandler handler);

        List<TemplateDefinition> GetTemplates();

        TemplateStorage GetStorage(TemplateDefinition definition);
    }
}
