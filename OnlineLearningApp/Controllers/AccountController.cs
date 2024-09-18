using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace OnlineLearningApp.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Login()
		{
			return View();
		}

		//this method will be called first, redirect user to the google login page 
		public async Task LoginGoogle()
		{
			await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
			{
				RedirectUri = Url.Action("GoogleResponse")
			});

		}
		//selection and proper authentication from google and token exchange is return
		//all claim value and print on the page
		public async Task<IActionResult> GoogleResponse()
		{
			var result= await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			var claim = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
			{
				claim.Issuer,
				claim.OriginalIssuer,
				claim.Type,
				claim.Value
			});
			//return Json(claim);
			return RedirectToAction("Index", "Home", new { area = "" });
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return View("Index");
		}
	}
}
