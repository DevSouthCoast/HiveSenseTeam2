using System.Collections;

namespace HiveSense.Extensions
{
    public static class ArrayListExtensions
    {
        /// <summary>
        /// Adds list to src
        /// </summary>
        public static ArrayList AddRange(this ArrayList src, ArrayList list)
        {
            foreach(var item in list)
            {
                src.Add(item);
            }
            return src;
        }
    }
}
