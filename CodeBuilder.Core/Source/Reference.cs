// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace CodeBuilder.Core.Source
{
    public class Reference : SchemaBase
    {
        public Reference(Table pkTable, Column pkColumn, Table fkTable, Column fkColumn)
        {
            PkTable = pkTable;
            PkColumn = pkColumn;
            FkTable = fkTable;
            FkColumn = fkColumn;
        }

        [Description("外键的名称。")]
        public string Name { get; set; }

        [Browsable(false)]
        public Table FkTable { get; private set; }

        [Browsable(false)]
        public Column FkColumn { get; private set; }

        [Browsable(false)]
        public Table PkTable { get; private set; }

        [Browsable(false)]
        public Column PkColumn { get; private set; }

        [Description("更新时的约束。")]
        public Constraint OnUpdate { get; set; }

        [Description("删除时的约束。")]
        public Constraint OnDelete { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? string.Format("{0}.{1}", PkTable.Name, PkColumn.Name) : Name;
        }
    }

    public enum Constraint
    {
        Restrict,
        Cascade,
        SetNull
    }
}
