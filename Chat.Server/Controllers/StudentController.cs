using Chat.Application.Services;
using Chat.Core.Models;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Chat.Server.Controllers;

[NonUnify]
[DynamicApiController]
public class StudentController : IDynamicApiController
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [DisplayName("批量插入学生数据")]
    public async Task<string> Get()
    {
        var result = await _studentService.BatchInsertStudentsAsync();
        return $"批量插入完成: {result}";
    }

    [DisplayName("批量插入学生数据(备用)")]
    public async Task<string> Insert()
    {
        var result = await _studentService.BatchInsertStudentsAsync();
        return $"批量插入完成: {result}";
    }

    [DisplayName("根据学号查询学生")]
    public async Task<Student?> GetByNo(string no)
    {
        return await _studentService.GetStudentByNoAsync(no);
    }

    [DisplayName("查询汉族男生")]
    public async Task<List<Student>> GetHanMale()
    {
        return await _studentService.GetHanMaleStudentsAsync();
    }

    [DisplayName("查询体重范围学生")]
    public async Task<List<Student>> GetByWeightRange(int minWeight, int maxWeight)
    {
        return await _studentService.GetStudentsByWeightRangeAsync(minWeight, maxWeight);
    }

    [DisplayName("查询指定日期后出生的学生")]
    public async Task<List<Student>> GetBornAfter(DateTime date)
    {
        return await _studentService.GetStudentsBornAfterAsync(date);
    }

    [DisplayName("查询指定籍贯的学生")]
    public async Task<List<Student>> GetByNativePlace(string place)
    {
        return await _studentService.GetStudentsByNativePlaceAsync(place);
    }

    [DisplayName("查询女生(按身高降序体重升序)")]
    public async Task<List<Student>> GetFemaleOrdered()
    {
        return await _studentService.GetFemaleStudentsOrderedByHeightAndWeightAsync();
    }

    [DisplayName("查询姓名和籍贯")]
    public async Task<List<(string Name, string NativePlace)>> GetNameAndNativePlace()
    {
        return await _studentService.GetStudentNameAndNativePlaceAsync();
    }

    [DisplayName("分页查询学生")]
    public async Task<List<Student>> GetByPage(int pageIndex, int pageSize)
    {
        return await _studentService.GetStudentsByPageAsync(pageIndex, pageSize);
    }

    [DisplayName("按月份分组统计人数")]
    public async Task<List<(int Month, int Count)>> GetGroupByBirthMonth()
    {
        return await _studentService.GetStudentsGroupByBirthMonthAsync();
    }

    [DisplayName("按籍贯统计平均身高")]
    public async Task<List<(string NativePlace, decimal AvgHeight)>> GetAvgHeightGroupByNativePlace()
    {
        return await _studentService.GetAvgHeightGroupByNativePlaceAsync();
    }

    [DisplayName("根据学号更新学生信息")]
    public async Task<string> UpdateByNo(string no, [FromBody] Student student)
    {
        var result = await _studentService.UpdateStudentByNoAsync(no, student);
        return result ? "更新成功" : "更新失败";
    }

    [DisplayName("仅更新身高和体重")]
    public async Task<string> UpdateHeightAndWeight(string no, decimal height, int weight)
    {
        var result = await _studentService.UpdateStudentHeightAndWeightAsync(no, height, weight);
        return result ? "更新成功" : "更新失败";
    }

    [DisplayName("04-05年女生体重减1")]
    public async Task<string> UpdateFemaleWeight(int startYear, int endYear)
    {
        var count = await _studentService.UpdateFemaleWeightByBirthYearRangeAsync(startYear, endYear);
        return $"更新 {count} 位女生体重";
    }

    [DisplayName("批量删除指定学号学生")]
    public async Task<string> DeleteByNos(List<string> nos)
    {
        var count = await _studentService.DeleteStudentsByNosAsync(nos);
        return $"删除 {count} 位同学";
    }

    [DisplayName("删除所有男生")]
    public async Task<string> DeleteAllMale()
    {
        var count = await _studentService.DeleteAllMaleStudentsAsync();
        return $"删除 {count} 位男生";
    }

    [DisplayName("清空全表数据")]
    public async Task<string> DeleteAll()
    {
        var count = await _studentService.DeleteAllStudentsAsync();
        return $"删除全表 {count} 条数据";
    }

    [DisplayName("大数据性能测试")]
    public async Task<string> TestPerformance()
    {
        await _studentService.TestPerformanceAsync();
        return "性能测试完成，请查看控制台输出";
    }
}