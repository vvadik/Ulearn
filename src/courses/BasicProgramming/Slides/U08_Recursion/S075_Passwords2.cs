using System;
using System.Collections.Generic;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Перебор паролей 2", "69F84DFBEF364D738C9C0F187D74337D")]
	public class S075_Passwords2 : SlideTestBase
	{
		/*
		Подобрав забытый пароль от своей почты, Вася сменил его на новый и... опять забыл!

		На этот раз он точно помнит, что он сконструировал пароль из обычного слова, поменяв регистр нескольких букв.
		Он, конечно, не хочет вам говорить ни слово, ни количество измененных им букв.
		Поэтому просит написать программу, которая по заданному слову перебирает все возможные пароли,
		полученные из этого слова заменой регистра у не более чем указанного количества букв.

		*/

		[ExpectedOutput(@"cat
caT
cAt
cAT
Cat
CaT
CAt
king


a
A
1
123
abc
abC
aBc
aBC
Abc
AbC
ABc
ABC
def
deF
dEf
dEF
Def
DeF
DEf
DEF
ghi
ghI
gHi
gHI
Ghi
GhI
GHi
GHI
k_l_m
k_L_m
k_L_M
K_l_m
K_l_M
K_L_m
K_L_M")]
		public static void Main()
		{
			MakePasswords("cat", 2);
			MakePasswords("king", 0);
			MakePasswords("", 0);
			MakePasswords("", 1000);
			MakePasswords("a", 1);
			MakePasswords("1", 1);
			MakePasswords("123", 2);
			MakePasswords("abc", 1000);
			MakePasswords("DEF", 1000);
			MakePasswords("gHi", 1000);
			MakePasswords("K_L_m", 2);
		}
		
		static void MakePasswords(string initialWord, int changes)
		{
			var passwords = new List<string>();
			MakePasswords(initialWord, new char[initialWord.Length], changes, 0, passwords);
			passwords.Sort();
			foreach (var password in passwords)
				Console.WriteLine(password);
		}

		[Exercise]
		static void MakePasswords(string initialWord, char[] password, 
			int caseChangesLeft, int position, List<string> passwords)
		{
			if (position == password.Length)
				passwords.Add(new string(password));
			else
			{
				if (caseChangesLeft > 0)
				{
					if (char.IsUpper(initialWord[position]))
					{
						password[position] = char.ToLower(initialWord[position]);
						MakePasswords(initialWord, password, caseChangesLeft - 1, position + 1, passwords);
					}
					else if (char.IsLower(initialWord[position]))
					{
						password[position] = char.ToUpper(initialWord[position]);
						MakePasswords(initialWord, password, caseChangesLeft - 1, position + 1, passwords);
					}
				}
				password[position] = initialWord[position];
				MakePasswords(initialWord, password, caseChangesLeft, position + 1, passwords);
			}
			/*uncomment
			if (position == password.Length)
				passwords.Add(new string(password));
			else
			{
				// ...
			}
			*/
		}
	}
}