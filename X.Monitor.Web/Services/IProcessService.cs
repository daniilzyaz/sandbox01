using System.Collections.Generic;
using X.Monitor.Web.Models;

namespace X.Monitor.Web.Services
{
	public interface IProcessService
	{
		IEnumerable<ProcessItemViewModel> Get();
	}
}
