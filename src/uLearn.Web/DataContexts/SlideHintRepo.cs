using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class SlideHintRepo
	{
		private readonly ULearnDb db;

		public SlideHintRepo() : this(new ULearnDb())
		{
			
		}

		public SlideHintRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task AddHint(string userId, int hintId, string courseId, string slideId)
		{
			if (db.Hints.Any(x => x.UserId == userId && x.HintId == hintId && x.SlideId == slideId && x.CourseId == courseId))
				return;
			db.Hints.Add(new SlideHint
			{
				UserId = userId,
				HintId = hintId,
				CourseId = courseId,
				SlideId = slideId
			});
			await db.SaveChangesAsync();
		}

		public string GetHint(string userId, string courseId, string slideId)
		{
			var answer = db.Hints.Where(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId).ToList();
			if (answer.Count == 0)
				return null;
			return string.Join(" ", answer.Select(x => x.HintId).ToList());
		}

		public int GetHintsCount(string slideId, string courseId)
		{
			return db.Hints.Count(x => x.CourseId == courseId && x.SlideId == slideId);
		}

		public int GetHintsCountForUser(string slideId, string courseId, string userId)
		{
			return db.Hints.Count(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId);
		}

		public int GetHintUsedPercent(string slideId, string courseId, int hintsCountOnSlide, int usersCount)
		{
			var hintsCount = GetHintsCount(slideId, courseId);
			var maxPossibleHintsCount = hintsCountOnSlide*usersCount;
			return (int)(100*(double)hintsCount/maxPossibleHintsCount);
		}

		public int GetHintUsedPercentForUser(string courseId, string slideId, string userId, int hintsCountOnSlide)
		{
			var hintsCount = GetHintsCountForUser(slideId, courseId, userId);
			return (int)(100 * (double)hintsCount / hintsCountOnSlide);
		}

		public async Task<string> LikeHint(string courseId, string slideId, int hintId, string userId)
		{
			var hint = db.Hints.FirstOrDefault(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId && x.HintId == hintId);
			if (hint == null)
				return "error";
			if (hint.IsHintHelped)
			{
				hint.IsHintHelped = false;
				await db.SaveChangesAsync();
				return "cancel";
			}
			hint.IsHintHelped = true;
			await db.SaveChangesAsync();
			return "success";
		}

		public HashSet<int> GetLikedHints(string courseId, string slideId, string userId)
		{
			return new HashSet<int>(db.Hints.Where(x => x.SlideId == slideId && x.CourseId == courseId && x.UserId == userId && x.IsHintHelped).Select(x => x.HintId));
		}
	}
}