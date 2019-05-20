﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BCC.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;

namespace Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddSingleton(typeof(IBankManager), typeof(BankManager));
            string enviroment = Environment.GetEnvironmentVariable("DATABSE_ENV");
            string connectionString = null;
            switch (enviroment)
            {
                case "Development":
                    connectionString = Configuration.GetConnectionString("bccDevelopment");
                    break;
                case "Production":
                    connectionString = Configuration.GetConnectionString("bccDevelopment");
                    break;
                default:
                    //"Local"
                    connectionString = Configuration.GetConnectionString("bccLocal");
                    break;
            }


            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "Cache";
                options.DefaultSlidingExpiration = TimeSpan.FromDays(30);
            });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromDays(14);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<BCC.Model.Models.BCCContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);
            services.AddScoped<IPresentationManager, PresentationManger>();
            services.AddHostedService<BankManager>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "ExchangeRate",
                    template: "{controller=ExchangeRate}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "TicketTable",
                    template: "{controller=TicketTable}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "TicketGraph",
                    template: "{controller=TicketGraph}/{action=Index}/{id?}");
            });


            
            
            if (env.IsProduction())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
        }
    }
}
