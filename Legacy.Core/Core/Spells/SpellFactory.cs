using System;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.Spells.CharacterSpells.Air;
using Legacy.Core.Spells.CharacterSpells.Dark;
using Legacy.Core.Spells.CharacterSpells.Earth;
using Legacy.Core.Spells.CharacterSpells.Fire;
using Legacy.Core.Spells.CharacterSpells.Light;
using Legacy.Core.Spells.CharacterSpells.Prime;
using Legacy.Core.Spells.CharacterSpells.Warfare;
using Legacy.Core.Spells.CharacterSpells.Water;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Spells
{
	public static class SpellFactory
	{
		public static MonsterSpell CreateMonsterSpell(EMonsterSpell p_spellType, String p_effectAnimationClip, Int32 p_castProbability)
		{
			switch (p_spellType)
			{
			case EMonsterSpell.FIREBALL:
				return new MonsterSpellFireBall(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.FIREBOLT:
				return new MonsterSpellFireBolt(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.BATTLE_SPIRIT:
				return new MonsterSpellBattleSpirit(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.ICE_SHARDS:
				return new MonsterSpellIceShards(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.DEEP_FREEZE:
				return new MonsterSpellDeepFreeze(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.LIQUID_MEMBRANE:
				return new MonsterSpellLiquidMembrane(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.ICE_STORM:
				return new MonsterSpellIceStorm(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CHAIN_LIGHTNING:
				return new MonsterSpellChainLightning(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.LIGHTNING_BOLT:
				return new MonsterSpellLightningBolt(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.PETRIFACTION:
				return new MonsterSpellPetrification(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.POISON_SPIT:
				return new MonsterSpellPoisonSpit(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.REGROWTH:
				return new MonsterSpellRegrowth(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.STONE_SKIN:
				return new MonsterSpellStoneSkin(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.GLARING_LIGHT:
				return new MonsterSpellGlaringLight(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.PURIFY:
				return new MonsterSpellPurify(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CONFUSION:
				return new MonsterSpellConfusion(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.AGONIZING_PURGE:
				return new MonsterSpellAgonizingPurge(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.TORPOR:
				return new MonsterSpellTorpor(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.OVERWHELMING_ASSAULT:
				return new MonsterSpellOverwhelmingAssault(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.DARK_NOVA:
				return new MonsterSpellDarkNova(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.DARK_RESTORATION:
				return new MonsterSpellDarkRestoration(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.WITHERING_BREATH:
				return new MonsterSpellWitheringBreath(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CRYSTAL_SHARDS:
				return new MonsterSpellCrystalShards(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CRYSTAL_SHELL:
				return new MonsterSpellCrystalShell(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CRYSTAL_CAGE:
				return new MonsterSpellCrystalCage(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.EXPLOSIVE_CRYSTAL_CAGE:
				return new MonsterSpellExplosiveCrystalCage(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.BLINK:
				return new MonsterSpellBlink(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.BACKSTAB:
				return new MonsterSpellBackstab(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.SWOOP:
				return new MonsterSpellSwoop(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.SHADOW_BOLT:
				return new MonsterSpellShadowBolt(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CRUSH:
				return new MonsterSpellCrush(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.CELESTIAL_ARMOUR:
				return new MonsterSpellCelestialArmour(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.HOUR_OF_JUSTICE:
				return new MonsterSpellHourOfJustice(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.SUN_RAY:
				return new MonsterSpellSunRay(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.MASS_HEAL:
				return new MonsterSpellMassHeal(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.FLICKERING_LIGHT:
				return new MonsterSpellFlickeringLight(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.FLICKER:
				return new MonsterSpellFlicker(p_effectAnimationClip, p_castProbability);
			case EMonsterSpell.PACIFICATION:
				return new MonsterSpellPacification(p_effectAnimationClip, p_castProbability);
			default:
				throw new NotImplementedException("This Monster Spell is not implemented " + p_spellType);
			}
		}

		public static CharacterSpell CreateCharacterSpell(ECharacterSpell p_spellType)
		{
			switch (p_spellType)
			{
			case ECharacterSpell.SPELL_FIRE_WARD:
				return new CharacterSpellFireWard();
			case ECharacterSpell.SPELL_FIRE_TORCHLIGHT:
				return new CharacterSpellTorchlight();
			case ECharacterSpell.SPELL_FIRE_BOLT:
				return new CharacterSpellFireBolt();
			case ECharacterSpell.SPELL_FIRE_DANGER_SENSE:
				return new CharacterSpellDangerSense();
			case ECharacterSpell.SPELL_FIRE_BURNING_DETERMINATION:
				return new CharacterSpellBurningDetermination();
			case ECharacterSpell.SPELL_FIRE_INNER_FIRE:
				return new CharacterSpellInnerFire();
			case ECharacterSpell.SPELL_FIRE_FIREBALL:
				return new CharacterSpellFireBall();
			case ECharacterSpell.SPELL_FIRE_FIRE_SHIELD:
				return new CharacterSpellFireShield();
			case ECharacterSpell.SPELL_FIRE_BLAST:
				return new CharacterSpellFireBlast();
			case ECharacterSpell.SPELL_FIRE_STORM:
				return new CharacterSpellFireStorm();
			case ECharacterSpell.SPELL_EARTH_WARD:
				return new CharacterSpellEarthWard();
			case ECharacterSpell.SPELL_EARTH_ENTANGLE:
				return new CharacterSpellEntangle();
			case ECharacterSpell.SPELL_EARTH_REGENERATION:
				return new CharacterSpellRegeneration();
			case ECharacterSpell.SPELL_EARTH_CURE_POISON:
				return new CharacterSpellCurePoison();
			case ECharacterSpell.SPELL_EARTH_POISON_SPRAY:
				return new CharacterSpellPoisonSpray();
			case ECharacterSpell.SPELL_EARTH_STONE_SKIN:
				return new CharacterSpellStoneSkin();
			case ECharacterSpell.SPELL_EARTH_STRENGTH_OF_THE_EARTH:
				return new CharacterSpellStrengthOfTheEarth();
			case ECharacterSpell.SPELL_EARTH_POISON_CLOUD:
				return new SummonSpell(ECharacterSpell.SPELL_EARTH_POISON_CLOUD);
			case ECharacterSpell.SPELL_EARTH_ACID_SPLASH:
				return new CharacterSpellAcidSplash();
			case ECharacterSpell.SPELL_EARTH_CRUSHING_WEIGHT:
				return new CharacterSpellCrushingWeight();
			case ECharacterSpell.SPELL_AIR_WARD:
				return new CharacterSpellAirWard();
			case ECharacterSpell.SPELL_AIR_SPARKS:
				return new CharacterSpellSparks();
			case ECharacterSpell.SPELL_AIR_WIND_SHIELD:
				return new CharacterSpellWindShield();
			case ECharacterSpell.SPELL_AIR_GUST_OF_WIND:
				return new CharacterSpellGustOfWind();
			case ECharacterSpell.SPELL_AIR_EAGLE_EYE:
				return new CharacterSpellEagleEye();
			case ECharacterSpell.SPELL_AIR_LIGHTNING_BOLT:
				return new CharacterSpellLightningBolt();
			case ECharacterSpell.SPELL_AIR_CLEAR_MIND:
				return new CharacterSpellClearMind();
			case ECharacterSpell.SPELL_AIR_CHAIN_LIGHTNING:
				return new CharacterSpellChainLightning();
			case ECharacterSpell.SPELL_AIR_CYCLONE:
				return new SummonSpell(ECharacterSpell.SPELL_AIR_CYCLONE);
			case ECharacterSpell.SPELL_AIR_THUNDER_STORM:
				return new SummonSpell(ECharacterSpell.SPELL_AIR_THUNDER_STORM);
			case ECharacterSpell.SPELL_WATER_WARD:
				return new CharacterSpellWaterWard();
			case ECharacterSpell.SPELL_WATER_CONSCIOUSNESS:
				return new CharacterSpellConsciousness();
			case ECharacterSpell.SPELL_WATER_FROZEN_GROUND:
				return new CharacterSpellFrozenGround();
			case ECharacterSpell.SPELL_WATER_ICE_BOLT:
				return new CharacterSpellIceBolt();
			case ECharacterSpell.SPELL_WATER_ICE_PRISON:
				return new CharacterSpellIcePrison();
			case ECharacterSpell.SPELL_WATER_ICE_RING:
				return new CharacterSpellIceRing();
			case ECharacterSpell.SPELL_WATER_FLOWS_FREELY:
				return new CharacterSpellWaterFlowsFreely();
			case ECharacterSpell.SPELL_WATER_BLIZZARD:
				return new CharacterSpellBlizzard();
			case ECharacterSpell.SPELL_WATER_LIQUID_MEMBRANE:
				return new CharacterSpellLiquidMembrane();
			case ECharacterSpell.SPELL_WATER_TSUNAMI:
				return new CharacterSpellTsunami();
			case ECharacterSpell.SPELL_LIGHT_WARD:
				return new CharacterSpellLightWard();
			case ECharacterSpell.SPELL_LIGHT_ORB:
				return new CharacterSpellLightOrb();
			case ECharacterSpell.SPELL_LIGHT_CELESTIAL_ARMOR:
				return new CharacterSpellCelestialArmor();
			case ECharacterSpell.SPELL_LIGHT_CLAIRVOYANCE:
				return new CharacterSpellClairvoyance();
			case ECharacterSpell.SPELL_LIGHT_CLEANSING_LIGHT:
				return new CharacterSpellCleansingLight();
			case ECharacterSpell.SPELL_LIGHT_HEAL:
				return new CharacterSpellHeal();
			case ECharacterSpell.SPELL_LIGHT_RADIANT_WEAPON:
				return new CharacterSpellRadiantWeapon();
			case ECharacterSpell.SPELL_LIGHT_RESURRECTION:
				return new CharacterSpellResurrection();
			case ECharacterSpell.SPELL_LIGHT_HEAL_PARTY:
				return new CharacterSpellHealParty();
			case ECharacterSpell.SPELL_LIGHT_WORD_OF_LIGHT:
				return new CharacterSpellWordOfLight();
			case ECharacterSpell.SPELL_DARK_WARD:
				return new CharacterSpellDarknessWard();
			case ECharacterSpell.SPELL_DARK_VISION:
				return new CharacterSpellDarkVision();
			case ECharacterSpell.SPELL_DARK_SHADOW_CLOAK:
				return new CharacterSpellShadowCloak();
			case ECharacterSpell.SPELL_DARK_WHISPERING_SHADOWS:
				return new CharacterSpellWhisperingShadows();
			case ECharacterSpell.SPELL_DARK_PURGE:
				return new CharacterSpellPurge();
			case ECharacterSpell.SPELL_DARK_DRAIN_LIFE:
				return new CharacterSpellDrainLife();
			case ECharacterSpell.SPELL_DARK_SLEEP:
				return new CharacterSpellSleep();
			case ECharacterSpell.SPELL_DARK_TERROR:
				return new CharacterSpellTerror();
			case ECharacterSpell.SPELL_DARK_AGONY:
				return new CharacterSpellAgony();
			case ECharacterSpell.SPELL_DARK_WEAKNESS:
				return new CharacterSpellWeakness();
			case ECharacterSpell.SPELL_PRIME_ARCANE_WARD:
				return new CharacterSpellArcaneWard();
			case ECharacterSpell.SPELL_PRIME_SUNDERING:
				return new CharacterSpellSundering();
			case ECharacterSpell.SPELL_PRIME_TIME_STASIS:
				return new CharacterSpellTimeStasis();
			case ECharacterSpell.SPELL_PRIME_MANA_SURGE:
				return new CharacterSpellManaSurge();
			case ECharacterSpell.SPELL_PRIME_DISPEL_MAGIC:
				return new CharacterSpellDispelMagic();
			case ECharacterSpell.SPELL_PRIME_HEROIC_DESTINY:
				return new CharacterSpellHeroicDestiny();
			case ECharacterSpell.SPELL_PRIME_IDENTIFY:
				return new CharacterSpellIdentify();
			case ECharacterSpell.SPELL_PRIME_IMPLOSION:
				return new CharacterSpellImplosion();
			case ECharacterSpell.SPELL_PRIME_SPIRIT_BEACON:
				return new CharacterSpellSpiritBeacon();
			case ECharacterSpell.SPELL_PRIME_HOUR_OF_POWER:
				return new CharacterSpellHourOfPower();
			case ECharacterSpell.WARFARE_CHALLENGE:
				return new CharacterWarfareChallenge();
			case ECharacterSpell.WARFARE_SKULL_CRACK:
				return new CharacterWarfareSkullCrack();
			case ECharacterSpell.WARFARE_UNSTOPPABLE_ASSAULT:
				return new CharacterWarfareUnstoppableAssault();
			case ECharacterSpell.WARFARE_INTERCEPT:
				return new CharacterWarfareIntercept();
			case ECharacterSpell.WARFARE_FURIOUS_BLOW:
				return new CharacterWarfareFuriousBlow();
			case ECharacterSpell.WARFARE_TAUNT:
				return new CharacterWarfareTaunt();
			case ECharacterSpell.WARFARE_FLAWLESS_ASSAULT:
				return new CharacterWarfareFlawlessAssault();
			case ECharacterSpell.SPELL_LIGHT_LAY_ON_HANDS:
				return new CharacterSpellLayOnHands();
			case ECharacterSpell.SPELL_PRIME_TIME_STOP:
				return new SummonSpell(ECharacterSpell.SPELL_PRIME_TIME_STOP);
			case ECharacterSpell.SPELL_EARTH_HARMONY:
				return new CharacterSpellHarmony();
			case ECharacterSpell.SPELL_FIRE_SEARING_RUNE:
				return new SummonSpell(ECharacterSpell.SPELL_FIRE_SEARING_RUNE);
			case ECharacterSpell.WARFARE_POINT_BLANK_SHOT:
				return new CharacterWarfarePointBlankShot();
			case ECharacterSpell.WARFARE_SNARING_SHOT:
				return new CharacterWarfareSnaringShot();
			case ECharacterSpell.WARFARE_CRIPPLING_TRAP:
				return new SummonSpell(ECharacterSpell.WARFARE_CRIPPLING_TRAP);
			case ECharacterSpell.SPELL_MANDATE_OF_HEAVEN:
				return new CharacterSpellMandateOfHeaven();
			case ECharacterSpell.SPELL_EARTH_NURTURE:
				return new CharacterSpellNurture();
			case ECharacterSpell.WARFARE_SNATCH:
				return new CharacterWarfareSnatch();
			case ECharacterSpell.WARFARE_DASH:
				return new CharacterWarfareDash();
			case ECharacterSpell.WARFARE_CARNAGE:
				return new CharacterWarfareCarnage();
			case ECharacterSpell.WARFARE_SHATTER:
				return new CharacterWarfareShatter();
			default:
				throw new NotImplementedException("This Character Spell is not implemented " + p_spellType);
			}
		}
	}
}
