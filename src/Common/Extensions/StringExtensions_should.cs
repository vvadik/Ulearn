using NUnit.Framework;

namespace Ulearn.Common.Extensions
{
	[TestFixture]
	public class StringExtensions_should
	{
		[Test]
		public void TestRenderSimpleMarkdownInTelegramMode()
		{
			var text = @"Выделен весь цикл `for`. Стоило бы написать:

			```
			for (int i = 0; i < 10; i++)
			{
				inputInt[i] = double.Parse(userStr[i]);
			}
			```
			";
			var rendered = text.RenderSimpleMarkdown(isHtml: false, telegramMode: true);
			Assert.True(rendered.IndexOf("<pre>") >= 0);
			Assert.True(rendered.IndexOf("</pre>") >= 0);
			Assert.True(rendered.IndexOf("<code>") >= 0);
			Assert.True(rendered.IndexOf("</code>") >= 0);
		}
	}
}
