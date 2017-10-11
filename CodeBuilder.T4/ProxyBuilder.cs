// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using CodeBuilder.Core;
using CodeBuilder.Core.Source;
using CodeBuilder.Core.Variable;
using Fireasy.Common.Emit;
using Fireasy.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeBuilder.T4
{
    public class ProxyBuilder
    {
        private static ProxyType proxyType;
        private static Type profileType;
        private static List<string> assemblyList = new List<string>();

        /// <summary>
        /// 对要生成的表进行重建。
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<dynamic> Rebuild(List<Table> tables)
        {
            using (var objectPoll = new ObjectPoll())
            {
                var result = new List<dynamic>();
                if (proxyType == null)
                {
                    proxyType = BuildSchemaProxyType(assemblyList);
                }

                foreach (var table in tables)
                {
                    var newtable = CloneProxyObj(table, proxyType.TableType, objectPoll);
                    result.Add(newtable);
                }

                return result;
            }
        }

        /// <summary>
        /// 对 <see cref="Profile"/> 对象进行重建。
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static dynamic Rebuild(Profile profile)
        {
            using (var objectPoll = new ObjectPoll())
            {
                if (profileType == null)
                {
                    profileType = BuildProfileProxyType(profile.GetType(), assemblyList);
                }

                return CloneProxyObj(profile, profileType, objectPoll);
            }
        }

        public static List<string> GetAssemblyList()
        {
            return assemblyList;
        }

        private static object CloneProxyObj(object source, Type objType, ObjectPoll objectPoll)
        {
            if (source == null)
            {
                return null;
            }

            var result = objType.New();
            objectPoll.Push(result);
            objType = result.GetType();

            var pid = objType.GetProperty("_ID");
            if (pid != null)
            {
                var idvalue = source.GetType().GetProperty("_ID").FastGetValue(source);
                pid.FastSetValue(result, idvalue);
            }

            foreach (var property in source.GetType().GetProperties())
            {
                if (property.IsDefined<DisGenerateAttribute>())
                {
                    continue;
                }

                var p = objType.GetProperty(property.Name);
                if (p.Name == "_ID")
                {
                    continue;
                }

                var value = property.FastGetValue(source);
                var list = value as IList;
                if (list != null)
                {
                    var newlist = p.PropertyType.New<IList>();
                    var eleType = p.PropertyType.GetEnumerableElementType();
                    foreach (var item in list)
                    {
                        object newobj = null;
                        if (!objectPoll.Find(item, ref newobj))
                        {
                            newobj = CloneProxyObj(item, eleType, objectPoll);
                        }

                        newlist.Add(newobj);
                    }

                    value = newlist;
                }
                else if (!p.PropertyType.IsValueType && p.PropertyType != typeof(string))
                {
                    if (!objectPoll.Find(value, ref value))
                    {
                        value = CloneProxyObj(value, p.PropertyType, objectPoll);
                    }
                }

                p.FastSetValue(result, value);
            }

            return result;
        }

        private static ProxyType BuildSchemaProxyType(List<string> assemblyList)
        {
            var proxyType = new ProxyType();
            var fileName = Util.GenerateTempFileName();
            assemblyList.Add(fileName);
            var assemblyBuilder = new DynamicAssemblyBuilder("__Schema_Proxy", fileName);

            var tableTypeBuilder = assemblyBuilder.DefineType("Table", baseType: typeof(MarshalByRefObject));
            var columnTypeBuilder = assemblyBuilder.DefineType("Column", baseType: typeof(MarshalByRefObject));
            var referenceTypeBuilder = assemblyBuilder.DefineType("Reference", baseType: typeof(MarshalByRefObject));

            tableTypeBuilder.ImplementInterface(typeof(_IIdentity));
            columnTypeBuilder.ImplementInterface(typeof(_IIdentity));
            referenceTypeBuilder.ImplementInterface(typeof(_IIdentity));

            proxyType.TableType = tableTypeBuilder.UnderlyingSystemType;
            proxyType.ColumnType = columnTypeBuilder.UnderlyingSystemType;
            proxyType.ReferenceType = referenceTypeBuilder.UnderlyingSystemType;

            //定义Table代理类的属性
            foreach (var property in SchemaExtensionManager.GetWrapType<Table>().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyType = Replace(property.PropertyType, proxyType);
                tableTypeBuilder.DefineProperty(property.Name, propertyType).DefineGetSetMethods();
            }

            //定义Table代理类的属性
            foreach (var property in SchemaExtensionManager.GetWrapType<Column>().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyType = Replace(property.PropertyType, proxyType);
                columnTypeBuilder.DefineProperty(property.Name, propertyType).DefineGetSetMethods();
            }

            //定义Table代理类的属性
            foreach (var property in SchemaExtensionManager.GetWrapType<Reference>().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyType = Replace(property.PropertyType, proxyType);
                referenceTypeBuilder.DefineProperty(property.Name, propertyType).DefineGetSetMethods();
            }

            assemblyBuilder.Save();
            return proxyType;
        }

        private static Type BuildProfileProxyType(Type profileType, List<string> assemblyList)
        {
            var fileName = Util.GenerateTempFileName();
            assemblyList.Add(fileName);
            var assemblyBuilder = new DynamicAssemblyBuilder("__Profile_Proxy", fileName);

            var typeBuilder = assemblyBuilder.DefineType("Profile", baseType: typeof(MarshalByRefObject));

            //定义Table代理类的属性
            foreach (var property in profileType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                typeBuilder.DefineProperty(property.Name, property.PropertyType).DefineGetSetMethods();
            }

            assemblyBuilder.Save();
            return typeBuilder.UnderlyingSystemType;
        }

        private static Type Replace(Type type, ProxyType proxyType)
        {
            if (type.IsGenericType)
            {
                var arguments = type.GetGenericArguments();
                var isReplace = false;
                for (var i = 0; i < arguments.Length; i++)
                {
                    var replaceType = Replace(arguments[i], proxyType);
                    if (replaceType != arguments[i])
                    {
                        arguments[i] = replaceType;
                        isReplace = true;
                    }
                }

                if (isReplace)
                {
                    type = type.GetGenericTypeDefinition().MakeGenericType(arguments); 
                }

                return type;
            }
            else if (typeof(Table).IsAssignableFrom(type))
            {
                return proxyType.TableType;
            }
            else if (typeof(Column).IsAssignableFrom(type))
            {
                return proxyType.ColumnType;
            }
            else if (typeof(Reference).IsAssignableFrom(type))
            {
                return proxyType.ReferenceType;
            }

            return type;
        }

        private class ProxyType
        {
            public Type TableType { get; set; }

            public Type ColumnType { get; set; }

            public Type ReferenceType { get; set; }
        }

        private class ObjectPoll : IDisposable
        {
            private List<_IIdentity> objectPolls = new List<_IIdentity>();

            public void Push(object obj)
            {
                var _identity = obj as _IIdentity;
                if (_identity == null)
                {
                    return;
                }

                objectPolls.Add(_identity);
            }

            public bool Find(object source, ref object target)
            {
                var _identity = source as _IIdentity;
                if (_identity == null)
                {
                    return false;
                }

                var obj = objectPolls.FirstOrDefault(s => s._ID == _identity._ID);
                if (obj != null)
                {
                    target = obj;
                    return true;
                }

                return false;
            }

            public void Dispose()
            {
                objectPolls.Clear();
            }
        }
    }
}
