using System;
using System.IO;

namespace Legacy.MSBuild
{
    public sealed class GameLocationInfo
    {
        public readonly String RootDirectory;

        public const String LauncherName = @"Might and Magic X Legacy.exe";
        private const String DataRelativePath = @"Might and Magic X Legacy_Data";
        private const String ManagedRelativePath = DataRelativePath + "\\Managed";
        private const String StreamingAssetsRelativePath = @"StreamingAssets";

        public String LauncherPath => Path.Combine(RootDirectory, LauncherName);
        public String DataPath => Path.Combine(RootDirectory, DataRelativePath);
        public String ManagedPath => Path.Combine(RootDirectory, ManagedRelativePath);
        public String StreamingAssetsPath => Path.Combine(RootDirectory, StreamingAssetsRelativePath);

        public GameLocationInfo(String rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public void Validate()
        {
            if (!File.Exists(LauncherPath))
                throw new FileNotFoundException(LauncherPath, LauncherPath);
        }
    }
}