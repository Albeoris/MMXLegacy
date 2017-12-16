using System;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class DoDamageInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private Int32 m_damage;

		private EDamageType m_damageType = EDamageType.NONE;

		private Boolean m_singleTarget = true;

		private InteractiveObject m_parent;

		private ChallengesStaticData data;

		public DoDamageInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
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
			m_damageType = data.DamageType;
			m_damage = data.Damage;
			m_singleTarget = data.SingleTarget;
		}

		public override void Execute()
		{
			if (m_damageType > EDamageType.NONE && m_damageType < EDamageType._MAX_ && m_damage > 0)
			{
				if (m_singleTarget)
				{
					LegacyLogic.Instance.WorldManager.Party.LastWhoWillCharacter.ChangeHP(-m_damage);
				}
				else
				{
					for (Int32 i = 0; i < 4; i++)
					{
						Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
						member.ChangeHP(-m_damage);
					}
				}
			}
			base.Execute();
		}
	}
}
