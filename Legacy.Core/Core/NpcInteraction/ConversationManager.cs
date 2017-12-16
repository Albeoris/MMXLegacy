using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Hints;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction
{
	public class ConversationManager
	{
		private Npc m_currentNpc;

		private Dialog m_currentDialog;

		private Boolean m_hidden;

		private Boolean m_isOpen;

		private Boolean m_containerOpened;

		private Boolean m_showNpcs = true;

		private Boolean m_showNameAndPortrait = true;

		private Int32 m_startDialogID = -1;

		private String m_level = String.Empty;

		private Int32 m_targetSpawnID;

		private Int32 m_cutsceneState;

		private Int32 m_overrideStartID = -1;

		public ConversationManager()
		{
			NPCs = new List<Npc>();
		}

	    public event EventHandler BeginConversation;

	    public event EventHandler EndConversation;

	    public event EventHandler ChangeConversation;

	    public event EventHandler<ChangedDialogEventArgs> DialogChanged;

	    public event EventHandler DialogClosed;

		public event EventHandler HideConversationForEntranceEvent;

		public event EventHandler HideNPCsEvent;

		public event EventHandler ShowNPCsEvent;

		public event EventHandler<ChangedCutsceneStateEventArgs> CutsceneStateChanged;

		public Int32 CutsceneState => m_cutsceneState;

	    public Int32 OverrideStartID
		{
			get => m_overrideStartID;
	        set => m_overrideStartID = value;
	    }

		public String IndoorScene { get; private set; }

		public List<Npc> NPCs { get; private set; }

		public Boolean ShowNpcs
		{
			get => m_showNpcs;
		    set => m_showNpcs = value;
		}

		public Boolean ShowNameAndPortrait => m_showNameAndPortrait;

	    public Int32 StartDialogID => m_startDialogID;

	    public Npc CurrentNpc => m_currentNpc;

	    public NpcConversation CurrentConversation
		{
			get
			{
				if (m_currentNpc != null)
				{
					return m_currentNpc.ConversationData;
				}
				return null;
			}
		}

		public Boolean IsOpen => m_isOpen;

	    public Boolean IsForEntrance { get; private set; }

		public String Level => m_level;

	    public String LastCutsceneVideoID { get; private set; }

		public void OpenNpcContainer(NpcContainer p_container, Int32 p_startNpcId)
		{
			if (m_containerOpened)
			{
				LegacyLogger.LogError("close the current container first");
				return;
			}
			Reset();
			m_containerOpened = true;
			Boolean flag = OpenConversation(p_container.Npcs, p_container.IndoorScene, p_startNpcId, p_container.StartDialogID, null, 0, false);
			if (flag)
			{
				if (BeginConversation != null)
				{
					BeginConversation(this, EventArgs.Empty);
				}
				if (NPCs.Count <= 1)
				{
					_HideNPCs();
				}
			}
			else
			{
				Reset();
			}
		}

		public void OpenDungeonEntrance(Entrance p_entrance, String p_level, Int32 p_targetSpawnID)
		{
			if (m_containerOpened)
			{
				LegacyLogger.LogError("close the current container first");
				return;
			}
			Reset();
			m_containerOpened = true;
			Boolean flag = OpenConversation(p_entrance.Npcs, p_entrance.IndoorScene, -1, -1, p_level, p_targetSpawnID, true);
			if (flag)
			{
				if (BeginConversation != null)
				{
					BeginConversation(this, EventArgs.Empty);
				}
				if (NPCs.Count == 0)
				{
					_HideConversationForEntrance();
				}
			}
			else
			{
				Reset();
			}
		}

		public void ChangeNpcContainer(NpcContainer p_container, Int32 p_startNpcId, Int32 p_dialogID)
		{
			m_overrideStartID = p_dialogID;
			ChangeNpcContainer(p_container, p_startNpcId);
		}

		public void ChangeNpcContainer(NpcContainer p_container, Int32 p_startNpcId)
		{
			if (!m_containerOpened)
			{
				LegacyLogger.LogError("no container open");
				return;
			}
			Reset();
			m_containerOpened = true;
			Boolean flag = OpenConversation(p_container.Npcs, p_container.IndoorScene, p_startNpcId, p_container.StartDialogID, null, 0, false);
			if (flag)
			{
				if (ChangeConversation != null)
				{
					ChangeConversation(this, EventArgs.Empty);
				}
				if (NPCs.Count <= 1)
				{
					_HideNPCs();
				}
			}
			else
			{
				Reset();
			}
		}

		public void OpenNpcDialog(Npc p_npc, Int32 p_startDialogID = -1)
		{
			if (m_containerOpened)
			{
				LegacyLogger.LogError("close the current container first");
				return;
			}
			Reset();
			m_containerOpened = true;
			NPCs.Add(p_npc);
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.SelectedInteractiveObject = null;
			m_cutsceneState = 0;
			LastCutsceneVideoID = null;
			m_level = String.Empty;
			m_targetSpawnID = 0;
			IndoorScene = String.Empty;
			IsForEntrance = false;
			m_isOpen = true;
			m_startDialogID = p_startDialogID;
			OpenDialog(p_npc);
			if (BeginConversation != null)
			{
				BeginConversation(this, EventArgs.Empty);
			}
		}

		private Boolean OpenConversation(List<Npc> npcs, String indoorScene, Int32 startNpcId, Int32 startDialogId, String p_level, Int32 p_targetSpawnID, Boolean isForEntrance)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.SelectedInteractiveObject = null;
			TokenHandler tokenHandler = party.TokenHandler;
			HirelingHandler hirelingHandler = party.HirelingHandler;
			Npc npc = null;
			foreach (Npc npc2 in npcs)
			{
				if (npc2.StaticID != 50 || tokenHandler.GetTokens(10) <= 0)
				{
					if (npc2.StaticID != 167 || tokenHandler.GetTokens(18) <= 0)
					{
						if (npc2.StaticID != 136 || tokenHandler.GetTokens(545) != 0)
						{
							if (!hirelingHandler.HirelingHired(npc2))
							{
								NPCs.Add(npc2);
								if (startNpcId > 0 && npc2.StaticData.StaticID == startNpcId)
								{
									npc = npc2;
								}
							}
						}
					}
				}
			}
			if (!isForEntrance && NPCs.Count == 0)
			{
				return false;
			}
			if (npc == null && NPCs.Count > 0)
			{
				npc = NPCs[0];
			}
			m_cutsceneState = 0;
			LastCutsceneVideoID = null;
			m_level = p_level;
			m_targetSpawnID = p_targetSpawnID;
			IndoorScene = indoorScene;
			IsForEntrance = isForEntrance;
			m_isOpen = true;
			m_startDialogID = startDialogId;
			if (npc != null)
			{
				OpenDialog(npc);
			}
			return true;
		}

		public void CloseNpcContainer(String cutsceneVideoID = null)
		{
			if (!m_containerOpened)
			{
				LegacyLogger.LogError("already closed");
				return;
			}
			m_containerOpened = false;
			LegacyLogic.Instance.CommandManager.AllowContinuousCommands = true;
			LastCutsceneVideoID = cutsceneVideoID;
			m_cutsceneState = 0;
			if (EndConversation != null)
			{
				EndConversation(this, EventArgs.Empty);
			}
			Reset();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.AutoSelectInteractiveObject();
			if (party.HirelingHandler.NumHirelingsHired() > 0)
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.HIRELINGS);
			}
		}

		public void UpdateCurrentDialog()
		{
			if (CurrentNpc != null && m_currentDialog != null)
			{
				_ChangeDialog(CurrentNpc.StaticID, m_currentDialog.ID);
			}
		}

		public void ChangeMap()
		{
			if (!m_containerOpened)
			{
				LegacyLogger.LogError("already closed");
				return;
			}
			LegacyLogic.Instance.WorldManager.Party.StartSpawnerID = m_targetSpawnID;
			LegacyLogic.Instance.MapLoader.LoadMap(m_level);
		}

		public void OpenBookmark(Npc p_npc, NpcConversationStaticData.DialogBookmark bookmark)
		{
			_ChangeDialog(p_npc.StaticID, bookmark.m_dialogID);
		}

		public void SwitchToDialogId(Int32 p_id)
		{
			if (m_currentNpc == null)
			{
				LegacyLogger.LogError("No NPC container opened, can't switch dialog!");
				return;
			}
			m_startDialogID = p_id;
			OpenDialog(m_currentNpc);
		}

		public void OpenDialog(Npc p_npc)
		{
			if (m_currentNpc != null)
			{
				if (m_currentNpc.TradingInventory.IsTrading)
				{
					m_currentNpc.TradingInventory.StopTrading();
				}
				if (m_currentNpc.TradingSpells.IsTrading)
				{
					m_currentNpc.TradingSpells.StopTrading();
				}
			}
			m_currentNpc = p_npc;
			if (m_startDialogID == -1)
			{
				Dialog rootDialog = m_currentNpc.ConversationData.RootDialog;
				_ChangeDialog(0, rootDialog.ID);
			}
			else
			{
				_ChangeDialog(0, m_startDialogID);
			}
		}

		public void CloseDialog()
		{
			m_currentNpc = null;
			if (DialogClosed != null)
			{
				DialogClosed(this, EventArgs.Empty);
			}
			LegacyLogic.Instance.CommandManager.AllowContinuousCommands = true;
		}

		public void Reset()
		{
			NPCs.Clear();
			m_containerOpened = false;
			m_currentNpc = null;
			m_currentDialog = null;
			m_cutsceneState = 0;
			m_hidden = false;
			m_isOpen = false;
			m_showNpcs = true;
			m_showNameAndPortrait = true;
			m_startDialogID = -1;
			m_level = String.Empty;
			m_targetSpawnID = 0;
		}

		internal void _ChangeDialog(Int32 p_npcID, Int32 p_dialogID)
		{
			if (p_npcID > 0)
			{
				m_currentNpc = LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_npcID);
			}
			Dialog dialog = m_currentNpc.ConversationData[p_dialogID];
			if (dialog != null)
			{
				if (m_currentNpc.TradingInventory.IsTrading)
				{
					m_currentNpc.TradingInventory.StopTrading();
				}
				if (m_currentNpc.TradingSpells.IsTrading)
				{
					m_currentNpc.TradingSpells.StopTrading();
				}
				m_currentDialog = dialog;
				dialog.CheckDialog(m_currentNpc);
				m_showNpcs = !dialog.HideNpcsAndCloseButton;
				m_showNameAndPortrait = !dialog.HideNpcAndPortrait;
				if (DialogChanged != null)
				{
					DialogChanged(this, new ChangedDialogEventArgs(dialog));
				}
			}
		}

		internal void _HideNPCs()
		{
			if (HideNPCsEvent != null)
			{
				HideNPCsEvent(this, EventArgs.Empty);
			}
		}

		internal void _ShowNPCs()
		{
			if (ShowNPCsEvent != null && NPCs.Count > 1)
			{
				ShowNPCsEvent(this, EventArgs.Empty);
			}
		}

		internal void _HideConversationForEntrance()
		{
			if (!m_hidden && HideConversationForEntranceEvent != null)
			{
				m_hidden = true;
				HideConversationForEntranceEvent(this, EventArgs.Empty);
			}
		}

		internal void _ChangeCutsceneState(Int32 state)
		{
			state = Math.Max(0, state);
			if (m_cutsceneState != state)
			{
				Int32 cutsceneState = m_cutsceneState;
				m_cutsceneState = state;
				if (CutsceneStateChanged != null)
				{
					CutsceneStateChanged(this, new ChangedCutsceneStateEventArgs(cutsceneState, m_cutsceneState));
				}
			}
		}
	}
}
