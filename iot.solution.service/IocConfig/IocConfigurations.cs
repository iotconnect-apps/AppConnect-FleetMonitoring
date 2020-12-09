using iot.solution.model;
using iot.solution.model.Models;
using iot.solution.model.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using System.Reflection;
using component.helper;
using component.helper.Interface;

namespace iot.solution.service.IocConfig
{
    public class IocConfigurations
    {
        public static void Initialize(IServiceCollection services)
        {
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IRoleRepository, RoleRepository>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IRoleService, RoleService>();

            
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DbContext, devfleetmonitoringContext>();
            //services.AddSingleton<IIotConnectProvider>(provider => new IotConnectProvider("","",""));

            //resolve dependency of processing & object services
            services.RegisterAssemblyPublicNonGenericClasses().Where(c => c.Name.EndsWith("Service")).AsPublicImplementedInterfaces();

            ////context 
            //services.AddDbContext<FormsDataDbContext>(options => options.UseSqlServer(connectionString), contextLifetime: ServiceLifetime.Transient);

            //repository
            services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(UserRepository))).Where(c => c.Name.EndsWith("Repository")).AsPublicImplementedInterfaces();

        }        
    }
}
