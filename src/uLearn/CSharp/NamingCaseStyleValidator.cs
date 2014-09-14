using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class NamingCaseStyleValidator : BaseNamingChecker
	{
		protected override IEnumerable<string> InspectName(SyntaxToken identifier)
		{
			var name = identifier.Text;
			if (string.IsNullOrEmpty(name)) yield break;
			var mustStartWithUpper = MustStartWithUpper(identifier.Parent);
			var mustStartWithLower = MustStartWithLower(identifier.Parent);
			var isUpper = char.IsUpper(name[0]);
			var isLower = char.IsLower(name[0]);

			if (mustStartWithLower && !isLower)
				yield return Report(identifier, "Имя должно начинаться с маленькой буквы");
			if (mustStartWithUpper && !isUpper)
				yield return Report(identifier, "Имя должно начинаться с большой буквы");
		}

		private bool MustStartWithUpper(SyntaxNode node)
		{
			return 
				node is BaseTypeDeclarationSyntax
				|| node is TypeParameterSyntax
				|| node is EnumMemberDeclarationSyntax
				|| node is MethodDeclarationSyntax && ((MethodDeclarationSyntax)node).Modifiers.Any(t => t.CSharpKind() == SyntaxKind.PublicKeyword)
				|| node is VariableDeclaratorSyntax && MustStartWithUpper((VariableDeclaratorSyntax)node);
		}

		private bool MustStartWithUpper(VariableDeclaratorSyntax variableDeclarator)
		{
			var field = AsField(variableDeclarator);
			return field != null && field.Modifiers.Any(m => m.CSharpKind() == SyntaxKind.PublicKeyword); // Публичное поле → с большой
		}

		private bool MustStartWithLower(VariableDeclaratorSyntax variableDeclarator)
		{
			var field = AsField(variableDeclarator);
			return field == null || field.Modifiers.Any(m => m.CSharpKind() == SyntaxKind.PrivateKeyword);
		}

		private static FieldDeclarationSyntax AsField(VariableDeclaratorSyntax variableDeclarator)
		{
			var parent = variableDeclarator.GetParents().FirstOrDefault(p => (p is FieldDeclarationSyntax) || (p is BlockSyntax));
			return parent as FieldDeclarationSyntax;
		}

		private bool MustStartWithLower(SyntaxNode node)
		{
			return
				node is ParameterSyntax
				|| node is VariableDeclaratorSyntax && MustStartWithLower((VariableDeclaratorSyntax)node);

		}
	}
}