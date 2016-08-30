using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class SlideCheckingsRepo
	{
		private readonly ULearnDb db;

		public SlideCheckingsRepo() : this(new ULearnDb())
		{

		}

		public SlideCheckingsRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task AddQuizAttemptForManualChecking(string courseId, Guid slideId, string userId)
		{
			var manualChecking = new ManualQuizChecking
			{
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,
			};
			db.ManualQuizCheckings.Add(manualChecking);

			await db.SaveChangesAsync();
		}

		public async Task AddQuizAttemptWithAutomaticChecking(string courseId, Guid slideId, string userId, int automaticScore)
		{
			var automaticChecking = new AutomaticQuizChecking
			{
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,
				Score = automaticScore,
			};
			db.AutomaticQuizCheckings.Add(automaticChecking);
			
			await db.SaveChangesAsync();
		}

		public async Task AddManualExerciseChecking(string courseId, Guid slideId, string userId)
		{
			var manualChecking = new ManualExerciseChecking
			{
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,
			};
			db.ManualExerciseCheckings.Add(manualChecking);

			await db.SaveChangesAsync();
		}

		private IEnumerable<T> GetSlideCheckingsByUser<T>(string courseId, Guid slideId, string userId) where T: AbstractSlideChecking
		{
			return db.Set<T>().Where(c => c.CourseId == courseId && c.SlideId == slideId && c.UserId == userId);
		}

		public async Task RemoveAttempts(string courseId, Guid slideId, string userId)
		{
			db.ManualQuizCheckings.RemoveRange(GetSlideCheckingsByUser<ManualQuizChecking>(courseId, slideId, userId));
			db.AutomaticQuizCheckings.RemoveRange(GetSlideCheckingsByUser<AutomaticQuizChecking>(courseId, slideId, userId));
			db.ManualExerciseCheckings.RemoveRange(GetSlideCheckingsByUser<ManualExerciseChecking>(courseId, slideId, userId));
			db.AutomaticExerciseCheckings.RemoveRange(GetSlideCheckingsByUser<AutomaticExerciseChecking>(courseId, slideId, userId));

			await db.SaveChangesAsync();
		}

		public bool IsSlidePassed(string courseId, Guid slideId, string userId)
		{
			return GetSlideCheckingsByUser<ManualQuizChecking>(courseId, slideId, userId).Any() ||
				GetSlideCheckingsByUser<AutomaticQuizChecking>(courseId, slideId, userId).Any() ||
				GetSlideCheckingsByUser<ManualExerciseChecking>(courseId, slideId, userId).Any() ||
				GetSlideCheckingsByUser<AutomaticExerciseChecking>(courseId, slideId, userId).Any(c => c.Score > 0);
		}
		
		private int GetUserScoreForSlide<T>(string courseId, Guid slideId, string userId) where T : AbstractSlideChecking
		{
			return GetSlideCheckingsByUser<T>(courseId, slideId, userId).Select(c => c.Score).DefaultIfEmpty(0).Max();
		}

		public int GetManualScoreForSlide(string courseId, Guid slideId, string userId)
		{
			var quizScore = GetUserScoreForSlide<ManualQuizChecking>(courseId, slideId, userId);
			var exerciseScore = GetUserScoreForSlide<ManualExerciseChecking>(courseId, slideId, userId);

			return Math.Max(quizScore, exerciseScore);
		}

		public int GetAutomaticScoreForSlide(string courseId, Guid slideId, string userId)
		{
			var quizScore = GetUserScoreForSlide<AutomaticQuizChecking>(courseId, slideId, userId);
			var exerciseScore = GetUserScoreForSlide<AutomaticExerciseChecking>(courseId, slideId, userId);

			return Math.Max(quizScore, exerciseScore);
		}

		public IEnumerable<T> GetManualCheckingQueue<T>(string courseId, IEnumerable<Guid> slidesIds=null) where T : AbstractManualSlideChecking
		{
			var query = db.Set<T>().Where(c => c.CourseId == courseId && !c.IsChecked);
			if (slidesIds != null)
				query = query.Where(c => slidesIds.Contains(c.SlideId));
			return query.OrderBy(c => c.Timestamp);
		}

		public IEnumerable<T> GetManualCheckingQueue<T>(string courseId, Guid slideId) where T : AbstractManualSlideChecking
		{
			return GetManualCheckingQueue<T>(courseId, new List<Guid> { slideId });
		}

		public T GetManualCheckingById<T>(int id) where T : AbstractManualSlideChecking
		{
			return db.Set<T>().Find(id);
		}

		public async Task LockManualChecking<T>(T checkingItem, string lockedById) where T : AbstractManualSlideChecking
		{
			checkingItem.LockedById = lockedById;
			checkingItem.LockedUntil = DateTime.Now.Add(TimeSpan.FromMinutes(30));
			await db.SaveChangesAsync();
		}

		public async Task MarkManualCheckingAsChecked<T>(T queueItem, int score) where T : AbstractManualSlideChecking
		{
			queueItem.LockedBy = null;
			queueItem.LockedUntil = null;
			queueItem.IsChecked = true;
			queueItem.Score = score;
			await db.SaveChangesAsync();
		}
	}
}