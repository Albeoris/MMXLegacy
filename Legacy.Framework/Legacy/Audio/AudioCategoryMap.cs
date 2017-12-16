using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Audio
{
	public class AudioCategoryMap : ScriptableObject
	{
		internal Dictionary<String, UInt16> m_cache;

		[SerializeField]
		internal String[] m_AudioItems = Arrays<String>.Empty;

		[SerializeField]
		internal String[] m_ControllerPaths = Arrays<String>.Empty;

		[SerializeField]
		internal Int32[] m_AudioItemRanges = Arrays<Int32>.Empty;

		[SerializeField]
		internal Int32[] m_ControllerTag = Arrays<Int32>.Empty;

		[SerializeField]
		internal String[] m_ControllerTagPaths = Arrays<String>.Empty;

		public String FindControllerName(String p_AudioID)
		{
			if (m_cache == null)
			{
				OnEnable();
			}
			UInt16 num;
			if (p_AudioID != null && m_cache.TryGetValue(p_AudioID, out num))
			{
				return m_ControllerPaths[num];
			}
			return null;
		}

		public Int32 FindAudioRange(String p_AudioID)
		{
			if (m_cache == null)
			{
				OnEnable();
			}
			UInt16 num;
			if (p_AudioID != null && m_cache.TryGetValue(p_AudioID, out num))
			{
				return m_AudioItemRanges[num];
			}
			return -1;
		}

		public ECategoryTag GetControllerTag(String p_controllerName)
		{
			if (m_cache == null)
			{
				OnEnable();
			}
			if (m_ControllerTagPaths != null && m_ControllerTagPaths.Length != 0)
			{
				for (Int32 i = 0; i < m_ControllerTagPaths.Length; i++)
				{
					if (m_ControllerTagPaths[i] == p_controllerName)
					{
						return (ECategoryTag)m_ControllerTag[i];
					}
				}
			}
			return ECategoryTag.None;
		}

		public Boolean HasAudioID(String p_audioID)
		{
			if (m_cache == null)
			{
				OnEnable();
			}
			return p_audioID != null && m_cache.ContainsKey(p_audioID);
		}

		public Boolean HasControllerName(String p_controllerName)
		{
			if (m_cache == null)
			{
				OnEnable();
			}
			if (p_controllerName != null)
			{
				for (Int32 i = 0; i < m_ControllerTagPaths.Length; i++)
				{
					if (m_ControllerTagPaths[i] == p_controllerName)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void OnEnable()
		{
			if (m_AudioItems == null || m_AudioItems.Length == 0 || m_ControllerPaths == null || m_ControllerPaths.Length == 0)
			{
				m_cache = new Dictionary<String, UInt16>();
			}
			else
			{
				m_cache = new Dictionary<String, UInt16>(m_AudioItems.Length);
				UInt16 num = 0;
				while (num < m_AudioItems.Length)
				{
					m_cache.Add(m_AudioItems[num], num);
					num += 1;
				}
			}
		}

		public enum ECategoryTag
		{
			None,
			Monster,
			Bark,
			Ambient
		}
	}
}
