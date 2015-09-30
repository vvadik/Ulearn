using System.Collections.Generic;
using System.Collections.Immutable;
using uLearn.Model.Edx.EdxComponents;

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

		public abstract Component ToEdxComponent(string displayName, Slide slide, int componentIndex);

		public virtual string TryGetText()
		{
			return null;
		}
	}
}