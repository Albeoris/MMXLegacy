using System;
using Legacy.Animations;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class EntityDisappearViewSphere : MonsterDisappearView
	{
		[SerializeField]
		private AnimatorControl m_AnimatorControl;

		[SerializeField]
		private GameObject m_Effect;

		[SerializeField]
		private Vector3 m_SpawnEffectOffset;

		[SerializeField]
		private Single m_EntitySpeed = 1f;

		public Single m_DisappearTimeSpan;

		private Single m_TargetTime;

		private Boolean m_Move;

		public Single m_RenderDisableTime;

		private Single m_RenderDisableTargetTime;

		public Single m_PauseBeforeMoveTime;

		private Single m_TargetMoveTime;

		private Object m_Sender;

		private EventArgs m_Event;

		private GameObject m_EffectInstance;

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
				m_TargetTime = Time.time + m_DisappearTimeSpan;
				m_RenderDisableTargetTime = Time.time + m_RenderDisableTime;
				m_TargetMoveTime = Time.time + m_PauseBeforeMoveTime;
				m_Sender = sender;
				m_Event = e;
				StartAnimation();
				StartEffect();
				m_Move = true;
			}
		}

		private void StartAnimation()
		{
			m_AnimatorControl.AttackMagic();
		}

		private void StartEffect()
		{
			m_EffectInstance = Helper.Instantiate<GameObject>(m_Effect);
			m_EffectInstance.transform.position = transform.position + m_SpawnEffectOffset;
		}

		private void Update()
		{
			if (m_Move)
			{
				if (m_TargetMoveTime < Time.time)
				{
					Vector3 position = m_EffectInstance.transform.position;
					position.y += Time.deltaTime * m_EntitySpeed;
					m_EffectInstance.transform.position = position;
				}
				if (Time.time > m_RenderDisableTargetTime)
				{
					Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer in componentsInChildren)
					{
						renderer.enabled = false;
					}
				}
			}
			if (m_TargetTime < Time.time && m_Move)
			{
				OnMonsterDisappeared(m_Sender, m_Event);
				m_Move = false;
			}
		}
	}
}
