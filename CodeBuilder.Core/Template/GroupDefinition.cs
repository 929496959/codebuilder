using System.Collections.Generic;

namespace CodeBuilder.Core.Template
{
    public class GroupDefinition
    {
        public GroupDefinition()
        {
            Groups = new List<GroupDefinition>();
            Partitions = new List<PartitionDefinition>();
        }

        public string Name { get; set; }

        public List<GroupDefinition> Groups { get; private set; }

        public List<PartitionDefinition> Partitions { get; private set; }
    }
}
