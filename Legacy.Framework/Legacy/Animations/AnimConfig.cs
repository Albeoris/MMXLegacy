using System;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimConfig : ScriptableObject
	{
		[SerializeField]
		internal AnimationClip[] m_AnimationMap = new AnimationClip[15];

		[SerializeField]
		private Single m_MoveSpeed = 1f;

		public Single MoveSpeed => m_MoveSpeed;

	    public Boolean IsClipDefined(EAnimType type)
		{
			return type < (EAnimType)m_AnimationMap.Length && m_AnimationMap[(Int32)type] != null;
		}

		public Single ClipLength(EAnimType type)
		{
			return (type >= (EAnimType)m_AnimationMap.Length || !(m_AnimationMap[(Int32)type] != null)) ? -1f : m_AnimationMap[(Int32)type].length;
		}

		public String ClipName(EAnimType type)
		{
			return (type >= (EAnimType)m_AnimationMap.Length || !(m_AnimationMap[(Int32)type] != null)) ? null : m_AnimationMap[(Int32)type].name;
		}

		public EAnimType ClipType(String clipName)
		{
			for (Int32 i = 0; i < m_AnimationMap.Length; i++)
			{
				if (m_AnimationMap[i] != null && m_AnimationMap[i].name == clipName)
				{
					return (EAnimType)i;
				}
			}
			return EAnimType._MAX_;
		}
	}
}
