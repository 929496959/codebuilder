using System.Collections.Generic;

namespace CodeBuilder.Core.Template
{
    public class TemplateDirectory
    {
        public TemplateDirectory(string name)
        {
            Name = name;
            Directories = new List<TemplateDirectory>();
            Files = new List<TemplateFile>();
        }

        public string Name { get; set; }

        public List<TemplateDirectory> Directories { get; private set; }

        public List<TemplateFile> Files { get; private set; }
    }
}
