using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using X.Monitor.Web.Models;
using X.Monitor.Web.Services;

namespace X.Monitor.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IProcessService _srv;

		public HomeController(IProcessService srv)
		{
			_srv = srv;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ProcessList()
		{
			return PartialView("_ProcessListPartial", _srv.Get().ToList());
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
