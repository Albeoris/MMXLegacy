using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class GiveXPInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		public const Int32 DEFAULT_XP = 0;

		public const Boolean DEFAULT_SINGLE = false;

		protected InteractiveObject m_interactiveObject;

		protected InteractiveObject m_parent;

		private Int32 m_xpAmount;

		private Boolean m_isSingle;

		public GiveXPInteraction()
		{
			m_xpAmount = 0;
			m_isSingle = false;
		}

		public GiveXPInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public Int32 XPAmount => m_xpAmount;

	    public Boolean IsSingle => m_isSingle;

	    protected override void DoExecute()
		{
			if (m_interactiveObject == null)
			{
				throw new NullReferenceException("Data could not be set because the interactive object is null! (" + m_parentID + ")");
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (m_isSingle)
			{
				Character member = party.GetMember(party.CurrentCharacter);
				member.AddExpAndFlushActionLog(m_xpAmount);
			}
			else
			{
				for (Int32 i = 0; i < 4; i++)
				{
					Character member2 = party.GetMember(i);
					member2.AddExpAndFlushActionLog(m_xpAmount);
				}
			}
			FinishExecution();
		}

		public void ParseExtraEditor(String p_extra)
		{
			ParseExtra(p_extra);
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
			if (!Int32.TryParse(array[0], out m_xpAmount))
			{
				throw new FormatException("First parameter " + array[0] + " was not an amount of XP!");
			}
			if (!Boolean.TryParse(array[1], out m_isSingle))
			{
				throw new FormatException("Second parameter " + array[1] + " was not a single flag!");
			}
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
