using System.Collections.Generic;

namespace RunCsJob.Api
{
    public static class EnumerableExtention
    {
        public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    for (var value = e.Current; e.MoveNext(); value = e.Current)
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}
