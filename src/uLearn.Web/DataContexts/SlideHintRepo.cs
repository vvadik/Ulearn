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

		public async Task AddHint(string userId, int hintId, string courseId, int slideId)
		{
			db.Hints.Add(new SlideHint
			{
				UserId = userId,
				HintId = hintId,
				CourseId = courseId,
				SlideId = slideId
			});
			await db.SaveChangesAsync();
		}

		public string GetHint(string userId, string courseId, int slideId)
		{
			var answer = db.Hints.Where(x => x.CourseId == courseId && x.SlideId == slideId && x.UserId == userId).ToList();
			if (answer.Count == 0)
				return null;
			return string.Join(" ", answer.Select(x => x.HintId).ToList());
		}
	}
}