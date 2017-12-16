using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundles.Core
{
	public class AssetBundleDatabase : IEnumerable<AssetBundleData>, IEnumerable
	{
		private Dictionary<String, AssetBundleData> m_AssetBundles;

		private Dictionary<String, AssetBundleData> m_Assets;

		public AssetBundleDatabase(AssetBundleData[] bundles) : this(bundles, CountAssets(bundles))
		{
		}

		public AssetBundleDatabase(AssetBundleData[] bundles, Int32 totalAssetCount)
		{
			if (bundles == null)
			{
				throw new ArgumentNullException("bundles");
			}
			if (totalAssetCount < 0)
			{
				throw new ArgumentOutOfRangeException("totalAssetCount", "totalAssetCount < 0");
			}
			m_AssetBundles = new Dictionary<String, AssetBundleData>(bundles.Length, StringComparer.InvariantCultureIgnoreCase);
			m_Assets = new Dictionary<String, AssetBundleData>(totalAssetCount, StringComparer.InvariantCultureIgnoreCase);
			foreach (AssetBundleData assetBundleData in bundles)
			{
				if (m_AssetBundles.ContainsKey(assetBundleData.Name))
				{
					Debug.LogError("AssetBundleDatabase BundleKey conflict! '" + assetBundleData.Name + "' already in map.");
				}
				else
				{
					m_AssetBundles.Add(assetBundleData.Name, assetBundleData);
					String[] assets = assetBundleData.Assets;
					for (Int32 j = 0; j < assets.Length; j++)
					{
						String text = assets[j];
						if (m_Assets.ContainsKey(text))
						{
							AssetBundleData assetBundleData2 = m_Assets[assets[j]];
							Debug.LogError(String.Concat(new String[]
							{
								"AssetBundleDatabase AssetName conflict! Name already in map.\nAssetName: ",
								text,
								"\nBundleA: ",
								assetBundleData2.Name,
								"\nBundleB: ",
								assetBundleData.Name
							}));
						}
						else
						{
							m_Assets.Add(text, assetBundleData);
						}
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_AssetBundles.Values.GetEnumerator();
		}

		public Int32 AssetBundleCount => m_AssetBundles.Count;

	    public Int32 AssetCount => m_Assets.Count;

	    public AssetBundleData FindBundleByAssetName(String assetName)
		{
			if (String.IsNullOrEmpty(assetName))
			{
				return null;
			}
			AssetBundleData result;
			m_Assets.TryGetValue(assetName, out result);
			return result;
		}

		public AssetBundleData FindBundleByBundleName(String bundleName)
		{
			if (String.IsNullOrEmpty(bundleName))
			{
				return null;
			}
			AssetBundleData result;
			m_AssetBundles.TryGetValue(bundleName, out result);
			return result;
		}

		public IEnumerator<AssetBundleData> GetEnumerator()
		{
			return m_AssetBundles.Values.GetEnumerator();
		}

		private static Int32 CountAssets(AssetBundleData[] bundles)
		{
			if (bundles != null)
			{
				Int32 num = 0;
				for (Int32 i = 0; i < bundles.Length; i++)
				{
					if (bundles[i].Assets != null)
					{
						num += bundles[i].Assets.Length;
					}
				}
				return num;
			}
			return 0;
		}
	}
}
