using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace X.Monitor.Web.WebSockets
{
	public class WebSocketItem
	{
		public WebSocketItem(Guid id, WebSocket ws, TaskCompletionSource<object> tcs)
		{
			Id = id;
			WebSocket = ws;
			TaskCompletionSource = tcs;
		}

		public Guid Id { get; }
		public WebSocket WebSocket { get; }
		public TaskCompletionSource<object> TaskCompletionSource { get; }
	}
}
