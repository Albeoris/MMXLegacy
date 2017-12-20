using System;

namespace Legacy.Utilities
{
    public static class TypeCache<T>
    {
        public static readonly Type Type = typeof(T);
    }
}
