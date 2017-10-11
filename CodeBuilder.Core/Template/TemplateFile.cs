using System.Collections.Generic;

namespace CodeBuilder.Core.Template
{
    public class TemplateFile
    {
        public TemplateFile(string name)
        {
            Name = name;
        }

        public TemplateFile(string name, string fileName)
            : this (name)
        {
            FileName = fileName;
        }

        public TemplateFile(string name, string fileName, string language)
            : this(name, fileName)
        {
            Language = language;
        }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string Language { get; set; }
    }
}
