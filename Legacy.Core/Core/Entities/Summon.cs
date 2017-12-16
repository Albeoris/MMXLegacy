using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.AI;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.StaticData;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities
{
	public class Summon : MovingEntity
	{
		private SummonStaticData m_staticData;

		private AIBrain m_brain;

		private Int32 m_casterId;

		private List<LogEntryEventArgs> m_logEntries;

		protected TriggerHelper m_viewIsDone = new TriggerHelper();

		public Summon(Int32 p_staticID, Int32 p_spawnID) : base(p_staticID, EObjectType.SUMMON, p_spawnID)
		{
			m_logEntries = new List<LogEntryEventArgs>();
			LifeTime = m_staticData.AILifetime;
			DestroyOnTargetContact = m_staticData.DestroyOnTargetContact;
			m_brain = new AIBrain(this, m_staticData.GetAIData());
		}

		public SummonStaticData StaticData => m_staticData;

	    public override ESize Size => ESize.NONE;

	    public Int32 CasterId
		{
			get => m_casterId;
	        set => m_casterId = value;
	    }

		public AIBrain Brain => m_brain;

	    public override String Prefab
		{
			get => StaticData.Prefab;
	        set => throw new NotSupportedException();
	    }

		public Int32 LifeTime { get; set; }

		public Boolean DestroyOnTargetContact { get; private set; }

		public TriggerHelper ViewIsDone => m_viewIsDone;

	    public void SetMagicFactor(Single p_magicFactor)
		{
			m_brain.MagicFactor = p_magicFactor;
		}

		public void SetDamageType(EDamageType p_damageType)
		{
			m_brain.DamageType = p_damageType;
		}

		public void SetIgnoreResistance(Int32 p_ignore)
		{
			m_brain.IgnoreResistance = p_ignore;
		}

		public override void BeginTurn()
		{
			base.BeginTurn();
			m_brain.BeginTurn();
		}

		public override void UpdateTurn()
		{
			m_brain.UpdateTurn();
		}

		public override void EndTurn()
		{
			base.EndTurn();
			m_brain.EndTurn();
		}

		public override Boolean CanPassTerrain(ETerrainType p_type)
		{
			return (p_type & ETerrainType.FLY_THROUGH) != ETerrainType.NONE || (p_type & ETerrainType.SHOOT_THROUGH) != ETerrainType.NONE || ((p_type & ETerrainType.BLOCKED) == ETerrainType.NONE && (p_type & ETerrainType.HAZARDOUS) == ETerrainType.NONE);
		}

		public override void Destroy()
		{
			base.Destroy();
			LegacyLogic.Instance.WorldManager.DestroyObject(this, Position);
		}

		public override void ApplyDamages(AttackResult p_result, Object p_source)
		{
		}

		public void FeedActionLog(LogEntryEventArgs p_args)
		{
			if (p_args != null)
			{
				m_logEntries.Add(p_args);
			}
		}

		public void FeedActionLog(SpellEventArgs p_args)
		{
			if (p_args == null)
			{
				return;
			}
			CastSpellEntryEventArgs item = new CastSpellEntryEventArgs(this, p_args);
			m_logEntries.Add(item);
		}

		public void FlushActionLog()
		{
			for (Int32 i = 0; i < m_logEntries.Count; i++)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(m_logEntries[i]);
			}
			m_logEntries.Clear();
		}

		protected override void LoadStaticData()
		{
			m_staticData = StaticDataHandler.GetStaticData<SummonStaticData>(EDataType.SUMMONS, StaticID);
			if (m_staticData == null)
			{
				throw new Exception("SummonStaticData ID=" + StaticID + " not defined!");
			}
		}
	}
}
