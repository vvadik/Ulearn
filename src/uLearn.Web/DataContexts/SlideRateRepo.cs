using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class SlideRateRepo
	{
		private readonly ULearnDb db;

		public SlideRateRepo() : this(new ULearnDb())
		{
			
		}

		public SlideRateRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task AddRate(string courseId, string slideId, string userId, SlideRates rate)
		{
			var lastRate = db.SlideRates.FirstOrDefault(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId);
			if (lastRate == null)
			{
				db.SlideRates.Add(new SlideRate
				{
					Rate = rate,
					UserId = userId,
					SlideId = slideId,
					CourseId = courseId
				});
				await db.SaveChangesAsync();
			}
			else
			{
				lastRate.Rate = rate;
				await db.SaveChangesAsync();
			}
		}

		public string FindRate(string courseId, string slideId, string userId)
		{
			var lastRate = db.SlideRates.FirstOrDefault(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId);
			return lastRate == null ? null : lastRate.Rate.ToString();
		}

		public Rates GetRates(string slideId, string courseId)
		{
			var rates = new Rates();
			var allRates = db.SlideRates.Where(x => x.CourseId == courseId && x.SlideId == slideId).ToList();
			foreach (var rate in allRates)
			{
				if (rate.Rate == SlideRates.Good)
					rates.AddGood();
				if (rate.Rate == SlideRates.NotUnderstand)
					rates.AddNotUnderstand();
				if (rate.Rate == SlideRates.NotWatched)
					rates.AddNotWatched();
				if (rate.Rate == SlideRates.Trivial)
					rates.AddTrivial();
			}
			return rates;
		}

		public string GetUserRate(string courseId, string slideId, string userId)
		{
			return ConvertMarkToPrettyString(db.SlideRates.FirstOrDefault(x => x.SlideId == slideId && x.UserId == userId && x.CourseId == courseId));
		}

		private string ConvertMarkToPrettyString(SlideRate slideRate)
		{
			return slideRate == null
				? null
				: slideRate.Rate == SlideRates.Good
					? "Хорошо"
					: slideRate.Rate == SlideRates.NotUnderstand
						? "Не понял"
						: slideRate.Rate == SlideRates.Trivial
							? "Тривиально"
							: null;
		}
	}
}