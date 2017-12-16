using System;
using Legacy.Core.Api;
using Legacy.Core.Quests;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.Achievements.Conditions
{
	public class QuestStepCompletedWithoutCastingSpell : AchievementCondition
	{
		private ECharacterSpell m_spell;

		private Int32 m_questID;

		public QuestStepCompletedWithoutCastingSpell(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		internal Int32 QuestID => m_questID;

	    internal ECharacterSpell SpellID => m_spell;

	    public override void ParseParameter(String p_parameterString)
		{
			String[] array = p_parameterString.Split(new Char[]
			{
				','
			});
			Int32.TryParse(array[0], out m_questID);
			if (Enum.IsDefined(typeof(ECharacterSpell), array[1]))
			{
				m_spell = (ECharacterSpell)Enum.Parse(typeof(ECharacterSpell), array[1]);
			}
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_questID);
			if (step != null && step.QuestState == EQuestState.SOLVED)
			{
				p_count = 1;
				return !LegacyLogic.Instance.WorldManager.AchievementManager.CastedSpellList.Contains(m_spell);
			}
			return false;
		}
	}
}
