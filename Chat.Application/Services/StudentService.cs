using Chat.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class StudentService : IStudentService
{
    private readonly ISqlSugarClient _db;

    public StudentService(ISqlSugarClient db)
    {
        _db = db;
        _db.CodeFirst.InitTables<Student>();
    }

    public async Task<List<Student>> GetStudentsAsync()
    {
        return await _db.Queryable<Student>().ToListAsync();
    }

    public async Task<bool> BatchInsertStudentsAsync()
    {
        var students = new List<Student>
        {
            new Student { No = "2024001", Name = "张伟", IdNumber = "110101200501151011", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.汉族, NativePlace = "北京", Birthday = new DateTime(2005, 1, 15), Weight = 65, Height = 1.78m },
            new Student { No = "2024002", Name = "李娜", IdNumber = "320102200402282022", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.汉族, NativePlace = "江苏", Birthday = new DateTime(2004, 2, 28), Weight = 52, Height = 1.65m },
            new Student { No = "2024003", Name = "王强", IdNumber = "440103200503121033", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.壮族, NativePlace = "广东", Birthday = new DateTime(2005, 3, 12), Weight = 70, Height = 1.82m },
            new Student { No = "2024004", Name = "刘芳", IdNumber = "310104200504202044", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.满族, NativePlace = "上海", Birthday = new DateTime(2005, 4, 20), Weight = 48, Height = 1.58m },
            new Student { No = "2024005", Name = "陈明", IdNumber = "610105200405181055", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.回族, NativePlace = "陕西", Birthday = new DateTime(2004, 5, 18), Weight = 58, Height = 1.72m },
            new Student { No = "2024006", Name = "杨丽", IdNumber = "530106200506252066", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.苗族, NativePlace = "云南", Birthday = new DateTime(2005, 6, 25), Weight = 50, Height = 1.62m },
            new Student { No = "2024007", Name = "赵磊", IdNumber = "650107200407121077", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.维吾尔族, NativePlace = "新疆", Birthday = new DateTime(2004, 7, 12), Weight = 72, Height = 1.85m },
            new Student { No = "2024008", Name = "周婷", IdNumber = "430108200508202088", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.土家族, NativePlace = "湖南", Birthday = new DateTime(2005, 8, 20), Weight = 55, Height = 1.68m },
            new Student { No = "2024009", Name = "吴刚", IdNumber = "510109200409151099", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.彝族, NativePlace = "四川", Birthday = new DateTime(2004, 9, 15), Weight = 62, Height = 1.75m },
            new Student { No = "2024010", Name = "郑雪", IdNumber = "150101200510102100", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.蒙古族, NativePlace = "内蒙古", Birthday = new DateTime(2005, 10, 10), Weight = 53, Height = 1.66m },
            new Student { No = "2024011", Name = "孙鹏", IdNumber = "520102200511221111", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.藏族, NativePlace = "贵州", Birthday = new DateTime(2005, 11, 22), Weight = 68, Height = 1.80m },
            new Student { No = "2024012", Name = "马超", IdNumber = "450103200412081122", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.布依族, NativePlace = "广西", Birthday = new DateTime(2004, 12, 8), Weight = 60, Height = 1.70m },
            new Student { No = "2024013", Name = "朱琳", IdNumber = "330104200502142133", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.侗族, NativePlace = "浙江", Birthday = new DateTime(2005, 2, 14), Weight = 49, Height = 1.60m },
            new Student { No = "2024014", Name = "胡军", IdNumber = "340105200403051144", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.瑶族, NativePlace = "安徽", Birthday = new DateTime(2004, 3, 5), Weight = 75, Height = 1.88m },
            new Student { No = "2024015", Name = "林静", IdNumber = "220106200504182155", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.朝鲜族, NativePlace = "吉林", Birthday = new DateTime(2005, 4, 18), Weight = 51, Height = 1.64m },
            new Student { No = "2024016", Name = "何伟", IdNumber = "530307200405251166", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.白族, NativePlace = "云南", Birthday = new DateTime(2004, 5, 25), Weight = 66, Height = 1.76m },
            new Student { No = "2024017", Name = "高燕", IdNumber = "620108200506122177", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.哈尼族, NativePlace = "甘肃", Birthday = new DateTime(2005, 6, 12), Weight = 54, Height = 1.67m },
            new Student { No = "2024018", Name = "罗明", IdNumber = "650209200407301188", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.哈萨克族, NativePlace = null, Birthday = new DateTime(2004, 7, 30), Weight = 71, Height = 1.83m },
            new Student { No = "2024019", Name = "梁红", IdNumber = "440301200508152199", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.黎族, NativePlace = "海南", Birthday = new DateTime(2005, 8, 15), Weight = 56, Height = 1.70m },
            new Student { No = "2024020", Name = "谢峰", IdNumber = "500102200409081200", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.傣族, NativePlace = "重庆", Birthday = new DateTime(2004, 9, 8), Weight = 63, Height = 1.74m },
            new Student { No = "2024021", Name = "韩梅", IdNumber = "320203200510202211", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.汉族, NativePlace = "江苏", Birthday = new DateTime(2005, 10, 20), Weight = 52, Height = 1.63m },
            new Student { No = "2024022", Name = "唐杰", IdNumber = "420104200411121222", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.畲族, NativePlace = "湖北", Birthday = new DateTime(2004, 11, 12), Weight = 67, Height = 1.77m },
            new Student { No = "2024023", Name = "冯丽", IdNumber = "370105200512252233", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.傈僳族, NativePlace = "山东", Birthday = new DateTime(2005, 12, 25), Weight = 47, Height = 1.59m },
            new Student { No = "2024024", Name = "许强", IdNumber = "130106200401081244", Gender = EnumGender.男, EthnicGroup = EnumEthnicGroup.仡佬族, NativePlace = "河北", Birthday = new DateTime(2004, 1, 8), Weight = 64, Height = 1.73m },
            new Student { No = "2024025", Name = "曹敏", IdNumber = "610307200502182255", Gender = EnumGender.女, EthnicGroup = EnumEthnicGroup.东乡族, NativePlace = null, Birthday = new DateTime(2005, 2, 18), Weight = 50, Height = 1.61m }
        };

        await _db.Insertable(students).ExecuteCommandAsync();
        return true;
    }

    public async Task<Student?> GetStudentByNoAsync(string no)
    {
        return await _db.Queryable<Student>().Where(s => s.No == no).SingleAsync();
    }

    public async Task<List<Student>> GetHanMaleStudentsAsync()
    {
        return await _db.Queryable<Student>()
            .Where(s => s.EthnicGroup == EnumEthnicGroup.汉族 && s.Gender == EnumGender.男)
            .ToListAsync();
    }

    public async Task<List<Student>> GetStudentsByWeightRangeAsync(int minWeight, int maxWeight)
    {
        return await _db.Queryable<Student>()
            .Where(s => s.Weight >= minWeight && s.Weight <= maxWeight)
            .ToListAsync();
    }

    public async Task<List<Student>> GetStudentsBornAfterAsync(DateTime date)
    {
        return await _db.Queryable<Student>()
            .Where(s => s.Birthday > date)
            .ToListAsync();
    }

    public async Task<List<Student>> GetStudentsByNativePlaceAsync(string nativePlace)
    {
        return await _db.Queryable<Student>()
            .Where(s => s.NativePlace == nativePlace)
            .ToListAsync();
    }

    public async Task<List<Student>> GetFemaleStudentsOrderedByHeightAndWeightAsync()
    {
        return await _db.Queryable<Student>()
            .Where(s => s.Gender == EnumGender.女)
            .OrderBy(s => s.Height, OrderByType.Desc)
            .OrderBy(s => s.Weight, OrderByType.Asc)
            .ToListAsync();
    }

    public async Task<List<(string Name, string NativePlace)>> GetStudentNameAndNativePlaceAsync()
    {
        var result = await _db.Queryable<Student>()
            .Where(s => !string.IsNullOrEmpty(s.NativePlace))
            .Select(s => new { s.Name, s.NativePlace })
            .ToListAsync();
        return result.Select(item => (item.Name, item.NativePlace!)).ToList();
    }

    public async Task<List<Student>> GetStudentsByPageAsync(int pageIndex, int pageSize)
    {
        return await _db.Queryable<Student>()
            .OrderBy(s => s.No)
            .ToPageListAsync(pageIndex, pageSize);
    }

    public async Task<List<(int Month, int Count)>> GetStudentsGroupByBirthMonthAsync()
    {
        var result = await _db.Queryable<Student>()
            .Select(s => new { s.Birthday.Month })
            .ToListAsync();
        var grouped = result.GroupBy(s => s.Month)
            .Select(g => (Month: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();
        return grouped;
    }

    public async Task<List<(string NativePlace, decimal AvgHeight)>> GetAvgHeightGroupByNativePlaceAsync()
    {
        var result = await _db.Queryable<Student>()
            .Where(s => !string.IsNullOrEmpty(s.NativePlace))
            .GroupBy(s => s.NativePlace)
            .Select(g => new { NativePlace = g.NativePlace!, AvgHeight = SqlFunc.AggregateAvg(g.Height) })
            .ToListAsync();
        return result
            .Where(x => x.AvgHeight >= 1.55m)
            .OrderByDescending(x => x.AvgHeight)
            .Select(x => (x.NativePlace, x.AvgHeight))
            .ToList();
    }

    public async Task<bool> UpdateStudentByNoAsync(string no, Student student)
    {
        var result = await _db.Updateable(student)
            .Where(s => s.No == no)
            .ExecuteCommandAsync();
        return result > 0;
    }

    public async Task<bool> UpdateStudentHeightAndWeightAsync(string no, decimal height, int weight)
    {
        var student = await GetStudentByNoAsync(no);
        if (student == null) return false;
        student.Height = height;
        student.Weight = weight;
        var result = await _db.Updateable(student)
            .Where(s => s.No == no)
            .UpdateColumns(s => new { s.Height, s.Weight })
            .ExecuteCommandAsync();
        return result > 0;
    }

    public async Task<int> UpdateFemaleWeightByBirthYearRangeAsync(int startYear, int endYear)
    {
        var students = await _db.Queryable<Student>()
            .Where(s => s.Gender == EnumGender.女 && s.Birthday.Year >= startYear && s.Birthday.Year <= endYear)
            .ToListAsync();
        foreach (var s in students)
        {
            s.Weight -= 1;
        }
        return await _db.Updateable(students).UpdateColumns(s => new { s.Weight }).ExecuteCommandAsync();
    }

    public async Task<int> DeleteStudentsByNosAsync(List<string> nos)
    {
        return await _db.Deleteable<Student>()
            .Where(s => nos.Contains(s.No))
            .ExecuteCommandAsync();
    }

    public async Task<int> DeleteAllMaleStudentsAsync()
    {
        return await _db.Deleteable<Student>()
            .Where(s => s.Gender == EnumGender.男)
            .ExecuteCommandAsync();
    }

    public async Task<int> DeleteAllStudentsAsync()
    {
        return await _db.Deleteable<Student>().ExecuteCommandAsync();
    }

    public async Task TestPerformanceAsync()
    {
        var stopwatch1 = Stopwatch.StartNew();
        for (var i = 0; i < 10000; i++)
        {
            var student = new Student
            {
                No = $"P{i:D10}",
                Name = $"测试学生{i}",
                IdNumber = $"11010120050101{i:D4}",
                Gender = i % 2 == 0 ? EnumGender.男 : EnumGender.女,
                EthnicGroup = EnumEthnicGroup.汉族,
                Birthday = new DateTime(2005, 1, 1),
                Weight = 50 + i % 30,
                Height = 1.60m + (decimal)(i % 30) / 100
            };
            await _db.Insertable(student).ExecuteCommandAsync();
        }
        stopwatch1.Stop();
        Console.WriteLine($"单条循环插入10000条数据耗时: {stopwatch1.ElapsedMilliseconds}ms");

        var batchStudents = new List<Student>();
        for (var i = 0; i < 10000; i++)
        {
            batchStudents.Add(new Student
            {
                No = $"B{i:D10}",
                Name = $"批量学生{i}",
                IdNumber = $"11010120050202{i:D4}",
                Gender = i % 2 == 0 ? EnumGender.男 : EnumGender.女,
                EthnicGroup = EnumEthnicGroup.汉族,
                Birthday = new DateTime(2005, 2, 1),
                Weight = 50 + i % 30,
                Height = 1.60m + (decimal)(i % 30) / 100
            });
        }

        var stopwatch2 = Stopwatch.StartNew();
        await _db.Insertable(batchStudents).ExecuteCommandAsync();
        stopwatch2.Stop();
        Console.WriteLine($"List批量插入10000条数据耗时: {stopwatch2.ElapsedMilliseconds}ms");

        var stopwatch3 = Stopwatch.StartNew();
        for (var batch = 0; batch < 10; batch++)
        {
            var pageStudents = new List<Student>();
            for (var i = 0; i < 1000; i++)
            {
                var idx = batch * 1000 + i;
                pageStudents.Add(new Student
                {
                    No = $"PG{idx:D10}",
                    Name = $"分页学生{idx}",
                    IdNumber = $"11010120050303{idx:D4}",
                    Gender = idx % 2 == 0 ? EnumGender.男 : EnumGender.女,
                    EthnicGroup = EnumEthnicGroup.汉族,
                    Birthday = new DateTime(2005, 3, 1),
                    Weight = 50 + idx % 30,
                    Height = 1.60m + (decimal)(idx % 30) / 100
                });
            }
            await _db.Insertable(pageStudents).ExecuteCommandAsync();
        }
        stopwatch3.Stop();
        Console.WriteLine($"分页插入10000条数据(每批1000条)耗时: {stopwatch3.ElapsedMilliseconds}ms");
    }
}