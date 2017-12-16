using System;
using Legacy.Core.Api;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class NpcContainer : IndoorSceneBase
	{
		private Int32 m_autoStartNPC = -1;

		private Int32 m_startDialogID = -1;

		private Boolean m_showNpcs = true;

		public NpcContainer() : this(0, 0)
		{
		}

		public NpcContainer(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.NPC_CONTAINER, p_spawnerID)
		{
		}

		public Int32 AutoStartNPC => m_autoStartNPC;

	    public Int32 StartDialogID => m_startDialogID;

	    public Boolean ShowNpcs => m_showNpcs;

	    public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.AUTOSTART_NPC)
			{
				m_autoStartNPC = Convert.ToInt32(p_value);
			}
			else if (p_key == EInteractiveObjectData.START_DIALOG_ID)
			{
				m_startDialogID = Convert.ToInt32(p_value);
			}
			else if (p_key == EInteractiveObjectData.SHOW_NPCS)
			{
				m_showNpcs = Convert.ToBoolean(p_value);
			}
			else
			{
				base.SetData(p_key, p_value);
			}
		}

		public void StartConversation()
		{
			LegacyLogic.Instance.ConversationManager.OpenNpcContainer(this, m_autoStartNPC);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Int32>("AutoStartNPC", m_autoStartNPC);
			p_data.Set<Int32>("StartDialogID", m_startDialogID);
			p_data.Set<Boolean>("ShowNpcs", m_showNpcs);
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_autoStartNPC = p_data.Get<Int32>("AutoStartNPC", -1);
			m_startDialogID = p_data.Get<Int32>("StartDialogID", -1);
			m_showNpcs = p_data.Get<Boolean>("ShowNpcs", true);
		}

		public override String ToString()
		{
			return String.Format("[NpcContainer: IndoorScene={0}, NPCs={1}]", IndoorScene, Npcs);
		}
	}
}
