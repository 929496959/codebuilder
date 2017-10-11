// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using System;
using System.Collections.Generic;

namespace CodeBuilder.Core.Source
{
    public class InitializerUnity
    {
        private static Dictionary<Type, List<IInitializer>> cache = new Dictionary<Type, List<IInitializer>>();

        public static void Register(Type schemaType, IInitializer initializer)
        {
            var list = cache.TryGetValue(schemaType, () => new List<IInitializer>());
            list.Add(initializer);
        }

        public static void Initialize(SchemaBase schema)
        {
            List<IInitializer> list;
            if (cache.TryGetValue(schema.GetType().BaseType, out list))
            {
                list.ForEach(s => s.Initialize(StaticUnity.Profile, schema));
            }
        }
    }
}
