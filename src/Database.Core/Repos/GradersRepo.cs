using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class GradersRepo : IGradersRepo
	{
		private readonly UlearnDb db;
		private readonly UlearnUserManager userManager;
		private readonly ILogger logger;

		public GradersRepo(UlearnDb db, ILogger logger, UlearnUserManager userManager)
		{
			this.db = db;
			this.userManager = userManager;
			this.logger = logger;
		}

		[CanBeNull]
		public GraderClient FindGraderClient(string courseId, Guid clientId)
		{
			var client = db.GraderClients.Find(clientId);
			if (client == null || !client.CourseId.EqualsIgnoreCase(courseId))
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
			var user = new ApplicationUser { UserName = $"__grader_client_{clientId.GetNormalizedGuid()}__" };
			var password = StringUtils.GenerateSecureAlphanumericString(10);
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

		public List<ExerciseSolutionByGrader> GetClientSolutions(GraderClient client, string search, int count, int offset = 0)
		{
			var solutions = db.ExerciseSolutionsByGrader.Where(s => s.ClientId == client.Id);
			if (!string.IsNullOrEmpty(search))
			{
				var solutionIds = GetGraderSolutionByClientUserId(search).Select(w => w.Id);
				solutions = solutions.Where(s => solutionIds.Contains(s.Id));
			}

			return solutions
				.OrderByDescending(s => s.Id)
				.Skip(offset)
				.Take(count)
				.ToList();
		}

		private IEnumerable<SubmissionIdWrapper> GetGraderSolutionByClientUserId(string clientUserId)
		{
			if (string.IsNullOrEmpty(clientUserId))
				return db.ExerciseSolutionsByGrader.Select(s => new SubmissionIdWrapper(s.Id));

			var splittedUserId = clientUserId.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			var query = string.Join(" & ", splittedUserId.Select(s => "\"" + s.Trim().Replace("\"", "\\\"") + "*\""));
			return db.ExerciseSolutionsByGrader
				.FromSqlInterpolated($"SELECT * FROM dbo.ExerciseSolutionByGraders WHERE CONTAINS([ClientUserId], {query})")
				.AsEnumerable()
				.Select(s => new SubmissionIdWrapper(s.Id));
		}
	}

	/* System.String is not available for table-valued functions so we need to create ComplexTyped wrapper */
	[ComplexType]
	public class SubmissionIdWrapper
	{
		public SubmissionIdWrapper(int submissionId)
		{
			Id = submissionId;
		}

		public int Id { get; set; }
	}
}