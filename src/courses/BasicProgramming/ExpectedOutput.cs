using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
    public class ExpectedOutputAttribute : Attribute
    {
        public ExpectedOutputAttribute(string s)
        {
            var newOut = new StringWriter();
            //null or "" ?!
            Console.SetOut(newOut);
            Assert.AreEqual(s, newOut.ToString().Trim());
        }
    }
}
