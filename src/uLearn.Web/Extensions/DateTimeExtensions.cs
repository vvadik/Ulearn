using System;

namespace uLearn.Web.Extensions
{
	public static class DateTimeExtensions
	{
		private const int secondsInHour = 60 * 60;
		private const int secondsInDay = 24 * secondsInHour;

		public static string ToPrettyString(this DateTime dateTime, bool withoutYearIfItsCurrent = false)
		{
			var utcTime = dateTime.ToUniversalTime();
			return $"{utcTime.ToDatePrettyString(withoutYearIfItsCurrent)} в {utcTime.ToShortTimeString()} (UTC)";
		}

		public static string ToDatePrettyString(this DateTime dateTime, bool withoutYearIfItsCurrent=false)
		{
			if (DateTime.Now.Year == dateTime.Year && withoutYearIfItsCurrent)
				return dateTime.ToString("d MMMM");
			return dateTime.ToLongDateString();
		}

		public static string ToAgoPrettyString(this DateTime from, bool showTimeForFarDate=false)
		{
			var diff = DateTime.Now.Subtract(from);
			if (diff.TotalDays > 10)
				return showTimeForFarDate ? from.ToPrettyString(true) : from.ToDatePrettyString(true);
			return diff.ToPrettyString();
		}

		public static string ToPrettyString(this TimeSpan timeSpan)
		{
			var daysAgo = (int)timeSpan.TotalDays;
			var secondsAgo = (int)timeSpan.TotalSeconds;

			if (daysAgo < 0)
				return null;
			
			if (daysAgo == 0)
			{
				if (secondsAgo < 60)
					return "только что";
				if (secondsAgo < secondsInHour)
				{
					var minutesAgo = secondsAgo / 60;
					return $"{minutesAgo.PluralizeInRussian(new RussianPluralizationOptions { One = "минуту", Two = "минуты", Five = "минут", Gender = Gender.Female })} назад";
				}
				if (secondsAgo < secondsInDay)
				{
					var hoursAgo = secondsAgo / secondsInHour;
					return $"{hoursAgo.PluralizeInRussian(new RussianPluralizationOptions { One = "час", Two = "часа", Five = "часов" })} назад";
				}
			}

			if (daysAgo == 1)
				return "вчера";

			return $"{daysAgo.PluralizeInRussian(new RussianPluralizationOptions { One = "день", Two = "дня", Five = "дней" })} назад";
		}
	}
}