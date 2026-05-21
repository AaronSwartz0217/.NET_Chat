using Chat.Core.Models;
using SqlSugar;

namespace Chat.Application.Services;

public class UserService : IUserService
{
    private readonly ISqlSugarClient _db;

    public UserService(ISqlSugarClient db)
    {
        _db = db;
        _db.CodeFirst.InitTables<User>();
    }

    public bool CreateUser(string userName, string password)
    {
        var u = _db.Queryable<User>()
            .First(w => w.UserName == userName);
        if (u != null)
        {
            return false;
        }

        var user = new User();
        user.UserName = userName;
        user.Password = password;

        var ret = _db.Insertable(user)
            .ExecuteCommand();

        return ret == 1;
    }

    public async Task<bool> CreateUserAsync(string userName, string password)
    {
        var u = await _db.Queryable<User>()
            .FirstAsync(w => w.UserName == userName);
        if (u != null)
        {
            return false;
        }

        var user = new User();
        user.UserName = userName;
        user.Password = password;

        var ret = await _db.Insertable(user)
            .ExecuteCommandAsync();

        return ret == 1;
    }

    public async Task<bool> EditUserAsync(string userName, string password)
    {
        var ret = await _db.Updateable<User>()
          .SetColumns(it => new User()
          {
              Password = password
          })
          .Where(it => it.UserName == userName)
          .ExecuteCommandAsync();

        return ret == 1;
    }
}

