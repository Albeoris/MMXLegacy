using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class GameMessageInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		protected InteractiveObject m_parent;

		private String m_messageKey;

		private Single m_messageDelay;

		public GameMessageInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			GameMessageEventArgs p_eventArgs = new GameMessageEventArgs(m_messageKey, m_messageDelay);
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, p_eventArgs);
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
			if (array.Length != 2)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					2
				}));
			}
			if (!Single.TryParse(array[1], out m_messageDelay))
			{
				throw new FormatException("Second parameter was not a float!");
			}
			m_messageKey = array[0];
		}
	}
}
