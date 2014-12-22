using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uLearn; 

namespace U24_Events
{
	[Slide("Событийная модель", "d7719339-6f14-43a3-a861-853be7018550")]
	class S020_Событийная_модель
	{
		//#video tF0eeUjWOvw
		/*
		## Заметки по лекции
		*/

		static void Main()
		{
			var form = new Form();
			form.FormClosing += (sender, args) =>
			{
				var result = MessageBox.Show("Действительно закрыть?", "", MessageBoxButtons.YesNo);
				if (result != DialogResult.Yes) args.Cancel = true;
			};

		}
    }
}
