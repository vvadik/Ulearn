using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Extensions;
using Serilog;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public class StatisticsParametersFinder
	{
		private readonly ILogger logger;
		private readonly PlagiarismDetector plagiarismDetector;

		public StatisticsParametersFinder(ILogger logger, PlagiarismDetector plagiarismDetector)
		{
			this.logger = logger;
			this.plagiarismDetector = plagiarismDetector;
		}

		public async Task<(List<double>, TaskStatisticsParameters)> FindStatisticsParametersAsync(List<Submission> submissions)
		{
			var pairs = submissions.SelectMany(s => submissions, Tuple.Create).Where(pair => pair.Item1.Id < pair.Item2.Id).ToList();

			var weights = new List<double>();
			var pairIndex = 0;
			foreach (var (firstSubmission, secondSubmission) in pairs)
				weights.Add(await GetLinkWeightAsync(firstSubmission, secondSubmission, pairIndex++, pairs.Count).ConfigureAwait(false));

			/* Remove all 1's from weights list.
			   Ones are weights for pairs of absolutely identical solutions (i.e. copied from the internet or shared between students).
			   They are negatively affect to the mean and standard deviation of weights. So we decided just remove them from the array before
			   calculation mean and deviation. See https://yt.skbkontur.ru/issue/ULEARN-78 for details. */
			weights = weights.Where(w => w < 1 - 1e-6).ToList();

			logger.Information($"Пересчитываю статистические параметры задачи (TaskStatisticsParameters) по следующему набору весов: [{string.Join(", ", weights)}]");

			var mean = weights.Mean();
			var deviation = weights.Deviation(mean);
			return (weights, new TaskStatisticsParameters
			{
				Mean = mean,
				Deviation = deviation,
			});
		}

		private Task<double> GetLinkWeightAsync(Submission first, Submission second, int index, int totalCount)
		{
			logger.Information($"Вычисляю коэффициент похожести решения #{first.Id} и #{second.Id} ({index} из {totalCount})");
			return plagiarismDetector.GetWeightAsync(first, second);
		}
	}
}