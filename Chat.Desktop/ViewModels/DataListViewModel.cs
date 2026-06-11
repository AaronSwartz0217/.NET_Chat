using Avalonia.Collections;
using Chat.Application.Dtos;
using Chat.Application.Services;
using Chat.Core.Models;
using CommunityToolkit.Mvvm.Input;
using Furion.HttpRemote;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ursa.Controls;
using Chat.Desktop.Views;

namespace Chat.Desktop.ViewModels;

public partial class DataListViewModel : ViewModelBase
{
    public AvaloniaList<StudentDto> Students { get; } = new();

    private readonly IHttpRemoteService _httpRemoteService;


    private readonly IStudentService _studentService;

    public DataListViewModel(IStudentService studentService
        , IHttpRemoteService httpRemoteService)
    {
        _studentService = studentService;

        _httpRemoteService = httpRemoteService;
    }

    

    public async Task LoadDataAsync()
    {
        Students.Clear();
        //var students = await _studentService.GetStudentsAsync();

        var students = await _httpRemoteService.PostAsAsync<List<StudentDto>>(
            "http://127.0.0.1:5002/api/student/students"
            , builder => builder.SetFormUrlEncodedContent(
                new
                {
                    No = "222",
                    Name = "bbb"
                }, contentEncoding: Encoding.UTF8));

        Students.AddRange(students);
    }


    [RelayCommand]
    private async Task EditClick(int id)
    {
        var vm = new DataEditViewModel();
        vm.Id = id;
        var ret = await OverlayDialog.ShowCustomAsync<DataEditView, DataEditViewModel, bool>(vm);
        if(ret)
        {

        }
    }

    [RelayCommand]
    private async Task DeleteClick(int id)
    {
        var ret = await MessageBox.ShowAsync(
            message: "确定要删除吗？",
            title: "谨慎操作",
            button: MessageBoxButton.YesNo,
            icon: MessageBoxIcon.Question);
        if (ret == MessageBoxResult.Yes)
        {

        }


    }

}
