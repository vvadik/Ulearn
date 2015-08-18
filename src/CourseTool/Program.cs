using CommandLine;

namespace uLearn.CourseTool
{
	public class Program
	{
		public static int Main(string[] args)
		{
			return Parser.Default.ParseArguments<ConvertOptions, ULearnPatchOptions, VideoPatchOptions, SampleCustomPatchOptions>(args).Return(
				(AbstractOptions options) => options.Execute(),
				_ => -1
			);
		}
	}
}
