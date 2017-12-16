using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class RemoveTokenInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Int32 m_tokenID;

		protected InteractiveObject m_interactiveObject;

		protected InteractiveObject m_parent;

		public RemoveTokenInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public RemoveTokenInteraction()
		{
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
			m_tokenID = Convert.ToInt32(array[0]);
		}

		protected override void DoExecute()
		{
			TokenHandler tokenHandler = LegacyLogic.Instance.WorldManager.Party.TokenHandler;
			tokenHandler.RemoveToken(m_tokenID);
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

		public override void PrewarmAfterCreate()
		{
			base.PrewarmAfterCreate();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ANNOUNCE_NEEDED_TOKEN, new TokenEventArgs(m_tokenID));
		}
	}
}
