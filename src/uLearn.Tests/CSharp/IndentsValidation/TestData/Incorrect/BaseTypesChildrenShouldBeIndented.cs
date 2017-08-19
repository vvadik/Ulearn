namespace Incorrect
{
	public class ClassShouldIntendChildrenAlways
	{ public static void I_Am_Not_Intented(string[] args)
		{

		}
	}

	public enum EnumShouldIntendChildrenAlways
	{ I_Am_Not_Intented, Me_Too
	}

	public interface InterfaceShouldIntendChildrenAlways
	{ void I_Am_Not_Intented(params object[] args);
		void Me_Too(params object[] args);
	}

	public struct StructureShouldIntendChildrenAlways
	{ public static void I_Am_Not_Intented(string[] args)
		{

		}
	}
}