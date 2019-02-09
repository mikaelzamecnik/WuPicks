using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wu17Picks.Data;
using Wu17Picks.Infrastructure.Extensions;
using Wu17Picks.Infrastructure.Interfaces;
using Wu17Picks.Infrastructure.Services;
using Wu17Picks.Services;

namespace Wu17Picks.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IImage, ImageService>();
            services.AddScoped<ICategory, CategoryService>();

            services.Configure<AppConfigHelper>(Configuration.GetSection("AzureSettings"));

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = ".Picks.RedisCache";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".Picks.Cart.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/Gallery/Error");
            }

            app.UseSession();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
