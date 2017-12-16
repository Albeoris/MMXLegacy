using System;
using UnityEngine;

namespace Legacy.Views
{
	public class ErebosSummonFX : BaseView
	{
		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private GameObject m_Effect;

		private Int32 m_LastHash;

		private Int32 m_EventHash;

		private Int32 m_CurrentHash;

		private Boolean m_FXObjectPlayed;

		protected override void Awake()
		{
			m_EventHash = Animator.StringToHash("EVENT");
		}

		private Int32 GetCurrentHash()
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
			Int32 tagHash = currentAnimatorStateInfo.tagHash;
			if (currentAnimatorStateInfo.IsName("Event.VictoryToCombat"))
			{
				return 0;
			}
			return tagHash;
		}

		private void Update()
		{
			m_CurrentHash = GetCurrentHash();
			if (!m_FXObjectPlayed && m_CurrentHash == m_EventHash)
			{
				GameObject gameObject = Helper.Instantiate<GameObject>(m_Effect);
				gameObject.transform.position = transform.position;
				m_FXObjectPlayed = true;
				m_LastHash = m_CurrentHash;
			}
			if (m_CurrentHash != m_LastHash)
			{
				m_FXObjectPlayed = false;
			}
			if (m_Effect == null)
			{
				Debug.Log("ErebosSummonFX: No FXPrefab attached to Script");
			}
			if (m_Animator == null)
			{
				Debug.Log("ErebosSummonFX: No Animator attached to Script");
			}
		}
	}
}
