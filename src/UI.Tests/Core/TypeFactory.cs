using System;

namespace UI.Tests.Core
{
	public class TypeFactory
	{
		public TypeFactory(Func<Type, bool> canCreate, Func<ValueConstructionInfo, object> create)
		{
			CanCreate = canCreate;
			Create = create;
		}

		public readonly Func<Type, bool> CanCreate;
		public readonly Func<ValueConstructionInfo, object> Create;
	}
}