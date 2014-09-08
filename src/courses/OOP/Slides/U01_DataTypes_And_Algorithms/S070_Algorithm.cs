using System;
using uLearn;

namespace OOP.Slides.U01_DataTypes_And_Algorithms
{
	[Slide("Класс-алгоритм", "{F46FAE78-4F89-47CF-AF87-027386D58104}")]
	public class S070_Algorithm
	{
		/*
		Настало время заняться интеллектом для нашего робота.

		Впрочем не пугайтесь, задача робота предельно проста — как можно быстрее добраться до заданной точки!

		Фактически, вам нужно реализовать функцию, котрая по заданным исходным данным — состоянию робота и целевой точке —  
		вычислит очередную команду для робота.

		Поговорим о двух корректных способах оформить алгоритм в коде и одном некорректном.

		## Процедурный подход

		Статический метод, получающий на вход всю, необходимую для принятия решения информацию.
		*/
		
		public static class RobotAi
		{
			public static RobotCommand MoveToDestination(Robot robot, Vector destination)
			{
				throw new NotImplementedException("TODO");
			}
		}

		/*
		## Объектно-ориентированный подход

		Под конкретную задачу (в данном случае — довести робота до точки) создается нестатический класс, экземпляр которого выполняет эту задачу.
		*/
		
		public class RobotNavigator
		{
			private readonly Vector destination;

			public RobotNavigator(Vector destination)
			{
				this.destination = destination;
			}

			public RobotCommand GetNextCommand(Robot robot)
			{
				throw new NotImplementedException("TODO");
			}
		}
		/*
		Заметьте, что целевую точку теперь можно передать в конструктор, и сохранить в поле класса.
		Это даст к ней доступ всем остальным методам класса и избавит от необходимости каждый раз передавать destination аргументом.

		Пока что разница с процедурным подходом минимальна, однако в следующем блоке задач мы увидим, 
		что ОО-подход обладает и некоторыми дополнительными преимуществами.
		*/
	
		[HideOnSlide]
		public class RobotCommand { }

		[HideOnSlide]
		public class Robot { }
	}
}