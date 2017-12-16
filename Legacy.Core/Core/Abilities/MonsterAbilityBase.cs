using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Abilities
{
	public abstract class MonsterAbilityBase
	{
		protected MonsterAbilityStaticData m_staticData;

		protected EExecutionPhase m_executionPhase;

		protected EExecutionPhase m_currentExecutionPhase;

		protected Int32 m_level = 1;

		private EMonsterAbilityType m_type;

		private Single m_triggerChance = 1f;

		public MonsterAbilityBase(EMonsterAbilityType p_type)
		{
			m_staticData = StaticDataHandler.GetStaticData<MonsterAbilityStaticData>(EDataType.MONSTER_ABILITIES, (Int32)p_type);
			m_type = p_type;
		}

		public Int32 Level
		{
			get => m_level;
		    set => m_level = value;
		}

		public EExecutionPhase ExecutionPhase => m_executionPhase;

	    public EExecutionPhase CurrentExecutionPhase
		{
			get => m_currentExecutionPhase;
	        set => m_currentExecutionPhase = value;
	    }

		public MonsterAbilityStaticData StaticData => m_staticData;

	    public EMonsterAbilityType AbilityType => m_type;

	    public Single TriggerChance
		{
			get => m_triggerChance;
	        set => m_triggerChance = value;
	    }

		public virtual Boolean IsForbiddenBuff(Int32 p_buffID, Monster p_monster, Boolean p_silent)
		{
			return false;
		}

		public virtual void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
		}

		public virtual void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
		}

		public virtual void ResetAbilityValues()
		{
		}

		public virtual String GetDescription()
		{
			return Localization.Instance.GetText(m_staticData.NameKey + "_INFO", m_staticData.GetValues(m_level)[0]);
		}
	}
}
