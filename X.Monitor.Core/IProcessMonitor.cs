using System;
using X.Monitor.Core.Model;

namespace X.Monitor.Core
{
	internal interface IProcessMonitor : IDisposable
	{
		bool TryCollectInfo(ProcessUsage processUsageInfo);
	}
}
