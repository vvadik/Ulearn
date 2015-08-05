using CommandLine;

namespace uLearnToEdx
{
	public class Program
	{
		public static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<StartOptions, ConvertOptions, ULearnPatchOptions, CustomPatchOptions>(args).Return(
				(IOptions options) => options.Execute(),
				_ => -1
			);
		}
	}
}
