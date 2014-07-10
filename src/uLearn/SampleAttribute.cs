using System;

namespace uLearn
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class ShowBodyOnSlideAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class ShowOnSlideAttribute : Attribute
	{
	}
}