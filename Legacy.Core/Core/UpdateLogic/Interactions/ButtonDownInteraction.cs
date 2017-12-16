using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ButtonDownInteraction : BaseButtonInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		public const Int32 DEFAULT_RELEASE_TIME = 0;

		private Int32 m_releaseTime;

		public ButtonDownInteraction()
		{
			m_releaseTime = 0;
		}

		public ButtonDownInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
		}

		public Int32 ReleaseTime => m_releaseTime;

	    protected override void SetStates()
		{
			if (m_targetButton == null)
			{
				throw new InvalidOperationException("Tried to release something that is not a button!");
			}
			if (m_targetButton is Button)
			{
				((Button)m_targetButton).Down(m_releaseTime);
			}
			else if (m_targetButton is PressurePlate)
			{
				((PressurePlate)m_targetButton).Down(m_releaseTime);
			}
		}

		protected override void ParseExtra(String p_extra)
		{
			base.ParseExtra(p_extra);
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
			if (!Int32.TryParse(array[0], out m_releaseTime))
			{
				throw new FormatException("First parameter " + array[0] + " was not a time count!");
			}
			m_releaseTime++;
		}
	}
}
