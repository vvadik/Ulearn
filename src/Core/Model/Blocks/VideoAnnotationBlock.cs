using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Blocks
{
	public class VideoAnnotationFragment
	{
		/// <summary>
		/// формат 'mm:ss'
		/// </summary>
		[XmlElement("start")]
		public string StartTimeAsString { get; set; }

		/// <summary>
		/// Markdown описания фрагмента видео-эпизода, начинающегося в момент времени start
		/// </summary>
		[XmlElement("text")]
		public string Text { get; set; }
	}

	[XmlType("annotation")]
	public class VideoAnnotationBlock : SlideBlock
	{
		/// <summary>
		/// Markdown описания всего видео-эпизода
		/// </summary>
		[XmlElement("text")]
		public string Text { get; set; }
		[XmlArray("fragments")]
		[XmlArrayItem("fragment")]
		public VideoAnnotationFragment[] Fragments { get; set; }

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			string FormatFragment(VideoAnnotationFragment f) 
				=> $"* {f.StartTimeAsString} — {f.Text}";

			var fragmentsList = string.Join("\n", Fragments.Select(FormatFragment));
			yield return new MdBlock($"## Содержание видео\n\n{Text}\n\n{fragmentsList}");
		}
	}
}