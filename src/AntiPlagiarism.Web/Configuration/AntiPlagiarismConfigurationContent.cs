using Ulearn.Core.Configuration;

namespace AntiPlagiarism.Web.Configuration
{
	public class AntiPlagiarismConfiguration : UlearnConfigurationBase
	{
		public AntiPlagiarismConfigurationContent AntiPlagiarism { get; set; }
	}

	public class AntiPlagiarismConfigurationContent
	{
		/// Длина сниппета в токенах
		public int SnippetTokensCount { get; set; }

		/// При большей длине в символах не используем код для плагиата
		public int MaxCodeLength { get; set; }

		// Параметры пересчета порога похожести решений, чтобы быть плагиатом
		public StatisticsAnalyzingConfiguration StatisticsAnalyzing { get; set; }

		// Настройки, используемые в методе GetPlagiarismsAsync
		public PlagiarismDetectorConfiguration PlagiarismDetector { get; set; }

		/// Настройки запросов контроллера
		public ActionsConfiguration Actions { get; set; }

		/// Число потоков для обработки очереди новых посылок
		public int ThreadsCount { get; set; }
	}

	public class StatisticsAnalyzingConfiguration
	{
		/// Последние посылки скольки авторов брать при пересчете статистических параметров
		public int CountOfLastAuthorsForCalculatingMeanAndDeviation { get; set; }

		/// Статически параметры задачи пересчитываются каждый раз после стольки-то решений
		public int RecalculateStatisticsAfterSubmissionsCount { get; set; }

		// Границы подозрительности вычисляются как мат ожидание + коэффициент * дисперсию

		/// Коэффициент, на который умножается дисперсия при вычислении слабой границы ("Похожие решения меньшей подозрительности") 
		public int FaintSuspicionCoefficient { get; set; }

		/// Коэффициент, на который умножается дисперсия при вычислении строгой границы 
		public int StrongSuspicionCoefficient { get; set; }

		/// После вычислений границы сликом большие и слишком маленькие округляются до 4 значений ниже
		public double MinFaintSuspicionLevel { get; set; }
		public double MinStrongSuspicionLevel { get; set; }
		public double MaxFaintSuspicionLevel { get; set; }
		public double MaxStrongSuspicionLevel { get; set; }
	}

	public class PlagiarismDetectorConfiguration
	{
		// При большем количестве авторов сниппет слишком типичен, поэтому не важен для плагиата
		public int SnippetAuthorsCountThreshold { get; set; }

		/// 1. При поиске плагиата сначала выбирается это количество сниппетов анализируемого решения с количеством авторов от 2 до SnippetAuthorsCountThreshold
		public int CountOfColdestSnippetsUsedToFirstSearch { get; set; }

		/// 2. Дальше берется это количество посылок с максимальным количеством совпадающих сниппетов + сниппеты, в которых количество совпадающих сниппетов больше 0.9*максимальное
		public int MaxSubmissionsAfterFirstSearch { get; set; }

		/// 3. Дальше выбирается это количество сниппетов анализируемого решения с количеством авторов от 0 до SnippetAuthorsCountThreshold
		public int CountOfColdestSnippetsUsedToSecondSearch { get; set; }

		// 4. Получаем вхождения сниппетов, найденных на 2 шаге, среди посылок, надйденных на втором шаге

		// 5. Длину совпадения находим как количество токенов обоих решений, которые принадлежат общим сниппетам из шага 4

		// 6. Получаем вес: делим длину совпадения на число токенов в обоих решениях. Делим на число типов сниппетов (для С# 2: тип токена и значение токена)

		// 7. Выкидываем подозрительное на плагиат решение, если его вес меньше FaintSuspicion (слабой границы)
	}

	public class ActionsConfiguration
	{
		public GetAuthorPlagiarismsConfiguration GetAuthorPlagiarisms { get; set; }
	}

	public class GetAuthorPlagiarismsConfiguration
	{
		public int MaxLastSubmissionsCount { get; set; }
	}
}