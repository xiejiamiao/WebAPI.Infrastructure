<!-- TOC -->

- [功能点](#功能点)
    - [排序](#排序)
- [第三方库](#第三方库)
    - [**Serilog**](#serilog)
    - [**AutoMap**](#automap)
    - [**Fluent Validation**](#fluent-validation)

<!-- /TOC -->

## 功能点
### 排序
- Get接口使用排序功能示例：
    - `api/order?orderBy=createdOn` -> 按createdOn正序排序
    - `api/order?orderBy=createdOn desc` -> 按createdOn倒序排序
    - `api/order?orderBy=createdOn,amount` -> 先按createdOn正序排序，再按amount正序排序
    - `api/order?orderBy=createdOn desc,amount` -> 先按createdOn倒序排序，再按amount正序排序
## 第三方库

### **Serilog**
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

### **AutoMap**
- 安装类库
    - AutoMapper.Extensions.Microsoft.DependencyInjection **(AutoMapper依赖注入)**
- 配置(简单配置)：
    - 创建 `MappingProfile.cs`，继承自`Profile`类
    - 在`MappingProfile`的构造方法中进行映射配置，例如：
        ```
        CreateMap<Order,OrderResourceModel>();
        CreateMap<OrderResourceModel,Order>();
        ```
    - 在`Satrtup`的`ConfigureServices`中注入服务
        ```
        services.AddAutoMapper(typeof(MappingProfile));
        ```

### **Fluent Validation**
- 安装类库
    - FluentValidation.AspNetCore **(会以来安装FluentValidation)**
- 配置
    - 创建继承`AbstractValidator<T>`的验证类，例如`AbstractValidator<OrderAddResource>`
    - 在类的构造方法中制定验证规则(示例,验证实体为 `OrderAddResource` )
        ```
        public OrderAddResourceValidator()
        {
            RuleFor(x => x.OrderNo)
                .NotNull()
                .WithName("订单号")
                .WithMessage("Required|{PropertyName}是必填的")
                .MaximumLength(50)
                .WithMessage("{PropertyName}的最大长度是{MaxLength}");
        }
        ```
    - 在`Startup`中进行以来注入配置
        ```
        services.AddMvc(options=>{
            //mvc配置
        }).AddFluentValidation();

        services.AddTransient<IValidator<OrderAddResource>, OrderAddResourceValidator>(); //多个验证实体规则依次添加即可
        ```
    - 在`controller`中验证
        ```
        if (!ModelState.IsValid)
        {
            //进行验证不通过操作
        }
        ```