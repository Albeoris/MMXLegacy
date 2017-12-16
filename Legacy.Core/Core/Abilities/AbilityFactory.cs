using System;
using Legacy.Core.Combat;

namespace Legacy.Core.Abilities
{
	public static class AbilityFactory
	{
		public static MonsterAbilityBase CreateMonsterAbility(EMonsterAbilityType p_type, Single p_magicPower)
		{
			switch (p_type)
			{
			case EMonsterAbilityType.REGENERATION:
				return new MonsterAbilityRegeneration();
			case EMonsterAbilityType.PAIN_REFLECTION:
				return new MonsterAbilityPainReflection();
			case EMonsterAbilityType.EXPLOSIVE:
				return new MonsterAbilityExplosive();
			case EMonsterAbilityType.RETALIATION:
				return new MonsterAbilityRetaliation();
			case EMonsterAbilityType.ENRAGED:
				return new MonsterAbilityEnraged();
			case EMonsterAbilityType.VINDICTIVE:
				return new MonsterAbilityVindictive();
			case EMonsterAbilityType.SWEEPING_ATTACK:
				return new MonsterAbilitySweepingAttack();
			case EMonsterAbilityType.RELENTLESS:
				return new MonsterAbilityRelentless();
			case EMonsterAbilityType.STUNNING_STRIKES:
				return new MonsterAbilityStunningStrikes();
			case EMonsterAbilityType.WEAKENING_STRIKES:
				return new MonsterAbilityWeakening();
			case EMonsterAbilityType.CURSING_STRIKES:
				return new MonsterAbilityCursing();
			case EMonsterAbilityType.CONFUSING_STRIKES:
				return new MonsterAbilityConfusingStrikes();
			case EMonsterAbilityType.POISONING_STRIKES:
				return new MonsterAbilityPoisoning();
			case EMonsterAbilityType.LARGE:
				return new MonsterAbilityLarge();
			case EMonsterAbilityType.MANA_LEECH:
				return new MonsterAbilityManaLeech();
			case EMonsterAbilityType.FOCUSED:
				return new MonsterAbilityFocused();
			case EMonsterAbilityType.BURNING:
				return new MonsterAbilityBurning();
			case EMonsterAbilityType.UNDEAD:
				return new MonsterAbilityUndead();
			case EMonsterAbilityType.DEMONIC_LINEAGE:
				return new MonsterAbilityDemonicLineage();
			case EMonsterAbilityType.VULNERABILITY_LIGHT:
				return new MonsterAbilityVulnerability(EMonsterAbilityType.VULNERABILITY_LIGHT, EDamageType.LIGHT);
			case EMonsterAbilityType.VULNERABILITY_AIR:
				return new MonsterAbilityVulnerability(EMonsterAbilityType.VULNERABILITY_AIR, EDamageType.AIR);
			case EMonsterAbilityType.VULNERABILITY_WATER:
				return new MonsterAbilityVulnerability(EMonsterAbilityType.VULNERABILITY_WATER, EDamageType.WATER);
			case EMonsterAbilityType.VULNERABILITY_EARTH:
				return new MonsterAbilityVulnerability(EMonsterAbilityType.VULNERABILITY_EARTH, EDamageType.EARTH);
			case EMonsterAbilityType.VULNERABILITY_FIRE:
				return new MonsterAbilityVulnerability(EMonsterAbilityType.VULNERABILITY_FIRE, EDamageType.FIRE);
			case EMonsterAbilityType.VULNERABILITY_DARKNESS:
				return new MonsterAbilityVulnerability(EMonsterAbilityType.VULNERABILITY_DARKNESS, EDamageType.DARK);
			case EMonsterAbilityType.CURLING:
				return new MonsterAbilityCurling();
			case EMonsterAbilityType.PUSH:
				return new MonsterAbilityPush();
			case EMonsterAbilityType.BOSS:
				return new MonsterAbilityBoss();
			case EMonsterAbilityType.SHADOW_CLOAK:
				return new MonsterAbilityShadowCloak();
			case EMonsterAbilityType.VANISH:
				return new MonsterAbilityVanish();
			case EMonsterAbilityType.STATIC_OBJECT:
				return new MonsterAbilityStaticObject();
			case EMonsterAbilityType.SPIRIT_BOUND:
				return new MonsterAbilitySpiritBound();
			case EMonsterAbilityType.MARTYR:
				return new MonsterAbilityMartyr(EMonsterAbilityType.MARTYR, p_magicPower);
			case EMonsterAbilityType.PIERCING_STRIKES:
				return new MonsterAbilityPiercingStrikes();
			case EMonsterAbilityType.UNSTOPPABLE_STRIKES:
				return new MonsterAbilityUnstoppableStrikes();
			case EMonsterAbilityType.SPEED_OF_LIGHT:
				return new MonsterAbilitySpeedOfLight();
			default:
				return null;
			}
		}
	}
}
