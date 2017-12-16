using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDHitEffects")]
	public class HUDHitEffects : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem m_leftEffect;

		[SerializeField]
		private ParticleSystem m_rightEffect;

		[SerializeField]
		private ParticleSystem m_bottomEffect;

		private void Awake()
		{
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
		}

		private void OnDestroy()
		{
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
		}

		private void OnMonsterAttacks(Object p_sender, EventArgs p_args)
		{
			OnMonsterAttacksGeneric(p_sender, p_args, false);
		}

		private void OnMonsterAttacksRanged(Object p_sender, EventArgs p_args)
		{
			OnMonsterAttacksGeneric(p_sender, p_args, true);
		}

		private void OnMonsterAttacksGeneric(Object p_sender, EventArgs p_args, Boolean p_isRanged)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
			{
				ShowHitEffect((Monster)p_sender);
			}
		}

		private void OnMonsterCastSpell(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			foreach (AttackedTarget attackedTarget in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
			{
				if (spellEventArgs.Spell.NameKey != "MONSTER_SPELL_LIQUID_MEMBRANE")
				{
					ShowHitEffect((Monster)p_sender);
				}
			}
		}

		private void ShowHitEffect(Monster mob)
		{
			if (mob != null)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(party.Position, mob.Position);
				if (lineOfSightDirection != party.Direction)
				{
					if (EDirectionFunctions.Add(lineOfSightDirection, 2) == party.Direction)
					{
						m_bottomEffect.Stop();
						m_bottomEffect.Play();
					}
					if (EDirectionFunctions.Add(lineOfSightDirection, 1) == party.Direction)
					{
						m_leftEffect.Stop();
						m_leftEffect.Play();
					}
					if (EDirectionFunctions.Add(lineOfSightDirection, -1) == party.Direction)
					{
						m_rightEffect.Stop();
						m_rightEffect.Play();
					}
				}
			}
		}
	}
}
