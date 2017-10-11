// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using Fireasy.Common.Extensions;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeBuilder.Core.Variable
{
    public class ProfileExtensionManager
    {
        private static Type wrapType;
        private static List<Type> extendTypes = new List<Type>();
        private static List<PropertyMap> propertyCache = null;

        public static void Initialize()
        {
            extendTypes = ComplileExtensionTypes();
            propertyCache = null;
            wrapType = GetWrapType();
        }

        public static Profile Build()
        {
            return wrapType.New<Profile>();
        }

        public static Type GetWrapType()
        {
            if (wrapType == null)
            {
                var properties = extendTypes.SelectMany(s => s.GetProperties(BindingFlags.Public | BindingFlags.Instance)).ToArray();

                if (properties.Length > 0)
                {
                    try
                    {
                        wrapType = BuildWrapType(properties);
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
            }
            else
            {
                wrapType = typeof(Profile);
            }

            return wrapType;
        }

        public static List<PropertyMap> GetPropertyMaps()
        {
            if (propertyCache == null)
            {
                var type = GetWrapType();
                propertyCache = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(s => s.Name != "_ID").Select(s => new PropertyMap(s)).ToList();
            }

            return propertyCache;
        }

        /// <summary>
        /// 动态编译扩展动态类。
        /// </summary>
        /// <returns></returns>
        private static List<Type> ComplileExtensionTypes()
        {
            var _pluginTypes = new List<Type>();
            foreach (var ext in new[] { ".cs", ".vb" })
            {
                var files = GetExtensionFiles(ext);

                if (files.Length == 0)
                {
                    continue;
                }

                var fileName = Util.GenerateTempFileName();
                StaticUnity.DynamicAssemblies.Add(fileName);

                var compiler = new Fireasy.Common.Compiler.CodeCompiler();

                compiler.OutputAssembly = fileName;
                compiler.CodeProvider = GetCodeProvider(ext);
                compiler.Assemblies.Add("System.Drawing.dll");
                compiler.Assemblies.Add("System.Windows.Forms.dll");
                compiler.Assemblies.Add("CodeBuilder.Core.dll");
                compiler.Assemblies.Add("Fireasy.Common.dll");
                _pluginTypes.AddRange(compiler.CompileAssembly(files).GetExportedTypes());
            }

            return _pluginTypes;
        }

        /// <summary>
        /// 获取目录下的可扩展的代码文件。
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        private static string[] GetExtensionFiles(string ext)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extensions\\profile");
            if (!Directory.Exists(path))
            {
                return new string[0];
            }

            return Directory.GetFiles(path, "*" + ext);
        }

        /// <summary>
        /// 根据文件扩展名获取相应的 <see cref="CodeDomProvider"/>。
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        private static CodeDomProvider GetCodeProvider(string ext)
        {
            if (ext == ".vb")
            {
                return new VBCodeProvider();
            }

            return new CSharpCodeProvider();
        }


        /// <summary>
        /// 使用扩展属性对架构类进行包装。
        /// </summary>
        /// <param name="schemaType">架构类。</param>
        /// <param name="properties">扩展的属性列表。</param>
        /// <returns></returns>
        private static Type BuildWrapType(PropertyInfo[] properties)
        {
            var dyAssemblyBuilder = new DynamicAssemblyBuilder("ProfileExtension");
            var dyTypeBuilder = dyAssemblyBuilder.DefineType("ProfileEx", baseType: typeof(Profile));

            foreach (var property in properties)
            {
                var defaultValue = GetPropertyDefaultValue(property);
                var dyPropertyBuilder = dyTypeBuilder.DefineProperty(property.Name, property.PropertyType);
                dyPropertyBuilder.DefineGetSetMethods();
                SetPropertyCustomAttributes(property, dyPropertyBuilder);
            }

            var wrapType = dyTypeBuilder.CreateType();
            propertyCache = wrapType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(s => new PropertyMap(s)).ToList();
            return wrapType;
        }

        /// <summary>
        /// 获取属性的默认值。
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static object GetPropertyDefaultValue(PropertyInfo property)
        {
            var defAttr = property.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
            if (defAttr != null)
            {
                return defAttr.Value;
            }

            return null;
        }

        /// <summary>
        /// 设置属性的自定义特性。
        /// </summary>
        /// <param name="property"></param>
        /// <param name="dyPropertyBuilder"></param>
        private static void SetPropertyCustomAttributes(PropertyInfo property, DynamicPropertyBuilder dyPropertyBuilder)
        {
            var broAttr = property.GetCustomAttributes<BrowsableAttribute>().FirstOrDefault();
            var catAttr = property.GetCustomAttributes<CategoryAttribute>().FirstOrDefault();
            var desAttr = property.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            var defAttr = property.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
            var disAttr = property.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault();

            if (broAttr != null)
            {
                dyPropertyBuilder.SetCustomAttribute<BrowsableAttribute>(broAttr.Browsable);
            }

            if (catAttr != null)
            {
                dyPropertyBuilder.SetCustomAttribute<CategoryAttribute>(catAttr.Category);
            }

            if (desAttr != null)
            {
                dyPropertyBuilder.SetCustomAttribute<DescriptionAttribute>(desAttr.Description);
            }

            if (defAttr != null)
            {
                dyPropertyBuilder.SetCustomAttribute<DefaultValueAttribute>(defAttr.Value);
            }

            if (disAttr != null)
            {
                dyPropertyBuilder.SetCustomAttribute<DisplayNameAttribute>(disAttr.DisplayName);
            }
        }
    }
}
