using Chat.Application.Services;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Server.Controllers;

[NonUnify]
public class LoginController : IDynamicApiController
{
    private readonly IUserService _userService;

    public LoginController(IUserService userService)
    {
        _userService = userService;
    }

    public string Get()
    {
        return "这是我新建的一个WEB服务端";
    }
}