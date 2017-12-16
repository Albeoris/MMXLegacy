using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class RemoveBuffInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Int32 m_buffID;

		protected InteractiveObject m_parent;

		public RemoveBuffInteraction()
		{
		}

		public RemoveBuffInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveBuff((EPartyBuffs)m_buffID);
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
			m_buffID = Convert.ToInt32(array[0]);
		}
	}
}
