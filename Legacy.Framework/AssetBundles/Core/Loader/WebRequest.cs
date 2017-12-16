using System;
using UnityEngine;

namespace AssetBundles.Core.Loader
{
	public class WebRequest : AssetBundleRequest
	{
		protected WWW m_WWW;

		private String m_Address;

		private WWWForm m_Header;

		private Single m_Progress;

		private Int32 m_TotalBytes;

		private Int32 m_ReceivedBytes;

		protected Boolean m_Finish;

		private String m_ErrorText;

		private AssetBundle m_Bundle;

		protected Boolean m_IsStarted;

		private Int32 m_Priority;

		public WebRequest(String address, Int32 priority, WWWForm form, Int32 fileSize)
		{
			m_Address = address;
			m_Priority = priority;
			m_Header = form;
			m_TotalBytes = fileSize;
		}

		public override Boolean IsStarted => m_IsStarted;

	    public override Single Progress => m_Progress;

	    public override Int32 TotalBytes => m_TotalBytes;

	    public override Int32 ReceivedBytes => m_ReceivedBytes;

	    public override String RequestAddress => m_Address;

	    public override String ErrorText => m_ErrorText;

	    public override Boolean IsDone => m_Finish;

	    public override Int32 Priority => m_Priority;

	    public override void Start()
		{
			if (m_WWW != null)
			{
				return;
			}
			m_IsStarted = true;
			m_Finish = false;
			if (m_Header != null)
			{
				m_WWW = new WWW(m_Address, m_Header);
			}
			else
			{
				m_WWW = new WWW(m_Address);
			}
		}

		public override void Abort()
		{
			if (m_WWW != null)
			{
				m_WWW.Dispose();
				m_WWW = null;
				m_Finish = false;
				m_IsStarted = false;
			}
		}

		public override Single Update()
		{
			Single result = 0f;
			if (m_WWW != null && !m_Finish)
			{
				Single progress = m_WWW.progress;
				if (m_Progress != progress)
				{
					result = progress - m_Progress;
					m_Progress = progress;
					if (m_WWW.isDone && m_WWW.error == null)
					{
						m_TotalBytes = m_WWW.size;
					}
					m_ReceivedBytes = (Int32)(m_TotalBytes * m_Progress);
				}
				m_ErrorText = m_WWW.error;
				if (!String.IsNullOrEmpty(m_ErrorText))
				{
					m_Finish = true;
				}
				else if (progress == 1f && m_WWW.isDone)
				{
					m_Finish = true;
					m_Bundle = m_WWW.assetBundle;
				}
			}
			return result;
		}

		public override void Dispose()
		{
			m_Bundle = null;
			Abort();
		}

		public override AssetBundle GetAssetBundle()
		{
			return m_Bundle;
		}
	}
}
