namespace Ulearn.Core.RunCheckerJobApi
{
	public enum Verdict
	{
		NA = 0,
		Ok = 1, // Означает, что всё штатно протестировалось. Возвращается в том числе если тесты не прошли
		CompilationError = 2,
		RuntimeError = 3,
		SecurityException = 4,
		SandboxError = 5,
		OutputLimit = 6,
		TimeLimit = 7,
		MemoryLimit = 8
	}
}