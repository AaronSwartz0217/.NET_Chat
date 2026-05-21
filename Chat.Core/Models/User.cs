using SqlSugar;

namespace Chat.Core.Models;

[SugarTable("users")]
public class User
{
    [SugarColumn(ColumnName = "id", 
        IsPrimaryKey = true, 
        IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "user_name", 
        Length = 30, 
        IsNullable = false)]
    public string UserName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "password", 
        Length = 200, 
        IsNullable = false)]
    public string Password { get; set; } = string.Empty;
}
