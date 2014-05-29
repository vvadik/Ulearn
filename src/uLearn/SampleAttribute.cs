using System;

namespace uLearn
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class SampleAttribute : Attribute
	{
	}
}