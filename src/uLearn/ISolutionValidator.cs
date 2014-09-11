namespace uLearn
{
	public interface ISolutionValidator
	{
		///<summary>Проверка всего решения на компилируемость</summary>
		string FindSyntaxError(string solution);

		///<summary>Проверка валидаторами кода, который написал студент</summary>
		string FindValidatorError(string userCode, string solution);
	}
}