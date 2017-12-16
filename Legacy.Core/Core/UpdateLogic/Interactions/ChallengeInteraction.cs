using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic.Preconditions;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ChallengeInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		protected InteractiveObject m_parent;

		private ChallengesStaticData data;

		public ChallengeInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void ParseExtra(String p_extra)
		{
			if (m_parent == null)
			{
				m_parent = Grid.FindInteractiveObject(m_parentID);
			}
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
			Int32 p_staticId = Convert.ToInt32(array[0]);
			data = StaticDataHandler.GetStaticData<ChallengesStaticData>(EDataType.CHALLENGES, p_staticId);
			m_preconditionString = String.Concat(new Object[]
			{
				data.Type,
				",WHO_WILL,",
				data.WhoWillText,
				",",
				data.SuccessText,
				",",
				data.FailText,
				",",
				data.Attribute,
				",",
				data.Value
			});
			if (data.Type == EPreconditionType.CHALLENGE)
			{
				m_preconditionString += ",0";
			}
		}

		public override void Execute()
		{
			base.Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute();
		}
	}
}
