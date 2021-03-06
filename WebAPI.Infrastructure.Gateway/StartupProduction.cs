using System;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using WebAPI.Infrastructure.Database;
using WebAPI.Infrastructure.Gateway.Extensions;
using WebAPI.Infrastructure.Interfaces;
using WebAPI.Infrastructure.Repositories;
using WebAPI.Infrastructure.ResourceModel.OrderResource;
using WebAPI.Infrastructure.ResourceModel.PropertyMapping;
using WebAPI.Infrastructure.ResourceModel.Validator;
using WebAPI.Infrastructure.Services;

namespace WebAPI.Infrastructure.Gateway
{
    public class StartupProduction
    {
        private readonly IConfiguration _configuration;

        public StartupProduction(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 8081;
            });
            
            services.AddDbContext<SolutionDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("WebAPI.Infrastructure.Gateway"));
            });
            
            //生产环境使用
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("example.com");
                options.ExcludedHosts.Add("www.example.com");
            });
            
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper(typeof(MappingProfile));
            
            services.AddSingleton<IActionContextAccessor,ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddMvc(options =>
                {
                    options.ReturnHttpNotAcceptable = true;
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    options.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(options));
                })
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })
                .AddFluentValidation(); // FluentValidation DI Step1;
            
            // FluentValidation DI Step2
            services.AddTransient<IValidator<OrderAddResource>, OrderAddResourceValidator>();
            services.AddTransient<IValidator<OrderUpdateResource>, OrderUpdateResourceValidator>();
            
            // Property mapping DI
            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<OrderPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);

            // Type Helper DI
            services.AddTransient<ITypeHelperService, TypeHelperService>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseSolutionExceptionHandler(false);
            app.UseMvc();
        }
    }
}