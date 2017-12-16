using System;

namespace AssetBundles.Core.Loader
{
	public class AssetBundleCachedWebLoader : AssetBundleWebLoader
	{
		protected override AssetBundleRequest GetAssetBundleRequest(String address, Int32 priority, Int32 fileSize, Int32 version, UInt32 crcValue)
		{
			return new CachedWebRequest(ParsedBaseAddress + "/" + address, priority, fileSize, version, crcValue);
		}
	}
}
