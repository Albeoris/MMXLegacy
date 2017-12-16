using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityMartyr : MonsterAbilityBase
	{
		private Single m_magicPower;

		public MonsterAbilityMartyr(EMonsterAbilityType abilityType, Single p_magicPower) : base(abilityType)
		{
			m_magicPower = p_magicPower;
			m_executionPhase = EExecutionPhase.MONSTER_DIES;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
			List<Monster> list = new List<Monster>();
			IList<MovingEntity> entities = slot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] != null && entities[i] != p_monster && entities[i] is Monster)
				{
					(entities[i] as Monster).AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.HOUR_OF_JUSTICE, p_monster.MagicPower, m_level));
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
		}

		public override String GetDescription()
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, 28);
			Int32 num = (Int32)Math.Round(staticData.GetBuffValues(m_level)[0] * m_magicPower * 100f, MidpointRounding.AwayFromZero);
			Int32 num2 = (Int32)Math.Round(staticData.GetBuffValues(m_level)[1] * m_magicPower * 100f, MidpointRounding.AwayFromZero);
			String[] args = new String[]
			{
				"[00FF00]",
				"[80FF80]",
				"[FF0000]",
				"[FFC080]",
				"[FFFF80]",
				"[80FFFF]",
				"[-]",
				num.ToString(),
				num2.ToString(),
				staticData.GetDuration(m_level).ToString(),
				String.Empty
			};
			return Localization.Instance.GetText("MONSTER_ABILITY_MARTYR_INFO", args);
		}
	}
}
