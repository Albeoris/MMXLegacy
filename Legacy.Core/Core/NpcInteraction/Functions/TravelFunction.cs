using System;
using System.IO;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class TravelFunction : DialogFunction
	{
		private Int32 m_price;

		private String m_mapName;

		private Int32 m_targetSpawnId;

		public TravelFunction()
		{
		}

		public TravelFunction(Int32 p_price, String p_mapName, Int32 p_targetSpawner)
		{
			m_price = p_price;
			m_mapName = p_mapName;
			m_targetSpawnId = p_targetSpawner;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

		[XmlAttribute("mapName")]
		public String MapName
		{
			get => m_mapName;
		    set => m_mapName = value;
		}

		[XmlAttribute("targetSpawnerId")]
		public Int32 TargetSpawnerId
		{
			get => m_targetSpawnId;
		    set => m_targetSpawnId = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			if (p_manager.IndoorScene != String.Empty)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INDOOR_SCENE_CLOSED, new EventHandler(OnSceneClosed));
			}
			else
			{
				p_manager.EndConversation += OnSceneClosed;
			}
			p_manager.CloseNpcContainer(null);
			p_manager.CloseDialog();
		}

		private void OnSceneClosed(Object p_sender, EventArgs p_args)
		{
			if (p_sender is ConversationManager)
			{
				((ConversationManager)p_sender).EndConversation -= OnSceneClosed;
			}
			else
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INDOOR_SCENE_CLOSED, new EventHandler(OnSceneClosed));
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
			LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
			LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.AUTO;
			LegacyLogic.Instance.WorldManager.SaveGameName = Localization.Instance.GetText("SAVEGAMETYPE_AUTO");
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_STARTED_SAVE, EventArgs.Empty);
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.SelectedInteractiveObject = null;
		}

		private void OnGameSaved(Object sender, EventArgs e)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
			LegacyLogic.Instance.WorldManager.Party.StartSpawnerID = m_targetSpawnId;
			LegacyLogic.Instance.MapLoader.LoadMap(Path.ChangeExtension(m_mapName, ".xml"));
		}
	}
}
