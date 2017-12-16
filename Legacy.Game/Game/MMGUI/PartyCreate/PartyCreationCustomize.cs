using System;
using Legacy.Audio;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.PartyCreate
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationCustomize")]
	public class PartyCreationCustomize : MonoBehaviour
	{
		[SerializeField]
		private UIInput m_nameInput;

		[SerializeField]
		private PortraitSelectButton m_charM1;

		[SerializeField]
		private PortraitSelectButton m_charM2;

		[SerializeField]
		private PortraitSelectButton m_charF1;

		[SerializeField]
		private PortraitSelectButton m_charF2;

		[SerializeField]
		private UITexture m_charPosing;

		[SerializeField]
		private VoiceSelectButton m_voiceHeroic;

		[SerializeField]
		private VoiceSelectButton m_voiceCynical;

		private PartyCreator m_partyCreator;

		private static String m_LastPlayedBark;

		private Boolean m_FirstTime = true;

		private static ERace m_LastRace;

		private static EGender m_LastGender;

		private static EVoice m_LastVoiceSetting;

		public void Init(PartyCreator p_partyCreator)
		{
			m_partyCreator = p_partyCreator;
			m_charM1.Init(EGender.MALE, 1);
			m_charM2.Init(EGender.MALE, 2);
			m_charF1.Init(EGender.FEMALE, 1);
			m_charF2.Init(EGender.FEMALE, 2);
			m_charM1.OnPicSelected += OnCharPicClick;
			m_charM2.OnPicSelected += OnCharPicClick;
			m_charF1.OnPicSelected += OnCharPicClick;
			m_charF2.OnPicSelected += OnCharPicClick;
			m_voiceHeroic.Init(EVoice.HEROIC);
			m_voiceCynical.Init(EVoice.CYNICAL);
			m_voiceHeroic.OnRaceSelected += OnVoiceSelected;
			m_voiceCynical.OnRaceSelected += OnVoiceSelected;
			m_LastPlayedBark = " ";
		}

		public void OnAfterActivate()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Gender == EGender.NOT_SELECTED)
			{
				selectedDummyCharacter.Gender = ((Random.Range(0, 100) < 50) ? EGender.FEMALE : EGender.MALE);
			}
			if (selectedDummyCharacter.PortraitID == 0)
			{
				selectedDummyCharacter.PortraitID = ((Random.Range(0, 100) < 50) ? 2 : 1);
			}
			if (selectedDummyCharacter.Name == String.Empty)
			{
				selectedDummyCharacter.Name = m_partyCreator.GetRandomName(selectedDummyCharacter);
			}
			UpdateButtons();
			UpdateDescription();
		}

		public void UndoSelection()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.Gender = EGender.NOT_SELECTED;
			selectedDummyCharacter.PortraitID = 0;
			selectedDummyCharacter.Name = String.Empty;
			selectedDummyCharacter.CustomName = false;
		}

		public void Cleanup()
		{
			m_charM1.OnPicSelected -= OnCharPicClick;
			m_charM2.OnPicSelected -= OnCharPicClick;
			m_charF1.OnPicSelected -= OnCharPicClick;
			m_charF2.OnPicSelected -= OnCharPicClick;
			m_voiceHeroic.OnRaceSelected -= OnVoiceSelected;
			m_voiceCynical.OnRaceSelected -= OnVoiceSelected;
		}

		public void OnCharPicClick(Object p_sender, EventArgs p_args)
		{
			OnSubmitName(m_nameInput.text);
			EGender gender = (p_sender as PortraitSelectButton).Gender;
			Int32 picIndex = (p_sender as PortraitSelectButton).PicIndex;
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (selectedDummyCharacter.Gender != gender)
			{
				selectedDummyCharacter.Gender = gender;
				if (!selectedDummyCharacter.CustomName)
				{
					selectedDummyCharacter.Name = m_partyCreator.GetRandomName(selectedDummyCharacter);
				}
			}
			selectedDummyCharacter.PortraitID = picIndex;
			SetCharacter(selectedDummyCharacter, false);
			UpdateButtons();
			UpdateDescription();
		}

		public void OnVoiceSelected(Object p_sender, EventArgs p_args)
		{
			OnSubmitName(m_nameInput.text);
			EVoice voice = (p_sender as VoiceSelectButton).Voice;
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			selectedDummyCharacter.Voice = voice;
			SetCharacter(selectedDummyCharacter, false);
			UpdateButtons();
			UpdateDescription();
		}

		private void OnDisable()
		{
			OnSubmitName(m_nameInput.text);
		}

		public void TriggerBark()
		{
			if (AudioController.IsPlaying(m_LastPlayedBark))
			{
				AudioController.Stop(m_LastPlayedBark);
			}
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			ERace race = selectedDummyCharacter.Race;
			EGender gender = selectedDummyCharacter.Gender;
			EVoice voice = selectedDummyCharacter.Voice;
			Int32 num = Random.Range(0, 8);
			Int32 num2 = num;
			if (num == num2)
			{
				if (num < 0)
				{
					num--;
				}
				else
				{
					num++;
				}
			}
			if (m_LastVoiceSetting != voice || m_LastGender != gender || m_LastRace != race)
			{
				m_FirstTime = true;
			}
			if (m_FirstTime)
			{
				num = 9;
				m_FirstTime = false;
			}
			String barkClipname = String.Empty;
			switch (num)
			{
			case 0:
				barkClipname = "Monster";
				break;
			case 1:
				barkClipname = "Trap";
				break;
			case 2:
				barkClipname = "Quest";
				break;
			case 3:
				barkClipname = "Danger";
				break;
			case 4:
				barkClipname = "Secret";
				break;
			case 5:
				barkClipname = "Loot";
				break;
			case 6:
				barkClipname = "Rest";
				break;
			case 7:
				barkClipname = "Explore3";
				break;
			case 8:
				barkClipname = "Explore6";
				break;
			case 9:
				barkClipname = "ChooseMe";
				break;
			}
			String audioID;
			String text;
			GenerateAudioID(selectedDummyCharacter, barkClipname, out text, out audioID);
			AudioManager.Instance.RequestByAudioID(audioID, 0, delegate(AudioRequest a)
			{
				AudioController.Play(audioID);
			});
		}

		private static void GenerateAudioID(DummyCharacter character, String barkClipname, out String category, out String audioID)
		{
			String text = character.Race.ToString().ToLowerInvariant();
			text = Char.ToUpperInvariant(text[0]) + text.Remove(0, 1);
			String text2 = character.Gender.ToString().ToLowerInvariant();
			text2 = Char.ToUpperInvariant(text2[0]) + text2.Remove(0, 1);
			String text3 = character.Voice.ToString().ToLowerInvariant();
			text3 = Char.ToUpperInvariant(text3[0]) + text3.Remove(0, 1);
			category = "Bark" + text + text2 + text3;
			audioID = String.Concat(new String[]
			{
				text,
				"_",
				text2,
				"_",
				text3,
				"_",
				barkClipname
			});
			m_LastRace = character.Race;
			m_LastGender = character.Gender;
			m_LastVoiceSetting = character.Voice;
			m_LastPlayedBark = audioID;
		}

		public void OnSubmitName(String p_name)
		{
			String text = p_name.Replace(" ", String.Empty).Replace("\t", String.Empty);
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			if (p_name.Length == 0 || text.Length == 0)
			{
				m_nameInput.text = selectedDummyCharacter.Name;
				SetCharacter(selectedDummyCharacter, false);
			}
			else
			{
				selectedDummyCharacter.CustomName = true;
				selectedDummyCharacter.Name = p_name;
				SetCharacter(selectedDummyCharacter, false);
			}
		}

		private void SetCharacter(DummyCharacter p_character, Boolean p_resetScrollbar)
		{
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.CONDITIONS);
			LegacyLogic.Instance.EventManager.InvokeEvent(p_character, EEventType.DUMMY_CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		private void UpdateButtons()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			m_charM1.UpdateTexture(selectedDummyCharacter.Race, selectedDummyCharacter.Class);
			m_charM2.UpdateTexture(selectedDummyCharacter.Race, selectedDummyCharacter.Class);
			m_charF1.UpdateTexture(selectedDummyCharacter.Race, selectedDummyCharacter.Class);
			m_charF2.UpdateTexture(selectedDummyCharacter.Race, selectedDummyCharacter.Class);
			m_charM1.SetSelected(selectedDummyCharacter.Gender, selectedDummyCharacter.PortraitID);
			m_charM2.SetSelected(selectedDummyCharacter.Gender, selectedDummyCharacter.PortraitID);
			m_charF1.SetSelected(selectedDummyCharacter.Gender, selectedDummyCharacter.PortraitID);
			m_charF2.SetSelected(selectedDummyCharacter.Gender, selectedDummyCharacter.PortraitID);
			m_voiceHeroic.SetSelected(selectedDummyCharacter.Voice);
			m_voiceCynical.SetSelected(selectedDummyCharacter.Voice);
		}

		private void UpdateDescription()
		{
			DummyCharacter selectedDummyCharacter = m_partyCreator.GetSelectedDummyCharacter();
			m_nameInput.text = selectedDummyCharacter.Name;
			if (selectedDummyCharacter.Class != EClass.NONE)
			{
				CharacterClassStaticData staticData = StaticDataHandler.GetStaticData<CharacterClassStaticData>(EDataType.CHARACTER_CLASS, (Int32)selectedDummyCharacter.Class);
				String text = "CharacterPosings/" + staticData.PosingBase;
				if (selectedDummyCharacter.Gender == EGender.MALE)
				{
					text = text + "_male_" + selectedDummyCharacter.PortraitID;
				}
				else
				{
					text = text + "_female_" + selectedDummyCharacter.PortraitID;
				}
				Texture mainTexture = m_charPosing.mainTexture;
				mainTexture.UnloadAsset();
				Texture mainTexture2 = Helper.ResourcesLoad<Texture2D>(text, false);
				m_charPosing.mainTexture = mainTexture2;
			}
		}
	}
}
