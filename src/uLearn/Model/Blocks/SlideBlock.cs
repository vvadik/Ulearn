using System.Collections.Generic;
using System.Collections.Immutable;

namespace uLearn.Model.Blocks
{
	public abstract class SlideBlock
	{
		public virtual void Validate()
		{
		}

		public virtual IEnumerable<SlideBlock> BuildUp(BuildUpContext context, IImmutableSet<string> filesInProgress)
		{
			yield return this;
		}
	}
}