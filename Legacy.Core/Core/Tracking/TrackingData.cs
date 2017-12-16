using System;
using System.Collections.Generic;

namespace Legacy.Core.Tracking
{
	public class TrackingData
	{
		private String m_tagName;

		private Dictionary<String, String> m_attributes = new Dictionary<String, String>();

		public TrackingData(String p_tagName)
		{
			m_tagName = p_tagName;
		}

		public String TagName => m_tagName;

	    public void AddAttribute(String m_name, Int32 m_value)
		{
			AddAttribute(m_name, m_value.ToString());
		}

		public void AddAttribute(String m_name, Int64 m_value)
		{
			AddAttribute(m_name, m_value.ToString());
		}

		public void AddAttribute(String m_name, Single m_value)
		{
			AddAttribute(m_name, m_value.ToString());
		}

		public void AddAttribute(String m_name, Boolean m_value)
		{
			AddAttribute(m_name, m_value.ToString());
		}

		public void AddAttribute(String m_name, String m_value)
		{
			m_attributes[m_name] = m_value;
		}

		public String GetAttributesAsString()
		{
			String text = String.Empty;
			foreach (KeyValuePair<String, String> keyValuePair in m_attributes)
			{
				if (text != String.Empty)
				{
					text += "&";
				}
				text = text + keyValuePair.Key + "=" + keyValuePair.Value;
			}
			return text;
		}
	}
}
