namespace uLearn.Courses.BasicProgramming.Slides.U07_Complexity
{
	[Slide("О-символика", "{0A165BD3-2893-447A-B873-75C0BCCAC0C3}")]
	class S050_OSymbol
	{
		//#video HOoYSOdTroU
		
		/*
		##Заметки по лекции
		*/
		/*
		###o-малое
		*/
		/*tex
		f(n)=o(g(n)) \Leftrightarrow 
		\forall k>0\ \exists n_0\ \forall n>n_0:\ f(n)<k\cdot g(n) \Leftrightarrow 
		\lim_{n\rightarrow \infty} \frac{f(n)}{g(n)}=0
		f(n)\prec g(n)
		*/
		/*
		###O-большое
		*/
		/*tex
		f(n)=O(g(n)) \Leftrightarrow 
		\exists k>0\ \exists n_0\ \forall n>n_0:\ f(n)<k\cdot g(n) \Leftrightarrow 
		\lim_{n\rightarrow \infty} \frac{f(n)}{g(n)}<\infty
		f(n)\preceq g(n)
		*/
		/*
		###Θ-большое
		*/
		/*tex
		f(n)=\Theta(g(n)) \Leftrightarrow 
		\exists k_1,k_2>0\ \exists n_{0}\ \forall n>n_{0}:\ k_1\cdot g(n)<f(n)<k_2 \cdot g(n) \Leftarrow
		\Leftarrow \lim_{n\rightarrow \infty} \frac{f(n)}{g(n)}=c>0
		f(n) \approx g(n)
		*/
	}
}
