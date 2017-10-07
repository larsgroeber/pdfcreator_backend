using System;
using System.Collections.Immutable;
using API.Contexts;
using API.Models;
using API.Mutations;
using API.Queries;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<PDFCreatorContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("PDFCreator")));

            services.AddTransient<RootQuery>();
            services.AddTransient<RootMutation>();
            services.AddTransient<UserService>();
            services.AddTransient<TemplateService>();
            services.AddScoped<AuthService>();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseMvc();
        }
    }
}