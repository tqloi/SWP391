﻿using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Instructor.Models;
using OnlineLearning.BackgroundServices;
using OnlineLearning.Email;
using OnlineLearning.Filter;
using OnlineLearning.Hubs;
using OnlineLearning.Models;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configure Kestrel to set the max request header size
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestHeadersTotalSize = 1048576; // Set the limit (in bytes)
    //1048576 bytes equals 1MB.
});

builder.Services.AddTransient<EmailSender>();
builder.Services.AddHttpClient();
//stringee
builder.Services.AddTransient<StringeeService>();

//file 
builder.Services.AddScoped<FileService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 209715200; // 200MB (tính bằng bytes)
});

//chathub
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

//connect database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

//dang ki background service
builder.Services.AddHostedService<NotificationCleanupService>();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(5); // Thời gian sống của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//notifycation
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomCenter;
}
);
builder.Services.Configure<ApiVideoSettings>(builder.Configuration.GetSection("ApiVideo"));
///telling the application to use the specific ClientId, and the ClientSecret
///redirect user to the google login page for authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
        options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;

        // Map claim để nhận URL ảnh đại diện
        options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
        options.AccessDeniedPath = "/Account/Login";
        // Lưu token nếu cần thiết
        options.SaveTokens = true;
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<CourseAccessFilter>();
builder.Services.AddScoped<LectureAccessFilter>();
builder.Services.AddScoped<AdminRedirectFilter>();
builder.Services.AddScoped<AssignmentAccessFilter>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    // options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    //options.User.AllowedUserNameCharacters =
    //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
   
});

//VnPay
builder.Services.AddSingleton<IVnPayService, VnPayService>();
//services for test scheduler
builder.Services.AddScoped<TestSchedulerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Chỉ hiển thị trang lỗi chi tiết khi ở môi trường phát triển
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Sử dụng trang lỗi tổng quát trong môi trường sản xuất
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseNotyf();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "Areas",
    areaName: "Instructor",
    pattern: "{area:exists}/{controller=Instructor}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "Areas",
    areaName:"Admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "Areas",
    areaName: "Student",
    pattern: "{area:exists}/{controller=Student}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

//404
app.UseStatusCodePagesWithReExecute("/Home/Error404");

app.MapHub<TestHub>("/testHub");


app.Run();
