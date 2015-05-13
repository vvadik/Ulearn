using System.Collections.Generic;

namespace UI.Tests.PageObjects.Interfaces
{
	public interface ITocUnit
	{
		void Click();
		IReadOnlyCollection<string> GetSlidesName();
		IReadOnlyCollection<ITocSlide> GetSlides();
		bool Collapse { get; }

	}
}
