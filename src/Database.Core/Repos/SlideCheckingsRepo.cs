using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Database.Models.Quizzes;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ulearn.Common.Extensions;

namespace Database.Repos
{
	public class SlideCheckingsRepo : ISlideCheckingsRepo
	{
		private readonly UlearnDb db;
		private readonly Lazy<IVisitsRepo> visitsRepo;

		public SlideCheckingsRepo(UlearnDb db, IServiceProvider serviceProvider)
		{
			this.db = db;
			visitsRepo = new Lazy<IVisitsRepo>(serviceProvider.GetRequiredService<IVisitsRepo>);
		}

		public async Task<ManualQuizChecking> AddManualQuizChecking(UserQuizSubmission submission, string courseId, Guid slideId, string userId)
		{
			var manualChecking = new ManualQuizChecking
			{
				Submission = submission,
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,
			};
			db.ManualQuizCheckings.Add(manualChecking);
			await db.SaveChangesAsync();

			return manualChecking;
		}

		public async Task<AutomaticQuizChecking> AddAutomaticQuizChecking(UserQuizSubmission submission, string courseId, Guid slideId, string userId, int automaticScore)
		{
			var automaticChecking = new AutomaticQuizChecking
			{
				Submission = submission,
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,
				Score = automaticScore,
			};
			db.AutomaticQuizCheckings.Add(automaticChecking);
			await db.SaveChangesAsync();

			return automaticChecking;
		}

		public async Task<List<ManualExerciseChecking>> GetUsersPassedManualExerciseCheckings(string courseId, string userId)
		{
			return (await db.ManualExerciseCheckings
					.Where(c => c.CourseId == courseId && c.UserId == userId && c.IsChecked)
					.ToListAsync())
				.DistinctBy(c => c.SlideId)
				.ToList();
		}

		public async Task<bool> HasManualExerciseChecking(string courseId, Guid slideId, string userId, int submissionId)
		{
			return await db.ManualExerciseCheckings
				.AnyAsync(c => c.CourseId == courseId && c.UserId == userId && c.SlideId == slideId && c.SubmissionId == submissionId);
		}

		public async Task<ManualExerciseChecking> AddManualExerciseChecking(string courseId, Guid slideId, string userId, UserExerciseSubmission submission)
		{
			var manualChecking = new ManualExerciseChecking
			{
				CourseId = courseId,
				SlideId = slideId,
				UserId = userId,
				Timestamp = DateTime.Now,
				SubmissionId = submission.Id,
			};
			db.ManualExerciseCheckings.Add(manualChecking);

			await db.SaveChangesAsync();

			return manualChecking;
		}

		public async Task RemoveWaitingManualCheckings<T>(string courseId, Guid slideId, string userId, bool startTransaction = true) where T : AbstractManualSlideChecking
		{
			using (var transaction = startTransaction ? db.Database.BeginTransaction() : null)
			{
				var checkings = (await GetSlideCheckingsByUser<T>(courseId, slideId, userId, noTracking: false)
					.ToListAsync())
					.Where(c => !c.IsChecked && !c.IsLocked)
					.ToList();
				foreach (var checking in checkings)
				{
					checking.PreRemove(db);
					db.Set<T>().Remove(checking);
				}

				await db.SaveChangesAsync();
				if (transaction != null)
					await transaction.CommitAsync();
			}
		}

		private IQueryable<T> GetSlideCheckingsByUser<T>(string courseId, Guid slideId, string userId, bool noTracking = true) where T : AbstractSlideChecking
		{
			return GetSlideCheckingsByUsers<T>(courseId, slideId, new List<string> { userId }, noTracking);
		}

		private IQueryable<T> GetSlideCheckingsByUsers<T>(string courseId, Guid slideId, IEnumerable<string> userIds, bool noTracking = true) where T : AbstractSlideChecking
		{
			IQueryable<T> dbRef = db.Set<T>();
			if (noTracking)
				dbRef = dbRef.AsNoTracking();
			var items = dbRef.Where(c => c.CourseId == courseId && c.SlideId == slideId && userIds.Contains(c.UserId));
			return items;
		}

