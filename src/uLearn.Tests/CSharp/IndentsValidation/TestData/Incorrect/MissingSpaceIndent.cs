    using System;

namespace uLearn.CSharp.IndentsValidation.TestData.Incorrect
{
    public class MissingSpaceIndent
    {
        public static class Hello
        {
            public static void Main()
            {
                Console.WriteLine();
            }
        }
    }
}