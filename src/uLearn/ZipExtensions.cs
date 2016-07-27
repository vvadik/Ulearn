using Ionic.Zip;

namespace uLearn
{
    public class ZipUpdateData
    {
        public string Path { get; set; }
        public byte[] Data { get; set; }
    }

    public static class ZipExtensions
    {
        public static void RemoveDirectory(this ZipFile zip, string directoryName)
        {
            var tr = zip.SelectEntries(string.Format("name = {0}*", directoryName));
            foreach (var zipEntry in tr)
                zip.RemoveEntry(zipEntry);
        }
    }
}