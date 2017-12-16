using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellSwoop : MonsterSpell
	{
		public MonsterSpellSwoop(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.SWOOP, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			if (Random.Value < m_staticData.GetAdditionalValue(m_level))
			{
				base.DoEffect(p_monster, p_target, p_spellArgs);
			}
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, m_staticData.GetAdditionalValue(m_level) * 100f);
			return Localization.Instance.GetText(m_staticData.NameKey + "_INFO", m_descriptionValues);
		}
	}
}
