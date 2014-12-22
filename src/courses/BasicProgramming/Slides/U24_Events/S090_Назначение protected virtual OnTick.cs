using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uLearn; 

namespace U24_Events
{
	[Slide("Назначение protected virtual OnTick", "9a0013bc-e475-484a-8efb-0c5097394b19")]
	class S090_Назначение_protected_virtual_OnTick
	{
		//#video rQJ1herEkYg
		/*
		## Заметки по лекции
		*/

		class MyWindow : Form
		{
			protected override void OnFormClosing(FormClosingEventArgs e)
			{
				var result = MessageBox.Show("Действительно закрыть?", "", MessageBoxButtons.YesNo);
				if (result != DialogResult.Yes) e.Cancel = true;
			}
		}

		static void Main()
		{
			//C Winforms можно работать так:
			var form = new Form();
			form.FormClosing += (sender, args) =>
			{
				var result = MessageBox.Show("Действительно закрыть?", "", MessageBoxButtons.YesNo);
				if (result != DialogResult.Yes) args.Cancel = true;
			};

			//Или же убрав всю функциональность по обработке событий
			//в соответствующие виртуальные методы
			var form1 = new MyWindow();
		}
    }
}
