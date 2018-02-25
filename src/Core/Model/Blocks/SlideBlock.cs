using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Xml.Serialization;
using Component = uLearn.Model.Edx.EdxComponents.Component;

namespace uLearn.Model.Blocks
{
	public abstract class SlideBlock
	{
		[XmlAttribute("hide")]
		[DefaultValue(false)]
		public bool Hide { get; set; }

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