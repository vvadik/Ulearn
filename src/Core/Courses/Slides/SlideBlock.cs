using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Component = Ulearn.Core.Model.Edx.EdxComponents.Component;

namespace Ulearn.Core.Courses.Slides
{
	[DataContract]
	public abstract class SlideBlock
	{
		[XmlAttribute("hide")]
		[DefaultValue(false)]
		[DataMember(Name="hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }

		public virtual void Validate(SlideBuildingContext slideBuildingContext)
		{
		}

		public virtual IEnumerable<SlideBlock> BuildUp(SlideBuildingContext context, IImmutableSet<string> filesInProgress)
		{
			yield return this;
		}

		public abstract Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot);

		public virtual string TryGetText()
		{
			return null;
		}

		public IEnumerable<IApiSlideBlock> ToApiSlideBlocks(ApiSlideBlockBuildingContext context)
		{
			if (context.RemoveHiddenBlocks && Hide)
				return new IApiSlideBlock[] {};
			var apiSlideBlocks = this switch // Порядок важен. На одном классе может быть сразу несколько интерфейсов.
			{
				IApiConvertibleSlideBlock convertible => convertible.ToApiSlideBlocks(context),
				IApiSlideBlock asb => new[] { asb },
				_ => Enumerable.Empty<IApiSlideBlock>()
			};
			if (context.RemoveHiddenBlocks)
				apiSlideBlocks = apiSlideBlocks.Where(b => !b.Hide);
			return apiSlideBlocks;
		}
	}
}