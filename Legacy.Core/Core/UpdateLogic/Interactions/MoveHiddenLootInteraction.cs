using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class MoveHiddenLootInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		protected InteractiveObject m_parent;

		private EHiddenInventory m_sourceInventory;

		public MoveHiddenLootInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public MoveHiddenLootInteraction()
		{
		}

		protected override void DoExecute()
		{
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			Container container = (Container)interactiveObject;
			if (container == null)
			{
				throw new InvalidOperationException("Tried to add to something that is not an container!");
			}
			Int32 num = 0;
			HiddenInventoryController hiddenInventory = LegacyLogic.Instance.WorldManager.Party.GetHiddenInventory((Int32)m_sourceInventory);
			Int32 maximumItemCount = hiddenInventory.GetMaximumItemCount();
			for (Int32 i = 0; i < maximumItemCount; i++)
			{
				BaseItem itemAt = hiddenInventory.GetItemAt(i);
				if (itemAt != null)
				{
					container.AddItem(itemAt);
					num++;
				}
			}
			hiddenInventory.Inventory.Clear();
			FinishExecution();
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 1)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					1
				}));
			}
			m_sourceInventory = (EHiddenInventory)Enum.Parse(typeof(EHiddenInventory), array[0]);
		}
	}
}
