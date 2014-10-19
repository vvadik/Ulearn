using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Перебор паролей", "{A859B4B5-BA6F-48C6-A745-19EA5F4A307C}")]
	public class S055_FixSubsets : SlideTestBase
	{
		/*
		Вася забыл пароль от своего почтового ящика, но он отчетливо помнит, что его пароль содержал только буквы a, b и с.
		Посмотрев предыдущее видео Вася легко написал программу перебора всех паролей из двух букв.
		Но ведь его пароль состоит из трех!

		Исправьте программу так, чтобы она перебирала все слова из букв a, b, c в лексикографическом порядке.
		*/

		[ExpectedOutput(@"a
b
c
aa
ab
ac
ba
bb
bc
ca
cb
cc

aaaa
aaab
aaac
aaba
aabb
aabc
aaca
aacb
aacc
abaa
abab
abac
abba
abbb
abbc
abca
abcb
abcc
acaa
acab
acac
acba
acbb
acbc
acca
accb
accc
baaa
baab
baac
baba
babb
babc
baca
bacb
bacc
bbaa
bbab
bbac
bbba
bbbb
bbbc
bbca
bbcb
bbcc
bcaa
bcab
bcac
bcba
bcbb
bcbc
bcca
bccb
bccc
caaa
caab
caac
caba
cabb
cabc
caca
cacb
cacc
cbaa
cbab
cbac
cbba
cbbb
cbbc
cbca
cbcb
cbcc
ccaa
ccab
ccac
ccba
ccbb
ccbc
ccca
cccb
cccc")]
		public static void Main()
		{
			WriteAllWordsOfSize(1);
			WriteAllWordsOfSize(2);
			WriteAllWordsOfSize(0);
			WriteAllWordsOfSize(4);
		}

		static void WriteAllWordsOfSize(int size)
		{
			MakeSubsets(new char[size]);
		}

		[Exercise]
		static void MakeSubsets(char[] subset, int position = 0)
		{
			if (position == subset.Length)
			{
				Console.WriteLine(new string(subset));
				return;
			}
			subset[position] = 'a';
			MakeSubsets(subset, position + 1);
			subset[position] = 'b';
			MakeSubsets(subset, position + 1);
			subset[position] = 'c';
			MakeSubsets(subset, position + 1);
			/*uncomment
			if (position == subset.Length)
			{
				Console.WriteLine(new string(subset));
				return;
			}
			subset[position] = 'a';
			MakeSubsets(subset, position + 1);
			subset[position] = 'b';
			MakeSubsets(subset, position + 1);
			*/
		}
	}
}