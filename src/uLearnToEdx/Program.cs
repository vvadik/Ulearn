using CommandLine;

namespace uLearnToEdx
{
	public class Program
	{
		public static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<ConvertOptions, ULearnPatchOptions, VideoPatchOptions, CustomPatchOptions>(args).Return(
				(AbstractOptions options) => options.Execute(),
				_ => -1
			);
		}
	}
}
