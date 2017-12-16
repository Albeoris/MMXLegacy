using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction
{
	public class Npc : BaseObject, ISaveGameObject
	{
		private NpcStaticData m_staticData;

		private NpcConversation m_conversation;

		private Dictionary<ETargetCondition, Int32> m_restCount;

		private Dictionary<ETargetCondition, Int32> m_dayCount;

		private Dictionary<ETargetCondition, Int32> m_perTurnCount;

		private TradingInventoryController m_tradingInventory;

		private TradingSpellController m_tradingSpells;

		private IdentifyInventoryController m_identifyController;

		private RepairInventoryController m_repairController;

		public Npc(Int32 p_staticID) : this(p_staticID, 0)
		{
		}

		public Npc(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.NPC, p_spawnerID)
		{
			m_restCount = new Dictionary<ETargetCondition, Int32>();
			m_dayCount = new Dictionary<ETargetCondition, Int32>();
			m_perTurnCount = new Dictionary<ETargetCondition, Int32>();
		}

		public NpcStaticData StaticData => m_staticData;

	    public NpcConversation ConversationData => m_conversation;

	    public TradingInventoryController TradingInventory => m_tradingInventory;

	    public TradingSpellController TradingSpells => m_tradingSpells;

	    public IdentifyInventoryController IdentifyController => m_identifyController;

	    public RepairInventoryController RepairController => m_repairController;

	    public Boolean IsTravelStation => m_staticData.TravelStationID != 0;

	    public Boolean IsEnabled => !LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(this);

	    public Boolean CanBeFired
		{
			get
			{
				if (GetNpcEffect(ETargetCondition.HIRE_MULE).TargetEffect != ETargetCondition.NONE)
				{
					return LegacyLogic.Instance.WorldManager.Party.MuleInventory.GetCurrentItemCount() == 0;
				}
				return m_staticData.CanBeFired;
			}
		}

		protected override void LoadStaticData()
		{
			m_staticData = StaticDataHandler.GetStaticData<NpcStaticData>(EDataType.NPC, StaticID);
			if (m_staticData == null)
			{
				LegacyLogger.Log("npc staticdata id " + StaticID + " not found");
			}
			try
			{
				NpcConversationStaticData staticData = XmlStaticDataHandler<NpcConversationStaticData>.GetStaticData(StaticData.ConversationKey);
				if (staticData == null)
				{
					LegacyLogger.Log(String.Concat(new Object[]
					{
						"npc staticdata id: ",
						StaticID,
						", Conversation staticdata ",
						StaticData.ConversationKey,
						" not found"
					}));
				}
				m_conversation = new NpcConversation(staticData);
				m_tradingInventory = new TradingInventoryController(staticData, this);
				m_tradingSpells = new TradingSpellController(staticData, this);
			}
			catch (Exception ex)
			{
				LegacyLogger.LogError(String.Concat(new Object[]
				{
					"Error load NPC data\nStaticId: ",
					StaticID,
					", ConversationKey: ",
					StaticData.ConversationKey,
					"\n",
					ex
				}));
			}
			m_identifyController = new IdentifyInventoryController(this);
			m_repairController = new RepairInventoryController();
		}

		public Int32 GetCosts(ETargetCondition p_conditionTarget)
		{
			switch (p_conditionTarget)
			{
			case ETargetCondition.REPAIR:
				return ConfigManager.Instance.Game.CostRepair;
			case ETargetCondition.CURE:
				return ConfigManager.Instance.Game.CostCure;
			case ETargetCondition.IDENTIFY:
				return ConfigManager.Instance.Game.CostIdentify;
			case ETargetCondition.RESURRECT:
				return ConfigManager.Instance.Game.CostResurrect;
			case ETargetCondition.RESTORATION:
				return ConfigManager.Instance.Game.CostRestoration;
			case ETargetCondition.CAST:
				return ConfigManager.Instance.Game.CostCast;
			case ETargetCondition.REST:
				return ConfigManager.Instance.Game.CostRest;
			case ETargetCondition.HIRE:
				return m_staticData.HirePrice;
			default:
				if (p_conditionTarget >= ETargetCondition.HIRE && p_conditionTarget <= ETargetCondition.FIRE)
				{
					NpcEffect npcEffect = GetNpcEffect(p_conditionTarget);
					if (npcEffect.TargetEffect != ETargetCondition.NONE)
					{
						return npcEffect.EffectPrice;
					}
				}
				return Int32.MaxValue;
			}
		}

		public NpcEffect GetNpcEffect(ETargetCondition p_conditionTarget)
		{
			for (Int32 i = 0; i < m_staticData.NpcEffects.Length; i++)
			{
				if (m_staticData.NpcEffects[i].TargetEffect == p_conditionTarget)
				{
					return m_staticData.NpcEffects[i];
				}
			}
			return new NpcEffect(ETargetCondition.NONE, EEffectPeriodicity.PERMANENT, 0f, Int32.MaxValue);
		}

		public Int32 GetDayCount(ETargetCondition p_target)
		{
			if (m_dayCount.ContainsKey(p_target))
			{
				return m_dayCount[p_target];
			}
			return -2;
		}

		public void SetDayBreakCount(ETargetCondition p_target)
		{
			SetDayBreakCount(p_target, LegacyLogic.Instance.GameTime.Time.Days);
		}

		public void SetDayBreakCount(ETargetCondition p_target, Int32 p_count)
		{
			Boolean flag = LegacyLogic.Instance.GameTime.Time.Hours > ConfigManager.Instance.Game.DayStartHours;
			if (flag)
			{
				SetDayCount(p_target, p_count);
			}
			else
			{
				SetDayCount(p_target, p_count - 1);
			}
		}

		public void SetDayBreakCountStrict(ETargetCondition p_target, Int32 p_count)
		{
			SetDayCount(p_target, p_count);
		}

		public Single GetSharePricePercent()
		{
			return m_staticData.HireShare;
		}

		public void SetDayCount(ETargetCondition p_target, Int32 p_count)
		{
			if (m_dayCount.ContainsKey(p_target))
			{
				m_dayCount[p_target] = p_count;
			}
			else
			{
				m_dayCount.Add(p_target, p_count);
			}
		}

		public Int32 GetRestCount(ETargetCondition p_target)
		{
			if (m_restCount.ContainsKey(p_target))
			{
				return m_restCount[p_target];
			}
			return -1;
		}

		public void SetRestCount(ETargetCondition p_target, Int32 p_count)
		{
			if (m_restCount.ContainsKey(p_target))
			{
				m_restCount[p_target] = p_count;
			}
			else
			{
				m_restCount.Add(p_target, p_count);
			}
		}

		public Int32 GetTurnCount(ETargetCondition p_target)
		{
			if (m_perTurnCount.ContainsKey(p_target))
			{
				return m_perTurnCount[p_target];
			}
			return 0;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_staticData = null;
			m_tradingInventory = null;
			m_tradingSpells = null;
			m_identifyController = null;
			m_repairController = null;
			m_conversation = null;
			m_restCount.Clear();
			m_restCount = null;
			m_dayCount.Clear();
			m_dayCount = null;
			m_perTurnCount.Clear();
			m_perTurnCount = null;
		}

		public Int32 GetHirePrice()
		{
			Int32 num = StaticData.HirePrice;
			if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
			{
				num = (Int32)(num * ConfigManager.Instance.Game.NpcHirelingCostsFactor + 0.5f);
			}
			return num;
		}

		public void SetTurnCount(ETargetCondition p_target, Int32 p_count)
		{
			if (m_perTurnCount.ContainsKey(p_target))
			{
				m_perTurnCount[p_target] = p_count;
			}
			else
			{
				m_perTurnCount.Add(p_target, p_count);
			}
		}

		public void ResetTurnCount()
		{
			m_perTurnCount.Clear();
		}

		public void Load(SaveGameData p_data)
		{
			if (m_tradingInventory != null)
			{
				m_tradingInventory.Load(p_data);
			}
			Int32 num = p_data.Get<Int32>("RestCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				m_restCount.Add(p_data.Get<ETargetCondition>("RestCountKey" + i, ETargetCondition.NONE), p_data.Get<Int32>("RestCountValue" + 1, 0));
			}
			m_dayCount.Clear();
			Int32 num2 = p_data.Get<Int32>("DayCount", 0);
			for (Int32 j = 0; j < num2; j++)
			{
				m_dayCount.Add(p_data.Get<ETargetCondition>("DayCountKey" + j, ETargetCondition.NONE), p_data.Get<Int32>("DayCountValue" + j, 0));
			}
		}

		public void Save(SaveGameData p_data)
		{
			if (m_tradingInventory != null)
			{
				m_tradingInventory.Save(p_data);
			}
			p_data.Set<Int32>("RestCount", m_restCount.Count);
			Int32 num = 0;
			foreach (KeyValuePair<ETargetCondition, Int32> keyValuePair in m_restCount)
			{
				p_data.Set<ETargetCondition>("RestCountKey" + num, keyValuePair.Key);
				p_data.Set<Int32>("RestCountValue" + num, keyValuePair.Value);
				num++;
			}
			p_data.Set<Int32>("DayCount", m_dayCount.Count);
			num = 0;
			foreach (KeyValuePair<ETargetCondition, Int32> keyValuePair2 in m_dayCount)
			{
				p_data.Set<ETargetCondition>("DayCountKey" + num, keyValuePair2.Key);
				p_data.Set<Int32>("DayCountValue" + num, keyValuePair2.Value);
				num++;
			}
		}
	}
}
