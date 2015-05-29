using System.Collections.Generic;
using System.Collections.Immutable;
using uLearn.CSharp;

namespace uLearn.Model.Blocks
{
	public abstract class SlideBlock
	{
		public virtual void Validate()
		{
		}

		public virtual IEnumerable<SlideBlock> BuildUp(IFileSystem fs, IImmutableSet<string> filesInProgress, CourseSettings settings, Lesson lesson)
		{
			yield return this;
		}
	}
}