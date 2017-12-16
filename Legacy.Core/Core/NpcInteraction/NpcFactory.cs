using System;
using System.Collections.Generic;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction
{
	public class NpcFactory : ISaveGameObject
	{
		private Dictionary<Int32, Npc> m_npcs = new Dictionary<Int32, Npc>();

		internal NpcFactory(EventManager p_eventManager)
		{
			p_eventManager.RegisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnGameTimeChangedEvent));
		}

		internal void Clear()
		{
			foreach (Npc npc in m_npcs.Values)
			{
				npc.Destroy();
			}
			m_npcs.Clear();
		}

		public Npc Get(Int32 p_staticID)
		{
			Npc npc;
			if (!m_npcs.TryGetValue(p_staticID, out npc) && p_staticID != 0)
			{
				npc = new Npc(p_staticID);
				if (npc.StaticData != null && npc.ConversationData != null)
				{
					m_npcs.Add(p_staticID, npc);
				}
				else
				{
					npc = null;
				}
			}
			if (npc == null)
			{
				LegacyLogger.LogError("Fail load NPC static ID:" + p_staticID);
			}
			return npc;
		}

		public Boolean Contains(Int32 p_npcId)
		{
			return m_npcs.ContainsKey(p_npcId);
		}

		private void OnGameTimeChangedEvent(Object p_sender, EventArgs p_eventArgs)
		{
			foreach (KeyValuePair<Int32, Npc> keyValuePair in m_npcs)
			{
				keyValuePair.Value.TradingInventory.UpdateOffers();
			}
		}

		public void Load(SaveGameData p_data)
		{
			Int32 num = p_data.Get<Int32>("NpcFactoryCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				Int32 num2 = p_data.Get<Int32>("NpcID" + i, 0);
				Npc npc = Get(num2);
				if (npc != null)
				{
					SaveGameData saveGameData = p_data.Get<SaveGameData>("Npc" + i, null);
					if (saveGameData != null)
					{
						npc.Load(saveGameData);
					}
					else
					{
						LegacyLogger.LogError("Fail load NPC savegame: " + num2);
					}
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("NpcFactoryCount", m_npcs.Count);
			Int32 num = 0;
			foreach (KeyValuePair<Int32, Npc> keyValuePair in m_npcs)
			{
				p_data.Set<Int32>("NpcID" + num, keyValuePair.Key);
				SaveGameData saveGameData = new SaveGameData("Npc" + num);
				keyValuePair.Value.Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				num++;
			}
		}
	}
}
