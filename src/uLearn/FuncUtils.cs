using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using NUnit.Framework;

namespace uLearn
{
	public class FuncUtils
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(FuncUtils));

		public static async Task<T> TrySeveralTimesAsync<T>(Func<Task<T>> func, int triesCount, Func<Task> runAfterFail, Type exceptionType)
		{
			for (var tryIndex = 0; tryIndex < triesCount; tryIndex++)
				try
				{
					return await func();
				}
				catch (Exception e) when (exceptionType.IsInstanceOfType(e))
				{
					if (tryIndex >= triesCount - 1)
						throw;
					log.Warn($"Не удалось, попробую ещё раз (попытка {tryIndex + 2} из {triesCount})");
					await runAfterFail();
				}
				catch (Exception e)
				{
					log.Info($"На попытке {tryIndex + 1} произошло исключение {e.Message}");
					throw;
				}
			throw new Exception($"Can\'t run function {func} for {triesCount} times");
		}

		public static async Task<T> TrySeveralTimesAsync<T>(Func<Task<T>> func, int triesCount, Func<Task> runAfterFail)
		{
			return await TrySeveralTimesAsync(func, triesCount, runAfterFail, typeof(Exception));
		}

		public static async Task<T> TrySeveralTimesAsync<T>(Func<Task<T>> func, int triesCount, Type exceptionType)
		{
			return await TrySeveralTimesAsync(func, triesCount, () => Task.FromResult(0), exceptionType);
		}

		public static async Task<T> TrySeveralTimesAsync<T>(Func<Task<T>> func, int triesCount)
		{
			return await TrySeveralTimesAsync(func, triesCount, () => Task.FromResult(0));
		}

		public static T TrySeveralTimes<T>(Func<T> func, int triesCount, Action runAfterFail, Type exceptionType)
		{
			return TrySeveralTimesAsync(() => Task.Run(func), triesCount, () => Task.Run(runAfterFail), exceptionType).Result;
		}

		public static T TrySeveralTimes<T>(Func<T> func, int triesCount, Action runAfterFail)
		{
			return TrySeveralTimes(func, triesCount, runAfterFail, typeof(Exception));
		}

		public static T TrySeveralTimes<T>(Func<T> func, int triesCount, Type exceptionType)
		{
			return TrySeveralTimes(func, triesCount, () => { }, exceptionType);
		}
		public static T TrySeveralTimes<T>(Func<T> func, int triesCount)
		{
			return TrySeveralTimes(func, triesCount, () => { });
		}
	}

	[TestFixture]
	public class FuncUtils_should
	{
		[SetUp]
		public void ConfigureLogger()
		{
			log4net.Config.BasicConfigurator.Configure(new ConsoleAppender { Threshold = log4net.Core.Level.Debug});
		}

		[Test]
		public void TrySeveralTimes_test()
		{
			var index = 0;
			FuncUtils.TrySeveralTimes<object>(() =>
			{
					index++;
					Console.WriteLine($"Run #{index}");
					if (index != 3)
						throw new Exception("exception");
					return null;
				},
				3,
				() =>
				{
					Thread.Sleep(200);
				});
			Assert.AreEqual(index, 3);
		}

		[Test]
		public async Task TrySeveralTimesAsync_test()
		{
			var index = 0;
			await FuncUtils.TrySeveralTimesAsync(async () =>
			{
				index++;
				Console.WriteLine($"Run #{index}");
				if (index != 3)
					throw new Exception("exception");
				await Task.Delay(100);
				return (object) null;
			}, 3, async () => await Task.Delay(200));
			Assert.AreEqual(index, 3);
		}
	}
}