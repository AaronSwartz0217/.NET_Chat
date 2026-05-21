using Chat.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public interface IStudentService
{
    // 插入操作
    Task<bool> BatchInsertStudentsAsync();

    // 查询操作
    Task<List<Student>> GetStudentsAsync();
    Task<Student?> GetStudentByNoAsync(string no);
    Task<List<Student>> GetHanMaleStudentsAsync();
    Task<List<Student>> GetStudentsByWeightRangeAsync(int minWeight, int maxWeight);
    Task<List<Student>> GetStudentsBornAfterAsync(DateTime date);
    Task<List<Student>> GetStudentsByNativePlaceAsync(string nativePlace);
    Task<List<Student>> GetFemaleStudentsOrderedByHeightAndWeightAsync();
    Task<List<(string Name, string NativePlace)>> GetStudentNameAndNativePlaceAsync();
    Task<List<Student>> GetStudentsByPageAsync(int pageIndex, int pageSize);
    Task<List<(int Month, int Count)>> GetStudentsGroupByBirthMonthAsync();
    Task<List<(string NativePlace, decimal AvgHeight)>> GetAvgHeightGroupByNativePlaceAsync();

    // 更新操作
    Task<bool> UpdateStudentByNoAsync(string no, Student student);
    Task<bool> UpdateStudentHeightAndWeightAsync(string no, decimal height, int weight);
    Task<int> UpdateFemaleWeightByBirthYearRangeAsync(int startYear, int endYear);

    // 删除操作
    Task<int> DeleteStudentsByNosAsync(List<string> nos);
    Task<int> DeleteAllMaleStudentsAsync();
    Task<int> DeleteAllStudentsAsync();

    // 大数据测试
    Task TestPerformanceAsync();
}