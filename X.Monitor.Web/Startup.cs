using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using X.Monitor.Core;
using X.Monitor.Web.BackgroundServices;
using X.Monitor.Web.Infrastructure;
using X.Monitor.Web.WebSockets;

namespace X.Monitor.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public ILifetimeScope AutofacContainer { get; private set; }

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddHostedService<BackgroundSocketService>();
			services.AddHostedService<BackgroundMonitorService>();

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			var builder = new ContainerBuilder();
			builder.Populate(services);

			builder.RegisterType<SocketConnectionManager>()
				   .As<ISocketConnectionManager>()
				   .SingleInstance();

			builder.RegisterInstance(MonitorService.Create());

			AutofacContainer = builder.Build();

			return new AutofacServiceProvider(AutofacContainer);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			var wsOptions = new WebSocketOptions()
			{
				KeepAliveInterval = TimeSpan.FromSeconds(120),
				ReceiveBufferSize = 4 * 1024
			};

			app.UseWebSockets(wsOptions);
			app.UseRequestWebSocket(); // custom request middleware

			//app.Use(async (context, next) => {
			//	//if (!context.WebSockets.IsWebSocketRequest)
			//	//	await next();
			//	//
			//	//var socket = await context.WebSockets.AcceptWebSocketAsync();
			//	//var socketFinishedTcs = new TaskCompletionSource<object>();
			//	//
			//	//BackgroundSocketService.AddSocket(socket, socketFinishedTcs);
			//	//
			//	//await socketFinishedTcs.Task;

			//	if (context.Request.Path == "/ws")
			//	{
			//		if (context.WebSockets.IsWebSocketRequest)
			//		{
			//			var socket = await context.WebSockets.AcceptWebSocketAsync();
			//			var socketFinishedTcs = new TaskCompletionSource<object>();

			//			var webSocketManager = AutofacContainer.Resolve<ISocketConnectionManager>();
			//			webSocketManager.TryAddSocket(new WebSocketItem(Guid.NewGuid(), socket, socketFinishedTcs));
						
			//			await socketFinishedTcs.Task;

			//			//var buffer = new byte[1024 * 4];
			//			//WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			//			//while (!result.CloseStatus.HasValue)
			//			//{
			//			//	await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
			//			//
			//			//	result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			//			//}
			//			//await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			//		}
			//		else
			//		{
			//			context.Response.StatusCode = 400;
			//		}
			//	}
			//	else
			//	{
			//		await next();
			//	}
			//});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
