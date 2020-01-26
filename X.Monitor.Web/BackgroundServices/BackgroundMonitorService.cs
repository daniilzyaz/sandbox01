using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using X.Monitor.Core;

namespace X.Monitor.Web.BackgroundServices
{
	public class BackgroundMonitorService : BackgroundService
	{
		public readonly MonitorService _monitorService;

		public BackgroundMonitorService(MonitorService ms)
		{
			_monitorService = ms;
		}

		protected async override Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				_monitorService.Collect();

				await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
			}
		}
	}
}
