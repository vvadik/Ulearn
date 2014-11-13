namespace uLearn
{
	public interface ISolutionValidator
	{
		///<summary>ѕроверка того, что код не €вл€етс€ полным исходником</summary>>
		string FindFullSourceError(string userCode);

		///<summary>ѕроверка всего решени€ на компилируемость</summary>
		string FindSyntaxError(string solution);

		///<summary>ѕроверка валидаторами кода, который написал студент</summary>
		string FindValidatorError(string userCode, string solution);
	}
}