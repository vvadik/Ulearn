namespace uLearn.Web.ExecutionService
{
	public static class StringExtention
	{
		public static string NormalizeEoln(this string str)
		{
			return str.LineEndingsToUnixStyle().Trim();
		}
	}
}