using System;

namespace DivisionByZeroSample
{
	class Program
	{
		static int Divide(int a, int b)
		{
			return a / b;
		}

		static void Main()
		{
			Console.WriteLine(Divide(1, 0));
		}

	}
}

/*
  Важная информация, которую можно извлечь из информации об исключении: 
    название, стэк вызовов

  System.DivideByZeroException was unhandled  
  StackTrace:
       at S04.Program.Divide(Int32 a, Int32 b) in Program.cs:line 13
       at S04.Program.Main() in Program.cs:line 18
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