using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uLearn; 

namespace U24_Events
{
	[Slide("Назначение sender", "7ab3bc19-5b3f-4f36-9125-75decc5416b1")]
	class S100_Назначение_sender
	{
		//#video cCmBRwPG2lo
		/*
		## Заметки по лекции
		*/

		static void CreateReport(string monthName)
		{
			MessageBox.Show("Создаю отчет за " + monthName + "...");
		}

		static void MenuItemSelected(object sender, EventArgs e)
		{
			var menuItem = sender as MenuItem;
			CreateReport(menuItem.Text);
		}

		public static void Main()
		{
			var monthNames = new[] { "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", " декабрь" };
			var menuItems = monthNames
				.Select(name =>
				{
					var menuItem = new MenuItem(name);
					menuItem.Click += (sender, args) => CreateReport(name);
					return menuItem;
				})
				.ToArray();

			//Но раньше, когда не было лямбда-выражений, приходилось писать так:
			menuItems = new MenuItem[monthNames.Length];
			for (int i = 0; i < menuItems.Length; i++)
			{
				var item = new MenuItem(monthNames[i]);
				item.Click += MenuItemSelected; //внутри метода, мы узнаем, кто его вызвал, с помощью sender
				menuItems[i] = item;
			}

			var mainMenu = new MainMenu(
				new[]
				{
					new MenuItem("Создать отчет", menuItems)
				});
			var form = new Form();
			form.Menu = mainMenu;
			Application.Run(form);
		}
    }
}
