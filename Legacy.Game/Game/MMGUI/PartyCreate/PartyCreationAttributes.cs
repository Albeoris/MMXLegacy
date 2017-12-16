using System;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.PartyCreate
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationAttributes")]
	public class PartyCreationAttributes : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private UILabel m_raceClassLabel;

		[SerializeField]
		private UISprite m_portrait;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private UILabel m_PointsLeftLabel;

		[SerializeField]
		private AttributeChanger m_mightAC;

		[SerializeField]
		private AttributeChanger m_magicAC;

		[SerializeField]
		private AttributeChanger m_perceptionAC;

		[SerializeField]
		private AttributeChanger m_destinyAC;

		[SerializeField]
		private AttributeChanger m_vitalityAC;

		[SerializeField]
		private AttributeChanger m_spiritAC;

		[SerializeField]
		private UILabel m_health;

		[SerializeField]
		private UILabel m_mana;

		private PartyCreator m_partyCreator;

		public void Init(PartyCreator p_partyCreator)
		{
			Cleanup();
			m_partyCreator = p_partyCreator;
			m_mightAC.OnAttributeRaised += OnAttributeRaised;
			m_magicAC.OnAttributeRaised += OnAttributeRaised;
			m_perceptionAC.OnAttributeRaised += OnAttributeRaised;
			m_destinyAC.OnAttributeRaised += OnAttributeRaised;
			m_vitalityAC.OnAttributeRaised += OnAttributeRaised;
			m_spiritAC.OnAttributeRaised += OnAttributeRaised;
			m_mightAC.OnAttributeLowered += OnAttributeLowered;
			m_magicAC.OnAttributeLowered += OnAttributeLowered;
			m_perceptionAC.OnAttributeLowered += OnAttributeLowered;
			m_destinyAC.OnAttributeLowered += OnAttributeLowered;
			m_vitalityAC.OnAttributeLowered += OnAttributeLowered;
			m_spiritAC.OnAttributeLowered += OnAttributeLowered;
		}

		public void Cleanup()
		{
			m_mightAC.OnAttributeRaised -= OnAttributeRaised;
			m_magicAC.OnAttributeRaised -= OnAttributeRaised;
			m_perceptionAC.OnAttributeRaised -= OnAttributeRaised;
			m_destinyAC.OnAttributeRaised -= OnAttributeRaised;
			m_vitalityAC.OnAttributeRaised -= OnAttributeRaised;
			m_spiritAC.OnAttributeRaised -= OnAttributeRaised;
			m_mightAC.OnAttributeLowered -= OnAttributeLowered;
			m_magicAC.OnAttributeLowered -= OnAttributeLowered;
			m_perceptionAC.OnAttributeLowered -= OnAttributeLowered;
			m_destinyAC.OnAttributeLowered -= OnAttributeLowered;
			m_vitalityAC.OnAttributeLowered -= OnAttributeLowered;
			m_spiritAC.OnAttributeLowered -= OnAttributeLowered;
		}

		public void OnAfterActivate()
		{
			NGUITools.SetActive(gameObject, true);
			UpdateCharacter();
			UpdateAttributes();
		}

		public void UndoSelection()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.ResetAttributes();
		}

		public void OnResetClick()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.ResetAttributes();
			UpdateCharacter();
			UpdateAttributes();
		}

		public void OnDefaultClick()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.SetDefaultAttributes();
			UpdateCharacter();
			UpdateAttributes();
		}

		public void OnAttributeLowered(Object p_sender, EventArgs p_args)
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			EPotionTarget attribute = (p_sender as AttributeChanger).Attribute;
			if (attribute == EPotionTarget.MIGHT)
			{
				selectedDummyCharacter.DecreaseMight();
			}
			else if (attribute == EPotionTarget.MAGIC)
			{
				selectedDummyCharacter.DecreaseMagic();
			}
			else if (attribute == EPotionTarget.PERCEPTION)
			{
				selectedDummyCharacter.DecreasePerception();
			}
			else if (attribute == EPotionTarget.DESTINY)
			{
				selectedDummyCharacter.DecreaseDestiny();
			}
			else if (attribute == EPotionTarget.VITALITY)
			{
				selectedDummyCharacter.DecreaseVitality();
			}
			else if (attribute == EPotionTarget.SPIRIT)
			{
				selectedDummyCharacter.DecreaseSpirit();
			}
			UpdateCharacter();
			UpdateAttributes();
		}

		private void OnAttributeRaised(Object p_sender, EventArgs p_args)
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			EPotionTarget attribute = (p_sender as AttributeChanger).Attribute;
			if (attribute == EPotionTarget.MIGHT)
			{
				selectedDummyCharacter.IncreaseMight();
			}
			else if (attribute == EPotionTarget.MAGIC)
			{
				selectedDummyCharacter.IncreaseMagic();
			}
			else if (attribute == EPotionTarget.PERCEPTION)
			{
				selectedDummyCharacter.IncreasePerception();
			}
			else if (attribute == EPotionTarget.DESTINY)
			{
				selectedDummyCharacter.IncreaseDestiny();
			}
			else if (attribute == EPotionTarget.VITALITY)
			{
				selectedDummyCharacter.IncreaseVitality();
			}
			else if (attribute == EPotionTarget.SPIRIT)
			{
				selectedDummyCharacter.IncreaseSpirit();
			}
			UpdateCharacter();
			UpdateAttributes();
		}

		private void UpdateCharacter()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)selectedDummyCharacter.Class);
				m_nameLabel.text = selectedDummyCharacter.Name;
				String text = staticData.NameKey;
				if (selectedDummyCharacter.Gender == EGender.MALE)
				{
					text += "_M";
				}
				else
				{
					text += "_F";
				}
				m_raceClassLabel.text = LocaManager.GetText(selectedDummyCharacter.GetRaceKey()) + " " + LocaManager.GetText(text);
				m_portrait.spriteName = selectedDummyCharacter.GetPortrait();
				m_body.spriteName = selectedDummyCharacter.GetBodySprite();
			}
		}

		private void UpdateAttributes()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			String arg = "[000000]";
			if (selectedDummyCharacter.GetAttributesToPickLeft() > 0)
			{
				arg = "[008000]";
			}
			m_PointsLeftLabel.text = LocaManager.GetText("GUI_POINTS_LEFT", arg, selectedDummyCharacter.GetAttributesToPickLeft());
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				Attributes classAttributes = selectedDummyCharacter.GetClassAttributes();
				GameConfig game = ConfigManager.Instance.Game;
				m_mightAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT"), LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT_TT", game.HealthPerMight), EPotionTarget.MIGHT, classAttributes.Might, selectedDummyCharacter.BaseAttributes.Might, selectedDummyCharacter, null);
				m_magicAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC"), LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC_TT", game.ManaPerMagic), EPotionTarget.MAGIC, classAttributes.Magic, selectedDummyCharacter.BaseAttributes.Magic, selectedDummyCharacter, null);
				m_perceptionAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION"), LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION_TT"), EPotionTarget.PERCEPTION, classAttributes.Perception, selectedDummyCharacter.BaseAttributes.Perception, selectedDummyCharacter, null);
				m_destinyAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY"), LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY_TT"), EPotionTarget.DESTINY, classAttributes.Destiny, selectedDummyCharacter.BaseAttributes.Destiny, selectedDummyCharacter, null);
				m_vitalityAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY"), LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY_TT", selectedDummyCharacter.GetHPPerVitality()), EPotionTarget.VITALITY, classAttributes.Vitality, selectedDummyCharacter.BaseAttributes.Vitality, selectedDummyCharacter, null);
				m_spiritAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT"), LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT_TT", game.ManaPerSpirit), EPotionTarget.SPIRIT, classAttributes.Spirit, selectedDummyCharacter.BaseAttributes.Spirit, selectedDummyCharacter, null);
				m_health.text = selectedDummyCharacter.BaseAttributes.HealthPoints.ToString();
				m_mana.text = selectedDummyCharacter.BaseAttributes.ManaPoints.ToString();
			}
		}
	}
}
