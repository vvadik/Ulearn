using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace UI.Tests.Core
{
	public class LazyUtils
	{

		private static readonly MethodInfo StronglyTypeGetterOfT = typeof(LazyUtils).GetMethod("MakeStronglyTyped", BindingFlags.NonPublic | BindingFlags.Static);

		[UsedImplicitly]
		private static Func<T> MakeStronglyTyped<T>(Func<object> getter)
		{
			return () => (T)getter();
		}

		private static object CreateStronglyTypedFuncOfT(Type resultType, Func<object> weaklyTypedFunc)
		{
			return StronglyTypeGetterOfT.MakeGenericMethod(resultType).Invoke(null, new object[] { weaklyTypedFunc });
		}

		public static object MakeLazy(Type lazyType, Func<object> get)
		{
			var resultType = lazyType.GetGenericArguments().Single();
			var func = CreateStronglyTypedFuncOfT(resultType, get);
			var constructorInfo = lazyType.GetConstructor(new[] { typeof(Func<>).MakeGenericType(resultType) });
			Debug.Assert(constructorInfo != null);
			return constructorInfo.Invoke(new[] { func });
		}
	}
	[TestFixture]
	public class LazyUtils_should
	{
		[Test]
		public void MakeLazy()
		{
			var lazy = (Lazy<int>)LazyUtils.MakeLazy(typeof(Lazy<int>), () => (object)42);
			Assert.AreEqual(42, lazy.Value);
		}
	}
}