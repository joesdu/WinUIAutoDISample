using EasilyNET.AutoDependencyInjection.Contexts;
using EasilyNET.AutoDependencyInjection.Modules;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace WinUIAutoDISample.ServiceModules;

internal sealed class LiteDBModule : AppModule
{
    public override void ConfigureServices(ConfigureServicesContext context)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "data");
        // 确保数据目录存在
        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(dbPath);
        }
        // 使用连接字符串创建LiteDatabase实例
        var connectionString = $"Filename={dbPath}{Path.DirectorySeparatorChar}cache.db;";
        var liteDb = new LiteDatabase(connectionString);
        context.Services.AddSingleton<ILiteDatabase>(liteDb);
    }
}