using AutoMapper;
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
using WebAPI.Infrastructure.ResourceModel.PropertyMapping;
using WebAPI.Infrastructure.Services;

namespace WebAPI.Infrastructure.Gateway
{
    public class StartupDevelopment
    {
        private readonly IConfiguration _configuration;

        public StartupDevelopment(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // HTTPS Redirect DI
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 8081;
            });
            
            // EntityFramework Core DI
            services.AddDbContext<SolutionDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("WebAPI.Infrastructure.Gateway"));
            });
            
            // Repository DI
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // AutoMapper DI
            services.AddAutoMapper(typeof(MappingProfile));
            
            // IUrlHelper DI
            services.AddSingleton<IActionContextAccessor,ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            // MVC DI
            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                options.InputFormatters.Add(new XmlDataContractSerializerInputFormatter(options));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // Property mapping DI
            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<OrderPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);

            // Type Helper DI
            services.AddTransient<ITypeHelperService, TypeHelperService>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseSolutionExceptionHandler(true);
            app.UseMvc();
        }
    }
}