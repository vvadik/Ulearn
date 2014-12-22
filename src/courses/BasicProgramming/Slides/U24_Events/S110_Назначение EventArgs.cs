using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uLearn; 

namespace U24_Events
{
	[Slide("Назначение EventArgs", "c2170f7b-91f1-4a60-82a1-748918d9d161")]
	class S110_Назначение_EventArgs
	{
		//#video o-353Evd7PY
		/*
		## Заметки по лекции
		*/

		class Revision
		{
			public string Text { get; set; }
			public int CoursorPosition { get; set; }
		}

		class Program
		{
			static TextBox box;
			static List<Revision> revisions = new List<Revision>();

			static void MakeRevision(object sender, EventArgs args)
			{
				revisions.Add(new Revision 
				{ 
					Text = box.Text, 
					CoursorPosition = box.SelectionStart 
				});
			}

			public static void Main()
			{
				box = new TextBox { Multiline = true };
				box.Dock = DockStyle.Fill;

				box.TextChanged += MakeRevision;
				box.MouseDown += MakeRevision; 
					//за счет того, что все классы аргументов событий выведены из EventArgs
					//мы можем подписывать универсальные методы на разные по типу события,
					//и при этом не использовать object

				var form = new Form();
				form.Controls.Add(box);
				Application.Run(form);
			}
		}
    }
}
