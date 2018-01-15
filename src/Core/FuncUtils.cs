using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
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
					return await func().ConfigureAwait(false);
				}
				catch (Exception e) when (exceptionType.IsInstanceOfType(e))
				{
					log.Error("Исключение:", e);
					if (tryIndex >= triesCount - 1)
						throw;
					log.Warn($"Попробую ещё раз (попытка {tryIndex + 2} из {triesCount})");
					await runAfterFail().ConfigureAwait(false);
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
			return await TrySeveralTimesAsync(func, triesCount, () => Task.Delay(100), exceptionType);
		}

		public static async Task<T> TrySeveralTimesAsync<T>(Func<Task<T>> func, int triesCount)
		{
			return await TrySeveralTimesAsync(func, triesCount, () => Task.Delay(100));
		}

		public static async Task TrySeveralTimesAsync(Func<Task> func, int triesCount)
		{
			await TrySeveralTimesAsync(async () =>
			{
				await func();
				return 0;
			}, triesCount, () => Task.Delay(100));
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
		
		public static void TrySeveralTimes(Action func, int triesCount)
		{
			TrySeveralTimes(() =>
			{
				func();
				return 0;
			}, triesCount, () => { });
		}
	}

	[TestFixture]
	public class FuncUtils_should
	{
		[SetUp]
		public void ConfigureLogger()
		{
			log4net.Config.BasicConfigurator.Configure(new Hierarchy(), new ConsoleAppender { Threshold = log4net.Core.Level.Debug });
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
				() => { Thread.Sleep(200); });
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
				return (object)null;
			}, 3, async () => await Task.Delay(200));
			Assert.AreEqual(index, 3);
		}
	}
}