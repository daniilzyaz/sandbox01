using Microsoft.Extensions.Hosting;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X.Monitor.Core;
using X.Monitor.Web.WebSockets;

namespace X.Monitor.Web.BackgroundServices
{
	public class BackgroundSocketService : BackgroundService
	{
		private readonly ISocketConnectionManager _webSocketManager;
		private readonly MonitorService _monitorService;

		private float CpuThreshold = 20;    // this is random % threshold, could be taken from a config file
		private float RamThreshold = 7000;  // this is random MB threshold, would be better to set it based on machine's physical RAM

		public BackgroundSocketService(ISocketConnectionManager webSocketManager, MonitorService monitorService)
		{
			_webSocketManager = webSocketManager;
			_monitorService = monitorService;
		}

		protected async override Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				foreach (var s in _webSocketManager.GetSockets())
				{
					var webSocket = s.WebSocket;
					if (webSocket.State == WebSocketState.Open)
					{
						var totalUsage = _monitorService.GetActiveProcessTotalUsage();
						var cpu = totalUsage.Cpu > CpuThreshold ? totalUsage.Cpu : default(float); 
						var ram = totalUsage.Ram > RamThreshold ? totalUsage.Ram : default(float);
						if (cpu > 0 || ram > 0)
						{
							var msgBytes = Encoding.UTF8.GetBytes(string.Format("{{ \"cpu\": {0}, \"ram\": {1} }}", cpu, ram));
							await webSocket.SendAsync(new ArraySegment<byte>(msgBytes, 0, msgBytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
						}
					}
					else
					{
						s.TaskCompletionSource.TrySetResult(null);
						_webSocketManager.TryRemoveSocket(s.Id);
					}
				}

				await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
			}
		}
	}
}
