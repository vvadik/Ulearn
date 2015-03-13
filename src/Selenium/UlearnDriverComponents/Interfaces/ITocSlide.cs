using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents.Interfaces
{
	public interface ITocSlide
	{
		PageType SlideType { get; }
		string Name { get; }
		SlideLabelInfo Item { get; }

		void Click();

	}
}
