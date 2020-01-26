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
using X.Monitor.Web.Services;
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

			builder.RegisterType<ProcessService>()
				   .As<IProcessService>();

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

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
