using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class TriggerBarkInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		private EBarks m_Bark;

		public Boolean m_LevelCheckDungeonEntry;

		public Int32 m_DungeonMinPartyLv;

		public Int32 m_DungeonMaxPartyLv;

		protected InteractiveObject m_parent;

		public TriggerBarkInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void ParseExtra(String p_BarkData)
		{
			String[] array = p_BarkData.Split(new Char[]
			{
				','
			});
			m_Bark = (EBarks)Enum.Parse(typeof(EBarks), array[0]);
			m_LevelCheckDungeonEntry = Convert.ToBoolean(array[1]);
			m_DungeonMinPartyLv = Convert.ToInt32(array[2]);
			m_DungeonMaxPartyLv = Convert.ToInt32(array[3]);
			if (!Enum.IsDefined(typeof(EBarks), m_Bark))
			{
				throw new FormatException("The string in TriggerBarkInteraction " + array[0] + " doesn't exists in EBarks");
			}
		}

		protected override void DoExecute()
		{
			Character[] array = new Character[4];
			Int32 num = 0;
			if (m_LevelCheckDungeonEntry && m_DungeonMinPartyLv != 0 && m_DungeonMaxPartyLv != 0)
			{
				Int32 num2 = 0;
				for (Int32 i = 0; i <= 3; i++)
				{
					Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
					num2 += member.Level;
				}
				num2 /= 4;
				if (num2 < m_DungeonMinPartyLv)
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.HIGH_LEVEL_DUNGEON);
				}
				if (num2 > m_DungeonMaxPartyLv)
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.LOW_LEVEL_DUNGEON);
				}
			}
			else
			{
				switch (m_Bark)
				{
				case EBarks.RACIAL_1:
				{
					for (Int32 j = 0; j < 4; j++)
					{
						Character member2 = LegacyLogic.Instance.WorldManager.Party.GetMember(j);
						if (member2.Class.Race == ERace.HUMAN)
						{
							array[num] = member2;
							num++;
						}
					}
					Int32 num3 = Random.Range(0, num);
					LegacyLogic.Instance.CharacterBarkHandler.TriggerBark(m_Bark, array[num3]);
					break;
				}
				case EBarks.RACIAL_2:
				{
					for (Int32 k = 0; k < 4; k++)
					{
						Character member3 = LegacyLogic.Instance.WorldManager.Party.GetMember(k);
						if (member3.Class.Race == ERace.DWARF)
						{
							array[num] = member3;
							num++;
						}
					}
					Int32 num3 = Random.Range(0, num);
					LegacyLogic.Instance.CharacterBarkHandler.TriggerBark(m_Bark, array[num3]);
					break;
				}
				case EBarks.RACIAL_3:
				{
					for (Int32 l = 0; l < 4; l++)
					{
						Character member4 = LegacyLogic.Instance.WorldManager.Party.GetMember(l);
						if (member4.Class.Race == ERace.ORC)
						{
							array[num] = member4;
							num++;
						}
					}
					Int32 num3 = Random.Range(0, num);
					LegacyLogic.Instance.CharacterBarkHandler.TriggerBark(m_Bark, array[num3]);
					break;
				}
				case EBarks.RACIAL_4:
				{
					for (Int32 m = 0; m < 4; m++)
					{
						Character member5 = LegacyLogic.Instance.WorldManager.Party.GetMember(m);
						if (member5.Class.Race == ERace.ELF)
						{
							array[num] = member5;
							num++;
						}
					}
					Int32 num3 = Random.Range(0, num);
					LegacyLogic.Instance.CharacterBarkHandler.TriggerBark(m_Bark, array[num3]);
					break;
				}
				case EBarks.RACIAL_5:
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(m_Bark);
					break;
				default:
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(m_Bark);
					break;
				}
			}
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
