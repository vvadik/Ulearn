namespace Correct
{
	public class BlockChildrenIndented
	{
		public BlockChildrenIndented()
		{
			var intendedVar = 0;
			int i = 0;
			foreach (var j in new[] { 0, 1, 2 })
			{
				intendedVar++;
			}

			for (i = 0; i < 5; i++)
			{
				i++;
			}

			while (++i < 5)
			{
				intendedVar = 1;
			}

			do
			{
				intendedVar = 1;
			} while (i++ < 10);
		}
	}
}