		public async Task RemoveAttempts(string courseId, Guid slideId, string userId, bool saveChanges = true)
		{
			db.ManualQuizCheckings.RemoveSlideAction(courseId, slideId, userId);
			db.AutomaticQuizCheckings.RemoveSlideAction(courseId, slideId, userId);
			db.ManualExerciseCheckings.RemoveSlideAction(courseId, slideId, userId);
			db.AutomaticExerciseCheckings.RemoveSlideAction(courseId, slideId, userId);
			if (saveChanges)
				await db.SaveChangesAsync();
		}

		public async Task<bool> IsSlidePassed(string courseId, Guid slideId, string userId)
		{
			return await GetSlideCheckingsByUser<ManualQuizChecking>(courseId, slideId, userId).AnyAsync() ||
				await GetSlideCheckingsByUser<AutomaticQuizChecking>(courseId, slideId, userId).AnyAsync() ||
				await GetSlideCheckingsByUser<ManualExerciseChecking>(courseId, slideId, userId).AnyAsync() ||
				await GetSlideCheckingsByUser<AutomaticExerciseChecking>(courseId, slideId, userId).AnyAsync(c => c.Score > 0);
		}


		#region Slide Score Calculating

		private async Task<int> GetUserScoreForSlide<T>(string courseId, Guid slideId, string userId) where T : AbstractSlideChecking
		{
			return await GetSlideCheckingsByUser<T>(courseId, slideId, userId).Select(c => c.Score).DefaultIfEmpty(0).MaxAsync();
		}

		private async Task<Dictionary<string, int>> GetUserScoresForSlide<T>(string courseId, Guid slideId, IEnumerable<string> userIds) where T : AbstractSlideChecking
		{
			return (await GetSlideCheckingsByUsers<T>(courseId, slideId, userIds)
					.Select(c => new { c.UserId, c.Score })
					.ToListAsync())
				.GroupBy(c => c.UserId)
				.Select(g => new { UserId = g.Key, Score = g.Select(c => c.Score).DefaultIfEmpty(0).Max() })
				.ToDictionary(g => g.UserId, g => g.Score);
		}

		public async Task<int> GetManualScoreForSlide(string courseId, Guid slideId, string userId)
		{
			var quizScore = await GetUserScoreForSlide<ManualQuizChecking>(courseId, slideId, userId);
			var exerciseScore = await GetUserScoreForSlide<ManualExerciseChecking>(courseId, slideId, userId);

			return Math.Max(quizScore, exerciseScore);
		}

		public async Task<Dictionary<string, int>> GetManualScoresForSlide(string courseId, Guid slideId, List<string> userIds)
		{
			var quizScore = await GetUserScoresForSlide<ManualQuizChecking>(courseId, slideId, userIds);
			var exerciseScore = await GetUserScoresForSlide<ManualExerciseChecking>(courseId, slideId, userIds);

			return userIds.ToDictSafe(
				userId => userId,
				userId => Math.Max(quizScore.GetOrDefault(userId, 0), exerciseScore.GetOrDefault(userId, 0))
			);
		}

		public async Task<int> GetAutomaticScoreForSlide(string courseId, Guid slideId, string userId)
		{
			var quizScore = await GetUserScoreForSlide<AutomaticQuizChecking>(courseId, slideId, userId);
			var exerciseScore = await GetUserScoreForSlide<AutomaticExerciseChecking>(courseId, slideId, userId);

			return Math.Max(quizScore, exerciseScore);
		}

		public async Task<Dictionary<string, int>> GetAutomaticScoresForSlide(string courseId, Guid slideId, List<string> userIds)
		{
			var quizScore = await GetUserScoresForSlide<AutomaticQuizChecking>(courseId, slideId, userIds);
			var exerciseScore = await GetUserScoresForSlide<AutomaticExerciseChecking>(courseId, slideId, userIds);

			return userIds.ToDictSafe(
				userId => userId,
				userId => Math.Max(quizScore.GetOrDefault(userId, 0), exerciseScore.GetOrDefault(userId, 0))
			);
		}

		#endregion


