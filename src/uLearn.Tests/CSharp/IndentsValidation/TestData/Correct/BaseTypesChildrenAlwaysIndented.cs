namespace Correct
{
	public class ClassIndentsChildrenAlways
	{
		public static void I_Am_Indented(string[] args)
		{

		}
	}

	public enum EnumIndentsChildrenAlways
	{
		I_Am_Indented,
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