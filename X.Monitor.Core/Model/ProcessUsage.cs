
namespace X.Monitor.Core.Model
{
	public class ProcessUsage
	{
		public ProcessUsage(string name)
		{
			Name = name;
		}

		public string Name { get; }
		public float Cpu { get; set; }
		public float Ram { get; set; }

		public override string ToString()
		{
			return $"{Name} => CPU: {Cpu:0.00} % | RAM: {Ram:0.00} MB";
		}
	}
}

