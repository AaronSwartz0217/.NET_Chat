using Avalonia.Collections;
using Chat.Application.Dtos;
using Chat.Application.Services;
using CommunityToolkit.Mvvm.Input;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Desktop.ViewModels;

public partial class DataListViewModel : ViewModelBase
{
    public AvaloniaList<StudentDto> Students { get; } = new();

    private readonly IStudentService _studentService;

    public DataListViewModel(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task LoadDataAsync()
    {
        Students.Clear();
        var students = await _studentService.GetStudentsAsync();
        Students.AddRange(students.Adapt<List<StudentDto>>());
    }

    [RelayCommand]
    private async Task EditClick(int id)
    {
        var vm = new DataEditViewModel();
        vm.Id = id;
    }

    [RelayCommand]
    private async Task DeleteClick(int id)
    {
    }
}
