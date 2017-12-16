using System;
using Legacy.Animations;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class LevelEntityMultiFXDieView : BaseView
	{
		private Single m_StartTime;

		private Single m_Targettime;

		private Boolean m_DieStart;

		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private AnimatorControl m_AnimatorControl;

		[SerializeField]
		private GameObject[] m_Effect;

		[SerializeField]
		private Single m_AnimTime = 3.8f;

		[SerializeField]
		private Single m_AnimFreezeTime;

		[SerializeField]
		private Single m_RendererDisableTime = 3.6f;

		[SerializeField]
		private Vector3 m_SpawnEffectOffset;

		public Boolean m_DisableRendererTimed;

		protected override void Awake()
		{
			base.Awake();
			if (m_Animator == null)
			{
				m_Animator = this.GetComponent<Animator>(true);
			}
			if (m_AnimatorControl == null)
			{
				m_AnimatorControl = this.GetComponent<AnimatorControl>(true);
			}
			m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && !(MyController is MovingEntity))
			{
				throw new NotSupportedException("Only MovingEntity objects\n" + MyController.GetType().FullName);
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnEntityDied));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnEntityDied));
			}
		}

		public void OnEntityDied(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				m_AnimatorControl.Die();
				if (MyController is Monster)
				{
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.ON_APPLY_MONSTER_BUFF);
					((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.MONSTER_DIES);
				}
				if (m_DisableRendererTimed)
				{
					m_StartTime = Time.time;
					m_Targettime = m_StartTime + m_RendererDisableTime;
					m_AnimFreezeTime = m_StartTime + m_AnimFreezeTime;
					m_DieStart = true;
					Destroy(this.gameObject, m_AnimTime);
					for (Int32 i = 0; i < m_Effect.Length; i++)
					{
						if (m_Effect[i] != null)
						{
							GameObject gameObject = Helper.Instantiate<GameObject>(m_Effect[i]);
							gameObject.transform.position = transform.position + m_SpawnEffectOffset;
							gameObject.SendEvent("SetMonster", new UnityEventArgs(this.gameObject, EventArgs.Empty));
						}
					}
				}
				if (!m_DisableRendererTimed)
				{
					DieFX dieFX = gameObject.AddComponent<DieFX>();
					dieFX.Init(MyController, EventArgs.Empty, 2f);
				}
				else
				{
					Destroy(gameObject, 8f);
				}
			}
		}

		private void Update()
		{
			if (m_DieStart)
			{
				if (Time.time > m_Targettime)
				{
					Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer in componentsInChildren)
					{
						if (!renderer.particleSystem)
						{
							renderer.enabled = false;
						}
					}
					m_DieStart = false;
				}
				if (Time.time > m_AnimFreezeTime)
				{
					m_Animator.speed = 0f;
				}
			}
		}

		private void FlushMonsterLogEntries()
		{
			Monster monster = (Monster)MyController;
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION);
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_AFTER_DAMAGE_REDUCTION);
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_COUNTER_ATTACK);
			monster.AbilityHandler.FlushActionLog(EExecutionPhase.ON_APPLY_MONSTER_BUFF);
			monster.BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
		}
	}
}
