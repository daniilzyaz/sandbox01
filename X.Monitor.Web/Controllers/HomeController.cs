using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using X.Monitor.Core;
using X.Monitor.Web.Models;

namespace X.Monitor.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly MonitorService _srv;

		public HomeController(MonitorService srv)
		{
			_srv = srv;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ProcessList()
		{
			var processes = _srv.GetProccesses();
			var vmProcessList = processes.Select(p => new ProcessItemViewModel()
			{
				Name = p.Name,
				Cpu = p.Cpu,
				Ram = p.Ram
			})
			.OrderByDescending(p => p.Cpu)
			.ToList();

			return PartialView("_ProcessListPartial", vmProcessList);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
