using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Build.Evaluation;

namespace uLearn
{
    public class ProjModifier
    {
        public static void RemoveCheckingFromCsproj(Project proj)
        {
            var toRemove = proj.Items.Where(pItem => pItem.EvaluatedInclude.StartsWith("checking" + Path.DirectorySeparatorChar)).ToList();
            proj.RemoveItems(toRemove);
        }

        public static void ChangeEntryPointToCheckingCheckerMain(Project proj)
        {
            proj.SetProperty("StartupObject", "checking.CheckerRunner");
        }

        public static byte[] ModifyCsproj(byte[] content, Action<Project> changingAction)
        {
            using (var inputMs = new MemoryStream(content))
            {
                var reader = XmlReader.Create(inputMs);
                var proj = new Project(reader);
	            changingAction(proj);
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    proj.Save(streamWriter);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}