using System.Collections.Generic;
using System.Linq;
using X.Monitor.Core;
using X.Monitor.Web.Models;

namespace X.Monitor.Web.Services
{
	public class ProcessService : IProcessService
	{
		private readonly MonitorService _srv;

		public ProcessService(MonitorService srv)
		{
			_srv = srv;
		}

		public IEnumerable<ProcessItemViewModel> Get()
		{
			var processes = _srv.GetProccesses();
			var vmProcessList = processes.Select(p => new ProcessItemViewModel()
			{
				Name = p.Name,
				Cpu = p.Cpu,
				Ram = p.Ram
			})
			.OrderByDescending(p => p.Cpu);

			return vmProcessList;
		}
	}
}
