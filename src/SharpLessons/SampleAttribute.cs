using System;

namespace SharpLessons
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class SampleAttribute : Attribute
	{
	}
}