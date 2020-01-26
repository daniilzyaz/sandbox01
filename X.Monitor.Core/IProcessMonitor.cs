using System;
using X.Monitor.Core.Model;

namespace X.Monitor.Core
{
	public interface IProcessMonitor : IDisposable
	{
		bool TryCollectInfo(ProcessUsage processUsageInfo);
	}
}
