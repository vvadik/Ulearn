using System.Linq;

namespace RunCheckerJob
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var isSelfCheck = args.Contains("--selfcheck");

			var program = new RunCheckerJobProgram();
			if (isSelfCheck)
				program.SelfCheck();
			else
				program.Run();
		}
	}
}