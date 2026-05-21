using SqlSugar;

namespace Chat.Core.Models;

[SugarTable("students", TableDescription = "学生信息表")]
public class Student
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)] // 主键+自增
    public int Id { get; set; }

    [SugarColumn(ColumnName = "no", Length = 10, IsNullable = false, ColumnDescription = "学号")]
    public string No { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "name", Length = 30, IsNullable = false, ColumnDescription = "姓名")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "id_number", Length = 18, IsNullable = false, ColumnDescription = "身份证")]
    public string IdNumber { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "gender", IsNullable = false, ColumnDescription = "性别")]
    public EnumGender Gender { get; set; }

    [SugarColumn(ColumnName = "ethnic_group", IsNullable = false, ColumnDescription = "民族")]
    public EnumEthnicGroup EthnicGroup { get; set; }

    [SugarColumn(ColumnName = "native_place", IsNullable = true, ColumnDescription = "籍贯地")]
    public string? NativePlace { get; set; }

    [SugarColumn(ColumnName = "birthday", IsNullable = false, ColumnDescription = "出生日期")]
    public DateTime Birthday { get; set; }

    [SugarColumn(ColumnName = "weight", ColumnDescription = "体重(公斤)")]
    public int Weight { get; set; }

    [SugarColumn(ColumnName = "height", DecimalDigits = 2, Length = 3, ColumnDescription = "身高(米)")]
    public decimal Height { get; set; }

    [SugarColumn(ColumnName = "created_time", IsOnlyIgnoreUpdate = true)]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
    public long Ver { get; set; }
}

public enum EnumGender
{
    男,
    女
}

public enum EnumEthnicGroup
{
    汉族,
    壮族,
    满族,
    回族,
    苗族,
    维吾尔族,
    土家族,
    彝族,
    蒙古族,
    藏族,
    布依族,
    侗族,
    瑶族,
    朝鲜族,
    白族,
    哈尼族,
    哈萨克族,
    黎族,
    傣族,
    畲族,
    傈僳族,
    仡佬族,
    东乡族,
    高山族,
    拉祜族,
    水族,
    佤族,
    纳西族,
    羌族,
    土族,
    仫佬族,
    锡伯族,
    柯尔克孜族,
    达斡尔族,
    景颇族,
    毛南族,
    撒拉族,
    布朗族,
    塔吉克族,
    阿昌族,
    普米族,
    鄂温克族,
    怒族,
    京族,
    基诺族,
    德昂族,
    保安族,
    俄罗斯族,
    裕固族,
    乌孜别克族,
    门巴族,
    鄂伦春族,
    独龙族,
    塔塔尔族,
    赫哲族,
    珞巴族
}