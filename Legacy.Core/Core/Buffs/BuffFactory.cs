using System;

namespace Legacy.Core.Buffs
{
	public static class BuffFactory
	{
		public static MonsterBuff CreateMonsterBuff(EMonsterBuffType p_type, Single p_castersMagicFactor, Int32 p_level)
		{
			MonsterBuff monsterBuff = CreateMonsterBuff(p_type, p_castersMagicFactor);
			monsterBuff.Level = p_level;
			monsterBuff.ResetDuration();
			return monsterBuff;
		}

		public static MonsterBuff CreateMonsterBuff(EMonsterBuffType p_type, Single p_castersMagicFactor)
		{
			switch (p_type)
			{
			case EMonsterBuffType.CHILLED:
				return new MonsterBuffChilled(p_castersMagicFactor);
			case EMonsterBuffType.FROSTBITE:
				return new MonsterBuffFrostBite(p_castersMagicFactor);
			case EMonsterBuffType.MANASURGE:
				return new MonsterBuffManaSurge(p_castersMagicFactor);
			case EMonsterBuffType.MEMORYGAP:
				return new MonsterBuffMemoryGap(p_castersMagicFactor);
			case EMonsterBuffType.IMMOBILISED:
				return new MonsterBuffImmobilised(p_castersMagicFactor);
			case EMonsterBuffType.POISONED:
				return new MonsterBuffPoisoned(p_castersMagicFactor);
			case EMonsterBuffType.SLEEPING:
				return new MonsterBuffSleeping(p_castersMagicFactor);
			case EMonsterBuffType.ACIDSPLASH:
				return new MonsterBuffAcidSplash(p_castersMagicFactor);
			case EMonsterBuffType.WEAKNESS:
				return new MonsterBuffWeakness(p_castersMagicFactor);
			case EMonsterBuffType.AGONY:
				return new MonsterBuffAgony(p_castersMagicFactor);
			case EMonsterBuffType.TERROR:
				return new MonsterBuffTerror(p_castersMagicFactor);
			case EMonsterBuffType.SUNDERING:
				return new MonsterBuffSundering(p_castersMagicFactor);
			case EMonsterBuffType.BATTLESPIRIT:
				return new MonsterBuffBattleSpirit(p_castersMagicFactor);
			case EMonsterBuffType.LIQUIDMEMBRANE:
				return new MonsterBuffLiquidMembrane(p_castersMagicFactor);
			case EMonsterBuffType.STONESKIN:
				return new MonsterBuffStoneSkin(p_castersMagicFactor);
			case EMonsterBuffType.MACE_STUN:
				return new MonsterBuffMaceStun(p_castersMagicFactor);
			case EMonsterBuffType.GASH:
				return new MonsterBuffGash(p_castersMagicFactor);
			case EMonsterBuffType.PROVOKE:
				return new MonsterBuffProvoke(p_castersMagicFactor);
			case EMonsterBuffType.IMPRISONED:
				return new MonsterBuffImprisoned(p_castersMagicFactor);
			case EMonsterBuffType.REGROWTH:
				return new MonsterBuffRegrowth(p_castersMagicFactor);
			case EMonsterBuffType.HARMONY:
				return new MonsterBuffHarmony(p_castersMagicFactor);
			case EMonsterBuffType.TIME_STOP:
				return new MonsterBuffTimeStop(p_castersMagicFactor);
			case EMonsterBuffType.SHADOW_CLOAK:
				return new MonsterBuffShadowCloak(p_castersMagicFactor);
			case EMonsterBuffType.CRYSTAL_SHELL:
				return new MonsterBuffCrystalShell(p_castersMagicFactor);
			case EMonsterBuffType.OSCILLATION:
				return new MonsterBuffOscillation(p_castersMagicFactor);
			case EMonsterBuffType.CRIPPLING_TRAP:
				return new MonsterBuffCripplingTrap(p_castersMagicFactor);
			case EMonsterBuffType.CELESTIAL_ARMOUR:
				return new MonsterBuffCelestialArmour(p_castersMagicFactor);
			case EMonsterBuffType.HOUR_OF_JUSTICE:
				return new MonsterBuffHourOfJustice(p_castersMagicFactor);
			case EMonsterBuffType.FLICKERING_LIGHT:
				return new MonsterBuffFlickeringLight(p_castersMagicFactor);
			default:
				throw new NotSupportedException("the given Type is not Supported");
			}
		}
	}
}
