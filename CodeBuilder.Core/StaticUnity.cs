// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CodeBuilder.Core.Template;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeBuilder.Core
{
    public class StaticUnity
    {
        static ITemplateProvider templateProvider;

        static StaticUnity()
        {
            DynamicAssemblies = new List<string>();
            Encoding = Encoding.Default;
        }

        public static ITemplateProvider TemplateProvider
        {
            get { return templateProvider; }
            set
            {
                templateProvider = value;
                PartitionWriter.ClearCache();
            }
        }

        public static TemplateDefinition Template { get; set; }

        public static List<string> DynamicAssemblies { get; set; }

        public static Profile Profile { get; set; }

        public string ProfileName { get; set; }

        public static Encoding Encoding { get; set; }

        public static Form Main { get; set; }
    }
}
