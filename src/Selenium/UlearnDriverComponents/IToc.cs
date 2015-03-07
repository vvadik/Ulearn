using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.UlearnDriverComponents.PageObjects;

namespace Selenium.UlearnDriverComponents
{
	public interface IToc
	{
		string[] GetUnitsName();
		TocUnit GetUnitControl(string unitName);
		bool IsCollapsed(string unitName);

	}
}
