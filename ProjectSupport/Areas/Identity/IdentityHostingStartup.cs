using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSupport.Areas.Identity.Data;
using ProjectSupport.Data;

[assembly: HostingStartup(typeof(ProjectSupport.Areas.Identity.IdentityHostingStartup))]
namespace ProjectSupport.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppDbContextConnection")));

                services.AddIdentity<AppUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();

                services.AddAuthentication()
                    .AddGoogle(o =>
                    {
                        o.ClientId = context.Configuration["Google:ClientId"];
                        o.ClientSecret = context.Configuration["Google:ClientSecret"];
                    })
                    .AddFacebook(o =>
                     {
                         o.AppId = context.Configuration["Facebook:AppId"];
                         o.AppSecret = context.Configuration["Facebook:AppSecret"];
                     });

                services.AddMvc();
            });
        }
    }
}