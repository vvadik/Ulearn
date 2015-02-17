using System;
using System.Drawing;
using uLearn;

namespace OOP.Slides.U10_NoIfs
{
	[Slide("Ветвления и полиморфизм", "28EA39D097A44D93BB1F506B96C65FF5")]
	public class S060_ChartTask
	{

		/*
		Часто конструкции if и switch применяются, чтобы определить различное поведение метода для объектов различных типов.

		В таких случаях стоит рассмотреть алтерантивный подход, в котором switch и if заменяются подклассами и полиморфизмом.

		Для каждой ветви (то есть для каждого типа) нужно создать отдельный класс-наследник, а исходный тип сделать абстрактным.

		Действуя таким алгоритмом из этого:
		*/

		public class Shape
		{
			public Shape(char type, Rectangle boundingRect)
			{
				this.type = type;
				this.boundingRect = boundingRect;
			}

			private readonly char type;
			private readonly Rectangle boundingRect;

			public string GetShapeName()
			{
				switch (type)
				{
					case 'P':
						return "Point";
					case 'C':
						return "Circle";
					case 'R':
						return boundingRect.Width == boundingRect.Height ? "Square" : "Rectangle";
				}
				throw new Exception("unknown type " + type);
			}
		}

		/*
		Можно получить вот такой код:
		*/
		public abstract class AbstractShape
		{
			protected AbstractShape(Rectangle boundingRect)
			{
				this.boundingRect = boundingRect;
			}

			protected readonly Rectangle boundingRect;
			public abstract string GetShapeName();
		}

		public class Point : AbstractShape
		{
			public Point(Rectangle boundingRect) : base(boundingRect) { }

			public override string GetShapeName()
			{
				return "Point";
			}
		}

		public class Circle : AbstractShape
		{
			public Circle(Rectangle boundingRect) : base(boundingRect) { }

			public override string GetShapeName()
			{
				return "Circle";
			}
		}

		public class Rect : AbstractShape
		{
			public Rect(Rectangle boundingRect) : base(boundingRect) { }

			public override string GetShapeName()
			{
				return boundingRect.Width == boundingRect.Height ? "Square" : "Rectangle"; ;
			}
		}

		/*
		Какие приемущества у этого кода?

		1. Каждый класс получился более простым.
		2. Появилась возможность расширять множество фигур без модификации уже существующего кода.
		3. При появлении новой фигуры невозможно забыть написать очередную ветку оператора switch — компилятор заставит реализовать абстрактный метод.

		*/


	}
}