using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace Selenium.UlearnDriverComponents
{
	public class Db
	{
		private readonly ULearnDb db = new ULearnDb();

		public Db() { }

		public string GetUserId(string currentUserName)
		{
			return db.Users.First(x => x.UserName == currentUserName).Id;
		}

		private static readonly Dictionary<SlideRates, Rate> rateConverter = new Dictionary<SlideRates, Rate>
		{
			{ SlideRates.Good, Rate.Good },
			{ SlideRates.NotUnderstand, Rate.NotUnderstand },
			{ SlideRates.NotWatched, Rate.NotWatched },
			{ SlideRates.Trivial, Rate.Trivial }
		};

		public Rate GetRate(string currentSlideId)
		{
			return rateConverter[db.SlideRates.First(x => x.SlideId == currentSlideId).Rate];
		}
	}
}
