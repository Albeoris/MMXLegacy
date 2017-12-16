using System;
using System.Collections.Generic;
using System.IO;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class NpcConversation
	{
		private Dialog m_rootDialog;

		private Dictionary<Int32, Dialog> m_dialogs = new Dictionary<Int32, Dialog>();

		private NpcConversationStaticData m_staticData;

		private List<Int32> m_neededTokens = new List<Int32>();

		public NpcConversation(NpcConversationStaticData p_data)
		{
			m_staticData = p_data;
			foreach (NpcConversationStaticData.Dialog p_dialogData in p_data.m_dialogs)
			{
				Dialog dialog = new Dialog(p_dialogData);
				m_dialogs.Add(p_dialogData.m_id, dialog);
				if (p_dialogData.m_id == p_data.m_rootDialogID)
				{
					m_rootDialog = dialog;
				}
				m_neededTokens.AddRange(dialog.NeededTokens);
			}
			if (m_rootDialog == null)
			{
				throw new InvalidDataException("RootDialog not found or defined! RootDialogID=" + p_data.m_rootDialogID);
			}
		}

		public Dialog this[Int32 p_dialogID] => m_dialogs[p_dialogID];

	    public Dialog RootDialog => m_rootDialog;

	    public List<Int32> NeededTokens => m_neededTokens;

	    public NpcConversationStaticData.DialogBookmark[] Bookmarks => m_staticData.m_bookmarks;
	}
}
