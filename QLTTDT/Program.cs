using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;

namespace QLTTDT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<QLTTDTDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
            });
            builder.Services.AddAuthentication("AuthenticationSchema")
            .AddCookie("AuthenticationSchema", options =>
            {
                options.LoginPath = "/Authentication/Login";
                options.AccessDeniedPath = "/Authentication/AccessDenied";
                options.Cookie.Name = "AuthenticationCookie";
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                options.AddPolicy("HocVien", policy => policy.RequireClaim(ClaimTypes.Role, "HocVien"));
            });
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();

            app.MapControllerRoute(
				name: "Dashboard-short",
				pattern: "Dashboard/{action}/{id?}",
                defaults: new { area="admin", controller = "Dashboard", action="Index" }
                )	
                .WithStaticAssets();

            app.MapAreaControllerRoute(
                name: "Dashboard",
                areaName: "Admin",
                pattern: "Dashboard/{controller}/{action}/{id?}",
                defaults: new { controller = "Dashboard", action = "Index" }
                )
                .WithStaticAssets();

            app.MapControllerRoute(
				name: "Profiles",
				pattern: "Profiles/{username}-{id}",
                defaults: new { controller = "Profiles", action="Index" }
                )	
                .WithStaticAssets();
            app.MapControllerRoute(
				name: "Course",
				pattern: "Course/{topicSlug}-{topicId}/{courseSlug}-{courseId}",
                defaults: new { controller = "Course", action="Index" }
                )	
                .WithStaticAssets();
			app.MapControllerRoute(
				name: "Topic",
				pattern: "Topic/{topicSlug}-{topicId}",
                defaults: new { controller = "AllTopic", action="Details" }
                )	
                .WithStaticAssets();
			app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
