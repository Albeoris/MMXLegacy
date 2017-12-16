using System;
using System.Collections.Generic;
using System.Globalization;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public abstract class IndoorSceneBase : InteractiveObject
	{
		private String m_indoorScene = String.Empty;

		private List<Npc> m_npcs = new List<Npc>();

		private String m_minimapTooltip;

		public IndoorSceneBase(Int32 p_staticID, EObjectType p_objectType, Int32 p_spawnerID) : base(p_staticID, p_objectType, p_spawnerID)
		{
		}

		public String IndoorScene
		{
			get => m_indoorScene;
		    set => m_indoorScene = value;
		}

		public Boolean HasScene => !String.IsNullOrEmpty(m_indoorScene);

	    public List<Npc> Npcs => m_npcs;

	    public Boolean IsEnabled
		{
			get
			{
				for (Int32 i = 0; i < m_npcs.Count; i++)
				{
					if (m_npcs[i].IsEnabled)
					{
						return true;
					}
				}
				return false;
			}
		}

		public String MinimapTooltipLocaKey => m_minimapTooltip;

	    public ENpcMinimapSymbol MinimapSymbol
		{
			get
			{
				ENpcMinimapSymbol enpcMinimapSymbol = ENpcMinimapSymbol.NORMAL;
				for (Int32 i = 0; i < m_npcs.Count; i++)
				{
					Npc npc = m_npcs[i];
					if (npc.IsEnabled)
					{
						NpcStaticData staticData = npc.StaticData;
						if (enpcMinimapSymbol < staticData.MinimapSymbol)
						{
							enpcMinimapSymbol = staticData.MinimapSymbol;
						}
					}
				}
				return enpcMinimapSymbol;
			}
		}

		public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.INDOOR_SCENE)
			{
				m_indoorScene = p_value;
			}
			else if (p_key == EInteractiveObjectData.NPC_IDS)
			{
				String[] array = p_value.Split(new Char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				for (Int32 i = 0; i < array.Length; i++)
				{
					Int32 p_staticID = Int32.Parse(array[i].Trim(), CultureInfo.InvariantCulture.NumberFormat);
					Npc npc = LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_staticID);
					if (npc != null)
					{
						npc.TradingInventory.UpdateOffers();
						m_npcs.Add(npc);
					}
				}
			}
			else if (p_key == EInteractiveObjectData.MINIMAP_TOOLTIP)
			{
				m_minimapTooltip = p_value;
			}
			else
			{
				base.SetData(p_key, p_value);
			}
		}

		public Boolean Contains(Npc p_npc)
		{
			return m_npcs.Contains(p_npc);
		}

		public Boolean Contains(Int32 p_npcStaticID)
		{
			for (Int32 i = 0; i < m_npcs.Count; i++)
			{
				if (m_npcs[i].StaticID == p_npcStaticID)
				{
					return true;
				}
			}
			return false;
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<String>("IndoorScene", m_indoorScene);
			if (!String.IsNullOrEmpty(m_minimapTooltip))
			{
				p_data.Set<String>("MiniMapTT", m_minimapTooltip);
			}
			p_data.Set<Int32>("NPCCount", m_npcs.Count);
			for (Int32 i = 0; i < m_npcs.Count; i++)
			{
				SaveGameData saveGameData = new SaveGameData("Npc" + i);
				saveGameData.Set<Int32>("NPCID", m_npcs[i].StaticID);
				saveGameData.Set<Int32>("SpawnerID", m_npcs[i].SpawnerID);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_indoorScene = p_data.Get<String>("IndoorScene", String.Empty);
			m_minimapTooltip = p_data.Get<String>("MiniMapTT", String.Empty);
			Int32 num = p_data.Get<Int32>("NPCCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("Npc" + i, null);
				if (saveGameData != null)
				{
					Int32 p_staticID = saveGameData.Get<Int32>("NPCID", 0);
					Int32 num2 = saveGameData.Get<Int32>("SpawnerID", 0);
					Npc npc = LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_staticID);
					if (npc != null)
					{
						m_npcs.Add(npc);
					}
				}
			}
		}
	}
}
