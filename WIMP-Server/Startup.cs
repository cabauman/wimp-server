using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WIMP_Server.Data;
using WIMP_Server.DataServices.Http;

namespace WIMP_Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _environment;

        public void ConfigureServices(IServiceCollection services)
        {
            if (_environment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });
            }

            if (_environment.IsProduction())
            {
                services.AddDbContext<WimpDbContext>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("WimpDatabase"))
                );
            }
            else
            {
                services.AddDbContext<WimpDbContext, WimpDbContextDev>(opt =>
                    opt.UseSqlite("Filename=wimp.sqlite")
                );
            }

            services.AddScoped<IWimpRepository, WimpRepository>();
            services.AddHttpClient<IEsiDataClient, EsiDataClient>();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "WIMP_Server", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WIMP_Server v1"));
            }

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            PrepareDatabase.Prepare(app);
        }
    }
}
