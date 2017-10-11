// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBuilder
{
    public class Config
    {
        static Config()
        {
            Instance = Read();
        }

        public static Config Instance;

        public string TemplateProvider { get; set; }

        public string TemplateFileName { get; set; }

        public string OutputDirectory { get; set; }

        public static Config Read()
        {
            var json = new JsonSerializer();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "app.cfg");
            var content = File.ReadAllText(path, Encoding.Default);
            return json.Deserialize<Config>(content);
        }

        public static void Save()
        {
            var option = new JsonSerializeOption { Indent = true };
            var json = new JsonSerializer(option);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "app.cfg");
            var content = json.Serialize(Config.Instance);
            File.WriteAllText(path, content, Encoding.Default);
        }
    }
}
