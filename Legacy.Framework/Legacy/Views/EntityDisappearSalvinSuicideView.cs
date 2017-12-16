using System;
using Legacy.Animations;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class EntityDisappearSalvinSuicideView : MonsterDisappearView
	{
		[SerializeField]
		private AnimatorControl m_AnimatorControl;

		[SerializeField]
		private GameObject m_Effect_1;

		[SerializeField]
		private GameObject m_Effect_2;

		[SerializeField]
		private Texture m_DissolveTexture;

		public Single m_DisappearTimeSpan;

		private Single m_DissolveStartTime = 5f;

		private Single m_DissolveTargetTime;

		private Single m_DissolveTimeUnitValue;

		private Single m_TargetTime;

		private Boolean m_Do;

		private Object m_Sender;

		private EventArgs m_Event;

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DISAPPEARED, new EventHandler(StartDisappearFX));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DISAPPEARED, new EventHandler(StartDisappearFX));
			}
		}

		protected override void OnMonsterDisappeared(Object sender, EventArgs e)
		{
			if (sender == MyController)
			{
				base.OnMonsterDisappeared(sender, e);
			}
		}

		private void StartDisappearFX(Object sender, EventArgs e)
		{
			if (sender == MyController)
			{
				m_Sender = sender;
				m_Event = e;
				m_TargetTime = Time.time + m_DisappearTimeSpan;
				m_DissolveTimeUnitValue = m_TargetTime - Time.time;
				m_DissolveTimeUnitValue = (m_DissolveTimeUnitValue - m_DissolveStartTime) / 100f;
				Single num = m_TargetTime - Time.time;
				m_DissolveTargetTime = m_TargetTime - (num - m_DissolveStartTime);
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.material.SetTexture("_DissolveTex", m_DissolveTexture);
				}
				StartFX();
				StartAnimation();
				m_Do = true;
			}
		}

		private void StartFX()
		{
			m_Effect_1.particleSystem.Play();
			GameObject gameObject = Helper.Instantiate<GameObject>(m_Effect_2);
			gameObject.transform.position = transform.position;
		}

		private void StartAnimation()
		{
			m_AnimatorControl.EventSummon(3);
		}

		private void Update()
		{
			if (m_Do && m_DissolveTargetTime < Time.time)
			{
				Single num = m_TargetTime - Time.time;
				Single num2 = num / m_DissolveTimeUnitValue / 100f;
				Single value = 1f - num2;
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.material.SetFloat("_DissolveValue", value);
				}
			}
			if (m_TargetTime < Time.time && m_Do)
			{
				OnMonsterDisappeared(m_Sender, m_Event);
			}
		}
	}
}
