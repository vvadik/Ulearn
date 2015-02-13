using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.UlearnDriver.PageObjects
{
	public class SomeoneSolution
	{
		private readonly IWebElement solution;
		private readonly LikeButton likeButton;

		public SomeoneSolution(IWebElement solution, LikeButton likeButton)
		{
			this.likeButton = likeButton;
			this.solution = solution;
		}

		public string GetSolutionText()
		{
			return solution.Text;
		}

		public LikeButton GetLikeButton()
		{
			return likeButton;
		}
	}
}
