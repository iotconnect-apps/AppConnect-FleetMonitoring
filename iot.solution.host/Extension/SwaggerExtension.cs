using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace iot.solution.host
{
    public static class SwaggerExtension
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<ExplictObsoleteRoutes>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = @"Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new OpenApiInfo { Title = configuration.GetSection("SwashbuckleSpec:Title").Value, Version = "v1" });
            });
        }

        public static void Configure(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("../swagger/v1/swagger.json", "Iot Solutions");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            #region IDS Swagger
            //app.UseSwaggerUI(s =>
            //{
            //    s.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

            //    s.OAuthClientId(component.helper.SolutionConfiguration.Configuration.Token.OAuthClientID);
            //    s.OAuthRealm(component.helper.SolutionConfiguration.Configuration.Token.OAuthRealm);
            //    s.OAuthAppName(component.helper.SolutionConfiguration.Configuration.Token.ApiName);
            //});
            #endregion
        }
    }

    public class ExplictObsoleteRoutes : IOperationFilter
    {
        private string[] solutionKeyHeaderRequiredPaths = { "/account/refreshtoken", "/subscriber", "/account/login", "/account/adminlogin", "/alert/addiotalert", "/chart/executecron" };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!solutionKeyHeaderRequiredPaths.Any(x => context.ApiDescription.RelativePath.Contains(x)))
            {
                operation.Parameters.Add(new OpenApiParameter { Name = "company-id", In = ParameterLocation.Header, Required = false });
            }
        }
    }
}

