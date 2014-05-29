using System;

namespace uLearn
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HintAttribute : Attribute
	{
		public string HintText { get; set; }

		public HintAttribute(params string[] hintLines)
		{
			HintText = string.Join("\n", hintLines);
		}
	}
}