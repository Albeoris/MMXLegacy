using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpell : Spell, IEquatable<MonsterSpell>
	{
		protected MonsterSpellStaticData m_staticData;

		protected Single m_monsterMagicPower;

		protected Single m_criticalDamage;

		protected Single m_criticalHitChance;

		protected Int32 m_castProbability;

		protected Int32 m_level = 1;

		protected Int32 m_cooldown;

		public MonsterSpell(EMonsterSpell p_type, String p_effectAnimationClip, Int32 p_castProbability)
		{
			m_staticData = StaticDataHandler.GetStaticData<MonsterSpellStaticData>(EDataType.MONSTER_SPELLS, (Int32)p_type);
			EffectAnimationClip = p_effectAnimationClip;
			m_monsterMagicPower = 1f;
			m_criticalDamage = 1f;
			m_castProbability = p_castProbability;
		}

		public override Int32 StaticID => m_staticData.StaticID;

	    public Int32 Level
		{
			get => m_level;
	        set => m_level = value;
	    }

		public override ETargetType TargetType => m_staticData.TargetType;

	    public override String NameKey => m_staticData.NameKey;

	    public override String EffectKey => m_staticData.EffectKey;

	    public EDamageType MagicSchool => m_staticData.MagicSchool;

	    public EMonsterSpell SpellType => (EMonsterSpell)m_staticData.StaticID;

	    public Single MonsterMagicPower
		{
			get => m_monsterMagicPower;
	        set => m_monsterMagicPower = value;
	    }

		public Single CriticalDamage
		{
			get => m_criticalDamage;
		    set => m_criticalDamage = value;
		}

		public Single CriticalHitChance
		{
			get => m_criticalHitChance;
		    set => m_criticalHitChance = value;
		}

		public String EffectAnimationClip { get; private set; }

		public DamageData[] BaseDamage => m_staticData.GetDamage(m_level);

	    public Int32 CastProbability
		{
			get => m_castProbability;
	        set => m_castProbability = value;
	    }

		public virtual Boolean CheckSpellPreconditions(Monster p_monster)
		{
			return true;
		}

		public virtual Attack GetAttack()
		{
			if (BaseDamage != null && BaseDamage.Length > 0 && BaseDamage[0].Type != EDamageType.HEAL)
			{
				Attack attack = new Attack(0f, m_criticalHitChance);
				for (Int32 i = 0; i < BaseDamage.Length; i++)
				{
					DamageData p_data = BaseDamage[i] * m_monsterMagicPower;
					attack.Damages.Add(Damage.Create(p_data, m_criticalDamage));
				}
				return attack;
			}
			return null;
		}

		public virtual void StartTurn()
		{
			if (m_cooldown > 0)
			{
				m_cooldown--;
			}
		}

		public virtual void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			foreach (ECondition p_condition in m_staticData.InflictedConditions)
			{
				p_target.ConditionHandler.AddCondition(p_condition);
			}
		}

		protected String GetDamageAsString()
		{
			Int32 minimum = BaseDamage[0].Minimum;
			Int32 maximum = BaseDamage[0].Maximum;
			if (minimum == maximum)
			{
				return minimum.ToString();
			}
			return minimum + " - " + maximum;
		}

		public virtual String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			return Localization.Instance.GetText(m_staticData.NameKey + "_INFO", m_descriptionValues);
		}

		public Boolean Equals(MonsterSpell other)
		{
			return other != null && other.SpellType == SpellType;
		}
	}
}
