using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class RestoreFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_price;

		public RestoreFunction(Int32 p_price, Int32 p_dialogID)
		{
			m_price = p_price;
			m_dialogID = p_dialogID;
		}

		public RestoreFunction()
		{
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

        public override void Trigger(ConversationManager p_manager)
		{
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.GetCharactersAlive())
			{
				if (!character.ConditionHandler.HasCondition(ECondition.DEAD) && (character.HealthPoints != character.MaximumHealthPoints || character.ManaPoints != character.MaximumManaPoints))
				{
					character.ChangeHP(character.MaximumHealthPoints + Math.Abs(character.HealthPoints));
					character.ChangeMP(character.MaximumManaPoints + Math.Abs(character.ManaPoints));
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PARTY_RESTORED, EventArgs.Empty);
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
			p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
		}
	}
}
