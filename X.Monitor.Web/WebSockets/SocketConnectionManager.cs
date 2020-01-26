using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace X.Monitor.Web.WebSockets
{
	public class SocketConnectionManager : ISocketConnectionManager
	{
		private ConcurrentDictionary<Guid, WebSocketItem> _sockets = new ConcurrentDictionary<Guid, WebSocketItem>();

		public IEnumerable<WebSocketItem> GetSockets()
		{
			foreach (var s in _sockets)
			{
				yield return s.Value; 
			}
		}

		public bool TryAddSocket(WebSocketItem wsi)
		{
			return _sockets.TryAdd(wsi.Id, wsi);
		}

		public bool TryRemoveSocket(Guid guid)
		{
			return _sockets.TryRemove(guid, out var removedSocket);
		}
	}
}
