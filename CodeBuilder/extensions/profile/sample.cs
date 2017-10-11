using System.ComponentModel;

public class ProfileExt1
{
    [Description("命名空间。")]
    public string Namespace { get; set; }
	
    [Description("项目名称。")]
    public string ProjectName { get; set; }
	
    [Description("公司名称。")]
    public string CompanyName { get; set; }

    [Description("作者。")]
    public string Author { get; set; }
    
    [Description("表名转类名的替换正则。")]
    public string TableRegex { get; set; }
        
    [Description("字段名转类名的替换正则。")]
    public string ColumneRegex { get; set; }

    [Description("Mvc中区域的名称。")]
    public string MvcArea { get; set; }

    [Description("实体模型。LighEntity 轻量型实体；DependProperty 依赖属性模式。")]
    public EntityMode EntityMode { get; set; }

    [Description("命名方式。Pascal 首字母大写，适用于用-分隔的命名；Extend 延续原始名称。")]
    public NameMode NameMode { get; set; }
}

public enum EntityMode
{
    LighEntity,
    DependProperty
}

public enum NameMode
{
    Extend, //延续
    Pascal //首字母大写，适用于用-分隔的命名
}