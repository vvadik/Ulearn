using System;

namespace uLearn
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HintAttribute : Attribute
	{
		public string HintText { get; set; }

		public HintAttribute(string hint)
		{
			HintText = hint;
		}
	}
}