using System;
using UnityEngine;

namespace AssetBundles.Core.Loader
{
	public class CachedWebRequest : WebRequest
	{
		private Int32 m_Version;

		private UInt32 m_CrcValue;

		public CachedWebRequest(String address, Int32 priority, Int32 fileSize, Int32 version, UInt32 crcValue) : base(address, priority, null, fileSize)
		{
			m_Version = version;
			m_CrcValue = crcValue;
		}

		public override void Start()
		{
			if (m_WWW != null)
			{
				return;
			}
			m_IsStarted = true;
			m_Finish = false;
			m_WWW = WWW.LoadFromCacheOrDownload(RequestAddress, m_Version, m_CrcValue);
		}
	}
}
