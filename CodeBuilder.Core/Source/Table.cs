// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CodeBuilder.Core.Source
{
    public class Table : SchemaBase, IObject, INotifyPropertyChanged
    {
        private string name;
        private string description;

        public event PropertyChangedEventHandler PropertyChanged;

        public Table()
        {
            SubKeys = new List<Reference>();
            ForeignKeys = new List<Reference>();
            Columns = new List<Column>();
            PrimaryKeys = new List<Column>();
        }

        [Description("表的名称。")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        [Description("生成的类名称。")]
        public string ClassName { get; set; }

        [Description("表的描述。")]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        [Browsable(false)]
        public List<Reference> ForeignKeys { get; private set; }

        [Browsable(false)]
        public List<Reference> SubKeys { get; private set; }

        [Browsable(false)]
        public List<Column> PrimaryKeys { get; private set; }

        [Browsable(false)]
        public List<Column> Columns { get; private set; }

        List<IField> IObject.Fields
        {
            get
            {
                return Columns.Cast<IField>().ToList();
            }
        }

        [DisGenerate]
        [Browsable(false)]
        public Host Host { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public Table Clone()
        {
            return (Table)MemberwiseClone();
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
