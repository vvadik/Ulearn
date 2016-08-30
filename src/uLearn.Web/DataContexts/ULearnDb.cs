using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using uLearn.Web.Migrations;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class ULearnDb : IdentityDbContext<ApplicationUser>
	{
		public ULearnDb()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<ULearnDb, Configuration>());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<CommentLike>()
				.HasRequired(x => x.Comment)
				.WithMany(x => x.Likes)
				.HasForeignKey(x => x.CommentId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<GroupMember>()
				.HasRequired(m => m.Group)
				.WithMany(g => g.Members)
				.HasForeignKey(m => m.GroupId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ExerciseCodeReview>()
				.HasRequired(r => r.ExerciseChecking)
				.WithMany(x => x.Reviews)
				.HasForeignKey(r => r.ExerciseCheckingId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Like>()
				.HasRequired(l => l.Submission)
				.WithMany(s => s.Likes)
				.HasForeignKey(l => l.SubmissionId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<UserExerciseSubmission>()
				.HasRequired(s => s.AutomaticChecking)
				.WithMany()
				.WillCascadeOnDelete(false);
		}
		
		public DbSet<UserQuestion> UserQuestions { get; set; }
		public DbSet<SlideRate> SlideRates { get; set; }
		public DbSet<Visit> Visits { get; set; }
		public DbSet<SlideHint> Hints { get; set; }
		public DbSet<Like> SolutionLikes { get; set; }
		public DbSet<UserQuiz> UserQuizzes { get; set; }
		public DbSet<UnitAppearance> Units { get; set; }
		public DbSet<TextBlob> Texts { get; set; }
		public DbSet<LtiConsumer> Consumers { get; set; }
		public DbSet<LtiSlideRequest> LtiRequests { get; set; }
		public DbSet<RestoreRequest> RestoreRequests { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Comment> Comments { get; set; }
		public DbSet<CommentLike> CommentLikes { get; set; }
		public DbSet<CommentsPolicy> CommentsPolicies { get; set; }

		public DbSet<QuizVersion> QuizVersions { get; set; }
		public DbSet<CourseVersion> CourseVersions { get; set; }

		public DbSet<ManualExerciseChecking> ManualExerciseCheckings { get; set; }
		public DbSet<AutomaticExerciseChecking> AutomaticExerciseCheckings { get; set; }
		public DbSet<ManualQuizChecking> ManualQuizCheckings { get; set; }
		public DbSet<AutomaticQuizChecking> AutomaticQuizCheckings { get; set; }
		public DbSet<UserExerciseSubmission> UserExerciseSubmissions { get; set; }
		public DbSet<ExerciseCodeReview> ExerciseCodeReviews { get; set; }

		public DbSet<Group> Groups { get; set; }
		public DbSet<GroupMember> GroupMembers { get; set; }
	}
}