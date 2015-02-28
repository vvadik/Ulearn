using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class LikeButton
	{
		private readonly IWebElement like;
		private bool isLiked;

		public LikeButton(IWebElement like)
		{
			if (like == null)
				throw new NotFoundException("like button not found");
			this.like = like;
			isLiked = UlearnDriverComponents.UlearnDriver.HasCss(like, "btn-primary");
		}

		public void Click()
		{
			like.Click();
			isLiked = !isLiked;
		}
	}
}
