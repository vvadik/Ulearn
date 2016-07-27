using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Build.Evaluation;

namespace uLearn
{
    public class ProjModifier
    {
        public static byte[] ModifyCsProj(byte[] content)
        {
            using (var inputMs = new MemoryStream(content))
            {
                var reader = XmlReader.Create(inputMs);
                var proj = new Project(reader);
                var toRemove = proj.Items.Where(pItem => pItem.EvaluatedInclude.StartsWith("checker" + Path.DirectorySeparatorChar)).ToList(); //TODO
                foreach (var pItem in toRemove)
                    proj.RemoveItem(pItem);
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