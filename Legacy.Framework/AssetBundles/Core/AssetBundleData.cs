using System;

namespace AssetBundles.Core
{
	public class AssetBundleData
	{
		public String Name;

		public String Path;

		public Int32 Size;

		public Int32 Version;

		public UInt32 CrcValue;

		public AssetBundleData[] BundleDependency;

		public String[] Assets;
	}
}
