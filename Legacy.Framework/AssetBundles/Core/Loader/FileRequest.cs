using System;
using System.IO;
using UnityEngine;

namespace AssetBundles.Core.Loader
{
	public class FileRequest : AssetBundleRequest
	{
		private String m_Address;

		private Single m_Progress;

		private Int32 m_ReceivedBytes;

		private String m_ErrorText;

		private AssetBundle m_Bundle;

		private FileInfo m_File;

		private Boolean m_IsStarted;

		private Int32 m_Priority;

		public FileRequest(String address, Int32 priority)
		{
			m_Address = address;
			m_Priority = priority;
			m_File = new FileInfo(address);
			if (!m_File.Exists)
			{
				m_ErrorText = "File not found. " + m_File.FullName;
			}
		}

		public override Boolean IsStarted => m_IsStarted;

	    public override Single Progress => m_Progress;

	    public override Int32 TotalBytes => (!m_File.Exists) ? 0 : ((Int32)m_File.Length);

	    public override Int32 ReceivedBytes => m_ReceivedBytes;

	    public override String RequestAddress => m_Address;

	    public override String ErrorText => m_ErrorText;

	    public override Int32 Priority => m_Priority;

	    public override void Start()
		{
			if (m_File.Exists)
			{
				m_Bundle = AssetBundle.CreateFromFile(m_Address);
				m_Progress = 1f;
				m_ReceivedBytes = (Int32)m_File.Length;
				m_IsStarted = true;
				if (m_Bundle == null)
				{
					m_ErrorText = "File corruption, CRC error or not compatible scripts. Check unity output log.";
				}
			}
		}

		public override void Abort()
		{
			m_Progress = 0f;
			m_ReceivedBytes = 0;
			m_Bundle = null;
			m_IsStarted = false;
		}

		public override Single Update()
		{
			return 0f;
		}

		public override void Dispose()
		{
			Abort();
		}

		public override AssetBundle GetAssetBundle()
		{
			return m_Bundle;
		}
	}
}
