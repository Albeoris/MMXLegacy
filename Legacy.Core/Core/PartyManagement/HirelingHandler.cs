using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.SaveGameManagement;
using Legacy.Utilities;

namespace Legacy.Core.PartyManagement
{
	public class HirelingHandler : ISaveGameObject
	{
		private const Int32 HIRELING_COUNT = 2;

		private readonly Npc[] m_hirelings;

		public HirelingHandler()
		{
			m_hirelings = new Npc[2];
		}

		public Single SharePricePercent
		{
			get
			{
				Single num = (m_hirelings[0] == null) ? 0f : (m_hirelings[0].GetSharePricePercent() * 0.01f);
				num += ((m_hirelings[1] == null) ? 0f : (m_hirelings[1].GetSharePricePercent() * 0.01f));
				return (Single)Math.Round(num, 2, MidpointRounding.AwayFromZero);
			}
		}

		public Npc[] Hirelings => m_hirelings;

	    public Int32 NumHirelingsHired()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null)
				{
					num++;
				}
			}
			return num;
		}

		public Boolean Hire(Npc p_npc)
		{
			if (HirelingHired(p_npc))
			{
				return false;
			}
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] == null)
				{
					m_hirelings[i] = p_npc;
					HirelingEventArgs p_eventArgs = new HirelingEventArgs(ETargetCondition.HIRE, p_npc, i);
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.HIRE_HIRELING);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_HIRELING_UPDATED, p_eventArgs);
					return true;
				}
			}
			return false;
		}

		public Boolean Fire(Npc p_npc)
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] == p_npc)
				{
					m_hirelings[i] = null;
					HirelingEventArgs p_eventArgs = new HirelingEventArgs(ETargetCondition.FIRE, p_npc, i);
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.FIRE_HIRELING);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_HIRELING_UPDATED, p_eventArgs);
					return true;
				}
			}
			return false;
		}

		public Boolean Fire(Int32 p_npcID)
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null && m_hirelings[i].StaticID == p_npcID)
				{
					return Fire(m_hirelings[i]);
				}
			}
			return false;
		}

		public Boolean FireByIndex(Int32 p_idx)
		{
			return m_hirelings[p_idx] != null && Fire(m_hirelings[p_idx]);
		}

		public Boolean HirelingHired(Int32 p_npcId)
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null && m_hirelings[i].StaticID == p_npcId)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean HirelingHired(Npc p_npc)
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null && m_hirelings[i] == p_npc)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean HasFreeSlot()
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] == null)
				{
					return true;
				}
			}
			return false;
		}

		public void ResetHirelingTurn()
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null)
				{
					m_hirelings[i].ResetTurnCount();
				}
			}
		}

		public Boolean AllowedPeriodicity(Npc p_npc, ETargetCondition p_targetEffect)
		{
			if (p_npc == null)
			{
				return false;
			}
			NpcEffect npcEffect = p_npc.GetNpcEffect(p_targetEffect);
			if (npcEffect.TargetEffect == ETargetCondition.NONE)
			{
				LegacyLogger.Log("This target cant have periodicity");
				return false;
			}
			switch (npcEffect.EffectPeriodicity)
			{
			case EEffectPeriodicity.PERMANENT:
				return true;
			case EEffectPeriodicity.ON_DEMAND:
				return true;
			case EEffectPeriodicity.ONCE_A_REST:
				return p_npc.GetRestCount(p_targetEffect) < LegacyLogic.Instance.WorldManager.Party.RestCount;
			case EEffectPeriodicity.ONCE_A_DAY:
			{
				Int32 dayCount = p_npc.GetDayCount(p_targetEffect);
				Int32 num = LegacyLogic.Instance.GameTime.Time.Days;
				if (LegacyLogic.Instance.GameTime.Time.Hours < ConfigManager.Instance.Game.DayStartHours)
				{
					num--;
				}
				return dayCount < num;
			}
			case EEffectPeriodicity.ONCE_A_TURN:
				return p_npc.GetTurnCount(p_targetEffect) < (Int32)npcEffect.EffectValue;
			default:
				return false;
			}
		}

		public Boolean HasEffect(ETargetCondition p_targetCondition)
		{
			NpcEffect npcEffect;
			return HasEffect(p_targetCondition, out npcEffect);
		}

		public Boolean HasEffect(ETargetCondition p_targetCondition, out NpcEffect p_npcEffect)
		{
			Npc npc;
			return HasEffect(p_targetCondition, out p_npcEffect, out npc);
		}

		public Boolean HasEffect(ETargetCondition p_targetCondition, out NpcEffect p_npcEffect, out Npc p_npc)
		{
			NpcEffect npcEffect = new NpcEffect(ETargetCondition.NONE, EEffectPeriodicity.PERMANENT, 0f, Int32.MaxValue);
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null)
				{
					npcEffect = m_hirelings[i].GetNpcEffect(p_targetCondition);
					if (npcEffect.TargetEffect != ETargetCondition.NONE)
					{
						p_npcEffect = npcEffect;
						p_npc = m_hirelings[i];
						return true;
					}
				}
			}
			p_npcEffect = npcEffect;
			p_npc = null;
			return false;
		}

		public void Cure()
		{
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			ECondition[] array = (ECondition[])Enum.GetValues(typeof(ECondition));
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i] != null)
				{
					for (Int32 j = 0; j < array.Length; j++)
					{
						if (array[j] != ECondition.DEAD)
						{
							members[i].ConditionHandler.RemoveCondition(array[j]);
						}
					}
				}
			}
		}

		public void Revive()
		{
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i] != null && members[i].ConditionHandler.HasCondition(ECondition.DEAD))
				{
					members[i].ConditionHandler.RemoveCondition(ECondition.DEAD);
					members[i].ChangeHP(-(members[i].HealthPoints - 1));
					members[i].ConditionHandler.AddCondition(ECondition.WEAK);
				}
			}
		}

		public void Restore()
		{
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i] != null && !members[i].ConditionHandler.HasCondition(ECondition.DEAD))
				{
					members[i].ChangeHP(-(members[i].HealthPoints - members[i].MaximumHealthPoints));
					members[i].ChangeMP(-(members[i].ManaPoints - members[i].MaximumManaPoints));
				}
			}
		}

		public void DefBuff()
		{
			PartyBuffHandler buffs = LegacyLogic.Instance.WorldManager.Party.Buffs;
			buffs.AddBuff(EPartyBuffs.ARCANE_WARD, 1f);
			buffs.AddBuff(EPartyBuffs.BURNING_DETERMINATION, 1f);
			buffs.AddBuff(EPartyBuffs.WIND_SHIELD, 1f);
			buffs.AddBuff(EPartyBuffs.STONE_SKIN, 1f);
			buffs.AddBuff(EPartyBuffs.HOUR_OF_POWER, 1f);
		}

		public void Identify()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.Inventory.HasUnidentifiedItems() || party.MuleInventory.HasUnidentifiedItems())
			{
				party.Inventory.IdentifyAllItems();
				party.MuleInventory.IdentifyAllItems();
			}
		}

		public Boolean FreeRest()
		{
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null)
				{
					if (m_hirelings[i].GetNpcEffect(ETargetCondition.HIRE_REST).TargetEffect != ETargetCondition.NONE)
					{
						if (AllowedPeriodicity(m_hirelings[i], ETargetCondition.HIRE_REST))
						{
							m_hirelings[i].SetDayBreakCountStrict(ETargetCondition.HIRE_REST, LegacyLogic.Instance.GameTime.Time.Days);
							return true;
						}
					}
				}
			}
			return false;
		}

		public void Load(SaveGameData p_data)
		{
			Int32 num = p_data.Get<Int32>("HirelingCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				Int32 num2 = p_data.Get<Int32>("Hireling" + i, -1);
				if (num2 != -1)
				{
					if (num2 == 203)
					{
						num2 = 210;
					}
					m_hirelings[i] = LegacyLogic.Instance.WorldManager.NpcFactory.Get(num2);
					HirelingEventArgs p_eventArgs = new HirelingEventArgs(ETargetCondition.HIRE, m_hirelings[i], i);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_HIRELING_UPDATED, p_eventArgs);
				}
				else
				{
					m_hirelings[i] = null;
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("HirelingCount", 2);
			for (Int32 i = 0; i < 2; i++)
			{
				if (m_hirelings[i] != null)
				{
					p_data.Set<Int32>("Hireling" + i, m_hirelings[i].StaticID);
				}
				else
				{
					p_data.Set<Int32>("Hireling" + i, -1);
				}
			}
		}
	}
}
