using OpenQA.Selenium;
using Selenium.UlearnDriverComponents.Interfaces;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class Hint
	{
		private readonly LikeButton likeButton;
		//private IWebDriver driver;
		private readonly IWebElement hint;

		public Hint(LikeButton likeButton, IWebElement hint)
		{
			//this.driver = driver;
			this.likeButton = likeButton;
			this.hint = hint;
		}

		public string GetHintText()
		{
			return hint.Text;
		}

		public LikeButton GetLikeButton()
		{
			return likeButton;
		}
	}
}
