// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CodeBuilder.Core.Designer;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
namespace CodeBuilder.Core.Source
{
    public class Column : SchemaBase, IField, INotifyPropertyChanged
    {
        private string name;
        private string description;
        private bool isPrimaryKey;

        public event PropertyChangedEventHandler PropertyChanged;

        public Column()
        {
        }

        public Column(Table owner)
        {
            Owner = owner;
        }

        [Description("字段的名称。")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        [Description("生成的属性名称。")]
        public string PropertyName { get; set; }

        [Description("生成的属性的类型名称。")]
        public string PropertyType { get; set; }

        [Description("字段的描述。")]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        [Description("字段的数据类型。")]
        public DbType? DbType { get; set; }

        [Description("字段的数据类型。")]
        public string DataType { get; set; }

        [Description("字段的类型(类型+长度/精度)。")]
        public string ColumnType { get; set; }

        [Description("字段的长度。")]
        public long? Length { get; set; }

        [Description("字段的默认值。")]
        public string DefaultValue { get; set; }

        [Description("数值的小数位。")]
        public int? Scale { get; set; }

        [Description("数值的精度。")]
        public int? Precision { get; set; }

        [Description("是否自动生成。")]
        public bool AutoIncrement { get; set; }

        [Description("字段是否可为空。")]
        public bool IsNullable { get; set; }

        [Description("排列的序号。")]
        public int Index { get; set; }

        [Description("字段是否为主键。")]
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set
            {
                if (isPrimaryKey != value)
                {
                    if (value)
                    {
                        Owner.PrimaryKeys.Add(this);
                    }
                    else
                    {
                        Owner.PrimaryKeys.Remove(this);
                    }
                }

                isPrimaryKey = value;
                OnPropertyChanged("IsPrimaryKey");
            }
        }

        [Description("关联的外键。")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Editor(typeof(ForeignKeyEditor), typeof(UITypeEditor))]
        public Reference ForeignKey { get; private set; }

        [DisGenerate]
        [Browsable(false)]
        public Table Owner { get; private set; }

        public virtual void BindForeignKey(Reference foreignKey)
        {
            if ((ForeignKey == null && foreignKey == null) ||
                (ForeignKey != null && foreignKey != null && ForeignKey.PkColumn == foreignKey.PkColumn))
            {
                return;
            }

            //解除
            if (ForeignKey != null)
            {
                ForeignKey.PkTable.SubKeys.Remove(ForeignKey);
                Owner.ForeignKeys.Remove(ForeignKey);
            }

            if (foreignKey != null)
            {
                foreignKey.PkTable.SubKeys.Add(foreignKey);
                Owner.ForeignKeys.Add(foreignKey);
            }

            ForeignKey = foreignKey;
            OnPropertyChanged("ForeignKey");
        }

        public virtual void UnbindForeignKey()
        {
            ForeignKey = null;
        }

        public override string ToString()
        {
            return Name;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