		public async Task<List<T>> GetManualCheckingQueue<T>(ManualCheckingQueueFilterOptions options) where T : AbstractManualSlideChecking
		{
			var query = GetManualCheckingQueueFilterQuery<T>(options);
			query = query.OrderByDescending(c => c.Timestamp);

			const int reserveForStartedReviews = 100;
			if (options.Count > 0)
				query = query.Take(options.Count + reserveForStartedReviews);

			IEnumerable<T> enumerable = await query.ToListAsync();
			// Отфильтровывает неактуальные начатые ревью
			enumerable = enumerable
				.GroupBy(g => new { g.UserId, g.SlideId })
				.Select(g => g.First())
				.OrderByDescending(c => c.Timestamp);
			if (options.Count > 0)
				enumerable = enumerable.Take(options.Count);

			return enumerable.ToList();
		}

		private IQueryable<T> GetManualCheckingQueueFilterQuery<T>(ManualCheckingQueueFilterOptions options) where T : AbstractManualSlideChecking
		{
			var query = db.Set<T>()
				.Include(c => c.User)
				.Where(c => c.CourseId == options.CourseId);
			if (options.OnlyChecked.HasValue)
				query = options.OnlyChecked.Value ? query.Where(c => c.IsChecked) : query.Where(c => !c.IsChecked);
			if (options.SlidesIds != null)
				query = query.Where(c => options.SlidesIds.Contains(c.SlideId));
			if (options.UserIds != null)
			{
				if (options.IsUserIdsSupplement)
					query = query.Where(c => !options.UserIds.Contains(c.UserId));
				else
					query = query.Where(c => options.UserIds.Contains(c.UserId));
			}
			return query;
		}

		/// For calculating not checked submissions as well as checked ones
		public async Task<int> GetQuizManualCheckingCount(string courseId, Guid slideId, string userId, DateTime? beforeTimestamp)
		{
			var queue = db.ManualQuizCheckings.Where(c =>
				c.CourseId == courseId
				&& c.SlideId == slideId
				&& c.UserId == userId
				&& !c.IgnoreInAttemptsCount);
			if (beforeTimestamp != null)
				queue = queue.Where(s => s.Timestamp < beforeTimestamp);
			return await queue.CountAsync();
		}

		public async Task<HashSet<Guid>> GetManualCheckingQueueSlideIds<T>(ManualCheckingQueueFilterOptions options)
			where T : AbstractManualSlideChecking
		{
			var query = GetManualCheckingQueueFilterQuery<T>(options);
			return (await query.Select(c => c.SlideId).Distinct().ToListAsync()).ToHashSet();
		}

		public async Task<T> FindManualCheckingById<T>(int id) where T : AbstractManualSlideChecking
		{
			return await db.Set<T>().FindAsync(id);
		}

