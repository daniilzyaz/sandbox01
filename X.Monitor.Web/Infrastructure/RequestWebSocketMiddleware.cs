using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.Monitor.Web.WebSockets;

namespace X.Monitor.Web.Infrastructure
{
	public class RequestWebSocketMiddleware
	{
		private readonly RequestDelegate _next;

		public RequestWebSocketMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, ISocketConnectionManager socketConnectionManager)
		{
			if (context.Request.Path == "/ws")
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					var socket = await context.WebSockets.AcceptWebSocketAsync();
					var socketFinishedTcs = new TaskCompletionSource<object>();

					socketConnectionManager.TryAddSocket(new WebSocketItem(Guid.NewGuid(), socket, socketFinishedTcs));

					await socketFinishedTcs.Task;
				}
				else
				{
					context.Response.StatusCode = 400;
				}
			}

			await _next(context);
		}
	}

	public static class RequestWebSocketMiddlewareExtensions
	{
		public static IApplicationBuilder UseRequestWebSocket(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<RequestWebSocketMiddleware>();
		}
	}
}
