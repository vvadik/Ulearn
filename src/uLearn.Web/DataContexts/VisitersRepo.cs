using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class VisitersRepo
	{
		private readonly ULearnDb db;

		public VisitersRepo() : this(new ULearnDb())
		{
			
		}

		public VisitersRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task AddVisiter(string courseId, string slideId, string userId)
		{
			if (db.Visiters.Any(x => x.UserId == userId && x.SlideId == slideId))
				return;
			db.Visiters.Add(new Visiters
			{
				UserId = userId,
				CourseId = courseId,
				SlideId = slideId,
				Timestamp = DateTime.Now
			});
			await db.SaveChangesAsync();
		}

		public int GetVisitersCount(string slideId, string courseId)
		{
			return db.Visiters.Count(x => x.SlideId == slideId);
		}

		public bool IsUserVisit(string courseId, string slideId, string userId)
		{
			return db.Visiters.Any(x => x.SlideId == slideId && x.UserId == userId);
		}

		public HashSet<string> GetIdOfVisitedSlides(string courseId, string userId)
		{
			return new HashSet<string>(db.Visiters.Where(x => x.UserId == userId && x.CourseId == courseId).Select(x => x.SlideId));
		}
		
		public bool HasVisitedSlides(string courseId, string userId)
		{
			return db.Visiters.Any(x => x.UserId == userId && x.CourseId == courseId);
		}

		private async Task UpdateAttempts(string slideId, string userId, Action<Visiters> action)
		{
			var visiters = db.Visiters.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
			if (visiters == null)
				return;
			action(visiters);
			await db.SaveChangesAsync();
		}

		public async Task RemoveAttempts(string slideId, string userId)
		{
			await UpdateAttempts(slideId, userId, visiters =>
			{
				visiters.AttemptsCount = 0;
				visiters.Score = 0;
			});
		}

		public async Task AddAttempt(string slideId, string userId, int score)
		{
			await UpdateAttempts(slideId, userId, visiters =>
			{
				visiters.AttemptsCount++;
				visiters.Score = score;
			});
		}

		public async Task DropAttempt(string slideId, string userId)
		{
			await UpdateAttempts(slideId, userId, visiters =>
			{
				visiters.Score = 0;
			});
		}

		public async Task AddSolutionAttempt(string slideId, string userId, bool isRightAnswer)
		{
			await UpdateAttempts(slideId, userId, visiters =>
			{
				visiters.AttemptsCount++;
				var newScore = isRightAnswer && !visiters.IsSkipped ? 5 : 0;
				if (newScore > visiters.Score)
					visiters.Score = newScore;
			});
		}

		public Dictionary<string, int> GetScoresForSlides(string courseId, string userId)
		{
			return db.Visiters
				.Where(v => v.CourseId == courseId && v.UserId == userId)
				.GroupBy(v => v.SlideId, (s, visiterses) => new { Key = s, Value = visiterses.FirstOrDefault()})
				.ToDictionary(g => g.Key, g => g.Value.Score);
		}

		public int GetScore(string slideId, string userId)
		{
			return db.Visiters
				.Where(v => v.SlideId == slideId && v.UserId == userId)
				.Select(v => v.Score)
				.FirstOrDefault();
		}

		public Visiters GetVisiter(string slideId, string userId)
		{
			return db.Visiters.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
		}

		public async Task SkipSlide(string slideId, string userId)
		{
			var visiter = db.Visiters.FirstOrDefault(v => v.SlideId == slideId && v.UserId == userId);
			if (visiter != null)
				visiter.IsSkipped = true;
			await db.SaveChangesAsync();
		}

		public bool IsSkipped(string slideId, string userId)
		{
			return db.Visiters.Any(v => v.SlideId == slideId && v.UserId == userId && v.IsSkipped);
		}
	}
}