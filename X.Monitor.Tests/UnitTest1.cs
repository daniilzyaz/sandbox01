using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using X.Monitor.Core;

namespace Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			var srv = MonitorService.Create();
			try
			{
				for (int i = 0; i < 4; i++)
				{
					srv.Collect();
					Thread.Sleep(500);
				}

				var proList = srv.GetProccesses().OrderByDescending(p => p.Cpu);
				foreach (var n in proList)
				{
					Console.WriteLine(n);
				}
			}
			finally
			{
				srv.Dispose();
			}

			Assert.Pass();
		}
	}
}