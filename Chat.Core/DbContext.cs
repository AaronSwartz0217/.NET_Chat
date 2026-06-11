using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Npgsql;
using SqlSugar;

namespace Chat.Core;

public static class DbContext
{
    public static void AddPgSetup(this IServiceCollection services)
    {
        var connStr = new NpgsqlConnectionStringBuilder()
        {
            Host = "127.0.0.1",
            Username = "postgres",
            Password = "",
            Database = "chat_db",
            Port = 5432
        }.ConnectionString;

        var configConnection = new ConnectionConfig()
        {
            DbType = DbType.PostgreSQL,
            //ConnectionString = "PORT=5432;DATABASE=chat_db;HOST=127.0.0.1;PASSWORD=;USER ID=postgre;",
            ConnectionString = connStr,
            IsAutoCloseConnection = true
        };

        SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
        db =>
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                // Console.WriteLine(sql);//输出sql
            };
        });

        services.AddSingleton<ISqlSugarClient>(sqlSugar);
    }

    public static void AddMySqlSetup(this IServiceCollection services)
    {
        var connStr = new MySqlConnectionStringBuilder()
        {
            Server = "localhost",
            UserID = "root",
            Password = "Z2971762643z",
            Database = "chat_db",
            Port = 3306,
            SslMode = MySqlSslMode.None,
            Pooling = true,
            CharacterSet = "utf8"
        }.ToString();

        var configConnection = new ConnectionConfig()
        {
            DbType = DbType.MySql,
            //ConnectionString = "Data Source=localhost;Database=Furion;User ID=root;Password=000000;pooling=true;port=3306;sslmode=none;CharSet=utf8;",
            ConnectionString = connStr,
            IsAutoCloseConnection = true
        };

        SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
        db =>
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                // Console.WriteLine(sql);//输出sql
            };
        });

        services.AddSingleton<ISqlSugarClient>(sqlSugar);
    }
}
