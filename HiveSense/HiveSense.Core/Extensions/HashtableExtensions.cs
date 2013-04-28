using System.Collections;
/*
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly |
        AttributeTargets.Class |
        AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    { }
}*/

namespace HiveSense.Extensions
{
    public static class HashtableExtensions
    {
        public static bool ContainsKey(this Hashtable hashTable, object key)
        {
            if (hashTable == null)
            {
                return false;
            }

            foreach (DictionaryEntry entry in hashTable)
            {
                if (entry.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddRange(this Hashtable hashTable, Hashtable range)
        {
            foreach(DictionaryEntry entry in range)
            {
                if(!hashTable.ContainsKey(entry.Key))
                {
                    hashTable.Add(entry.Key, entry.Value);
                }
                else
                {
                    hashTable[entry.Key] = entry.Value;
                }
            }
        }
    }
}
