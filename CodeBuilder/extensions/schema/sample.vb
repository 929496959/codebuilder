Imports System.ComponentModel
Imports CodeBuilder.Core.Source

<SchemaExtension(GetType(Table))>
Public Class TableExt1
    <Description("��˾����")>
    <DefaultValue("fireasy")>
    Public Property Author As String
        Get
        End Get
        Set(value As String)
        End Set
    End Property
End Class

<SchemaExtension(GetType(Column))>
Public Class ColumnExt1
    <Description("��������")>
    <DefaultValue("fireasy")>
    Public Property Author As String
        Get
        End Get
        Set(value As String)
        End Set
    End Property
End Class