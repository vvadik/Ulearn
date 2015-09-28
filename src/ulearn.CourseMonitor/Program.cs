using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CourseTool;

namespace ulearn.CourseMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length!=2)
            {
                Console.WriteLine("Failed to start monitor. 2 parameters required. Do not start monitor directly, use `course` tool");
            }
            Monitor.Start(args[0], args[1]);
        }
    }
}
