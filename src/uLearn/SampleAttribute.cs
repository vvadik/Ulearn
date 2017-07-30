using System;

namespace uLearn
{
	public class ShowBodyOnSlideAttribute : Attribute
	{
	}

	public class HideOnSlideAttribute : Attribute
	{
	}

	public class ExcludeFromSolutionAttribute : Attribute
	{
		public bool IsSolutionPart { get; set; }

		public ExcludeFromSolutionAttribute(bool isSolutionPart = true)
		{
			IsSolutionPart = isSolutionPart;
		}
	}
}