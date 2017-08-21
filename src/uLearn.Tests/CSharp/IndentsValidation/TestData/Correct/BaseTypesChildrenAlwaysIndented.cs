namespace Correct
{
	public class ClassWithoutMembers { }

	public class ClassIndentsChildrenAlways
	{
		public static void I_Am_Indented(string[] args)
		{
			var a = 0;
		}
	}

	public enum EnumIndentsChildrenAlways
	{
		I_Am_Indented, // todo любой токен в начале строки, кроме фигурной скобки, - ошибка
		Me_Too
	}

	public interface InterfaceIndentsChildrenAlways
	{
		void I_Am_Indented(params object[] args);
		void Me_Too(params object[] args);
	}

	public struct StructureIndentsChildrenAlways
	{
		public static void I_Am_Indented(string[] args)
		{

		}
	}
}