using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using log4net;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class GradersRepo
	{
		private readonly ULearnDb db;

		private static readonly ILog log = LogManager.GetLogger(typeof(GradersRepo));
		private readonly ULearnUserManager userManager;

		public GradersRepo(ULearnDb db)
		{
			this.db = db;
			userManager = new ULearnUserManager(db);
		}

		[CanBeNull]
		public GraderClient FindGraderClient(string courseId, Guid clientId)
		{
			var client = db.GraderClients.Find(clientId);
			if (client == null || client.CourseId != courseId)
				return null;
			return client;
		}

		public List<GraderClient> GetGraderClients(string courseId)
		{
			return db.GraderClients.Where(c => c.CourseId == courseId).ToList();
		}

		public async Task<GraderClient> AddGraderClient(string courseId, string name)
		{
			var clientId = Guid.NewGuid();
			var user = new ApplicationUser { UserName = $"__grader_client_{clientId.GetNormalizedGuid()}__"};
			var password = Helpers.StringUtils.GenerateSecureAlphanumericString(10);
			await userManager.CreateAsync(user, password);

			var client = new GraderClient
			{
				Id = clientId,
				CourseId = courseId,
				Name = name,
				User = user,
			};
			db.GraderClients.Add(client);
			await db.SaveChangesAsync();

			return client;
		}

		public async Task<ExerciseSolutionByGrader> AddSolutionFromGraderClient(Guid clientId, int submissionId, string clientUserId)
		{
			var exerciseSolutionByGrader = new ExerciseSolutionByGrader
			{
				ClientId = clientId,
				SubmissionId = submissionId,
				ClientUserId = clientUserId,
			};
			db.ExerciseSolutionsByGrader.Add(exerciseSolutionByGrader);
			await db.SaveChangesAsync();

			return exerciseSolutionByGrader;
		}

		public ExerciseSolutionByGrader FindSolutionFromGraderClient(int solutionId)
		{
			return db.ExerciseSolutionsByGrader.Find(solutionId);
		}
	}
}