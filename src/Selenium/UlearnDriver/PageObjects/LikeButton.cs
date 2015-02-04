using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.UlearnDriver.PageObjects
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
			isLiked = UlearnDriver.HasCss(like, "btn-primary");
		}

		public void Click()
		{
			like.Click();
			isLiked = !isLiked;
		}
	}
}
