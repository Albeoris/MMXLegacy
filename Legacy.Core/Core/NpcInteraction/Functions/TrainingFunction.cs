using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class TrainingFunction : DialogFunction
	{
		private Int32 m_skillID;

		private ETier m_tier;

		private Character m_character;

		private Int32 m_price;

		private Int32 m_dialogID;

		public TrainingFunction(Int32 p_skillID, ETier p_tier, Character p_character, Int32 p_dialogID, Int32 p_price)
		{
			m_skillID = p_skillID;
			m_tier = p_tier;
			m_character = p_character;
			m_dialogID = p_dialogID;
			m_price = p_price;
		}

		public TrainingFunction()
		{
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("skillID")]
		public Int32 SkillID
		{
			get => m_skillID;
		    set => m_skillID = value;
		}

		[XmlAttribute("tier")]
		public ETier Tier
		{
			get => m_tier;
		    set => m_tier = value;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

	    public override void OnShow(Func<String, String> localisation)
	    {
	        RaiseEventShow(localisation, m_skillID, m_tier);
	    }

	    public static void RaiseEventShow(Func<String, String> localisation, Int32 skillId, ETier skillRank)
	    {
	        LegacyLogic.Instance.EventManager.Get<InitTrainingDialogArgs>().TryInvoke(() =>
	        {
	            String caption = localisation("GUI_PARTY_CREATION_SKILLS_TRAINING");

	            var skillData = StaticDataHandler.GetStaticData<SkillStaticData>(EDataType.SKILL, skillId);
	            var skillName = localisation(skillData.Name);

	            return new InitTrainingDialogArgs(caption, skillRank, skillData, skillName);
	        });
	    }

	    public override void Trigger(ConversationManager p_manager)
	    {
	        m_character.SkillHandler.SetSkillTier(m_skillID, m_tier);
	        LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
	        p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
	    }
	}
}
