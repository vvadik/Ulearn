namespace uLearn.Web.Extensions
{
	public static class PluralizeExtensions
	{
		public static string SelectPluralWordInRussian(this int number, RussianPluralizationOptions options)
		{
			var lastDigit = number % 10;
			var word = options.Five;
			if (number % 100 < 10 || number % 100 > 20)
			{
				if (lastDigit == 1)
					word = options.One;
				if (lastDigit >= 2 && lastDigit <= 4)
					word = options.Two;
			}
			return word;
		}

		public static string PluralizeInRussian(this int number, RussianPluralizationOptions options)
		{
			if (options.smallNumbersAreWords)
			{
				switch (number)
				{
					case 1:
						var one = options.Gender == Gender.Male ? "один" : "одна";
						return $"{(options.hideNumberOne ? "" : one + " ")}{options.One}";
					case 2:
						var two = options.Gender == Gender.Male ? "два" : "две";
						return $"{two} {options.Two}";
					case 3:
						return $"три {options.Two}";
				}
			}
			var word = number.SelectPluralWordInRussian(options);
			return $"{number} {word}";
		}
	}

	public class RussianPluralizationOptions
	{
		public string One;
		public string Two;
		public string Five;

		public Gender Gender = Gender.Male;

		public bool smallNumbersAreWords = true;
		public bool hideNumberOne = true;
	}

	public enum Gender
	{
		Male,
		Female
	}
}