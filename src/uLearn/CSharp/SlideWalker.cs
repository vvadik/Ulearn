using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class SlideWalker : CSharpSyntaxWalker
	{
		public readonly List<SlideBlock> Blocks = new List<SlideBlock>();
		public string ExerciseInitialCode { get; private set; }
		public bool IsExercise { get; private set; }
		public string ExpectedOutput { get; private set; }
		public readonly List<string> Hints = new List<string>();
		private List<string> _withoutAttributes = new List<string>();
		private readonly List<string> _withAttributes = new List<string>();
		private string WithExersiceAttribute { get; set; }
		public MethodDeclarationSyntax ExerciseNode;
		public string InitialDataForSolution;
		public SolutionForTesting Solution { get; private set; }

		public SlideWalker() : base(SyntaxWalkerDepth.Trivia)
		{
		}

		public override void Visit(SyntaxNode node)
		{
			base.Visit(node);
			var usings = node.DescendantNodes().Where(x => x is UsingDirectiveSyntax);
			InitialDataForSolution = "";
			InitialDataForSolution = usings.Aggregate("", (x, y) => x + y.ToString());
			InitialDataForSolution += "namespace u \n{\n public class S\n {\n";
			}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);
			if (node.AttributeLists.Count == 0)
			{
				foreach (var nod in _withoutAttributes.Where(x => node.ToString().Contains(x)).ToList())
				{
					_withoutAttributes.Remove(nod);
				}
				_withoutAttributes.Add("\t\t" + node);
			}

			if (ShowOnSlide(node))
			{
				AddSampleBlock(node);
			}
		}

		private void AddSampleBlock(MemberDeclarationSyntax node)
		{
			var sampleBlock = (SlideBlock)CreateSampleBlock((dynamic)node);
			SlideBlock lastBlock = Blocks.LastOrDefault();
			if (lastBlock != null && lastBlock.IsCode)
				Blocks[Blocks.Count - 1] = SlideBlock.FromCode(lastBlock.Text + "\r\n\r\n" + sampleBlock.Text);
			else
				Blocks.Add(sampleBlock);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			base.VisitMethodDeclaration(node);
			if (node.AttributeLists.Count == 0)
			{
				_withoutAttributes.Add("\t\t" + node);
			}
			else
			{
				_withAttributes.Add(node.ToString());
			}
			if (node.HasAttribute<ExpectedOutputAttribute>())
			{
				IsExercise = true;
				ExpectedOutput = node.GetAttributes<ExpectedOutputAttribute>().Select(attr => attr.GetArgument()).FirstOrDefault();
			}
			if (node.HasAttribute<HintAttribute>())
			{
				Hints.AddRange(node.GetAttributes<HintAttribute>().Select(attr => attr.GetArgument()));
			}

			if (ShowOnSlide(node))
			{
				AddSampleBlock(node);
			}
			else if (node.HasAttribute<ExerciseAttribute>())
			{
				_withoutAttributes.Add("\t\t" + node);
				WithExersiceAttribute = node.ToString();
				ExerciseNode = node;
				ExerciseInitialCode = GetExerciseCode(node);
			}
		}

		private static bool ShowOnSlide(MemberDeclarationSyntax node)
		{
			return node.HasAttribute<ShowBodyOnSlideAttribute>() || node.HasAttribute<ShowOnSlideAttribute>();
		}

		private SlideBlock CreateSampleBlock(MethodDeclarationSyntax node)
		{
			var code = node.HasAttribute<ShowOnSlideAttribute>() ? node.WithoutAttributes().ToPrettyString() : node.Body.Statements.ToFullString();
			return SlideBlock.FromCode(code.RemoveCommonNesting());
		}

		private SlideBlock CreateSampleBlock(ClassDeclarationSyntax node)
		{
			string code = node.WithAttributeLists(new SyntaxList<AttributeListSyntax>()).ToFullString().RemoveCommonNesting();
			return SlideBlock.FromCode(code);
		}

		private string GetExerciseCode(MethodDeclarationSyntax method)
		{
			var codeLines = method.TransformExercise().ToFullString().SplitToLines().RemoveCommonNesting();

			return string.Join("\n", FilterSpecialComments(codeLines));
		}

		private IEnumerable<string> FilterSpecialComments(IEnumerable<string> lines)
		{
			var inUncomment = false;
			foreach (var line in lines)
			{
				if (!inUncomment && line.Trim() == "/*uncomment")
				{
					inUncomment = true;
				}
				else if (inUncomment && line.Trim() == "*/")
				{
					inUncomment = false;
				}
				else
					yield return line;
			}
		}

		public override void VisitTrivia(SyntaxTrivia trivia)
		{
			base.VisitTrivia(trivia);
			if (trivia.CSharpKind() == SyntaxKind.MultiLineCommentTrivia)
			{
				var firstLine = trivia.ToString().SplitToLines().First().Trim();
				if (firstLine != "/*uncomment")
					Blocks.Add(ExtractMarkDownFromComment(trivia));
			}
		}

		public static SlideBlock ExtractMarkDownFromComment(SyntaxTrivia comment)
		{
			int identation = comment.SyntaxTree.GetLineSpan(comment.FullSpan).StartLinePosition.Character;
			string[] commentLines = comment.ToString().SplitToLines();
			var sb = new StringBuilder();
			foreach (string line in commentLines.Skip(1).Take(commentLines.Length - 2))
			{
				if (line.Trim() != "")
				{
					if (line.Length < identation || line.Substring(0, identation).Trim() != "")
						throw new Exception("Wrong identation in line: " + line);
					sb.AppendLine(line.Substring(identation));
				}
				else
				{
					sb.AppendLine();
				}
			}
			return SlideBlock.FromMarkdown(sb.ToString());
		}

		public void CleanWithoutAttributes()
		{
						_withoutAttributes = _withoutAttributes
				.Select(x => x.Replace(WithExersiceAttribute, ExerciseInitialCode
					.Split('\n')
					.First()))
				.Select(RemoveBlocksFromSolution)
				.ToList();
		}

		private string RemoveBlocksFromSolution(string arg)
		{
			return _withAttributes.Aggregate(arg, (current, block) => current.Replace(block, ""));
		}

		public void CreateSolution()
		{
			CleanWithoutAttributes();
			var withoutAttribute = _withoutAttributes.Aggregate("", (current, v) => current + (v + "\n"));
			withoutAttribute = CleanFromComments(withoutAttribute);
			withoutAttribute = withoutAttribute
				.Split('\n')
				.Where(x => x.Length > 2)
				.Select(x => x.Substring(2))
				.Aggregate("", (current, v) => current + (v + "\n"));
			//there is alignment for tabs
			var indexForInsert = withoutAttribute.IndexOf(ExerciseInitialCode.Split('\n').First(), StringComparison.Ordinal);
			var sb = new StringBuilder();
			for (var i = indexForInsert - 1; i >= 0 && withoutAttribute[i] == '\t'; i--)
				sb.Append('\t');
			var tabs = sb.ToString();
			for (var i = indexForInsert; i < withoutAttribute.Length; i++)
				if (withoutAttribute[i] == '\n')
				{
					withoutAttribute = withoutAttribute.Insert(i, "\n" + tabs + "{\n" + tabs + "}\n");
					break;
				}
			Solution = new SolutionForTesting(InitialDataForSolution, withoutAttribute, indexForInsert);
		}



		public string CleanFromComments(string content)
		{
			var s = new StringBuilder();
			var isOpen = false;
			for (var i = 0; i < content.Length; i++)
			{
				if (content[i] == '/' && content[i + 1] == '*')
					isOpen = true;
				if (!isOpen) s.Append(content[i]);
				if (content[i] != '*' || content[i + 1] != '/') continue;
				i++;
				isOpen = false;
			}
			return s.ToString();
		}
	}
}