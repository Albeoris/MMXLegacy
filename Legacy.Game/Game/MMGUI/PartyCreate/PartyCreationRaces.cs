using System;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.PartyCreate
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationRaces")]
	public class PartyCreationRaces : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_raceDesc;

		[SerializeField]
		private UILabel m_abilitiesCaption;

		[SerializeField]
		private UILabel m_raceHeader;

		[SerializeField]
		private UILabel[] m_abilities;

		[SerializeField]
		private UILabel[] m_abilitiesHeaders;

		[SerializeField]
		private UISprite[] m_abilityIcons;

		[SerializeField]
		private UISprite[] m_borders;

		[SerializeField]
		private RaceSelectButton m_btnHuman;

		[SerializeField]
		private RaceSelectButton m_btnElves;

		[SerializeField]
		private RaceSelectButton m_btnDwarfs;

		[SerializeField]
		private RaceSelectButton m_btnOrcs;

		[SerializeField]
		private UISprite m_textBG;

		[SerializeField]
		private UISprite m_textBorder;

		[SerializeField]
		private GameObject m_textRoot;

		private PartyCreator m_partyCreator;

		public void Init(PartyCreator p_partyCreator)
		{
			m_partyCreator = p_partyCreator;
			m_btnHuman.Init(ERace.HUMAN);
			m_btnElves.Init(ERace.ELF);
			m_btnDwarfs.Init(ERace.DWARF);
			m_btnOrcs.Init(ERace.ORC);
			m_btnHuman.OnRaceSelected += OnRaceSelected;
			m_btnElves.OnRaceSelected += OnRaceSelected;
			m_btnDwarfs.OnRaceSelected += OnRaceSelected;
			m_btnOrcs.OnRaceSelected += OnRaceSelected;
		}

		public void OnAfterActivate()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Race == ERace.NONE)
			{
				selectedDummyCharacter.Race = m_partyCreator.GetUnusedRace();
			}
			UpdateDescription();
			UpdateButtons();
		}

		public void UndoSelection()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.Race = ERace.NONE;
		}

		public void Cleanup()
		{
			m_btnHuman.OnRaceSelected -= OnRaceSelected;
			m_btnElves.OnRaceSelected -= OnRaceSelected;
			m_btnDwarfs.OnRaceSelected -= OnRaceSelected;
			m_btnOrcs.OnRaceSelected -= OnRaceSelected;
		}

		private void OnRaceSelected(Object p_sender, EventArgs p_args)
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			ERace race = (p_sender as RaceSelectButton).Race;
			if (race != selectedDummyCharacter.Race)
			{
				selectedDummyCharacter.Race = race;
				selectedDummyCharacter.Class = EClass.NONE;
			}
			UpdateButtons();
			UpdateDescription();
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.CONDITIONS);
			LegacyLogic.Instance.EventManager.InvokeEvent(selectedDummyCharacter, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		private void UpdateButtons()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			m_btnHuman.SetSelected(selectedDummyCharacter.Race);
			m_btnElves.SetSelected(selectedDummyCharacter.Race);
			m_btnDwarfs.SetSelected(selectedDummyCharacter.Race);
			m_btnOrcs.SetSelected(selectedDummyCharacter.Race);
		}

		private void UpdateDescription()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			m_raceHeader.text = LocaManager.GetText(selectedDummyCharacter.GetRaceKey());
			m_raceDesc.text = LocaManager.GetText(selectedDummyCharacter.GetRaceKey() + "_DESCRIPTION");
			Int32 num = UpdateAbilities(selectedDummyCharacter.Race);
			Single num2 = m_raceDesc.relativeSize.y * m_raceDesc.transform.localScale.y;
			Vector3 localPosition = m_raceDesc.transform.localPosition;
			m_abilitiesCaption.transform.localPosition = new Vector3(m_abilitiesCaption.transform.localPosition.x, localPosition.y - num2 - 50f, m_abilitiesCaption.transform.localPosition.z);
			num2 = m_abilitiesCaption.relativeSize.y * m_abilitiesCaption.transform.localScale.y;
			Single num3 = m_abilitiesCaption.transform.localPosition.y - num2 - 30f;
			Single y = m_abilitiesHeaders[0].cachedTransform.localScale.y;
			for (Int32 i = 0; i < num; i++)
			{
				m_abilitiesHeaders[i].transform.localPosition = new Vector3(m_abilitiesHeaders[i].transform.localPosition.x, num3 + 10f, m_abilitiesHeaders[i].transform.localPosition.z);
				m_abilities[i].transform.localPosition = new Vector3(m_abilities[i].transform.localPosition.x, m_abilitiesHeaders[i].transform.localPosition.y - y, m_abilities[i].transform.localPosition.z);
				m_abilityIcons[i].transform.localPosition = new Vector3(m_abilityIcons[i].transform.localPosition.x, num3, m_abilityIcons[i].transform.localPosition.z);
				m_borders[i].transform.localPosition = new Vector3(m_abilityIcons[i].transform.localPosition.x - 8f, num3 + 8f, m_borders[i].transform.localPosition.z);
				Single val = m_abilities[i].relativeSize.y * m_abilities[i].transform.localScale.y;
				Single y2 = m_borders[i].transform.localScale.y;
				Single num4 = Math.Max(val, y2) + 20f;
				num3 -= num4;
			}
			Single num5 = Math.Abs(m_borders[num - 1].transform.localPosition.y) + m_borders[num - 1].transform.localScale.y + 20f;
			m_textBG.transform.localScale = new Vector3(m_textBG.transform.localScale.x, num5, m_textBG.transform.localScale.z);
			m_textBorder.transform.localScale = new Vector3(m_textBorder.transform.localScale.x, num5, m_textBorder.transform.localScale.z);
			m_textRoot.transform.localPosition = new Vector3(m_textRoot.transform.localPosition.x, 434f - (760f - num5) / 2f, m_textRoot.transform.localPosition.z);
		}

		private Int32 UpdateAbilities(ERace p_race)
		{
			for (Int32 i = 0; i < 3; i++)
			{
				m_abilities[i].text = String.Empty;
				m_abilitiesHeaders[i].text = String.Empty;
				NGUITools.SetActive(m_abilityIcons[i].gameObject, false);
				NGUITools.SetActive(m_borders[i].gameObject, false);
			}
			Int32 num = 0;
			List<RacialAbilitiesStaticData> list = new List<RacialAbilitiesStaticData>(StaticDataHandler.GetIterator<RacialAbilitiesStaticData>(EDataType.RACIAL_ABILITIES));
			foreach (RacialAbilitiesStaticData racialAbilitiesStaticData in list)
			{
				if (racialAbilitiesStaticData.Race == p_race)
				{
					m_abilities[num].text = racialAbilitiesStaticData.GetDescription();
					m_abilitiesHeaders[num].text = LocaManager.GetText(racialAbilitiesStaticData.NameKey);
					m_abilityIcons[num].spriteName = racialAbilitiesStaticData.Icon;
					NGUITools.SetActive(m_abilityIcons[num].gameObject, true);
					NGUITools.SetActive(m_borders[num].gameObject, true);
					num++;
				}
			}
			return num;
		}
	}
}
