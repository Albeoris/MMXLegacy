using System;
using Legacy.Animations;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy.Views
{
	public class MonsterDieView : BaseView
	{
		[SerializeField]
		private MonsterCombatView m_monsterCombatView;

		protected Boolean m_DieOnce = true;

		public void Die()
		{
			if (m_DieOnce)
			{
				OnDie();
				m_DieOnce = false;
			}
		}

		protected virtual void OnDie()
		{
			FXArgs pArgs = new FXArgs(gameObject, gameObject, gameObject, gameObject, Vector3.zero, Vector3.forward, Vector3.left, Vector3.zero);
			m_monsterCombatView.HandleAnimationOverrideFX(EAnimType.DIE, pArgs);
			Single p_dieAnimationDuration = m_monsterCombatView.PlayAnimation(EAnimType.DIE, CombatViewBase.EState.HIT, -1f, 1f);
			DieFX dieFX = gameObject.AddComponent<DieFX>();
			dieFX.Init(MyController, EventArgs.Empty, p_dieAnimationDuration);
		}
	}
}
