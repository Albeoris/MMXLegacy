using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.TrapEffects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class PortraitFXView : MonoBehaviour
	{
		private const Int32 FPS_MELEE_HIT_ANIM = 10;

		private Character m_character;

		private HUDDamageText m_HUDDamageText;

		public void Init(Character p_character, HUDDamageText p_HUDDamageText)
		{
			m_character = p_character;
			m_HUDDamageText = p_HUDDamageText;
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.REFLECTED_MAGIC_DAMAGE, new EventHandler(OnReceivedReflectedDamage));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_POTION_USED, new EventHandler(OnCharacterPotionUsed));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_DEFEND, new EventHandler(OnDefend));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_TAKE_POISON_DAMAGE, new EventHandler(OnPoisonDamage));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_HEALS, new EventHandler(OnHeal));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SUMMON_CAST_SPELL, new EventHandler(OnSummonCastSpell));
		}

		public void ChangeCharacter(Character p_character)
		{
			m_HUDDamageText.Clear();
			m_character = p_character;
		}

		public void CleanUp()
		{
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksRanged));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.REFLECTED_MAGIC_DAMAGE, new EventHandler(OnReceivedReflectedDamage));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_POTION_USED, new EventHandler(OnCharacterPotionUsed));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_DEFEND, new EventHandler(OnDefend));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_TAKE_POISON_DAMAGE, new EventHandler(OnPoisonDamage));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_HEALS, new EventHandler(OnHeal));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SUMMON_CAST_SPELL, new EventHandler(OnSummonCastSpell));
			Destroy(this);
		}

		public void OnMonsterCounterAttack(EventArgs p_args)
		{
			OnMonsterAttacksGeneric(null, p_args, false);
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
				if (attackedTarget.AttackTarget == m_character)
				{
					ShowDamageTextAndAnimationAndPlaySound(attackedTarget.AttackResult, p_isRanged, false, false);
				}
			}
		}

		private void OnMonsterCastSpell(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			foreach (AttackedTarget attackedTarget in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
			{
				if (attackedTarget.Target == m_character && attackedTarget.Result != null)
				{
					ShowDamageTextAndAnimationAndPlaySound(attackedTarget.Result, false, false, spellEventArgs.DamageType != EDamageType.PHYSICAL);
				}
			}
		}

		private void OnReceivedReflectedDamage(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_character && p_args is AttacksEventArgs)
			{
				OnMonsterAttacksGeneric(p_sender, p_args, false);
			}
		}

		private void OnCharacterPotionUsed(Object sender, EventArgs e)
		{
			if (sender == m_character)
			{
				AudioController.Play("UsePotion", FXHelper.GetCharacterGO(m_character.Index).transform);
			}
		}

		private void OnDefend(Object sender, EventArgs e)
		{
			if (sender == m_character)
			{
				AudioController.Play("Defend", FXHelper.GetCharacterGO(m_character.Index).transform);
			}
		}

		private void OnTrapTriggered(Object sender, EventArgs e)
		{
			if (m_character != null && sender is DamageDealingTrapEffect)
			{
				DamageDealingTrapEffect damageDealingTrapEffect = (DamageDealingTrapEffect)sender;
				ShowDamageTextAndAnimationAndPlaySound(damageDealingTrapEffect.AttackResults[LegacyLogic.Instance.WorldManager.Party.GetMemberIndex(m_character)], false, false, false);
			}
		}

		private void OnPoisonDamage(Object sender, EventArgs e)
		{
			if (m_character != null && sender == m_character && e is DamageEventArgs)
			{
				DamageEventArgs damageEventArgs = (DamageEventArgs)e;
				ShowDamageTextAndAnimationAndPlaySound(damageEventArgs.AttackResult, false, true, true);
			}
		}

		private void OnHeal(Object sender, EventArgs e)
		{
			if (m_character != null && sender == m_character && e is DamageEventArgs)
			{
				DamageEventArgs damageEventArgs = (DamageEventArgs)e;
				ShowDamageTextAndAnimationAndPlaySound(damageEventArgs.AttackResult, false, false, true);
			}
		}

		private void OnSummonCastSpell(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			foreach (AttackedTarget attackedTarget in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
			{
				if (attackedTarget.Target == m_character)
				{
					ShowDamageTextAndAnimationAndPlaySound(attackedTarget.Result, false, false, spellEventArgs.DamageType != EDamageType.PHYSICAL);
				}
			}
		}

		private void ShowDamageTextAndAnimationAndPlaySound(AttackResult pAttackResult, Boolean pIsRange, Boolean pIsPoison, Boolean p_isMagical)
		{
			if (m_HUDDamageText != null)
			{
				GUIHUDText.PrintPortrait(m_HUDDamageText, pAttackResult, p_isMagical);
			}
			if (pAttackResult.Result == EResultType.BLOCK)
			{
				if (!pIsRange && !pIsPoison)
				{
					AudioController.Play("Blocked_Melee", FXHelper.GetCharacterGO(m_character.Index).transform);
				}
			}
			else if (pAttackResult.Result == EResultType.EVADE)
			{
				if (!pIsRange && !pIsPoison)
				{
					AudioController.Play("Miss_Attack", FXHelper.GetCharacterGO(m_character.Index).transform);
				}
			}
			else if (pIsPoison)
			{
				AudioController.Play("Poison_Damage", FXHelper.GetCharacterGO(m_character.Index).transform);
			}
			else if (pAttackResult.Result == EResultType.HIT && pAttackResult.DamageDone > 0)
			{
				AudioController.Play("Hit_Melee", FXHelper.GetCharacterGO(m_character.Index).transform);
			}
			else if (pAttackResult.Result == EResultType.CRITICAL_HIT)
			{
				AudioController.Play("Hit_Melee", FXHelper.GetCharacterGO(m_character.Index).transform);
			}
		}
	}
}
