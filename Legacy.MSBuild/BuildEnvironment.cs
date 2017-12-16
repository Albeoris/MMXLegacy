using System;

namespace Legacy.MSBuild
{
    internal static class BuildEnvironment
    {
        public static Boolean IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}