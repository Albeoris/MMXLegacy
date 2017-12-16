using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.UpdateLogic;
using Legacy.Core.WorldMap;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellSpiritBeaconPopup")]
	public class SpellSpiritBeaconPopup : MonoBehaviour, IIngameContext
	{
		private Boolean m_isActive;

		private Boolean m_isForbiddenLocation;

		[SerializeField]
		private UILabel m_textLabel;

		[SerializeField]
		private GameObject m_TravelButton;

		[SerializeField]
		private GameObject m_middleButton;

		[SerializeField]
		private GameObject m_setPointButton;

		[SerializeField]
		private UILabel m_middleButtonLabel;

		public event EventHandler CloseSpellSpiritBeaconPopup;

		public SpiritBeaconEventArgs.EResult Result { get; private set; }

		public CharacterSpell Spell { get; set; }

		public Scroll Scroll { get; set; }

		private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCancelAction));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCancelAction));
		}

		private void OnCancelButtonClicked()
		{
			Result = SpiritBeaconEventArgs.EResult.CANCEL;
			Close();
		}

		private void OnCancelAction(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_isActive && CloseSpellSpiritBeaconPopup != null)
			{
				Result = SpiritBeaconEventArgs.EResult.CANCEL;
				Close();
			}
		}

		private void OnTravelSelected()
		{
			Result = SpiritBeaconEventArgs.EResult.TRAVEL;
			CastSpell();
			Close();
		}

		private void OnSetPointSelected()
		{
			Result = SpiritBeaconEventArgs.EResult.SET_POINT;
			CastSpell();
			Close();
		}

		private void OnClickMiddleButton()
		{
			if (m_isForbiddenLocation)
			{
				Result = SpiritBeaconEventArgs.EResult.CANCEL;
				Close();
			}
			else
			{
				OnSetPointSelected();
			}
		}

		private void CastSpell()
		{
			if (Scroll != null)
			{
				CharacterSpell p_spell = SpellFactory.CreateCharacterSpell((ECharacterSpell)Scroll.SpellID);
				CastSpellCommand p_command = new CastSpellCommand(p_spell, null, Scroll);
				LegacyLogic.Instance.CommandManager.AddCommand(p_command);
			}
			else
			{
				CastSpellCommand p_command2 = new CastSpellCommand(Spell, null, null);
				LegacyLogic.Instance.CommandManager.AddCommand(p_command2);
			}
			if (Result == SpiritBeaconEventArgs.EResult.SET_POINT)
			{
				LegacyLogic.Instance.WorldManager.SpiritBeaconController.Action = ESpiritBeaconAction.SET_POINT;
			}
			else if (Result == SpiritBeaconEventArgs.EResult.TRAVEL)
			{
				LegacyLogic.Instance.WorldManager.SpiritBeaconController.Action = ESpiritBeaconAction.TRAVEL;
			}
		}

		private void InitForForbiddenMap()
		{
			m_isForbiddenLocation = true;
			m_middleButton.SetActive(true);
			m_middleButtonLabel.text = LocaManager.GetText("POPUP_REQUEST_OK");
			m_TravelButton.SetActive(false);
			m_setPointButton.SetActive(false);
			m_textLabel.text = LocaManager.GetText("SPIRIT_BEACON_POPUP_FORBIDDEN_MAP");
		}

		private void InitForForbiddenQuest()
		{
			m_isForbiddenLocation = true;
			m_middleButton.SetActive(true);
			m_middleButtonLabel.text = LocaManager.GetText("POPUP_REQUEST_OK");
			m_TravelButton.SetActive(false);
			m_setPointButton.SetActive(false);
			m_textLabel.text = LocaManager.GetText("SPIRIT_BEACON_POPUP_FORBIDDEN_QUEST");
		}

		private void Close()
		{
			if (CloseSpellSpiritBeaconPopup != null)
			{
				CloseSpellSpiritBeaconPopup(this, EventArgs.Empty);
			}
		}

		public void Activate()
		{
			Result = SpiritBeaconEventArgs.EResult.CANCEL;
			m_isActive = true;
			DragDropManager.Instance.CancelDragAction();
			NGUITools.SetActive(gameObject, true);
			if (LegacyLogic.Instance.MapLoader.Grid.Type == EMapType.DUNGEON)
			{
				InitForForbiddenMap();
				return;
			}
			if (LegacyLogic.Instance.MapLoader.Grid.Name == "SummerPalace_1")
			{
				InitForForbiddenMap();
				return;
			}
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(68);
			if (step != null && step.QuestState == EQuestState.ACTIVE)
			{
				InitForForbiddenQuest();
				return;
			}
			Boolean existent = LegacyLogic.Instance.WorldManager.SpiritBeaconController.Existent;
			String text = LocaManager.GetText(LegacyLogic.Instance.WorldManager.SpiritBeaconController.SpiritBeacon.LocalizedMapnameKey);
			if (text.LastIndexOf('@') != -1)
			{
				text = text.Remove(text.LastIndexOf('@'));
			}
			m_TravelButton.SetActive(existent);
			m_setPointButton.SetActive(existent);
			m_middleButtonLabel.text = LocaManager.GetText("POPUP_REQUEST_SPIRIT_BEACON_NEW");
			m_middleButton.SetActive(!existent);
			if (existent)
			{
				m_textLabel.text = LocaManager.GetText("POPUP_REQUEST_SPIRIT_BEACON_TEXT", text);
			}
			else
			{
				m_textLabel.text = LocaManager.GetText("POPUP_REQUEST_SPIRIT_BEACON_NO_POINT_SET");
			}
		}

		public void Deactivate()
		{
			m_isActive = false;
			m_isForbiddenLocation = false;
			NGUITools.SetActive(gameObject, false);
		}
	}
}
