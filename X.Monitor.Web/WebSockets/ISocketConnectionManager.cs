using System;
using System.Collections.Generic;

namespace X.Monitor.Web.WebSockets
{
	public interface ISocketConnectionManager
	{
		IEnumerable<WebSocketItem> GetSockets();
		bool TryAddSocket(WebSocketItem wsi);
		bool TryRemoveSocket(Guid guid);
	}
}
