using Chat.Core.Models;
using System;
using System.Collections.Generic;

namespace Chat.Application.Dtos;

/// <summary>
/// 组合的账号档案模型 - 合并User和Student
/// User字段（必填）：UserName, Password
/// Student档案字段（可选）：No, Name, IdNumber, Gender, EthnicGroup, NativePlace, Birthday, Weight, Height
/// </summary>
public class AccountProfileDto
{
    // ===== User表字段（账号信息）=====
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime UserCreatedTime { get; set; }

    // ===== Student表字段（学生档案，全部可选）=====
    /// <summary>学号（可选）</summary>
    public string? No { get; set; }

    /// <summary>姓名（可选）</summary>
    public string? Name { get; set; }

    /// <summary>身份证号（可选）</summary>
    public string? IdNumber { get; set; }

    /// <summary>性别（可选）</summary>
    public EnumGender? Gender { get; set; }

    /// <summary>民族（可选）</summary>
    public EnumEthnicGroup? EthnicGroup { get; set; }

    /// <summary>籍贯（可选）</summary>
    public string? NativePlace { get; set; }

    /// <summary>出生日期（可选）</summary>
    public DateTime? Birthday { get; set; }

    /// <summary>体重（可选）</summary>
    public int? Weight { get; set; }

    /// <summary>身高（可选）</summary>
    public decimal? Height { get; set; }

    /// <summary>档案是否已完善（有任意学生字段即为true）</summary>
    public bool HasProfile { get; set; }
}

/// <summary>
/// 创建账号请求（可同时完善档案）
/// </summary>
public class CreateAccountRequest
{
    // 必填
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // 可选：学生档案
    public string? No { get; set; }
    public string? Name { get; set; }
    public string? IdNumber { get; set; }
    public EnumGender? Gender { get; set; }
    public EnumEthnicGroup? EthnicGroup { get; set; }
    public string? NativePlace { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Weight { get; set; }
    public decimal? Height { get; set; }
}

/// <summary>
/// 登录返回结果
/// </summary>
public class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AccountProfileDto? Account { get; set; }
}

/// <summary>
/// 账号列表项（用于展示）
/// </summary>
public class AccountListItem
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? No { get; set; }
    public bool HasProfile { get; set; }
    public DateTime UserCreatedTime { get; set; }
}
