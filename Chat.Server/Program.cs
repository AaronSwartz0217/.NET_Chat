using Chat.Application.Services;
using Chat.Core;
using Chat.Server;

await Serve.RunAsync(services =>
{
    //services.AddPgSetup();  //使用PostgreSQL（二选一）
    services.AddMySqlSetup();    //使用MySql（二选一）
    services.AddTransient<IUserService, UserService>();
    services.AddTransient<IStudentService, StudentService>();
    services.AddHostedService<Worker>();
}, urls: "http://0.0.0.0:5002");