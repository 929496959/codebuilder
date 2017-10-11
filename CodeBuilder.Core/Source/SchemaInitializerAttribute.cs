// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace CodeBuilder.Core.Source
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaInitializerAttribute : Attribute
    {
        public SchemaInitializerAttribute(Type schemaType)
        {
            SchemaType = schemaType;
        }

        public Type SchemaType { get; set; }
    }
}
