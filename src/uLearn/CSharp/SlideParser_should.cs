using System;
using System.Linq;
using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class SlideParser_should
	{
		[Test]
		public void make_markdown_from_comments()
		{
			Slide slide = GenerateSlide("SingleComment.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(1));
			Assert.That(slide.Blocks[0].IsCodeSample, Is.False);
			Assert.That(slide.Blocks[0].Text, Is.EqualTo("==Multiline comment\r\nShould become markdown text"));
		}

		[Test]
		public void fail_on_wrong_identation()
		{
			Assert.That(() => GenerateSlide("WrongIdentation.cs"), Throws.Exception);
		}

		[Test]
		public void ignore_wrong_identation_of_empty_lines()
		{
			var slide = GenerateSlide("WrongIdentationOfEmptyLines.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(1));
			Assert.That(slide.Blocks[0].Text.SplitToLines().Length, Is.EqualTo(1));
		}

		[Test]
		public void ignore_comments_inside_methods()
		{
			var slide = GenerateSlide("CommentsInsideSample.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(1));
			Assert.That(slide.Blocks[0].IsCodeSample);
		}

		[Test]
		public void make_separate_blocks_from_separate_comments()
		{
			Slide slide = GenerateSlide("ManyComments.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(3));
			Assert.That(slide.Blocks[1].Text, Is.EqualTo("2nd block"));
		}

		[Test]
		public void make_sample_as_code_block()
		{
			Slide slide = GenerateSlide("Sample.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(1));
			Assert.That(slide.Blocks[0].IsCodeSample);
			Assert.That(slide, Is.TypeOf<Slide>());
		}

		[Test]
		public void make_sample_from_class()
		{
			Slide slide = GenerateSlide("SampleClass.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(1));
			Assert.That(slide.Blocks[0].IsCodeSample);
			Assert.That(slide.Blocks[0].Text, Is.StringContaining("class Point"));
		}

		[Test]
		public void remove_method_header_from_code_sample_block()
		{
			Slide slide = GenerateSlide("Sample.cs");
			Assert.That(slide.Blocks[0].Text, Is.EqualTo("Console.WriteLine(\"Hello Sample!\");"));
		}

		[Test]
		public void preserve_blocks_order_as_in_source_file()
		{
			Slide slide = GenerateSlide("SampleWithComments.cs");
			Assert.That(slide.Blocks.Length, Is.EqualTo(3));
			Assert.That(slide.Blocks[0].Text, Is.StringStarting("Comment"));
			Assert.That(slide.Blocks[1].IsCodeSample);
			Assert.That(slide.Blocks[2].Text, Is.StringStarting("Final"));
		}

		[Test]
		public void make_excercise_slide_from_method_with_exercise_attribute()
		{
			var slide = (ExerciseSlide) GenerateSlide("Exercise.cs");
			Assert.That(slide.Blocks.Single().Text, Is.EqualTo("Add 2 and 3 please!"));
			Assert.That(slide.Exercise.IsCodeSample);
			Assert.That(slide.Exercise.Text, Is.StringStarting("public int Add_2_and_3()"));
			Assert.That(slide.Exercise.Text, Is.Not.StringContaining("NotImplementedException"));
			Assert.That(slide.HintsHtml.Length, Is.EqualTo(0));
		}

		[Test]
		public void not_count_exercise_as_code_sample_block()
		{
			var slide = GenerateSlide("Exercise.cs");
			Assert.That(slide.Blocks.Where(b => b.IsCodeSample), Is.Empty);
		}

		[Test]
		public void uncomment_special_comments_with_starter_code()
		{
			var slide = (ExerciseSlide) GenerateSlide("ExerciseWithStarterCode.cs");
			var exerciseLines = slide.Exercise.Text.SplitToLines();
			Assert.That(exerciseLines.Length, Is.EqualTo(4));
			Assert.That(exerciseLines[2], Is.EqualTo("	return x + y;"));
		}

		[Test]
		public void make_hints_from_hint_attributes()
		{
			var slide = (ExerciseSlide) GenerateSlide("ExerciseWithHints.cs");
			Assert.That(slide.HintsHtml, Is.EqualTo(new[] {"<p>hint1</p>\n", "<p>hint2</p>\n"}).AsCollection);
		}

		[Test]
		public void provide_solution_for_server()
		{
			var slide = (ExerciseSlide) GenerateSlide("ReadNumbersExcercise.cs");
			Console.WriteLine(slide.Head);
			Console.WriteLine("**************");
			Console.WriteLine(slide.WithoutAttribut);
			Assert.That(0, Is.EqualTo(1));
		}

		private static Slide GenerateSlide(string name)
		{
			return SlideParser.ParseSlide(@".\tests\" + name);
		}
	}
}