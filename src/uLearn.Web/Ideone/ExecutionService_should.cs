using System;
using NUnit.Framework;

namespace uLearn.Web.Ideone
{
	[TestFixture]
	public class ExecutionService_should
	{
		private readonly ExecutionService service = new ExecutionService();

		[Test]
		[Explicit]
		public void make_submition()
		{
			GetSubmitionDetailsResult res =
				service.Submit(@"using System; public class M{static void Main(){System.Console.WriteLine(42);}}",
					"")
					.Result;
			Console.WriteLine(res.Error);
			Console.WriteLine(res.Status);
			Console.WriteLine(res.Result);
			Console.WriteLine(res.LangId);
			Console.WriteLine(res.LangName);
			Console.WriteLine(res.LangVersion);
			Console.WriteLine(res.IsPublic);
			Console.WriteLine(res.Date);
			Console.WriteLine(res.Source);
			Console.WriteLine(res.CompilationError);
			Console.WriteLine(res.Input);
			Console.WriteLine(res.Output);
			Console.WriteLine(res.StdErr);
			Console.WriteLine(res.Signal);
			Console.WriteLine(res.Time);
			Console.WriteLine(res.Memory);
		}
		
		[Test]
		[Explicit]
		public void compile_tuple()
		{
			GetSubmitionDetailsResult res =
				service.Submit(@"using System; public class M{static void Main(){System.Console.WriteLine(Tuple.Create(1, 2));}}",
					"")
					.Result;
			Console.WriteLine(res.Error);
			Console.WriteLine(res.Status);
			Console.WriteLine(res.Result);
			Console.WriteLine(res.LangId);
			Console.WriteLine(res.LangName);
			Console.WriteLine(res.LangVersion);
			Console.WriteLine(res.IsPublic);
			Console.WriteLine(res.Date);
			Console.WriteLine(res.Source);
			Console.WriteLine(res.CompilationError);
			Console.WriteLine(res.Input);
			Console.WriteLine(res.Output);
			Console.WriteLine(res.StdErr);
			Console.WriteLine(res.Signal);
			Console.WriteLine(res.Time);
			Console.WriteLine(res.Memory);
			Assert.AreEqual(SubmitionResult.Success, res.Result);
		}
		[Test]
		public void get_supported_languages()
		{
			foreach (var lang in service.GetSupportedLanguages())
			{
				Console.WriteLine(lang);
			}
		}

	}

}
	/*
<item><key xsi:type="xsd:int">7</key><value xsi:type="xsd:string">Ada (gnat-4.6)</value></item>
<item><key xsi:type="xsd:int">13</key><value xsi:type="xsd:string">Assembler (nasm-2.10.01)</value></item>
<item><key xsi:type="xsd:int">45</key><value xsi:type="xsd:string">Assembler (gcc-4.8.1)</value></item>
<item><key xsi:type="xsd:int">104</key><value xsi:type="xsd:string">AWK (gawk) (gawk-3.1.6)</value></item>
<item><key xsi:type="xsd:int">105</key><value xsi:type="xsd:string">AWK (mawk) (mawk-1.3.3)</value></item>
<item><key xsi:type="xsd:int">28</key><value xsi:type="xsd:string">Bash (bash 4.0.35)</value></item>
<item><key xsi:type="xsd:int">110</key><value xsi:type="xsd:string">bc (bc-1.06.95)</value></item>
<item><key xsi:type="xsd:int">12</key><value xsi:type="xsd:string">Brainf**k (bff-1.0.3.1)</value></item>
<item><key xsi:type="xsd:int">11</key><value xsi:type="xsd:string">C (gcc-4.8.1)</value></item>
<item><key xsi:type="xsd:int">27</key><value xsi:type="xsd:string">C# (mono-2.8)</value></item>
<item><key xsi:type="xsd:int">41</key><value xsi:type="xsd:string">C++ 4.3.2 (gcc-4.3.2)</value></item>
<item><key xsi:type="xsd:int">1</key><value xsi:type="xsd:string">C++ 4.8.1 (gcc-4.8.1)</value></item>
<item><key xsi:type="xsd:int">44</key><value xsi:type="xsd:string">C++11 (gcc-4.8.1)</value></item>
<item><key xsi:type="xsd:int">34</key><value xsi:type="xsd:string">C99 strict (gcc-4.8.1)</value></item>
<item><key xsi:type="xsd:int">14</key><value xsi:type="xsd:string">CLIPS (clips 6.24)</value></item>
<item><key xsi:type="xsd:int">111</key><value xsi:type="xsd:string">Clojure (clojure 1.5.0-RC2)</value></item>
<item><key xsi:type="xsd:int">118</key><value xsi:type="xsd:string">COBOL (open-cobol-1.0)</value></item>
<item><key xsi:type="xsd:int">106</key><value xsi:type="xsd:string">COBOL 85 (tinycobol-0.65.9)</value></item>
<item><key xsi:type="xsd:int">32</key><value xsi:type="xsd:string">Common Lisp (clisp) (clisp 2.47)</value></item>
<item><key xsi:type="xsd:int">102</key><value xsi:type="xsd:string">D (dmd) (dmd-2.042)</value></item>
<item><key xsi:type="xsd:int">36</key><value xsi:type="xsd:string">Erlang (erl-5.7.3)</value></item>
<item><key xsi:type="xsd:int">124</key><value xsi:type="xsd:string">F# (fsharp-2.0.0)</value></item>
<item><key xsi:type="xsd:int">123</key><value xsi:type="xsd:string">Factor (factor-0.93)</value></item>
<item><key xsi:type="xsd:int">125</key><value xsi:type="xsd:string">Falcon (falcon-0.9.6.6)</value></item>
<item><key xsi:type="xsd:int">107</key><value xsi:type="xsd:string">Forth (gforth-0.7.0)</value></item>
<item><key xsi:type="xsd:int">5</key><value xsi:type="xsd:string">Fortran (gfortran-4.8)</value></item>
<item><key xsi:type="xsd:int">114</key><value xsi:type="xsd:string">Go (1.0.3)</value></item>
<item><key xsi:type="xsd:int">121</key><value xsi:type="xsd:string">Groovy (groovy-2.1.6)</value></item>
<item><key xsi:type="xsd:int">21</key><value xsi:type="xsd:string">Haskell (ghc-7.6.3)</value></item>
<item><key xsi:type="xsd:int">16</key><value xsi:type="xsd:string">Icon (iconc 9.4.3)</value></item>
<item><key xsi:type="xsd:int">9</key><value xsi:type="xsd:string">Intercal (c-intercal 28.0-r1)</value></item>
<item><key xsi:type="xsd:int">10</key><value xsi:type="xsd:string">Java (sun-jdk-1.7.0_25)</value></item>
<item><key xsi:type="xsd:int">55</key><value xsi:type="xsd:string">Java7 (sun-jdk-1.7.0_10)</value></item>
<item><key xsi:type="xsd:int">35</key><value xsi:type="xsd:string">JavaScript (rhino) (rhino-1.7R4)</value></item>
<item><key xsi:type="xsd:int">112</key><value xsi:type="xsd:string">JavaScript (spidermonkey) (spidermonkey-1.7)</value></item>
<item><key xsi:type="xsd:int">26</key><value xsi:type="xsd:string">Lua (luac 5.1.4)</value></item>
<item><key xsi:type="xsd:int">30</key><value xsi:type="xsd:string">Nemerle (ncc 0.9.3)</value></item>
<item><key xsi:type="xsd:int">25</key><value xsi:type="xsd:string">Nice (nicec 0.9.6)</value></item>
<item><key xsi:type="xsd:int">122</key><value xsi:type="xsd:string">Nimrod (nimrod-0.8.8)</value></item>
<item><key xsi:type="xsd:int">56</key><value xsi:type="xsd:string">Node.js (0.8.11)</value></item>
<item><key xsi:type="xsd:int">43</key><value xsi:type="xsd:string">Objective-C (gcc-4.5.1)</value></item>
<item><key xsi:type="xsd:int">8</key><value xsi:type="xsd:string">Ocaml (ocamlopt 3.10.2)</value></item>
<item><key xsi:type="xsd:int">127</key><value xsi:type="xsd:string">Octave (3.6.2)</value></item>
<item><key xsi:type="xsd:int">119</key><value xsi:type="xsd:string">Oz (mozart-1.4.0)</value></item>
<item><key xsi:type="xsd:int">57</key><value xsi:type="xsd:string">PARI/GP (2.5.1)</value></item>
<item><key xsi:type="xsd:int">22</key><value xsi:type="xsd:string">Pascal (fpc) (fpc 2.6.2)</value></item>
<item><key xsi:type="xsd:int">2</key><value xsi:type="xsd:string">Pascal (gpc) (gpc 20070904)</value></item>
<item><key xsi:type="xsd:int">3</key><value xsi:type="xsd:string">Perl (perl 5.16.2)</value></item>
<item><key xsi:type="xsd:int">54</key><value xsi:type="xsd:string">Perl 6 (rakudo-2010.08)</value></item>
<item><key xsi:type="xsd:int">29</key><value xsi:type="xsd:string">PHP (php 5.4.4)</value></item>
<item><key xsi:type="xsd:int">19</key><value xsi:type="xsd:string">Pike (pike 7.6.86)</value></item>
<item><key xsi:type="xsd:int">108</key><value xsi:type="xsd:string">Prolog (gnu) (gprolog-1.3.1)</value></item>
<item><key xsi:type="xsd:int">15</key><value xsi:type="xsd:string">Prolog (swi) (swipl 5.6.64)</value></item>
<item><key xsi:type="xsd:int">4</key><value xsi:type="xsd:string">Python (python 2.7.3)</value></item>
<item><key xsi:type="xsd:int">116</key><value xsi:type="xsd:string">Python 3 (python-3.2.3)</value></item>
<item><key xsi:type="xsd:int">117</key><value xsi:type="xsd:string">R (R-2.11.1)</value></item>
<item><key xsi:type="xsd:int">17</key><value xsi:type="xsd:string">Ruby (ruby-1.9.3)</value></item>
<item><key xsi:type="xsd:int">39</key><value xsi:type="xsd:string">Scala (scala-2.10.2)</value></item>
<item><key xsi:type="xsd:int">33</key><value xsi:type="xsd:string">Scheme (guile) (guile 1.8.5)</value></item>
<item><key xsi:type="xsd:int">23</key><value xsi:type="xsd:string">Smalltalk (gst 3.1)</value></item>
<item><key xsi:type="xsd:int">40</key><value xsi:type="xsd:string">SQL (sqlite3-3.7.3)</value></item>
<item><key xsi:type="xsd:int">38</key><value xsi:type="xsd:string">Tcl (tclsh 8.5.7)</value></item>
<item><key xsi:type="xsd:int">62</key><value xsi:type="xsd:string">Text (text 6.10)</value></item>
<item><key xsi:type="xsd:int">115</key><value xsi:type="xsd:string">Unlambda (unlambda-2.0.0)</value></item>
<item><key xsi:type="xsd:int">101</key><value xsi:type="xsd:string">VB.NET (mono-2.4.2.3)</value></item>
<item><key xsi:type="xsd:int">6</key><value xsi:type="xsd:string">Whitespace (wspace 0.3)</value></item></value>ns2:Map
*/