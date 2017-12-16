using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.UpdateLogic;
using UnityEngine;
using Object = System.Object;

namespace Legacy.EffectEngine.Effects
{
	public abstract class HitAbsorbingFXBase : MonoBehaviour
	{
		public Boolean m_isMeleeAbsorbing = true;

		public Boolean m_isRangeAbsorbing;

		protected Boolean m_isAbsorbingHit;

		protected Boolean m_isStopingForPartyTurn;

		protected Single m_stopForPartyTurnAtTime = -1f;

		protected virtual void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ROUND_FINISHED, new EventHandler(OnMonsterRoundFinished));
			StopAllCoroutines();
			m_isAbsorbingHit = false;
			m_stopForPartyTurnAtTime = -1f;
			StartCoroutine(ShowStandbyFX());
		}

		protected virtual void Update()
		{
			if (!LegacyLogic.Instance.WorldManager.Party.HasAggro() && m_isAbsorbingHit)
			{
				StopAllCoroutines();
				m_isAbsorbingHit = false;
				StartCoroutine(ShowStandbyFX());
			}
			if (m_isStopingForPartyTurn)
			{
				if (m_stopForPartyTurnAtTime == -1f)
				{
					m_stopForPartyTurnAtTime = Time.time + MinFXTimeDuringPartyTurn();
				}
				else if (m_stopForPartyTurnAtTime <= Time.time)
				{
					StopAllCoroutines();
					m_isStopingForPartyTurn = false;
					m_isAbsorbingHit = false;
					m_stopForPartyTurnAtTime = -1f;
					StartCoroutine(ShowStandbyFX());
				}
			}
			else
			{
				m_stopForPartyTurnAtTime = -1f;
			}
		}

		protected virtual void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ROUND_FINISHED, new EventHandler(OnMonsterRoundFinished));
		}

		private void OnMonsterAttacks(Object p_sender, EventArgs p_args)
		{
			if (m_isMeleeAbsorbing)
			{
				OnMonsterAttacksGeneric();
			}
		}

		private void OnMonsterAttacksRanged(Object p_sender, EventArgs p_args)
		{
			if (m_isRangeAbsorbing)
			{
				OnMonsterAttacksGeneric();
			}
		}

		private void OnMonsterAttacksGeneric()
		{
			StopAllCoroutines();
			m_isAbsorbingHit = true;
			ShowAbsorbHitFX();
			UpdateManager updateManager = LegacyLogic.Instance.UpdateManager;
			m_isStopingForPartyTurn = (updateManager.CurrentTurnActor == updateManager.PartyTurnActor);
		}

		private void OnMonsterRoundFinished(Object p_sender, EventArgs p_args)
		{
			StopAllCoroutines();
			m_isAbsorbingHit = false;
			m_stopForPartyTurnAtTime = -1f;
			StartCoroutine(ShowStandbyFX());
			m_isStopingForPartyTurn = false;
		}

		protected abstract void ShowAbsorbHitFX();

		protected abstract IEnumerator ShowStandbyFX();

		protected abstract Single MinFXTimeDuringPartyTurn();
	}
}
