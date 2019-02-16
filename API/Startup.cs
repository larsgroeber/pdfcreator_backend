using System;
using System.Collections.Immutable;
using System.Text;
using API.Contexts;
using API.Models;
using API.Mutations;
using API.Queries;
using API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
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
            services.AddTransient<DocumentService>();
            services.AddTransient<LatexService>();
            services.AddTransient<CompileService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<AuthService>();

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "Frontend/dist"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSpaStaticFiles();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");

            app.UseMvc();
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Frontend";

                if (env.IsDevelopment())
                {
//                    spa.UseAngularCliServer(npmScript: "start");
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}