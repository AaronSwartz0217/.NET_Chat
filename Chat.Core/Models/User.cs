using SqlSugar;
using System;

namespace Chat.Core.Models;

[SugarTable("users")]
public class User
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "user_name", Length = 30, IsNullable = false)]
    public string UserName { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "password", Length = 200, IsNullable = false)]
    public string Password { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "email", Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    [SugarColumn(ColumnName = "nickname", Length = 50, IsNullable = true)]
    public string? Nickname { get; set; }

    [SugarColumn(ColumnName = "avatar", Length = 255, IsNullable = true)]
    public string? Avatar { get; set; }

    [SugarColumn(ColumnName = "signature", Length = 500, IsNullable = true)]
    public string? Signature { get; set; }

    [SugarColumn(ColumnName = "online_status")]
    public bool OnlineStatus { get; set; } = false;

    [SugarColumn(ColumnName = "last_login_time", IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    [SugarColumn(ColumnName = "role", Length = 20)]
    public string Role { get; set; } = "user";
}
