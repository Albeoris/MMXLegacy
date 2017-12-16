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
	public class LevelEntityDieViewChangeModel : BaseView
	{
		private Single m_StartTime;

		private Single m_Targettime;

		private Boolean m_DieStart;

		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private AnimatorControl m_AnimatorControl;

		[SerializeField]
		private GameObject m_Model;

		[SerializeField]
		private Single m_RendererDisableTime;

		[SerializeField]
		private Vector3 m_SpawnEffectOffset;

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
				if (MyController is Monster)
				{
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.ON_APPLY_MONSTER_BUFF);
					((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.MONSTER_DIES);
				}
				m_StartTime = Time.time;
				m_Targettime = m_StartTime + m_RendererDisableTime;
				m_DieStart = true;
				Destroy(this.gameObject, 10f);
				if (m_Model != null)
				{
					GameObject gameObject = Helper.Instantiate<GameObject>(m_Model);
					gameObject.transform.position = transform.position + m_SpawnEffectOffset;
					gameObject.transform.rotation = transform.rotation;
					gameObject.SendEvent("SetMonster", new UnityEventArgs(this.gameObject, EventArgs.Empty));
					DieFX dieFX = gameObject.AddComponent<DieFX>();
					dieFX.Init(MyController, EventArgs.Empty, 2f);
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
