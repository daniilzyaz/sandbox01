using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using X.Monitor.Core.Model;

namespace X.Monitor.Core
{
	public class MonitorService : IDisposable
	{
		private ConcurrentDictionary<string, ProcessUsage> Processes = new ConcurrentDictionary<string, ProcessUsage>();

		private List<IProcessMonitor> ProcessMonitors = new List<IProcessMonitor>();

		private MonitorService() { }

		public static MonitorService Create()
		{
			var service = new MonitorService();
			service.ProcessMonitors.Add(new CpuProcessMonitor());
			service.ProcessMonitors.Add(new RamProcessMonitor());

			return service;
		}

		public IEnumerable<ProcessUsage> GetProccesses()
		{
			if (Processes.IsEmpty)
				RefreshProcesses();

			foreach (var p in Processes)
			{
				yield return p.Value;
			}
		}
		
		public void Collect()
		{
			RefreshProcesses();
			foreach (var pm in ProcessMonitors)
			{
				foreach (var p in Processes)
				{
					pm.TryCollectInfo(p.Value);
				}
			}
		}

		public ActiveProcessesTotalUsage GetActiveProcessTotalUsage()
		{
			var totals = new ActiveProcessesTotalUsage();
			if (Processes.IsEmpty)
				return totals;

			var processes = Processes.Where(x => x.Key != "_Total" && x.Key != "Idle")
									 .Select(x => x.Value);

			totals.Cpu = processes.Sum(p => p.Cpu);
			totals.Ram = processes.Sum(p => p.Ram);

			return totals;
		}

		private void RefreshProcesses()
		{
			var processNames = GetAllProcessNames();
			foreach (string pName in processNames)
			{
				Processes.TryAdd(pName, new ProcessUsage(pName));
			}
		}

		private string[] GetAllProcessNames()
		{
			var processWorkingSetCategory = new PerformanceCounterCategory("Process");
			return processWorkingSetCategory.GetInstanceNames();
		}

		public void Dispose()
		{
			foreach (var pm in ProcessMonitors)
			{
				pm.Dispose();
			}
		}
	}
}
