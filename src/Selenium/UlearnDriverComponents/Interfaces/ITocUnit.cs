using System.Collections.Generic;

namespace Selenium.UlearnDriverComponents.Interfaces
{
	public interface ITocUnit
	{
		void Click();
		IReadOnlyCollection<string> GetSlidesName();
		IReadOnlyCollection<ITocSlide> GetSlides();
		
	}
}
