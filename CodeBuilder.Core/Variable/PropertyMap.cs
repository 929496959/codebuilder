// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Reflection;

namespace CodeBuilder.Core.Variable
{
    public class PropertyMap
    {
        public PropertyMap(PropertyInfo property)
        {
            Name = property.Name;
            Type = property.PropertyType;
            TypeName = Util.GetTypeName(Type);
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public string TypeName { get; set; }
    }
}
