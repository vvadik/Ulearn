using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class AnalyticsTableInfo
	{
		public int VisitersCount { get; set; }
		public int SolversCount { get; set; }
		public Marks Marks { get; set; }
	}

	public class Marks
	{
		public int Good { get; private set; }
		public int NotUnderstand { get; private set; }
		public int Trivial { get; private set; }
		public int NotWatched { get; private set; }

		public int Count
		{
			get { return Good + NotUnderstand + NotWatched + Trivial; }
		}

		public Marks()
		{
			Good = 0;
			NotUnderstand = 0;
			NotWatched = 0;
			Trivial = 0;
		}

		public void AddGood()
		{
			Good++;
		}

		public void AddNotUnderstand()
		{
			NotUnderstand++;
		}

		public void AddTrivial()
		{
			Trivial++;
		}

		public void AddNotWatched()
		{
			NotWatched++;
		}
	}
}
