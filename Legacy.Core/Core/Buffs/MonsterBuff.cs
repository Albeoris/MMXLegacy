using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Buffs
{
	public class MonsterBuff : BuffBase, IEquatable<MonsterBuff>
	{
		public const Int32 PERMANENT_BUFF = -1;

		protected MonsterBuffStaticData m_staticData;

		protected Single m_castersMagicFactor = 1f;

		protected Character m_causer;

		protected Int32 m_duration;

		protected Boolean m_dontTriggerOnFirstDamage;

		protected Int32 m_level = 1;

		protected AttackResult m_delayedDamage;

		public MonsterBuff(Int32 p_staticID, Single p_castersMagicFactor)
		{
			m_staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, p_staticID);
			m_castersMagicFactor = p_castersMagicFactor;
			ResetDuration();
		}

		public Boolean Stackable => m_staticData.Stackable;

	    public Character Causer
		{
			get => m_causer;
	        set => m_causer = value;
	    }

		public Int32 Level
		{
			get => m_level;
		    set => m_level = value;
		}

		public Single[] BuffValues => m_staticData.GetBuffValues(m_level);

	    public EMonsterBuffType Type => (EMonsterBuffType)m_staticData.StaticID;

	    public Single CastersMagicValue => m_castersMagicFactor;

	    public Int32 StaticID => m_staticData.StaticID;

	    public String NameKey => m_staticData.NameKey;

	    public Boolean IsDebuff => m_staticData.IsDebuff;

	    public String Icon => m_staticData.Icon;

	    public String Gfx => m_staticData.Gfx;

	    public Boolean DontTriggerOnFirstDamage
		{
			get => m_dontTriggerOnFirstDamage;
	        set => m_dontTriggerOnFirstDamage = value;
	    }

		public Int32 DurationMaxValue => (m_staticData.GetDuration(m_level) != -1) ? m_staticData.GetDuration(m_level) : -1;

	    public Int32 Duration
		{
			get => (m_duration != -1) ? m_duration : -1;
	        set
			{
				if (m_duration != -1)
				{
					m_duration = value;
				}
			}
		}

		public Boolean IsExpired => m_duration != -1 && m_duration <= 0;

	    public Boolean IsPermanent => m_staticData.GetDuration(m_level) == -1;

	    public AttackResult DelayedDamage => m_delayedDamage;

	    public virtual void DoEffect(Monster p_monster)
		{
		}

		public virtual void DoImmediateEffect(Monster p_monster)
		{
		}

		public virtual void DoEndOfTurnEffect(Monster p_monster)
		{
		}

		public virtual void DoOnCastSpellEffect(Monster p_monster)
		{
		}

		public virtual void DoOnGetDamageEffect(Monster p_monster)
		{
		}

		public virtual void DoOnGetDamageEffect(Monster p_monster, List<DamageResult> p_results)
		{
		}

		public virtual void Stack(MonsterBuff p_buff, Monster p_monster)
		{
		}

		public virtual void ManipulateAttack(Attack p_attack, Monster p_monster)
		{
		}

		public virtual Single GetBuffValue(Int32 p_valueIndex)
		{
			Single[] buffValues = m_staticData.GetBuffValues(m_level);
			if (p_valueIndex < 0 || p_valueIndex >= buffValues.Length)
			{
				return 0f;
			}
			Single num = buffValues[p_valueIndex];
			if (num <= 1f)
			{
				return num * m_castersMagicFactor;
			}
			return (Single)Math.Round(num * m_castersMagicFactor, MidpointRounding.AwayFromZero);
		}

		public virtual String GetBuffValueForTooltip(Int32 p_valueIndex)
		{
			return GetBuffValue(p_valueIndex).ToString();
		}

		public void DecreaseDuration()
		{
			if (m_duration > 0)
			{
				m_duration--;
			}
		}

		public void ResetDuration()
		{
			m_duration = m_staticData.GetDuration(m_level);
			if (m_duration == 0)
			{
				m_duration = 1;
			}
		}

		public void ExpireBuff()
		{
			m_duration = 0;
		}

		public void ScaleDuration(Single p_scaleFactor)
		{
			m_duration = (Int32)(p_scaleFactor * m_duration + 0.5f);
		}

		public Boolean Equals(MonsterBuff other)
		{
			return other.Type == Type;
		}
	}
}