		public async Task<bool> IsProhibitedToSendExerciseToManualChecking(string courseId, Guid slideId, string userId)
		{
			return await GetSlideCheckingsByUser<ManualExerciseChecking>(courseId, slideId, userId)
				.AnyAsync(c => c.ProhibitFurtherManualCheckings);
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

		// Помечает оцененными посещенные но не оцененные старые ревью
		public async Task MarkManualCheckingAsCheckedBeforeThis<T>(T queueItem) where T : AbstractManualSlideChecking
		{
			var itemsForMark = await db.Set<T>()
				.Where(c => c.CourseId == queueItem.CourseId && c.UserId == queueItem.UserId && c.SlideId == queueItem.SlideId && c.Timestamp < queueItem.Timestamp)
				.ToListAsync();
			var score = 0;
			var changed = false;
			foreach (var item in itemsForMark)
			{
				if (item.IsChecked)
					score = item.Score;
				else
				{
					item.LockedBy = null;
					item.LockedUntil = null;
					item.IsChecked = true;
					queueItem.Score = score;
					changed = true;
				}
			}
			if (changed)
				await db.SaveChangesAsync();
		}

		public async Task ProhibitFurtherExerciseManualChecking(ManualExerciseChecking checking)
		{
			checking.ProhibitFurtherManualCheckings = true;
			await db.SaveChangesAsync();
		}

		public async Task ResetManualCheckingLimitsForUser(string courseId, string userId)
		{
			await DisableProhibitFurtherManualCheckings(courseId, userId);
			await NotCountOldAttemptsToQuizzesWithManualChecking(courseId, userId);
		}

		public async Task ResetAutomaticCheckingLimitsForUser(string courseId, string userId)
		{
			await NotCountOldAttemptsToQuizzesWithAutomaticChecking(courseId, userId);
			await visitsRepo.Value.UnskipAllSlides(courseId, userId);
		}

		public async Task DisableProhibitFurtherManualCheckings(string courseId, string userId)
		{
			var checkingsWithProhibitFurther = await db.ManualExerciseCheckings
				.Where(c => c.CourseId == courseId && c.UserId == userId && c.ProhibitFurtherManualCheckings)
				.ToListAsync();
			foreach (var checking in checkingsWithProhibitFurther)
				checking.ProhibitFurtherManualCheckings = false;
			await db.SaveChangesAsync();
		}

		public async Task NotCountOldAttemptsToQuizzesWithManualChecking(string courseId, string userId)
		{
			var checkings = await db.ManualQuizCheckings
				.Where(c => c.CourseId == courseId && c.UserId == userId && c.IsChecked)
				.ToListAsync();
			foreach (var checking in checkings)
				checking.IgnoreInAttemptsCount = true;
			await db.SaveChangesAsync();
		}

		public async Task NotCountOldAttemptsToQuizzesWithAutomaticChecking(string courseId, string userId)
		{
			var checkings = await db.AutomaticQuizCheckings
				.Where(c => c.CourseId == courseId && c.UserId == userId)
				.ToListAsync();
			foreach (var checking in checkings)
				checking.IgnoreInAttemptsCount = true;
			await db.SaveChangesAsync();
		}

		private async Task<ExerciseCodeReview> AddExerciseCodeReview([CanBeNull] UserExerciseSubmission submission, [CanBeNull] ManualExerciseChecking checking, string userId, int startLine, int startPosition, int finishLine, int finishPosition, string comment, bool setAddingTime)
		{
			var review = db.ExerciseCodeReviews.Add(new ExerciseCodeReview
			{
				AuthorId = userId,
				Comment = comment,
				ExerciseCheckingId = checking?.Id,
				SubmissionId = submission?.Id,
				StartLine = startLine,
				StartPosition = startPosition,
				FinishLine = finishLine,
				FinishPosition = finishPosition,
				AddingTime = setAddingTime ? DateTime.Now : ExerciseCodeReview.NullAddingTime,
			});

			await db.SaveChangesAsync();

			/* Extract review from database to fill review.Author by EF's DynamicProxy */
			return db.ExerciseCodeReviews.AsNoTracking().FirstOrDefault(r => r.Id == review.Entity.Id);
		}

		public Task<ExerciseCodeReview> AddExerciseCodeReview(ManualExerciseChecking checking, string userId, int startLine, int startPosition, int finishLine, int finishPosition, string comment, bool setAddingTime = true)
		{
			return AddExerciseCodeReview(null, checking, userId, startLine, startPosition, finishLine, finishPosition, comment, setAddingTime);
		}

		public Task<ExerciseCodeReview> AddExerciseCodeReview(UserExerciseSubmission submission, string userId, int startLine, int startPosition, int finishLine, int finishPosition, string comment, bool setAddingTime = false)
		{
			return AddExerciseCodeReview(submission, null, userId, startLine, startPosition, finishLine, finishPosition, comment, setAddingTime);
		}

		public async Task<ExerciseCodeReview> FindExerciseCodeReviewById(int reviewId)
		{
			return await db.ExerciseCodeReviews.FindAsync(reviewId);
		}

		public async Task DeleteExerciseCodeReview(ExerciseCodeReview review)
		{
			review.IsDeleted = true;
			await db.SaveChangesAsync();
		}

		public async Task UpdateExerciseCodeReview(ExerciseCodeReview review, string newComment)
		{
			review.Comment = newComment;
			await db.SaveChangesAsync();
		}

		public async Task<Dictionary<int, List<ExerciseCodeReview>>> GetExerciseCodeReviewForCheckings(IEnumerable<int> checkingsIds)
		{
			return (await db.ExerciseCodeReviews
				.Where(r => r.ExerciseCheckingId.HasValue && checkingsIds.Contains(r.ExerciseCheckingId.Value) && !r.IsDeleted)
				.ToListAsync())
				.GroupBy(r => r.ExerciseCheckingId)
				.ToDictionary(g => g.Key.Value, g => g.ToList());
		}

		public async Task<List<string>> GetTopUserReviewComments(string courseId, Guid slideId, string userId, int count)
		{
			var result = await db.ExerciseCodeReviews.Include(r => r.ExerciseChecking)
				.Where(r => r.ExerciseChecking.CourseId == courseId &&
							r.ExerciseChecking.SlideId == slideId &&
							r.AuthorId == userId &&
							!r.HiddenFromTopComments &&
							!r.IsDeleted)
				.Select(r => new { r.Comment, r.ExerciseChecking.Timestamp })
				.GroupBy(r => r.Comment)
				.Select(g => new { 
					g.Key,
					Count = g.Count(),
					Timestamp = g.Max(r => r.Timestamp) })
				.OrderByDescending(g => g.Count)
				.ThenByDescending(g => g.Timestamp)
				.Take(count)
				.Select(g => g.Key)
				.ToListAsync();
			return result;
		}

		public async Task<List<string>> GetTopOtherUsersReviewComments(string courseId, Guid slideId, string userId, int count, List<string> excludeComments)
		{
			var excludeCommentsSet = excludeComments.ToHashSet();
			var result = (await db.ExerciseCodeReviews.Include(r => r.ExerciseChecking)
				.Where(r => r.ExerciseChecking.CourseId == courseId &&
							r.ExerciseChecking.SlideId == slideId &&
							r.AuthorId != userId &&
							!r.HiddenFromTopComments &&
							!r.IsDeleted)
				.GroupBy(r => r.Comment)
				.Select(g => new { g.Key, Count = g.Count() })
				.OrderByDescending(g => g.Count)
				.Take(count * 2)
				.Select(g => g.Key)
				.ToListAsync())
				.Where(c => !excludeCommentsSet.Contains(c))
				.Take(count)
				.ToList();
			return result;
		}

		public async Task<Dictionary<string, List<Guid>>> GetSlideIdsWaitingForManualExerciseCheckAsync(string courseId, IEnumerable<string> userIds)
		{
			return (await db.ManualExerciseCheckings
					.Where(c => c.CourseId == courseId && userIds.Contains(c.UserId) && !c.IsChecked)
					.Select(c => new {c.UserId, c.SlideId})
					.Distinct()
					.ToListAsync())
				.GroupBy(c => c.UserId)
				.ToDictionary(g => g.Key, g => g.Select(c => c.SlideId).ToList());
		}

		public async Task HideFromTopCodeReviewComments(string courseId, Guid slideId, string userId, string comment)
		{
			var reviews = await db.ExerciseCodeReviews.Include(r => r.ExerciseChecking)
				.Where(r => r.ExerciseChecking.CourseId == courseId 
					&& r.ExerciseChecking.SlideId == slideId
					&& r.AuthorId == userId
					&& r.Comment == comment
					&& !r.IsDeleted)
				.ToListAsync();

			foreach (var review in reviews)
				review.HiddenFromTopComments = true;
			await db.SaveChangesAsync();
		}

		public async Task<List<ExerciseCodeReview>> GetLastYearReviewComments(string courseId, Guid slideId)
		{
			var lastYear = DateTime.Today.AddYears(-1);
			var result = await db.ExerciseCodeReviews.Where(
				r => r.ExerciseChecking.CourseId == courseId &&
					r.ExerciseChecking.SlideId == slideId &&
					!r.IsDeleted &&
					r.ExerciseChecking.Timestamp > lastYear
			).ToListAsync();
			return result;
		}

		public async Task<ExerciseCodeReviewComment> AddExerciseCodeReviewComment(string authorId, int reviewId, string text)
		{
			var codeReviewComment = new ExerciseCodeReviewComment
			{
				AuthorId = authorId,
				ReviewId = reviewId,
				Text = text,
				IsDeleted = false,
				AddingTime = DateTime.Now,
			};

			db.ExerciseCodeReviewComments.Add(codeReviewComment);
			await db.SaveChangesAsync();

			/* Extract review from database to fill review.Author by EF's DynamicProxy */
			return await db.ExerciseCodeReviewComments.AsNoTracking().FirstOrDefaultAsync(r => r.Id == codeReviewComment.Id);
		}

		public async Task<ExerciseCodeReviewComment> FindExerciseCodeReviewCommentById(int commentId)
		{
			return await db.ExerciseCodeReviewComments.FindAsync(commentId);
		}

		public async Task DeleteExerciseCodeReviewComment(ExerciseCodeReviewComment comment)
		{
			comment.IsDeleted = true;
			await db.SaveChangesAsync();
		}
	}
}