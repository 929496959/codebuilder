using System.Collections.Generic;

namespace CodeBuilder.Core.Template
{
    public class TemplateStorage
    {
        public TemplateStorage()
        {
            Directories = new List<TemplateDirectory>();
            Files = new List<TemplateFile>();
        }

        public List<TemplateDirectory> Directories { get; set; }

        public List<TemplateFile> Files { get; set; }

        public void FromDefinition(TemplateDefinition definition)
        {
            FillFiles(Directories, Files, definition.Groups, definition.Partitions);
        }

        private void FillFiles(List<TemplateDirectory> dItems, List<TemplateFile> fItems, List<GroupDefinition> groups, List<PartitionDefinition> partitions)
        {
            foreach (var p in groups)
            {
                var dir = new TemplateDirectory(p.Name);
                dItems.Add(dir);
                FillFiles(dir.Directories, dir.Files, p.Groups, p.Partitions);
            }

            foreach (var p in partitions)
            {
                fItems.Add(new TemplateFile(string.Format("{1} ({0})", p.Name, p.FileName), p.FilePath, "C#"));
            }
        }

    }
}
