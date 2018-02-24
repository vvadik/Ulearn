using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace uLearn.CSharp.ArrayGetLengthValidation
{
	[TestFixture]
	public class ArrayLengthStylePrimitives_should
	{
		[Test]
		public void get_parent_for_cycle_if_exists()
		{
			var code = @"
public void A()
{
	for(int i = 0; i < k; i++)
	{
		var a = GetSomething();
	}	
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var cycleNode = nodes.OfType<ForStatementSyntax>().First();
			var foundCycleNode = syntaxNode.GetParentCycle();
			foundCycleNode.Should().Be(cycleNode);
		}

		[Test]
		public void get_parent_while_cycle_if_exists()
		{
			var code = @"
public void A()
{
	while((new Random()).Next() > 1)
	{
		var a = GetSomething();
	}
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var cycleNode = nodes.OfType<WhileStatementSyntax>().First();
			var foundCycleNode = syntaxNode.GetParentCycle();
			foundCycleNode.Should().Be(cycleNode);
		}

		[Test]
		public void get_parent_do_while_cycle_if_exists()
		{
			var code = @"
public void A()
{
	do
	{
		var a = GetSomething();
	} while((new Random()).Next() > 1)
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var cycleNode = nodes.OfType<DoStatementSyntax>().First();
			var foundCycleNode = syntaxNode.GetParentCycle();
			foundCycleNode.Should().Be(cycleNode);
		}
		
		[Test]
		public void get_parent_foreach_cycle_if_exists()
		{
			var code = @"
public void A()
{
	foreach(var number in Enumerable.Range(1, 485))
	{
		var a = GetSomething();
	}
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var cycleNode = nodes.OfType<ForEachStatementSyntax>().First();
			var foundCycleNode = syntaxNode.GetParentCycle();
			foundCycleNode.Should().Be(cycleNode);
		}
		
		[Test]
		public void get_null_if_no_parent_cycle()
		{
			var code = @"
public void A()
{
	var a = GetSomething();
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var foundCycleNode = syntaxNode.GetParentCycle();
			foundCycleNode.Should().Be(null);
		}
		
		[Test]
		public void get_correct_parent_cycle_if_several_exist()
		{
			var code = @"
public void A()
{
 	foreach(var number in new List<int> {1})
 	{	
 		for (int i = 1; i < 485; i++)
 		{
 			var a = GetSomething();
 		}
 	}
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var cycleNode = nodes.OfType<ForStatementSyntax>().First();
			var foundCycleNode = syntaxNode.GetParentCycle();
			foundCycleNode.Should().Be(cycleNode);
		}
		
		[Test]
		public void Test()
		{
			var code = @"
using System;

namespace B
{
	public class C()
	{
		var a = GetSomething();
		public void A()
		{
			foreach(var number in new List<int> {1})
			{	
				for (int i = 1; i < 485; i++)
				{
					a = GetSomething();
				}
			}
		}
	}
}
";
			var syntaxTree = CSharpSyntaxTree.ParseText(code);
			var nodes = syntaxTree.GetRoot().DescendantNodes().ToList();
			var syntaxNode = nodes.OfType<InvocationExpressionSyntax>().First();
			var cycleNode = nodes.OfType<ForStatementSyntax>().First();
			cycleNode.ContainsAssignmentOf("a").Should().Be(true);
		}
	}
}