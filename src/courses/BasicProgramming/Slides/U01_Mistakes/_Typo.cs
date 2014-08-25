using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slide03
{
	class Program
	{

		static void Main() //закомментируйте эту строку и раскомментируйте следующую, чтобы получить ошибку
		//static void Main[]
		{
			int a = 0;
			int b = a + 5;
			Console.WriteLine(b);
		}


	}
}

/*
 * Список ошибок:
 * Type or namespace definition, or end-of-file expected	C
 * Invalid token '{' in class, struct, or interface member declaration	
 * Invalid token ')' in class, struct, or interface member declaration	
 * Invalid token '(' in class, struct, or interface member declaration
 * Bad array declarator: To declare a managed array the rank specifier precedes the variable's identifier. To declare a fixed size buffer field, use the fixed keyword before the field type.
 * A field initializer cannot reference the non-static field, method, or property 'Slide03.Program.a'	
 * ; expected	
 * 'System.Console.WriteLine(string, params object[])' is a 'method' but is used like a 'type'	
 * 'Slide03.Program.b' is a 'field' but is used like a 'type'	
 * 
 * Если при небольшой правке вываливается много ошибок сразу, возможно, речь идет об опечатке.
 * В этом случае надо начать с той ошибке, которая находится в файле раньше всего.
 */