using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Hints;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class TriggerHintInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private EHintType m_hint;

		protected InteractiveObject m_parent;

		public TriggerHintInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
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
			m_hint = (EHintType)Enum.Parse(typeof(EHintType), array[0]);
		}

		protected override void DoExecute()
		{
			HintManager hintManager = LegacyLogic.Instance.WorldManager.HintManager;
			hintManager.TriggerHint(m_hint);
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
	}
}
