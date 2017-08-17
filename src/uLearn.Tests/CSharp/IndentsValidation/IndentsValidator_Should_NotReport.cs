using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class IndentsValidator_Should_NotReport
	{
		private IndentsValidator indentsValidator = new IndentsValidator();
		private StringBuilder validatorOut = new StringBuilder();

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			indentsValidator.Warning += msg => validatorOut.AppendLine(msg);
		}

		[TearDown]
		public void TearDown()
		{
			validatorOut.Clear();
		}

		public static CodeTestCase[] consistentIndents =
		{
			new CodeTestCase
			{
				Name = "no extra tabs indents",
				Code = @"using System;
namespace uLearn.CSharp
{
	public static class Hello
	{
		public static void Main()
		{
			Console.WriteLine("""");
		}
	}
}"
			},
			new CodeTestCase
			{
				Name = "no extra spaces indents",
				Code = @"using System;
namespace uLearn.CSharp
{
    public static class Hello
    {
        public static void Main()
        {
            Console.WriteLine();
        }
    }
}"
			},
			new CodeTestCase
			{
				Name = "extra empty line",
				Code = @"using System;

namespace uLearn.CSharp"
			},
			new CodeTestCase
			{
				Name = "extra line with tabs and spaces",
				Code = @"using System;
		   
namespace uLearn.CSharp"
			},
			new CodeTestCase
			{
				Name = "one extra tab indent",
				Code = @"	using System;
	namespace uLearn.CSharp
	{
		public static class Hello
		{
			public static void Main()
			{
				Console.WriteLine();
			}
		}
	}"
			},
			new CodeTestCase
			{
				Name = "one extra spaces indent",
				Code = @"        using System;
        namespace uLearn.CSharp
        {
            public static class Hello
            {
                public static void Main()
                {
                    Console.WriteLine();
                }
        }
    }"
			}
		};

		[TestCaseSource(nameof(consistentIndents))]
		public void On_ConsistentIndents(CodeTestCase testCase)
		{
			indentsValidator.ReportWarningIfIndentsNotValidated(testCase.Code);
			validatorOut.ToString().Should().BeEmpty();
		}
	}
}