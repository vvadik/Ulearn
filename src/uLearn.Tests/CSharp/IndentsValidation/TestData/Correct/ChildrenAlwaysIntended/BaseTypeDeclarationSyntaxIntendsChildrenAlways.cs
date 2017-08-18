namespace uLearn.CSharp.IndentsValidation.TestData.Correct
{
	public class ClassIntendsChildrenAlways
	{
		public static void I_Am_Intented(string[] args)
		{
			
		}
	}

	public enum EnumIntendsChildrenAlways
	{
		I_Am_Intented,
		Me_Too
	}

	public interface InterfaceIntendsChildrenAlways
	{
		void I_Am_Intented(params object[] args);
		void Me_Too(params object[] args);
	}

	public struct StructureIntendsChildrenAlways
	{
		public static void I_Am_Intented(string[] args)
		{

		}
	}
}