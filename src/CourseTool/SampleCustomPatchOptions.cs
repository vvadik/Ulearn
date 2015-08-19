using System;
using CommandLine;
using uLearn.Model.Edx;

namespace uLearn.CourseTool
{
	[Verb("sample_custom_patch", HelpText = "See SampleCustomPatchOptions.cs file for extension example")]
	class SampleCustomPatchOptions : PatchOptions
	{
		[Option('s', "source", HelpText = "Source directory for custom slides", Required = true)]
		public string SourceDir { get; set; }

		public override void Patch(OlxPatcher patcher, Config config, EdxCourse edxCourse)
		{
			// input
			//			 patcher.PatchComponents(...);
			//			 patcher.PatchVerticals(...);
		}
	}
}