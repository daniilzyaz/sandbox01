using System;
using System.Collections.Generic;
using System.Diagnostics;
using X.Monitor.Core.Model;

namespace X.Monitor.Core
{
	internal class RamProcessMonitor : IProcessMonitor
	{
		private Dictionary<string, PerformanceCounter> PerformanceCounters = new Dictionary<string, PerformanceCounter>();

		public bool TryCollectInfo(ProcessUsage processUsageInfo)
		{
			if (!PerformanceCounters.ContainsKey(processUsageInfo.Name))
				PerformanceCounters.Add(processUsageInfo.Name, new PerformanceCounter("Process", "Working Set", processUsageInfo.Name, true));

			try
			{
				var ram = PerformanceCounters[processUsageInfo.Name].NextValue();
				processUsageInfo.Ram = (ram / 1024 / 1024);

				return true;
			}
			catch
			{
				PerformanceCounters[processUsageInfo.Name].Dispose();
				PerformanceCounters.Remove(processUsageInfo.Name);
			}

			return false;
		}

		public void Dispose()
		{
			foreach (var pc in PerformanceCounters)
			{
				pc.Value.Dispose();
			}
		}
	}
}
