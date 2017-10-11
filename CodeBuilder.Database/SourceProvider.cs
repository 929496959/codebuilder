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
using Fireasy.Common.Extensions;
using Fireasy.Data.Extensions;
using Fireasy.Data.Provider;
using Fireasy.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows.Forms;
using Schema = Fireasy.Data.Schema;

namespace CodeBuilder.Database
{
    [Export(typeof(ISourceProvider))]
    public class SourceProvider : ISourceProvider
    {
        private DbSourceStruct con;

        public string Name
        {
            get { return "Database"; }
        }

        public List<Table> Preview()
        {
            IEnumerable<Table> tables = null;
            using (var frm = new frmSourceMgr())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    con = frm.DbConStr;
                    tables = OpenDb();
                }
            }

            if (tables == null)
            {
                return null;
            }

            using (var frm = new frmTableSelector(tables))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return frm.Selected.Count == 0 ? null : frm.Selected;
                }
            }

            return null;
        }

        public List<Table> GetSchema(List<Table> tables, TableSchemaProcessHandler reding)
        {
            try
            {
                return GetDbSchema(tables, reding);
            }
            catch (Exception exp)
            {
                ErrorMessageBox.Show("获取数据架构时出错。", exp);
                return null;
            }
        }
        
        public List<string> GetHistory()
        {
            return new List<string>();
        }

        private IEnumerable<Table> OpenDb()
        {
            try
            {
                var provider = ProviderHelper.GetDefinedProviderInstance(con.Type);
                using (var db = new Fireasy.Data.Database(con.ConnectionString, provider))
                {
                    var schemaName = db.Provider.GetConnectionParameter(db.ConnectionString).Schema;
                    var schema = db.Provider.GetService<Schema.ISchemaProvider>();
                    var tables = schema.GetSchemas<Schema.Table>(db, s => s.Schema == schemaName);
                    return ConvertSchemaTables(tables);
                }
            }
            catch (Exception exp)
            {
                ErrorMessageBox.Show("获取数据架构时出错。", exp);
                return null;
            }
        }

        private List<Table> GetDbSchema(IList<Table> tables, TableSchemaProcessHandler processHandler)
        {
            var result = new List<Table>();
            var provider = ProviderHelper.GetDefinedProviderInstance(con.Type);
            using (var db = new Fireasy.Data.Database(con.ConnectionString, provider))
            {
                var providerName = db.Provider.GetType().Name;
                var schemaName = db.Provider.GetConnectionParameter(db.ConnectionString).Schema;
                var schema = db.Provider.GetService<Schema.ISchemaProvider>();
                var foreignKeys = schema.GetSchemas<Schema.ForeignKey>(db, s => s.Schema == schemaName).ToList();
                var dbTypes = schema.GetSchemas<Schema.DataType>(db).ToList();
                var tableCount = tables.Count;
                var index = 0;

                var calc = new Func<int, int>(i =>
                    {
                        return (int)((i / (tableCount * 1.0)) * 100);
                    });

                var host = new Host();

                foreach (var t in tables)
                {
                    if (Processor.IsCancellationRequested())
                    {
                        return null;
                    }

                    if (processHandler != null)
                    {
                        processHandler(t.Name, calc(++index));
                    }

                    var columns = schema.GetSchemas<Schema.Column>(db, s => s.Schema == schemaName && s.TableName == t.Name);
                    t.Columns.AddRange(ConvertSchemaColumns(t, columns, dbTypes, providerName));
                    result.Add(t);
                    host.Attach(t);
                }

                ProcessForeignKeys(result, foreignKeys);
            }

            return result;
        }

        private IEnumerable<Table> ConvertSchemaTables(IEnumerable<Schema.Table> tables)
        {
            foreach (var t in tables)
            {
                if (t.Type.ToUpper().Contains("SYS"))
                {
                    continue;
                }

                var table = SchemaExtensionManager.Build<Table>();
                table.Name = t.Name;
                table.Description = t.Description ?? string.Empty;

                InitializerUnity.Initialize(table);

                yield return table;
            }
        }

        private IEnumerable<Column> ConvertSchemaColumns(Table table, IEnumerable<Schema.Column> columns, List<Schema.DataType> dbTypes, string providerName)
        {
            foreach (var c in columns)
            {
                if (Processor.IsCancellationRequested())
                {
                    yield break;
                }

                var column = SchemaExtensionManager.Build<Column>(table);

                column.Name = c.Name;
                column.AutoIncrement = c.Autoincrement;
                column.Description = c.Description ?? string.Empty;
                column.IsNullable = c.IsNullable;
                column.DataType = c.DataType;
                column.Length = c.Length;
                column.Scale = c.NumericScale;
                column.Precision = c.NumericPrecision;
                column.IsPrimaryKey = c.IsPrimaryKey;
                column.DbType = ConvertDbType(column, dbTypes, providerName);

                InitializerUnity.Initialize(column);

                yield return column;
            }
        }

        /// <summary>
        /// 处理外键。
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="foreignKeys"></param>
        private void ProcessForeignKeys(List<Table> tables, List<Schema.ForeignKey> foreignKeys)
        {
            foreach (var sfk in foreignKeys)
            {
                if (Processor.IsCancellationRequested())
                {
                    break;
                }

                var pkTable = tables.FirstOrDefault(s => s.Name.Equals(sfk.PKTable, StringComparison.CurrentCultureIgnoreCase));
                var fkTable = tables.FirstOrDefault(s => s.Name.Equals(sfk.TableName, StringComparison.CurrentCultureIgnoreCase));
                if (pkTable != null && fkTable != null)
                {
                    var pkColumn = pkTable.Columns.FirstOrDefault(s => s.Name.Equals(sfk.PKColumn, StringComparison.CurrentCultureIgnoreCase));
                    var fkColumn = fkTable.Columns.FirstOrDefault(s => s.Name.Equals(sfk.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                    
                    if (pkColumn != null && fkColumn != null)
                    {
                        var fk = SchemaExtensionManager.Build<Reference>(pkTable, pkColumn, fkTable, fkColumn);
                        fk.Name = sfk.Name;
                        fkColumn.BindForeignKey(fk);
                    }
                }
            }
        }

        private DbType? ConvertDbType(Column column, List<Schema.DataType> dbTypes, string providerName)
        {
            if (providerName == "OleDbProvider")
            {
                var oleDbType = (OleDbType)Convert.ToInt32(column.DataType);
                column.DataType = oleDbType.ToString();
                switch (oleDbType)
                {
                    case OleDbType.VarChar:
                    case OleDbType.VarWChar:
                    case OleDbType.WChar:
                    case OleDbType.Char:
                        return DbType.String;
                    case OleDbType.SmallInt:
                        return DbType.Int16;
                    case OleDbType.Integer:
                        return DbType.Int32;
                    case OleDbType.BigInt:
                        return DbType.Int64;
                    case OleDbType.Binary:
                        return DbType.Binary;
                    case OleDbType.Boolean:
                        return DbType.Boolean;
                    case OleDbType.Currency:
                        return DbType.Currency;
                    case OleDbType.Date:
                    case OleDbType.DBDate:
                    case OleDbType.DBTime:
                    case OleDbType.DBTimeStamp:
                        return DbType.DateTime;
                    case OleDbType.Numeric:
                    case OleDbType.Decimal:
                    case OleDbType.VarNumeric:
                        return DbType.Decimal;
                    case OleDbType.Double:
                        return DbType.Double;
                    case OleDbType.Single:
                        return DbType.Single;
                    case OleDbType.TinyInt:
                        return DbType.SByte;
                    case OleDbType.UnsignedSmallInt:
                        return DbType.UInt16;
                    case OleDbType.UnsignedInt:
                        return DbType.UInt32;
                    case OleDbType.UnsignedBigInt:
                        return DbType.UInt64;
                    case OleDbType.UnsignedTinyInt:
                        return DbType.Byte;
                    case OleDbType.Guid:
                        return DbType.Guid;
                }
            }
            else
            {
                var dbType = dbTypes.FirstOrDefault(s => s.Name.Equals(column.DataType, StringComparison.CurrentCultureIgnoreCase));
                if (dbType != null)
                {
                    return dbType.SystemType.GetDbType();
                }
            }

            return null;
        }
    }
}
