using System.Collections.Generic;
using System.Linq;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace AntiPlagiarism.Tests.CodeAnalyzing.CSharp
{
	[TestFixture]
	public class CodeUnitsExtractor_should
	{
		private CodeUnitsExtractor extractor;

		[SetUp]
		public void SetUp()
		{
			extractor = new CodeUnitsExtractor();
		}

		[Test]
		public void TestExtract()
		{
			var codeUnits = extractor.Extract(TestData.SimpleProgramWithMethodAndProperty);
			
			Assert.AreEqual(3, codeUnits.Count);
			CollectionAssert.AreEqual(new List<int> { 31, 45, 51 }, codeUnits.Select(u => u.FirstTokenIndex));

			var methodBodyUnit = codeUnits[0];
			var methodBodyExpectedKinds = new List<SyntaxKind>
			{
				SyntaxKind.OpenBraceToken,
				SyntaxKind.IdentifierToken,
				SyntaxKind.DotToken,
				SyntaxKind.IdentifierToken,
				SyntaxKind.OpenParenToken,
				SyntaxKind.StringLiteralToken,
				SyntaxKind.CloseParenToken,
				SyntaxKind.SemicolonToken,
				SyntaxKind.CloseBraceToken,
			};
			Assert.AreEqual(methodBodyExpectedKinds.Count, methodBodyUnit.Tokens.Count);
			CollectionAssert.AreEqual(methodBodyExpectedKinds, methodBodyUnit.Tokens.Select(t => t.Kind()));
		}

		[Test]
		public void TestGetNodeName()
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(TestData.SimpleProgramWithMethodAndProperty);
			var syntaxTreeRoot = syntaxTree.GetRoot();
			
			Assert.AreEqual("ROOT", CodeUnitsExtractor.GetNodeName(syntaxTreeRoot));
			
			var namespaceDeclaration = syntaxTreeRoot.ChildNodes().First(n => n.Kind() == SyntaxKind.NamespaceDeclaration);
			Assert.AreEqual("HelloWorld.Namespace", CodeUnitsExtractor.GetNodeName(namespaceDeclaration));
			
			var classDeclaration = namespaceDeclaration.ChildNodes().First(n => n.Kind() == SyntaxKind.ClassDeclaration);
			Assert.AreEqual("Program", CodeUnitsExtractor.GetNodeName(classDeclaration));
			
			var methodDeclaration = classDeclaration.ChildNodes().First(n => n.Kind() == SyntaxKind.MethodDeclaration);
			Assert.AreEqual("Main", CodeUnitsExtractor.GetNodeName(methodDeclaration));
			
			var propertyDeclaration = classDeclaration.ChildNodes().First(n => n.Kind() == SyntaxKind.PropertyDeclaration);
			Assert.AreEqual("A", CodeUnitsExtractor.GetNodeName(propertyDeclaration));
		}
	}
}