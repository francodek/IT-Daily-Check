using IT_Daily_Check.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using IT_Daily_Check.Models;
using AutoMapper;
using IT_Daily_Check.Settings;
using IT_Daily_Check.Factory;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// we added this after the database model was created
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services
        .AddIdentity<User, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAutoMapper(typeof(Program));


builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.AccessDeniedPath = "/Account/AccessDenied";
});




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
app.UseAuthentication();;

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DailyChecks}/{action=GetCurrentDayCheck}/{id?}");

app.MapRazorPages();

app.Run();
