using System;

namespace Ulearn.Core
{
	public static class DoIt
	{
		public static T TryOrDefault<T>(Func<T> tryFunc, Func<T> defaultFunc)
		{
			try
			{
				return tryFunc();
			}
			catch (Exception)
			{
				return defaultFunc();
			}
		}
	}
}