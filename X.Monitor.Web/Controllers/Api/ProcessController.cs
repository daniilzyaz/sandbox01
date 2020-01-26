using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using X.Monitor.Web.Models;
using X.Monitor.Web.Services;

namespace X.Monitor.Web.Controllers.Api
{
    [Route("api/processes")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
		private readonly IProcessService _srv;

		public ProcessController(IProcessService srv)
		{
			_srv = srv;
		}

		[HttpGet]
		public IEnumerable<ProcessItemViewModel> Get()
		{
			return _srv.Get(); // todo handle IEnumerable errors
		}
	}
}