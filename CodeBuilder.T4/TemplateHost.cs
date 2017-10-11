// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBuilder.T4
{
    [Serializable]
    public class TemplateHost : ITextTemplatingEngineHost
    {
        private CompilerErrorCollection errorCollection;
        private string path;
        private string fileExtention;
        private Encoding fileEncoding;
        private List<string> assemblyLocationList = new List<string>();
        private List<string> namespaceList = new List<string>();

        public TemplateHost(string path, dynamic tables, dynamic references, List<string> assemblyList)
        {
            this.path = path;
            Tables = tables;
            References = references;

            Initialize();

            if (assemblyList != null)
            {
                assemblyLocationList.AddRange(assemblyList);
            }
        }

        public dynamic Tables { get; private set; }

        public dynamic References { get; private set; }

        public dynamic Current { get; set; }

        public dynamic Profile { get; set; }

        public object GetHostOption(string optionName)
        {
            object obj2 = null;
            string str;
            if (((str = optionName) != null) && (str == "CacheAssemblies"))
            {
                obj2 = true;
            }
            return obj2;
        }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = string.Empty;
            location = string.Empty;
            requestFileName = Path.Combine(path, requestFileName);
            
            if (File.Exists(requestFileName))
            {
                content = File.ReadAllText(requestFileName, Encoding.Default);
                return true;
            }

            return false;
        }

        public void LogErrors(CompilerErrorCollection errors)
        {
            errorCollection = errors;
        }

        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            return SingleAppDomainScope.Current.AppDomain;
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyReference);
            if (File.Exists(path))
            {
                return path;
            }

            return string.Empty;
        }

        public Type ResolveDirectiveProcessor(string processorName)
        {
            string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase);
            throw new Exception("没有找到指令处理器");
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException("directiveId");
            }

            if (processorName == null)
            {
                throw new ArgumentNullException("processorName");
            }

            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }

            return string.Empty;
        }

        public string ResolvePath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (!File.Exists(path))
            {
                string str = Path.Combine(Path.GetDirectoryName(TemplateFile), path);
                if (File.Exists(str))
                {
                    return str;
                }
            }

            return path;
        }

        public void SetFileExtension(string extension)
        {
            fileExtention = extension;
        }

        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            fileEncoding = encoding;
        }

        public IList<string> StandardAssemblyReferences
        {
            get { return assemblyLocationList; }
        }

        public IList<string> StandardImports
        {
            get { return namespaceList; }
        }

        public string TemplateFile { get; set; }

        public CompilerErrorCollection Errors
        {
            get { return errorCollection; }
        }

        private void Initialize()
        {
            assemblyLocationList.Add(typeof(System.String).Assembly.Location);
            assemblyLocationList.Add(typeof(System.Diagnostics.Trace).Assembly.Location);
            assemblyLocationList.Add(typeof(System.Linq.Expressions.Expression).Assembly.Location);
            assemblyLocationList.Add(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location);
            assemblyLocationList.Add(typeof(System.Data.DbType).Assembly.Location);
            assemblyLocationList.Add("CodeBuilder.Core.dll");
            assemblyLocationList.Add("Fireasy.Common.dll");
            assemblyLocationList.Add("CodeBuilder.T4.dll");
            namespaceList.Add("System");
            namespaceList.Add("System.IO");
            namespaceList.Add("System.Text");
            namespaceList.Add("System.Diagnostics");
            namespaceList.Add("System.Collections");
            namespaceList.Add("System.Collections.Generic");
            namespaceList.Add("System.Linq");
            namespaceList.Add("System.Data");
            namespaceList.Add("System.Dynamic");
            namespaceList.Add("CodeBuilder.T4");
            namespaceList.Add("Fireasy.Common");
            namespaceList.Add("Fireasy.Common.Extensions");
        }
    }
}
