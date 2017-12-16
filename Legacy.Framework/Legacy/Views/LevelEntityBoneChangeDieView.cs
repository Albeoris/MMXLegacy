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
	public class LevelEntityBoneChangeDieView : BaseView
	{
		[SerializeField]
		private Animator m_Animator;

		[SerializeField]
		private AnimatorControl m_AnimatorControl;

		[SerializeField]
		private SkinnedMeshRenderer m_Normal;

		private Transform m_sourceRoot;

		[SerializeField]
		private SkinnedMeshRenderer m_Die;

		private Transform m_targetRoot;

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
				m_Normal.renderer.enabled = false;
				m_Die.renderer.enabled = true;
				if (MyController is Monster)
				{
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.ON_APPLY_MONSTER_BUFF);
					((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE);
					((Monster)MyController).AbilityHandler.FlushActionLog(EExecutionPhase.MONSTER_DIES);
				}
				DieFX dieFX = gameObject.AddComponent<DieFX>();
				dieFX.Init(MyController, EventArgs.Empty, 2f);
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
