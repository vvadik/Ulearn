namespace uLearn
{
	public interface ISolutionValidator
	{
		///<summary>Проверка того, что код не является полным исходником</summary>>
		string FindFullSourceError(string userCode);

		///<summary>Проверка всего решения на компилируемость</summary>
		string FindSyntaxError(string solution);

		///<summary>Проверка валидаторами кода, который написал студент</summary>
		string FindValidatorError(string userCode, string solution);
	}
}