using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ChangeMapInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		public const String DEFAULT_LEVEL = "";

		protected Entrance m_parent;

		private String m_level = String.Empty;

		public ChangeMapInteraction()
		{
			m_level = String.Empty;
		}

		public ChangeMapInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = (Entrance)Grid.FindInteractiveObject(m_parentID);
		}

		public String Level => m_level;

	    protected override void DoExecute()
		{
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
			if (ParseExtraX(m_extra))
			{
				if (m_parent.HasScene)
				{
					m_parent.StartConversation(m_level, m_targetSpawnID);
				}
				else
				{
					LegacyLogic.Instance.WorldManager.Party.StartSpawnerID = m_targetSpawnID;
					LegacyLogic.Instance.MapLoader.LoadMap(m_level);
				}
			}
			FinishExecution();
		}

		public void ParseExtraEditor(String p_extra)
		{
			ParseExtra(p_extra);
		}

		private Boolean ParseExtraX(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 1)
			{
				return false;
			}
			m_level = array[0];
			if (!m_level.Contains(".xml"))
			{
				m_level += ".xml";
			}
			return true;
		}

		protected override void ParseExtra(String p_extra)
		{
			ParseExtraX(p_extra);
		}
	}
}
