using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class StopFXDelayedAfterDieAnim : MonoBehaviour
	{
		[SerializeField]
		private Animator m_Animator;

		public Single m_DelayedFXDestroyTime;

		private Int32 m_DieHash;

		private Int32 m_CurrentHash;

		private Boolean m_DestroyFXObject;

		private void Awake()
		{
			m_DieHash = Animator.StringToHash("DIE");
		}

		private Int32 GetCurrentHash()
		{
			return m_Animator.GetCurrentAnimatorStateInfo(0).tagHash;
		}

		private void Update()
		{
			if (!m_DestroyFXObject)
			{
				m_CurrentHash = GetCurrentHash();
				if (m_CurrentHash == m_DieHash && m_Animator.playbackTime == -1f)
				{
					particleSystem.Stop();
					m_DestroyFXObject = true;
				}
			}
			if (m_DestroyFXObject)
			{
				Destroy(gameObject, m_DelayedFXDestroyTime);
			}
			if (m_Animator == null)
			{
				Debug.Log("StopFXDelayedAfterDieAnim: No Animator attached to Script");
			}
		}
	}
}
