namespace Ulearn.Core.RunCheckerJobApi
{
	public class StyleError
	{
		/// <summary>
		/// id типа ошибки
		/// </summary>
		public string ErrorType;

		public StyleErrorSpan Span;

		/// <summary>
		/// Текст, показываемый пользователю
		/// </summary>
		public string Message;
	}

	public class StyleErrorSpan
	{
		/// <summary>
		/// Позиция первого символа ошибочного кода в строке
		/// </summary>
		public StyleErrorSpanPosition StartLinePosition;

		/// <summary>
		/// Позиция последнего символа ошибочного кода в строке
		/// </summary>
		public StyleErrorSpanPosition EndLinePosition; 
	}

	public class StyleErrorSpanPosition
	{
		/// <summary>
		/// Номер строки с нуля
		/// </summary>
		public int Line;

		/// <summary>
		/// Номер символа в строке с нуля
		/// </summary>
		public int Character;
	}
}