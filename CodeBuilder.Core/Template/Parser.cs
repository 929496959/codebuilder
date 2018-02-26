using Fireasy.Common.Extensions;
using Fireasy.Common.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeBuilder.Core.Template
{
    public class Parser
    {
        private Dictionary<string, List<AccessorMap>> cache = new Dictionary<string, List<AccessorMap>>();

        public string Parse(object schema, object profile, string output)
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
