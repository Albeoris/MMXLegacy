using System;

namespace Legacy.Core.Spells
{
	public abstract class Spell
	{
		public const String COLOR_POSITIVE_EFFECT_HEX = "[00FF00]";

		public const String COLOR_ALTERNATIVE_POSITIVE_EFFECT_HEX = "[80FF80]";

		public const String COLOR_NEGATIVE_EFFECT_HEX = "[FF0000]";

		public const String COLOR_ALTERNATIVE_NEGATIVE_EFFECT_HEX = "[FFC080]";

		public const String COLOR_YELLOW_EFFECT_HEX = "[FFFF80]";

		public const String COLOR_BLUE_EFFECT_HEX = "[80FFFF]";

		public const String COLOR_TAG_END = "[-]";

		protected String[] m_descriptionValues = new String[11];

		public Spell()
		{
			m_descriptionValues[0] = "[00FF00]";
			m_descriptionValues[1] = "[80FF80]";
			m_descriptionValues[2] = "[FF0000]";
			m_descriptionValues[3] = "[FFC080]";
			m_descriptionValues[4] = "[FFFF80]";
			m_descriptionValues[5] = "[80FFFF]";
			m_descriptionValues[6] = "[-]";
			m_descriptionValues[7] = String.Empty;
			m_descriptionValues[8] = String.Empty;
			m_descriptionValues[9] = String.Empty;
			m_descriptionValues[10] = String.Empty;
		}

		public abstract Int32 StaticID { get; }

		public abstract ETargetType TargetType { get; }

		public abstract String NameKey { get; }

		public abstract String EffectKey { get; }

		protected void SetDescriptionValue(Int32 p_valueNr, String p_value)
		{
			m_descriptionValues[p_valueNr + 7] = p_value;
		}

		protected void SetDescriptionValue(Int32 p_valueNr, Int32 p_value)
		{
			SetDescriptionValue(p_valueNr, p_value.ToString());
		}

		protected void SetDescriptionValue(Int32 p_valueNr, Single p_value)
		{
			SetDescriptionValue(p_valueNr, p_value.ToString());
		}

		public override String ToString()
		{
			return String.Format("[Spell: StaticID={0}, TargetType={1}, NameKey={2}, EffectKey={3}]", new Object[]
			{
				StaticID,
				TargetType,
				NameKey,
				EffectKey
			});
		}
	}
}
