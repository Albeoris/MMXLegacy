using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using Legacy.Game.IngameManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/Spellbook")]
	public class Spellbook : MonoBehaviour
	{
		[SerializeField]
		private TabController m_categoryTabs;

		[SerializeField]
		private SpellViewPanel m_spellViewPanel;

		[SerializeField]
		private SkillViewPanel m_skillViewPanel;

		[SerializeField]
		private UILabel m_name;

		[SerializeField]
		private UILabel m_class;

		[SerializeField]
		private UISprite m_portraitHead;

		[SerializeField]
		private UISprite m_portraitBody;

		private Boolean m_isOpen;

		private IngameController m_ingameController;

		private Int32 m_characterChangeId;

		private Party m_party;

		public Boolean IsOpen
		{
			get => m_isOpen;
		    set => m_isOpen = value;
		}

		public Party Party => m_party;

	    public void Init(Party p_party, IngameController p_ingameController)
		{
			m_ingameController = p_ingameController;
			m_party = p_party;
			m_skillViewPanel.Init(p_party);
			m_spellViewPanel.Init(p_party);
			m_categoryTabs.TabIndexChanged += OnCategoryTabChanged;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_POTION_USED, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_DEFEND, new EventHandler(OnCharacterAction));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_POTION_USED, new EventHandler(OnCharacterAction));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_DEFEND, new EventHandler(OnCharacterAction));
		}

		private void OnCharacterSelected(Object p_sender, EventArgs p_args)
		{
			if (!m_isOpen)
			{
				return;
			}
			if (m_categoryTabs.CurrentTabIndex == 0)
			{
				m_spellViewPanel.CharacterChanged();
			}
			else
			{
				m_skillViewPanel.CharacterChanged();
			}
			UpdateCharacterData();
		}

		private void UpdateCharacterData()
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			m_name.text = selectedCharacter.Name;
			m_portraitHead.spriteName = selectedCharacter.Portrait;
			m_portraitBody.spriteName = selectedCharacter.Body;
			String text = selectedCharacter.Class.NameKey;
			if (selectedCharacter.Gender == EGender.MALE)
			{
				text += "_M";
			}
			else
			{
				text += "_F";
			}
			m_class.text = LocaManager.GetText(selectedCharacter.Class.RaceKey) + " " + LocaManager.GetText(text);
		}

		private void OnCharacterAction(Object p_sender, EventArgs p_args)
		{
			if (IsOpen)
			{
				Close();
			}
		}

		public void ToggleSpellview()
		{
			if (IsOpen)
			{
				if (m_categoryTabs.CurrentTabIndex == 0)
				{
					Close();
				}
				else
				{
					m_categoryTabs.SelectTab(0, false);
				}
			}
			else
			{
				Open();
				m_categoryTabs.SelectTab(0, true);
			}
		}

		public void ToggleSkillview()
		{
			if (IsOpen)
			{
				if (m_categoryTabs.CurrentTabIndex == 1)
				{
					Close();
				}
				else
				{
					m_categoryTabs.SelectTab(1, false);
				}
			}
			else
			{
				Open();
				m_categoryTabs.SelectTab(1, true);
			}
		}

		private void Open()
		{
			m_party.LockCharacterSelect(new Party.CharacterSelectRequest(CharacterSelectRequest));
			LegacyLogic.Instance.UpdateManager.PartyTurnActor.SetLock(new PartyTurnActor.ReleaseLockRequest(ReleasePartyTurnActorLockRequest));
			LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.SPELLBOOK);
			IsOpen = true;
			m_ingameController.UpdateLogs();
			NGUITools.SetActiveSelf(gameObject, true);
			UpdateCharacterData();
			AudioController.Play("ButtonClickBookOpen");
		}

		private void CharacterSelectRequest(Int32 p_character)
		{
			if (m_party.SelectedCharacter.SkillHandler.HasTemporarySpendPoints())
			{
				m_characterChangeId = p_character;
				String text = LocaManager.GetText("SPELLBOOK_CONFIRM_SKILLPOINT_DISTRIBUTION");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, new PopupRequest.RequestCallback(PopupRequestCharacterChangeCallback));
			}
			else
			{
				m_party.ConfirmCharacterSelect(p_character);
			}
		}

		private void ReleasePartyTurnActorLockRequest()
		{
			if (m_party.SelectedCharacter.SkillHandler.HasTemporarySpendPoints())
			{
				String text = LocaManager.GetText("SPELLBOOK_CONFIRM_SKILLPOINT_DISTRIBUTION");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, new PopupRequest.RequestCallback(PopupRequestReleaseLockCallback));
			}
			else
			{
				ConfirmClose();
			}
		}

		public void Close()
		{
			DragDropManager.Instance.CancelDragAction();
			if (m_party.SelectedCharacter.SkillHandler.HasTemporarySpendPoints())
			{
				String text = LocaManager.GetText("SPELLBOOK_CONFIRM_SKILLPOINT_DISTRIBUTION");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, new PopupRequest.RequestCallback(PopupRequestCloseCallback));
			}
			else
			{
				ConfirmClose();
			}
			AudioController.Play("ButtonClickBookClose");
		}

		public Boolean DelayOtherProcesses(PopupRequest.RequestCallback p_callbackMethod)
		{
			if (m_party.SelectedCharacter.SkillHandler.HasTemporarySpendPoints())
			{
				String text = LocaManager.GetText("SPELLBOOK_CONFIRM_SKILLPOINT_DISTRIBUTION");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, p_callbackMethod);
				return true;
			}
			return false;
		}

		private void PopupRequestCloseCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				ConfirmClose();
			}
		}

		private void PopupRequestTabChangeCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				NGUITools.SetActiveSelf(m_skillViewPanel.gameObject, false);
				NGUITools.SetActiveSelf(m_spellViewPanel.gameObject, true);
				ApplySkillpoints();
				m_spellViewPanel.Open();
			}
			else
			{
				m_categoryTabs.SelectTab(1, false);
			}
		}

		private void PopupRequestCharacterChangeCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				ApplySkillpoints();
				m_party.ConfirmCharacterSelect(m_characterChangeId);
			}
		}

		private void PopupRequestReleaseLockCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				ConfirmClose();
			}
			else
			{
				LegacyLogic.Instance.UpdateManager.PartyTurnActor.CancelReleaseLockRequest();
			}
		}

		public void ConfirmClose()
		{
			IsOpen = false;
			m_party.ReleaseCharacterSelectLock();
			LegacyLogic.Instance.UpdateManager.PartyTurnActor.ReleaseLock();
			m_ingameController.UpdateLogs();
			NGUITools.SetActiveSelf(gameObject, false);
			m_spellViewPanel.Close();
			ApplySkillpoints();
		}

		private void ApplySkillpoints()
		{
			Character selectedCharacter = m_party.SelectedCharacter;
			if (selectedCharacter != null)
			{
				selectedCharacter.SkillHandler.FinalizeTemporarySpendPoints();
			}
		}

		private void OnCategoryTabChanged(Object p_sender, EventArgs p_args)
		{
			if (m_categoryTabs.CurrentTabIndex == 0)
			{
				if (m_party.SelectedCharacter.SkillHandler.HasTemporarySpendPoints())
				{
					String text = LocaManager.GetText("SPELLBOOK_CONFIRM_SKILLPOINT_DISTRIBUTION");
					PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, new PopupRequest.RequestCallback(PopupRequestTabChangeCallback));
				}
				else
				{
					NGUITools.SetActiveSelf(m_skillViewPanel.gameObject, false);
					NGUITools.SetActiveSelf(m_spellViewPanel.gameObject, true);
					m_spellViewPanel.Open();
				}
			}
			else
			{
				NGUITools.SetActiveSelf(m_skillViewPanel.gameObject, true);
				NGUITools.SetActiveSelf(m_spellViewPanel.gameObject, false);
				m_spellViewPanel.Close();
				m_skillViewPanel.Open();
			}
		}

		public void OnPrevPage()
		{
			if (m_categoryTabs.CurrentTabIndex == 0)
			{
				m_spellViewPanel.UpdatePage(m_spellViewPanel.Page - 1);
			}
			else
			{
				m_skillViewPanel.UpdatePage(m_skillViewPanel.Page - 1);
			}
		}

		public void OnNextPage()
		{
			if (m_categoryTabs.CurrentTabIndex == 0)
			{
				m_spellViewPanel.UpdatePage(m_spellViewPanel.Page + 1);
			}
			else
			{
				m_skillViewPanel.UpdatePage(m_skillViewPanel.Page + 1);
			}
		}

		public void ChangeParty(Party p_party)
		{
			m_party = p_party;
			m_skillViewPanel.ChangeParty(p_party);
			m_spellViewPanel.ChangeParty(p_party);
		}

		private enum ECategory
		{
			SPELLS,
			SKILLS
		}
	}
}
