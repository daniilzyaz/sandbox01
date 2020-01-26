using System;
using System.Collections.Generic;
using System.Diagnostics;
using X.Monitor.Core.Model;

namespace X.Monitor.Core
{
	internal class CpuProcessMonitor : IProcessMonitor
	{
		private Dictionary<string, PerformanceCounter> PerformanceCounters = new Dictionary<string, PerformanceCounter>();

		public bool TryCollectInfo(ProcessUsage processUsageInfo)
		{
			if (!PerformanceCounters.ContainsKey(processUsageInfo.Name))
				PerformanceCounters.Add(processUsageInfo.Name, new PerformanceCounter("Process", "% Processor Time", processUsageInfo.Name, true));

			try
			{
				var cpu = PerformanceCounters[processUsageInfo.Name].NextValue();
				processUsageInfo.Cpu = cpu / Environment.ProcessorCount;

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
