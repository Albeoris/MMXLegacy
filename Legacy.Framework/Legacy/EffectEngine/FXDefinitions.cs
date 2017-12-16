using System;
using Legacy.Animations;
using UnityEngine;

namespace Legacy.EffectEngine
{
	[AddComponentMenu("MM Legacy/Effects/Definitions")]
	public class FXDefinitions : MonoBehaviour
	{
		[SerializeField]
		internal FXDescriptionRef[] m_EffectAddOn = new FXDescriptionRef[15];

		public Boolean Contains(EAnimType p_type)
		{
			return !String.IsNullOrEmpty(m_EffectAddOn[(Int32)p_type].m_PrefabPath);
		}

		public FXDescription Load(EAnimType p_type)
		{
			return m_EffectAddOn[(Int32)p_type].Load();
		}

		[Serializable]
		internal class FXDescriptionRef
		{
			public String m_AssetGUID;

			public String m_PrefabPath;

			public FXDescription Load()
			{
				if (String.IsNullOrEmpty(m_PrefabPath))
				{
					return null;
				}
				return Helper.Instantiate<FXDescription>(Helper.ResourcesLoad<FXDescription>(m_PrefabPath));
			}
		}
	}
}
