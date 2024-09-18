using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.EntityFrameworkCore;
using OnlineLearningApp.Respositories;

namespace OnlineLearningApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

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
				});
			// connect database

			builder.Services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnect"]);
			});
			// Add services to the container.
			builder.Services.AddControllersWithViews().AddRazorOptions(options =>
			{

				options.ViewLocationFormats.Add("/Views/User/{1}/{0}.cshtml");
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

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
