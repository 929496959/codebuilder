// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Common.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeBuilder.Core.Template
{
    public class PartitionWriter
    {
        private static Dictionary<string, List<AccessorMap>> cache = new Dictionary<string, List<AccessorMap>>();

        internal static void ClearCache()
        {
            cache.Clear();
        }

        public static void Write(GenerateResult result, object schema, object profile, string output)
        {
            var fileName = result.Partition.Output;
            fileName = RegexOutput(schema, profile, fileName);

            var path = Path.Combine(output, fileName);
            var dir = path.Substring(0, path.LastIndexOf("\\"));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, result.Content, Encoding.Default);
        }

        private static string RegexOutput(object schema, object profile, string output)
        {
            var mappers = cache.TryGetValue(output, () =>
                {
                    var list = new List<AccessorMap>();
                    var schemaType = schema != null ? schema.GetType() : null;
                    var profileType = profile != null ? profile.GetType() : null;
                    var regex = new Regex(@"\{(\w+)\}");
                    var matches = regex.Matches(output);
                    foreach (Match match in matches)
                    {
                        var group = match.Groups[0];
                        var name = group.Value.Substring(1, group.Value.Length - 2);

                        if (schemaType != null)
                        {
                            var property = schemaType.GetProperty(name);
                            if (property != null)
                            {
                                list.Add(new AccessorMap
                                {
                                    GroupName = group.Value,
                                    Accessor = ReflectionCache.GetAccessor(property),
                                    ObjectType = ObjectType.Schema
                                });
                            }
                        }

                        if (profileType != null)
                        {
                            var property = profileType.GetProperty(name);
                            if (property != null)
                            {
                                list.Add(new AccessorMap
                                {
                                    GroupName = group.Value,
                                    Accessor = ReflectionCache.GetAccessor(property),
                                    ObjectType = ObjectType.Profile
                                });
                            }
                        }
                    }

                    return list;
                });

            foreach (var p in mappers)
            {
                var value = GetPropertyValue(schema, profile, p);
                output = output.Replace(p.GroupName, value);
            }

            return output;
        }

        private static string GetPropertyValue(object schema, object profile, AccessorMap map)
        {
            if (map.ObjectType == ObjectType.Schema)
            {
                return map.Accessor.GetValue(schema).ToStringSafely();
            }
            else if (map.ObjectType == ObjectType.Profile)
            {
                return map.Accessor.GetValue(profile).ToStringSafely();
            }

            return string.Empty;
        }

        private class AccessorMap
        {
            public string GroupName { get; set; }

            public PropertyAccessor Accessor { get; set; }

            public ObjectType ObjectType { get; set; }
        }

        private enum ObjectType
        {
            Schema,
            Profile
        }
    }
}
