using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo_CookieAuthentication_MVC
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
            services.AddControllersWithViews();

            services.AddAuthentication("Login.Cookie")
                .AddCookie("Login.Cookie",
                    options =>
                    {
                        options.Cookie.Name = "Login.Cookie";
                        options.LoginPath = new PathString("/Home/Login");
                        options.Cookie.HttpOnly = false;
                        options.ExpireTimeSpan = TimeSpan.FromHours(12);
                        options.SlidingExpiration = false;
                        options.AccessDeniedPath = new PathString("/Home/Login");
                    });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Only4PE", policy => policy.RequireClaim("Location", "PE"));//SE PUEDE PONER PARA COMPARAR CON MAS VALORES DEL CLAIM
                options.AddPolicy("Only4CO", policy => policy.RequireClaim("Location", "CO"));
                //https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-5.0
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
