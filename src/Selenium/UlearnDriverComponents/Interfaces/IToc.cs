namespace Selenium.UlearnDriverComponents.Interfaces
{
	public interface IToc
	{
		string[] GetUnitsName();
		ITocUnit GetUnitControl(string unitName);
		bool IsCollapsed(string unitName);

	}
}
