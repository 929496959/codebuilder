// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core.Variable;
using Fireasy.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBuilder.Core
{
    public class ProfileUnity
    {
        public static Profile LoadCurrent()
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "profile.cfg");
            return LoadFile(fileName);
        }

        public static void SaveFile(Profile profile)
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "profile.cfg");
            SaveFile(profile, fileName);
        }

        public static Profile LoadFile(string fileName)
        {
            var profile = ProfileExtensionManager.Build();
            var profileType = profile.GetType();
            var content = File.ReadAllText(fileName, Encoding.Default);
            var json = new JsonSerializer();
            var dyobj = (IDictionary<string, object>)json.Deserialize<dynamic>(content);

            foreach (var kvp in dyobj)
            {
                var p = profileType.GetProperty(kvp.Key);
                if (p != null)
                {
                    p.SetValue(profile, kvp.Value, null);
                }
            }

            return profile;
        }

        public static void SaveFile(Profile profile, string fileName)
        {
            var option = new JsonSerializeOption { Indent = true };
            var json = new JsonSerializer(option);
            var content = json.Serialize(profile);
            File.WriteAllText(fileName, content, Encoding.Default);
        }
    }
}