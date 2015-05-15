using UI.Tests.PageObjects.PageObjects;

namespace UI.Tests.PageObjects.Interfaces
{
	public interface ITocSlide
	{
		PageType SlideType { get; }
		string Name { get; }
		SlideLabelInfo Info { get; }

		void Click();

	}
}
