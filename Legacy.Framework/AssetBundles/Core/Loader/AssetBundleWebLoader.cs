using System;
using UnityEngine;

namespace AssetBundles.Core.Loader
{
	public class AssetBundleWebLoader : AssetBundleLoader
	{
		public WWWForm RequestHead { get; set; }

		protected override AssetBundleRequest GetAssetBundleRequest(String address, Int32 priority, Int32 fileSize, Int32 version, UInt32 crcValue)
		{
			return new WebRequest(ParsedBaseAddress + "/" + address, priority, RequestHead, fileSize);
		}
	}
}
