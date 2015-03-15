using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Interfaces
{
	public interface ITocSlide
	{
		PageType SlideType { get; }
		string Name { get; }
		SlideLabelInfo Info { get; }

		void Click();

	}
}
