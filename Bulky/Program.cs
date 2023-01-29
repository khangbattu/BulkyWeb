using Bulky.DataAccess;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Bulky.Utility;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var connection = builder.Configuration.GetConnectionString("DefaultConnect");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

		builder.Services.AddDbContext<Application>(option => option.UseMySql(connection, serverVersion));
        builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<Application>();
		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddSingleton<IEmailSender, EmailSender>();
        builder.Services.AddRazorPages();
        builder.Services.ConfigureApplicationCookie(option =>
        {
            option.LoginPath = $"/Identity/Account/Login";
            option.LogoutPath = $"/Identity/Account/Logout";
            option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
        }
        );

		var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
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

        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}



