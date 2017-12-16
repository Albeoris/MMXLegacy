using System;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellOverwhelmingAssault : MonsterSpell
	{
		public MonsterSpellOverwhelmingAssault(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.OVERWHELMING_ASSAULT, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			Attack singleMeleeAttack = p_monster.CombatHandler.GetSingleMeleeAttack();
			AttackResult attackResult = p_target.FightHandler.AttackEntity(singleMeleeAttack, true, MagicSchool, true, 0, false);
			p_target.ApplyDamages(attackResult, p_monster);
			p_target.FightHandler.CurrentMeleeBlockAttempts = 0;
			p_target.FightHandler.CurrentGeneralBlockAttempts = 0;
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(false);
			attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(p_target, attackResult));
			LegacyLogic.Instance.EventManager.InvokeEvent(p_monster, EEventType.MONSTER_ATTACKS, attacksEventArgs);
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			return Localization.Instance.GetText("MONSTER_SPELL_OVERWHELMING_ASSAULT_INFO", m_descriptionValues);
		}
	}
}
