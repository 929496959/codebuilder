using CodeBuilder.Core.Source;
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
    public class SchemaExtensionManager
    {
        private static List<Type> compiledTypes = new List<Type>();
        private static Dictionary<Type, List<Type>> extensionTypes = new Dictionary<Type, List<Type>>();
        private static Dictionary<Type, Type> cache = new Dictionary<Type, Type>();
        private static Dictionary<Type, List<PropertyMap>> propertyCache = new Dictionary<Type, List<PropertyMap>>();

        public static void Initialize()
        {
            compiledTypes.Clear();
            extensionTypes.Clear();
            cache.Clear();
            propertyCache.Clear();

            compiledTypes = ComplileExtensionTypes();
            cache.Add(typeof(Table), GetWrapType<Table>());
            cache.Add(typeof(Column), GetWrapType<Column>());
            cache.Add(typeof(Reference), GetWrapType<Reference>());
        }

        /// <summary>
        /// 构造一个代理对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments">构造器参数。</param>
        /// <returns></returns>
        public static T Build<T>(params object[] arguments)
        {
            try
            {
                var wrapType = GetWrapType<T>();
                return InitializeDefaultValue(wrapType.New<T>(arguments));
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        /// <summary>
        /// 获取包装过的类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type GetWrapType<T>()
        {
            var schemaType = typeof(T);
            Type wrapType = null;
            if (cache.TryGetValue(schemaType, out wrapType))
            {
                return wrapType;
            }

            var extendTypes = FindExtensionTypes(schemaType);
            var properties = extendTypes.SelectMany(s => s.GetProperties(BindingFlags.Public | BindingFlags.Instance)).ToArray();
            if (properties.Length > 0)
            {
                return BuildWrapType(schemaType, properties);
            }

            return typeof(T);
        }

        public static List<PropertyMap> GetPropertyMaps<T>()
        {
            var type = GetWrapType<T>();
            return propertyCache.TryGetValue(typeof(T), () =>
                {
                    return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(s => s.Name != "_ID" && !s.IsDefined<DisGenerateAttribute>())
                        .Select(s => new PropertyMap(s)).ToList();
                });
        }

        /// <summary>
        /// 在已编译的类列表中查找适合扩展的类。
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        private static List<Type> FindExtensionTypes(Type schemaType)
        {
            return extensionTypes.TryGetValue(schemaType, () =>
                {
                    var result = new List<Type>();

                    foreach (var type in compiledTypes)
                    {
                        foreach (var attr in type.GetCustomAttributes<SchemaExtensionAttribute>(false).Where(s => s.SchemaType == schemaType))
                        {
                            result.Add(type);
                        }

                        foreach (var attr in type.GetCustomAttributes<SchemaInitializerAttribute>(false).Where(s => s.SchemaType == schemaType))
                        {
                            var initializer = type.New<IInitializer>();
                            if (initializer != null)
                            {
                                InitializerUnity.Register(schemaType, initializer);
                            }
                        }
                    }

                    return result;
                });
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

                var compiler = new Fireasy.Common.Compiler.CodeCompiler();
                var fileName = Util.GenerateTempFileName();
                StaticUnity.DynamicAssemblies.Add(fileName);

                compiler.OutputAssembly = fileName;
                compiler.CodeProvider = GetCodeProvider(ext);
                compiler.Assemblies.Add("System.Core.dll");
                compiler.Assemblies.Add("Microsoft.CSharp.dll");
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
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extensions\\schema");
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
        /// 初始化对象的默认值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T InitializeDefaultValue<T>(T obj)
        {
            var map = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(s => s.CanWrite)
                .Select(s => new { Property = s, DefaultValue = s.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault() })
                .Where(s => s.DefaultValue != null)
                .ToArray();

            foreach (var item in map)
            {
                item.Property.FastSetValue(obj, item.DefaultValue.Value);
            }

            return obj;
        }

        /// <summary>
        /// 使用扩展属性对架构类进行包装。
        /// </summary>
        /// <param name="schemaType">架构类。</param>
        /// <param name="properties">扩展的属性列表。</param>
        /// <returns></returns>
        private static Type BuildWrapType(Type schemaType, PropertyInfo[] properties)
        {
            var dyAssemblyBuilder = new DynamicAssemblyBuilder("SchemaExtension_" + schemaType.Name);
            var dyTypeBuilder = dyAssemblyBuilder.DefineType(schemaType.Name, baseType: schemaType);

            foreach (var property in properties)
            {
                var defaultValue = GetPropertyDefaultValue(property);
                var dyPropertyBuilder = dyTypeBuilder.DefineProperty(property.Name, property.PropertyType);
                dyPropertyBuilder.DefineGetSetMethods();
                SetPropertyCustomAttributes(property, dyPropertyBuilder);
            }

            foreach (var con in schemaType.GetConstructors())
            {
                var parameters = con.GetParameters();
                var dyConsBuilder = dyTypeBuilder.DefineConstructor(parameters.Select(s => s.ParameterType).ToArray(), ilCoding: (b) =>
                    {
                        b.Emitter
                            .ldarg_0
                            .For(1, parameters.Length + 1, (e, i) => e.ldarg(i))
                            .call(con).ret();
                    });
            }

            return dyTypeBuilder.CreateType();
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
            var ediAttr = property.GetCustomAttributes<EditorAttribute>().FirstOrDefault();

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

            if (ediAttr != null)
            {
                var editorType = Type.GetType(ediAttr.EditorTypeName);
                if (editorType == null)
                {
                    editorType = compiledTypes.FirstOrDefault(s => s.AssemblyQualifiedName == ediAttr.EditorTypeName);
                }

                dyPropertyBuilder.SetCustomAttribute<EditorAttribute>(editorType, Type.GetType(ediAttr.EditorBaseTypeName));
            }
        }
    }
}
