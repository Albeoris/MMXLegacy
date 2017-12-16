using System;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class SummonMovementTimingView : MonoBehaviour, ISummonMovementTiming
	{
		[SerializeField]
		private SummonView m_summonView;

		[SerializeField]
		private Single m_movementDoneMinDistance = 1.5f;

		private OnSummonMoveFinishedCallback m_callback;

		public void OnMoveEntity(Object p_sender, EventArgs p_args, OnSummonMoveFinishedCallback p_callback)
		{
			if (m_callback != null)
			{
				Debug.LogError("SummonMovementTimingView: core moved this summon for a second time, before the first movement was fully executed!");
			}
			m_callback = p_callback;
		}

		private void Start()
		{
			if (m_summonView == null)
			{
				Debug.LogError("SummonMovementTimingView: m_summonView is not set!");
				return;
			}
			m_summonView.SetMovementTiming(this);
		}

		private void Update()
		{
			if (m_callback != null && (m_summonView.transform.position - transform.position).magnitude <= m_movementDoneMinDistance)
			{
				m_callback();
				m_callback = null;
			}
		}
	}
}
