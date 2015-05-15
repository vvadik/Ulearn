namespace UI.Tests.Core
{
	public interface IGetContext
	{
		TPageObject Get<TPageObject>();
		TPageObject[] All<TPageObject>();
	}
}