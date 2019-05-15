

## 项目框架
- WebAPI.Infrastructure.Database
- WebAPI.Infrastructure.Gateway
- WebAPI.Infrastructure.Interfaces
- WebAPI.Infrastructure.ModelDomain
- WebAPI.Infrastructure.ModelResource
- WebAPI.Infrastructure.Repositories

## 第三方库

- **[Serilog](https://serilog.net/)**
    - 安装类库:
        - Serilog **（日志主程序）**
        - Serilog.AspNetCore **（asp.net core类型项目配置使用，包含IWebHostBuilder的拓展方法）**
        - Serilog.Sinks.Console **（输出至控制台）**
        - Serilog.Sinks.File **（输出至文本文件）**
    - 配置(简单配置)：
        - 在`Program.cs`中进行日志配置
        - 在`Main`方法中添加配置
            ```
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsort", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            ```
        - 在`CreateWebHostBuilder`方法中使用`serilog`
            ```
            WebHost.CreateDefaultBuilder(args)
            .UseStartup(typeof(Startup).GetTypeInfo().Assembly.FullName)
            .UseSerilog();
            ```
- **[AutoMap]**