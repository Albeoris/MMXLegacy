using System;
using System.IO;

namespace AssetBundles.Core.Loader
{
	public class AssetBundleRawFileLoader : AssetBundleLoader
	{
		protected override AssetBundleRequest GetAssetBundleRequest(String address, Int32 priority, Int32 fileSize, Int32 version, UInt32 crcValue)
		{
			return new FileRequest(Path.Combine(ParsedBaseAddress, address), priority);
		}
	}
}
