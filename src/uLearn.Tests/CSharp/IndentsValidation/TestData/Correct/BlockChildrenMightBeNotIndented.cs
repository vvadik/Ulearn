using System;

namespace Correct
{
	public class BlockChildrenMightBeNotIndented
	{
		private string prop;
		public string Prop1
		{
			get
			{
				return prop;
			}
			set
			{
				prop = value;
			}
		}

		public string Prop2
		{ 
			get { return prop; }
			set { prop = value; }
		}

		public event Action Ev1
		{
			add { var a = 0; }
			remove { var a = 0; }
		}

		public event Action Ev2
		{
			add
			{
				var a = 0;
			}
			remove
			{
				var a = 0;
			}
		}
	}
}