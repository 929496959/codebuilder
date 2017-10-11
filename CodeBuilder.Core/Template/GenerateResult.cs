// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace CodeBuilder.Core.Template
{
    public class GenerateResult
    {
        public GenerateResult(PartitionDefinition part, string content)
        {
            Partition = part;
            Content = content;
            WriteToDisk = !string.IsNullOrEmpty(part.FilePath);
        }

        public string Content { get; private set; }

        public PartitionDefinition Partition { get; private set; }

        public bool WriteToDisk { get; private set; }
    }
}
