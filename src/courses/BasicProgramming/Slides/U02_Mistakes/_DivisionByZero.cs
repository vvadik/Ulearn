using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S04
{
	class Program
	{
		static int Divide(int a, int b)
		{
			return a / b;
		}

		static void MainX()
		{
			Console.WriteLine(Divide(1, 0));
		}

	}
}

/*
  Важная информация, которую можно извлечь из информации об исключении: название, стэк вызовов

  System.DivideByZeroException was unhandled  
  StackTrace:
       at S04.Program.Divide(Int32 a, Int32 b) in c:\Users\user\Desktop 3\BasicProgramming\CS\Часть 1 - Ошибки компиляции и выполнения\S04 - Деление на 0.cs:line 13
       at S04.Program.Main() in c:\Users\user\Desktop 3\BasicProgramming\CS\Часть 1 - Ошибки компиляции и выполнения\S04 - Деление на 0.cs:line 18
       at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)
       at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
       at Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
       at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
       at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
       at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
       at System.Threading.ThreadHelper.ThreadStart()
  InnerException: 
*/