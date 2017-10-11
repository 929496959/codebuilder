using System;
using System.ComponentModel;
using CodeBuilder.Core.Source;

[SchemaExtension(typeof(Column))]
public class ColumnExtForEasyUI
{
    [Category("EasyUI")]
    [Description("EasyUI控件类别。")]
    public EasyUIFieldType ControlType { get; set; }

    [Category("EasyUI")]
    [Description("是否生成域。")]
    [DefaultValue(true)]
    public bool GenerateField { get; set; }
}

public enum EasyUIFieldType
{
    TextBox,
    NumberBox,
    ComboBox,
    ComboTree,
    DateBox,
    DateTimeBox,
}