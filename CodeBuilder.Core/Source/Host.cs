// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace CodeBuilder.Core.Source
{
    public sealed class Host
    {
        public Host()
        {
            Tables = new List<Table>();
        }

        public List<Table> Tables { get; private set; }

        public void Attach(Table table)
        {
            Tables.Add(table);
            table.Host = this;
        }
    }
}
