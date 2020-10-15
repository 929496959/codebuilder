using System.ComponentModel;
using CodeBuilder.Core;
using CodeBuilder.Core.Source;
using System.Text;
using System.Text.RegularExpressions;

public enum NameMode
{
    Inherit, //延续
    Pascal //首字母大写，适用于用-分隔的命名
}

[SchemaExtension(typeof(Table))]
public class TableExt1
{
    [Description("公司名称")]
    [DefaultValue("fireasy")]
    public string Company { get; set; }
}

[SchemaExtension(typeof(Column))]
public class ColumnExt1
{
    [Description("公司名称")]
    [DefaultValue("fireasy")]
    public string Company { get; set; }
}

[SchemaInitializer(typeof(Table))]
public class ClassNameInitializer : IInitializer
{
    public void Initialize(dynamic profile, SchemaBase schema)
    {
        var table = schema as Table;
        var tableName = table.Name;

        if (!string.IsNullOrEmpty(profile.TableRegex))
        {
            var regx = new Regex(profile.TableRegex);
            if (regx.IsMatch(tableName))
            {
                tableName = regx.Replace(tableName, string.Empty);
            }
        }

        if ((int)profile.NameMode == (int)NameMode.Pascal)
        {
            table.ClassName = SchemaNameFormatter.Format(profile, tableName);
        }
        else
        {
            table.ClassName = tableName;
        }
    }
}

[SchemaInitializer(typeof(Column))]
public class PropertyNameInitializer : IInitializer
{
    public void Initialize(dynamic profile, SchemaBase schema)
    {
        var column = schema as Column;
        var columnName = column.Name;

        if (!string.IsNullOrEmpty(profile.ColumeRegex))
        {
            var regx = new Regex(profile.ColumeRegex);
            if (regx.IsMatch(columnName))
            {
                columnName = regx.Replace(columnName, string.Empty);
            }
        }

        if ((int)profile.NameMode == (int)NameMode.Pascal)
        {
            column.PropertyName = SchemaNameFormatter.Format(profile, columnName);
        }
        else
        {
            column.PropertyName = columnName;
        }
    }
}

internal class SchemaNameFormatter
{
    public static string Format(dynamic profile, string name)
    {
        var sb = new StringBuilder();
        foreach (var arr in name.Split('_', ' '))
        {
            if (arr.Length > 0)
            {
                sb.Append(arr.Substring(0, 1).ToUpper());
                sb.Append(arr.Substring(1).ToLower());
            }
        }

        return sb.ToString();
    }
}