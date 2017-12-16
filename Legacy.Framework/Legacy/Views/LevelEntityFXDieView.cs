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
	public class LevelEntityFXDieView : BaseView
	{
		private Single m_StartTime;

		private Single m_Targettime;

		private Boolean m_DieStart;

		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private AnimatorControl m_AnimatorControl;

		[SerializeField]
		private GameObject m_Effect;

		[SerializeField]
		private Single m_AnimTime = 3.8f;

		[SerializeField]
		private Single m_RendererDisableTime = 3.6f;

		[SerializeField]
		private Vector3 m_SpawnEffectOffset;

		[SerializeField]
		private Animation m_LightOffAnim_1;

		[SerializeField]
		private Animation m_LightOffAnim_2;

		public Boolean m_DisableRendererTimed;

		private Boolean m_Died;

		private Int32 m_DieHash;

		private Single m_Time;

		private Boolean m_DieRecalled;

		protected override void Awake()
		{
			m_DieHash = Animator.StringToHash("DIE");
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
			if (m_DisableRendererTimed)
			{
				AbilityViewMonster component = GetComponent<AbilityViewMonster>();
				if (component != null)
				{
					component.ChangeExplosiveAbilityDelay(m_RendererDisableTime);
				}
			}
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
				if (m_LightOffAnim_1 != null)
				{
					m_LightOffAnim_1.Play();
				}
				if (m_LightOffAnim_2 != null)
				{
					m_LightOffAnim_2.Play();
				}
				if (m_DisableRendererTimed)
				{
					m_StartTime = Time.time;
					m_Targettime = m_StartTime + m_RendererDisableTime;
					m_DieStart = true;
					Destroy(this.gameObject, m_AnimTime);
					if (m_Effect != null)
					{
						GameObject gameObject = Helper.Instantiate<GameObject>(m_Effect);
						gameObject.transform.position = transform.position + m_SpawnEffectOffset;
						gameObject.SendEvent("SetMonster", new UnityEventArgs(this.gameObject, EventArgs.Empty));
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
			if (m_DieStart && Time.time > m_Targettime)
			{
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = false;
				}
				m_DieStart = false;
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
