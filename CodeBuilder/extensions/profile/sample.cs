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
    public string ColumeRegex { get; set; }

    [Description("Mvc中区域的名称。")]
    public string MvcArea { get; set; }

    [Description("命名方式。Pascal 首字母大写，适用于用-分隔的命名；Inherit 延续原始名称。")]
    public NameMode NameMode { get; set; }
}

public enum NameMode
{
    Inherit, //延续
    Pascal //首字母大写，适用于用-分隔的命名
}