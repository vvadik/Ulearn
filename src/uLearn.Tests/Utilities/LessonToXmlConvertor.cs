using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using uLearn.Model;
using uLearn.Model.Blocks;
using uLearn.Quizes;

namespace uLearn.Utilities
{
	public class LessonToXmlConvertor
	{
		private readonly XmlSerializer lessonSerializer = GetLessonSerializer();

		private static XmlSerializer GetLessonSerializer()
		{
			var attrs = new XmlAttributes { XmlIgnore = true };

			var attrOverrides = new XmlAttributeOverrides();
			attrOverrides.Add(typeof(ExerciseBlock), "LegacyExerciseInitialCode", attrs);
			
			return new XmlSerializer(typeof(Lesson), attrOverrides);
		}

		[Test]
		[Explicit]
		public void ConvertLessonSlidesToXml()
		{
			var coursesDirectory = new DirectoryInfo(@"C:\tmp\ulearn\10 course conversion");
			var courseDirectories = coursesDirectory.GetDirectories("Slides", SearchOption.AllDirectories);
			foreach (var courseDirectory in courseDirectories)
			{
				var course = new CourseLoader().LoadCourse(courseDirectory);
				Console.WriteLine($"course {course.Id}");
				foreach (var slide in course.Slides)
				{
					ConvertSlide(slide);
				}
			}
		}

		private void ConvertSlide(Slide slide)
		{
			if (slide is QuizSlide)
				return;

			if (!string.Equals(slide.Info.SlideFile.Extension, ".cs", StringComparison.InvariantCultureIgnoreCase))
				return;
			
			Console.WriteLine("Converting " + slide.Info.SlideFile.FullName);			
			
			if (slide is ExerciseSlide exerciseSlide)
			{
				var copiedFileName = RemoveFirstPathFromSlideName(exerciseSlide.Info.SlideFile.Name);
				var copiedFilePath = Path.Combine(exerciseSlide.Info.SlideFile.Directory.FullName, copiedFileName);
				File.Copy(exerciseSlide.Info.SlideFile.FullName, copiedFilePath);
				exerciseSlide.Exercise.CodeFile = copiedFileName;
				
				/* Set correct SolutionLable */
				if (exerciseSlide.Exercise is SingleFileExerciseBlock block)
				{
					if (block.ExcludedFromSolution.Count > 1)
						Console.WriteLine($"WARNING: {block.ExcludedFromSolution.Count} blocks have been excluded from solution. Can't decide what is correct solution label");
					Assert.IsTrue(block.ExcludedFromSolution.Count >= 1);
					block.SolutionLabel = new Label { Name = block.ExcludedFromSolution[0] };
				}

				/* Remove ulearn-specific attribute from code */
				var ulearnAttributes = new List<string>
				{
					"CommentAfterExerciseIsSolved", "ExcludeFromSolution", "Exercise", "ExpectedOutput", 
					"HideExpectedOutputOnError", "HideOnSlide", "Hint", "IsStaticMethod", "RecursionStyleValidator", 
					"ShowBodyOnSlide", "SingleStatementMethod", "Slide",
				};
				var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(copiedFilePath));
				var afterUlearnAttributeRemoval = new RemoveUlearnAttributesRewriter(ulearnAttributes).Visit(tree.GetRoot());
				File.WriteAllText(copiedFilePath, afterUlearnAttributeRemoval.ToFullString());
			}
			
			var lesson = new Lesson(slide.Title, slide.Id, slide.Blocks);
			var path = Path.ChangeExtension(slide.Info.SlideFile.FullName, "lesson.xml");
			using (var writer = new StreamWriter(path, false, Encoding.UTF8))
				lessonSerializer.Serialize(writer, lesson);
			slide.Info.SlideFile.Delete();
		}

		private string RemoveFirstPathFromSlideName(string slideFileName)
		{
			var parts = slideFileName.Split(new [] {'_', '-'}, 2, StringSplitOptions.RemoveEmptyEntries);
			return "_" + parts[1].Trim();
		}
	}

	public class RemoveUlearnAttributesRewriter: CSharpSyntaxRewriter
	{
		private readonly List<string> attributeNames;

		public RemoveUlearnAttributesRewriter(List<string> attributeNames)
		{
			this.attributeNames = attributeNames;
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			return base.VisitMethodDeclaration(VisitAndRemoveAttributes(node) as MethodDeclarationSyntax);
		}
		
		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			return base.VisitClassDeclaration(VisitAndRemoveAttributes(node) as ClassDeclarationSyntax);
		}

		public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			return base.VisitFieldDeclaration(VisitAndRemoveAttributes(node) as FieldDeclarationSyntax);
		}

		private SyntaxNode VisitAndRemoveAttributes(SyntaxNode node)
		{
			var newAttributes = new SyntaxList<AttributeListSyntax>();

			foreach (AttributeListSyntax attributeList in ((dynamic) node).AttributeLists)
			{
				var nodesToRemove = attributeList.Attributes.Where(AttributeNameMatches).ToArray();
	
				if (nodesToRemove.Length != attributeList.Attributes.Count)
				{
					//We want to remove only some of the attributes
					var newAttribute = (AttributeListSyntax)VisitAttributeList(attributeList.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia));
						
					newAttributes = newAttributes.Add(newAttribute);
				}			
			}

			//Get the leading trivia (the newlines and comments)
			var leadTriv = node.GetLeadingTrivia();
			node = ((dynamic) node).WithAttributeLists(newAttributes);

			//Append the leading trivia to the method
			node = node.WithLeadingTrivia(leadTriv);
			return node;
		}

		private static SimpleNameSyntax GetSimpleNameFromNode(AttributeSyntax node)
		{
			var identifierNameSyntax = node.Name as IdentifierNameSyntax;
			var qualifiedNameSyntax = node.Name as QualifiedNameSyntax;
		
			return identifierNameSyntax ?? qualifiedNameSyntax?.Right ?? (node.Name as AliasQualifiedNameSyntax).Name;
		}

		private bool AttributeNameMatches(AttributeSyntax attribute)
		{
			foreach (var attributeName in attributeNames)
				if (GetSimpleNameFromNode(attribute).Identifier.Text.StartsWith(attributeName))
					return true;
			return false;
		}
	}
}