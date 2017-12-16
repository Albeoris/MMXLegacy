using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core
{
	public class CharacterAttributeChanger
	{
		private Character m_character;

		private Attributes m_temporaryPoints;

		public CharacterAttributeChanger(Character p_character)
		{
			m_character = p_character;
			m_temporaryPoints = Attributes.Empty;
		}

		public Character Character => m_character;

	    public Attributes TemporaryPoints => m_temporaryPoints;

	    public Boolean HasTemporarySpendPoints()
		{
			return m_temporaryPoints.Might > 0 || m_temporaryPoints.Magic > 0 || m_temporaryPoints.Perception > 0 || m_temporaryPoints.Destiny > 0 || m_temporaryPoints.Vitality > 0 || m_temporaryPoints.Spirit > 0;
		}

		private Int32 SumTemporarySpendPoints()
		{
			return m_temporaryPoints.Might + m_temporaryPoints.Magic + m_temporaryPoints.Perception + m_temporaryPoints.Destiny + m_temporaryPoints.Vitality + m_temporaryPoints.Spirit;
		}

		public void Reset()
		{
			m_temporaryPoints = Attributes.Empty;
		}

		public void FinalizeTemporarySpendPoints()
		{
			m_character.BaseAttributes += m_temporaryPoints;
			m_character.CalculateCurrentAttributes();
			m_character.AttributePoints -= SumTemporarySpendPoints();
			m_temporaryPoints = Attributes.Empty;
		}

		public void IncreaseMight()
		{
			m_temporaryPoints.Might = m_temporaryPoints.Might + 1;
			m_character.TemporaryAttributePoints--;
		}

		public void IncreaseMagic()
		{
			m_temporaryPoints.Magic = m_temporaryPoints.Magic + 1;
			m_character.TemporaryAttributePoints--;
		}

		public void IncreasePerception()
		{
			m_temporaryPoints.Perception = m_temporaryPoints.Perception + 1;
			m_character.TemporaryAttributePoints--;
		}

		public void IncreaseDestiny()
		{
			m_temporaryPoints.Destiny = m_temporaryPoints.Destiny + 1;
			m_character.TemporaryAttributePoints--;
		}

		public void IncreaseVitality()
		{
			m_temporaryPoints.Vitality = m_temporaryPoints.Vitality + 1;
			m_character.TemporaryAttributePoints--;
		}

		public void IncreaseSpirit()
		{
			m_temporaryPoints.Spirit = m_temporaryPoints.Spirit + 1;
			m_character.TemporaryAttributePoints--;
		}

		public void DecreaseMight()
		{
			m_temporaryPoints.Might = m_temporaryPoints.Might - 1;
			m_character.TemporaryAttributePoints++;
		}

		public void DecreaseMagic()
		{
			m_temporaryPoints.Magic = m_temporaryPoints.Magic - 1;
			m_character.TemporaryAttributePoints++;
		}

		public void DecreasePerception()
		{
			m_temporaryPoints.Perception = m_temporaryPoints.Perception - 1;
			m_character.TemporaryAttributePoints++;
		}

		public void DecreaseDestiny()
		{
			m_temporaryPoints.Destiny = m_temporaryPoints.Destiny - 1;
			m_character.TemporaryAttributePoints++;
		}

		public void DecreaseVitality()
		{
			m_temporaryPoints.Vitality = m_temporaryPoints.Vitality - 1;
			m_character.TemporaryAttributePoints++;
		}

		public void DecreaseSpirit()
		{
			m_temporaryPoints.Spirit = m_temporaryPoints.Spirit - 1;
			m_character.TemporaryAttributePoints++;
		}
	}
}
