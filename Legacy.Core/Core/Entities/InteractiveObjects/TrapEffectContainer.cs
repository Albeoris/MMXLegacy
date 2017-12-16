using System;
using Legacy.Core.Entities.TrapEffects;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class TrapEffectContainer : InteractiveObject
	{
		private Int32 m_trapEffectID;

		private BaseTrapEffect m_trapEffect;

		private Boolean m_isTrapActive;

		public TrapEffectContainer() : this(0, 0)
		{
		}

		public TrapEffectContainer(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.TRAP_EFFECT_CONTAINER, p_spawnerID)
		{
		}

		public Int32 TrapEffectID
		{
			get => m_trapEffectID;
		    set => m_trapEffectID = value;
		}

		public ETrapRefresh TrapRefresh => m_trapEffect.StaticData.Refresh;

	    public void Resolve(Party p_party)
		{
			m_trapEffect.ResolveEffect(p_party);
			m_isTrapActive = (m_trapEffect.StaticData.Refresh == ETrapRefresh.AFTER_USAGE);
		}

		public Boolean IsTrapActive()
		{
			return m_isTrapActive;
		}

		public void SetTrapActive(Boolean p_value)
		{
			m_isTrapActive = p_value;
		}

		public void NotifyLevelLoaded()
		{
			if (m_trapEffect.StaticData.Refresh == ETrapRefresh.LEVEL_ENTER)
			{
				m_isTrapActive = true;
			}
		}

		public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.TRAP_EFFECT_DATA)
			{
				TrapEffectID = Convert.ToInt32(p_value);
				m_trapEffect = TrapEffectFactory.CreateTrapEffect(TrapEffectID, this);
				m_isTrapActive = true;
			}
			else
			{
				base.SetData(p_key, p_value);
			}
		}

		public override void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("TrapEffectID", m_trapEffectID);
			p_data.Set<Boolean>("IsTrapActive", m_isTrapActive);
			base.Save(p_data);
		}

		public override void Load(SaveGameData p_data)
		{
			m_trapEffectID = p_data.Get<Int32>("TrapEffectID", 0);
			m_isTrapActive = p_data.Get<Boolean>("IsTrapActive", false);
			if (m_trapEffectID > 0)
			{
				m_trapEffect = TrapEffectFactory.CreateTrapEffect(TrapEffectID, this);
			}
			base.Load(p_data);
		}
	}
}
