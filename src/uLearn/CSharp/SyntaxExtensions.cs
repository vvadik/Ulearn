using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public static class SyntaxExtensions
	{

		public static IEnumerable<SyntaxNode> GetParents(this SyntaxNode node)
		{
			while (node.Parent != null)
				yield return node = node.Parent;
		}

		public static IEnumerable<SyntaxNode> GetParents(this SyntaxToken token)
		{
			return new[] {token.Parent}.Concat(token.Parent.GetParents());
		}

		public static IEnumerable<SyntaxNode> GetParents(this SyntaxTrivia trivia)
		{
			return trivia.Token.GetParents();
		}

		public static SyntaxToken Identifier(this MemberDeclarationSyntax syntax)
		{
			return ((dynamic)syntax).Identifier;
		}

		public static bool HasAttribute<TAttr>(this MemberDeclarationSyntax node) where TAttr : Attribute
		{
			return node.GetAttributes<TAttr>().Any();
		}

		public static string GetArgument(this AttributeSyntax attribute, int index)
		{
			var expr = (LiteralExpressionSyntax)attribute.ArgumentList.Arguments[index].Expression;
			return (string)expr.Token.Value;
		}

		private static SyntaxList<AttributeListSyntax> AttributeLists(this MemberDeclarationSyntax node)
		{
			return (SyntaxList<AttributeListSyntax>)((dynamic)node).AttributeLists;
		}

		public static IEnumerable<AttributeSyntax> GetAttributes<TAttr>(this MemberDeclarationSyntax node)
			where TAttr : Attribute
		{
			string attrShortName = GetAttributeShortName<TAttr>();
			return node.AttributeLists()
				.SelectMany(a => a.Attributes)
				.Where(a => a.Name.ToString() == attrShortName);
		}

		public static string GetAttributeShortName<TAttr>()
		{
			string attrName = typeof (TAttr).Name;
			return attrName.EndsWith("Attribute") ? attrName.Substring(0, attrName.Length - "Attribute".Length) : attrName;
		}

		public static MethodDeclarationSyntax WithoutAttributes(this MethodDeclarationSyntax node)
		{
			return node.WithAttributeLists(new SyntaxList<AttributeListSyntax>());
		}

		public static MemberDeclarationSyntax WithoutAttributes(this MemberDeclarationSyntax node)
		{
			return (MemberDeclarationSyntax)((dynamic)node).WithAttributeLists(new SyntaxList<AttributeListSyntax>());
		}

		private static string PrettyString(SyntaxNode node, SyntaxToken tokenToAlignLeft)
		{
			int bodyNestingSize = node.SyntaxTree.GetLineSpan(tokenToAlignLeft.Span).StartLinePosition.Character;
			return (new string('\t', bodyNestingSize) + node).RemoveCommonNesting();
		}

		private static string PrettyString(MethodDeclarationSyntax node)
		{
			return PrettyString(node, node.Body.OpenBraceToken);
		}

		private static string PrettyString(BaseTypeDeclarationSyntax node)
		{
			return PrettyString(node, node.OpenBraceToken);
		}

		private static string PrettyString(MemberDeclarationSyntax node)
		{
			if (node is FieldDeclarationSyntax)
				return node.ToString().RemoveCommonNesting();
			return node.ToFullString().RemoveCommonNesting();
		}

		public static string ToPrettyString(this SyntaxNode node)
		{
			return PrettyString((dynamic)node);
		}

		public static string ToNotIdentedString(this SyntaxNode node)
		{
			return node.ToString().RemoveCommonNesting();
		}

		public static MethodDeclarationSyntax TransformExercise(this MethodDeclarationSyntax method)
		{
			return method
				.WithoutAttributes()
				.WithBody(method.Body.WithStatements(new SyntaxList<StatementSyntax>()));
		}
	}
}