using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class ConsumeItemAction : BaseCharacterAction
	{
		public ConsumeItemAction(Int32 charaIndex) : base(charaIndex)
		{
			m_consumeType = EConsumeType.CONSUME_CHARACTER_TURN;
		}

		public override Boolean CanDoAction(Command p_command)
		{
			ConsumeCommand consumeCommand = (ConsumeCommand)p_command;
			Consumable consumable = (Consumable)consumeCommand.Slot.GetItem();
			Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(consumeCommand.TargetCharacter);
			if (consumable is Potion)
			{
				Potion potion = (Potion)consumable;
				switch (potion.Target)
				{
				case EPotionTarget.HP:
					return member.HealthPoints < member.MaximumHealthPoints;
				case EPotionTarget.MANA:
					return member.ManaPoints < member.MaximumManaPoints;
				case EPotionTarget.MANA_AND_HP:
					return member.HealthPoints < member.MaximumHealthPoints || member.ManaPoints < member.MaximumManaPoints;
				case EPotionTarget.CONDITION_CONFUSED:
					return member.ConditionHandler.HasCondition(ECondition.CONFUSED);
				case EPotionTarget.CONDITION_POISONED:
					return member.ConditionHandler.HasCondition(ECondition.POISONED);
				case EPotionTarget.CONDITION_WEAK:
					return member.ConditionHandler.HasCondition(ECondition.WEAK);
				}
				if (potion.PotionType == EPotionType.ELIXIR || potion.PotionType == EPotionType.DROPPABLE_ELIXIR)
				{
					return true;
				}
			}
			else if (consumable is Scroll)
			{
				return true;
			}
			return false;
		}

		public override void DoAction(Command p_command)
		{
			ConsumeCommand consumeCommand = (ConsumeCommand)p_command;
			Consumable consumable = (Consumable)consumeCommand.Slot.GetItem();
			Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(consumeCommand.TargetCharacter);
			PartyInventoryController partyInventoryController = (PartyInventoryController)consumeCommand.Slot.Inventory;
			if (consumable is Potion)
			{
				Potion p_potion = (Potion)consumable;
				member.UsePotion(p_potion);
				partyInventoryController.ConsumeSuccess(consumeCommand.Slot.Slot);
			}
			else if (consumable is Scroll)
			{
				Scroll p_scroll = (Scroll)consumable;
				m_consumeType = EConsumeType.NONE;
				Character.UseScroll(p_scroll);
			}
		}

		public override Boolean IsActionDone()
		{
			return true;
		}

		public override Boolean ActionAvailable()
		{
			return !Character.DoneTurn && !Character.ConditionHandler.CantDoAnything();
		}

		public override void Finish()
		{
			m_consumeType = EConsumeType.CONSUME_CHARACTER_TURN;
		}
	}
}
