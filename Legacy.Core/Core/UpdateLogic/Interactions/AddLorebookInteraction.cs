using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class AddLorebookInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Int32 m_lorebookStaticID;

		private InteractiveObject m_parent;

		public AddLorebookInteraction()
		{
		}

		public AddLorebookInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			if (m_parent == null)
			{
				LegacyLogger.LogError("Not found parent trigger ID: " + p_parentID);
			}
		}

		protected override void DoExecute()
		{
			LegacyLogic.Instance.WorldManager.LoreBookHandler.AddLoreBook(m_lorebookStaticID);
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
			m_lorebookStaticID = Convert.ToInt32(array[0]);
		}
	}
}
