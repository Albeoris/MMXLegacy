using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class AddBuffFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_buffID;

		private Single m_magicFactor;

		public AddBuffFunction()
		{
		}

		public AddBuffFunction(Int32 p_buffID, Single p_magicFactor, Int32 p_dialogID)
		{
			m_buffID = p_buffID;
			m_magicFactor = p_magicFactor;
			m_dialogID = p_dialogID;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("buffID")]
		public Int32 BuffID
		{
			get => m_buffID;
		    set => m_buffID = value;
		}

		[XmlAttribute("magicFactor")]
		public Single MagicFactor
		{
			get => m_magicFactor;
		    set => m_magicFactor = value;
		}

	    public override void OnShow(Func<String, String> localisation)
	    {
	        LegacyLogic.Instance.EventManager.Get<InitUniqueDialogArgs>().TryInvoke(() =>
	        {
	            PartyBuffStaticData buffData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, BuffID);
	            String caption = localisation(buffData.Name);

	            return new InitUniqueDialogArgs(caption);
	        });
	    }

	    public override void Trigger(ConversationManager p_manager)
		{
			LegacyLogic.Instance.WorldManager.Party.Buffs.AddBuff((EPartyBuffs)m_buffID, m_magicFactor);
			p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
		}
	}
}
