using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class ChoiceGroupPageModel
	{
		public string[] Groups
		{
			get
			{
				return new[]
				{
					"ФТ-101",
					"ФТ-102",
					"КН-101",
					"КН-102",
					"КН-103",
					"ПИ-102",
					"ПИ-101"
				};
			}
		}
	}
}
