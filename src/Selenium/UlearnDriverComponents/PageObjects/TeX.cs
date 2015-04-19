using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class TeX
	{
		public TeX(bool isRender)
		{
			IsRender = isRender;
		}

		public bool IsRender { get; private set; }
	}
}
