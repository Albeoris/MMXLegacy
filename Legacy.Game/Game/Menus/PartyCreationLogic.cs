using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Game.Context;
using Legacy.Game.MMGUI;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/PartyCreationLogic")]
	public class PartyCreationLogic : MonoBehaviour
	{
		[SerializeField]
		private PartyCreationSelectDifficulty m_partySelectDifficulty;

		[SerializeField]
		private PartyCreationSelectMode m_partySelectMode;

		[SerializeField]
		private PartyCreationCustom m_partyCreationCustom;

		[SerializeField]
		private PartyCreationSummary m_partyCreationSummary;

		[SerializeField]
		private UICamera m_camera;

		private void Awake()
		{
			ActivateStep(ECreationStep.STEP_DIFFICULTY);
			m_camera.tooltipDelay = Mathf.Abs(ConfigManager.Instance.Options.TooltipDelay);
		}

		public void OnBackClick()
		{
			LegacyLogic.Instance.WorldManager.ClearAndDestroy();
			Cleanup();
			ContextManager.ChangeContext(EContext.Mainmenu);
		}

		private void ActivateStep(ECreationStep p_step)
		{
			NGUITools.SetActive(m_partySelectDifficulty.gameObject, p_step == ECreationStep.STEP_DIFFICULTY);
			NGUITools.SetActive(m_partySelectMode.gameObject, p_step == ECreationStep.STEP_SELECT_MODE);
			NGUITools.SetActive(m_partyCreationCustom.gameObject, p_step == ECreationStep.STEP_CREATE_CUSTOM);
			NGUITools.SetActive(m_partyCreationSummary.gameObject, p_step == ECreationStep.STEP_SUMMARY);
			if (p_step == ECreationStep.STEP_CREATE_CUSTOM)
			{
				m_partyCreationCustom.Init();
			}
		}

		private void Cleanup()
		{
			m_partySelectDifficulty.Cleanup();
			m_partySelectMode.Cleanup();
			m_partyCreationCustom.Cleanup();
			m_partyCreationSummary.Cleanup();
		}

		public void OnSelectDiffAdventurer()
		{
			SetDifficulty(EDifficulty.NORMAL);
		}

		public void OnSelectDiffWarrior()
		{
			SetDifficulty(EDifficulty.HARD);
		}

		private void SetDifficulty(EDifficulty p_difficulty)
		{
			LegacyLogic.Instance.WorldManager.Difficulty = p_difficulty;
			ActivateStep(ECreationStep.STEP_SELECT_MODE);
		}

		public void OnDefaultParty()
		{
			LegacyLogic.Instance.WorldManager.PartyCreator.InitManualCreation();
			LegacyLogic.Instance.WorldManager.PartyCreator.CreateDefaultParty();
			m_partyCreationSummary.Init(false, LegacyLogic.Instance.WorldManager.PartyCreator);
			ActivateStep(ECreationStep.STEP_SUMMARY);
		}

		public void OnCustomParty()
		{
			LegacyLogic.Instance.WorldManager.PartyCreator.InitManualCreation();
			ActivateStep(ECreationStep.STEP_CREATE_CUSTOM);
		}

		public void OnCustomDone()
		{
			m_partyCreationSummary.Init(false, LegacyLogic.Instance.WorldManager.PartyCreator);
			ActivateStep(ECreationStep.STEP_SUMMARY);
		}

		public void OnRandomParty()
		{
			LegacyLogic.Instance.WorldManager.PartyCreator.InitManualCreation();
			LegacyLogic.Instance.WorldManager.PartyCreator.RandomizeParty();
			m_partyCreationSummary.Init(true, LegacyLogic.Instance.WorldManager.PartyCreator);
			ActivateStep(ECreationStep.STEP_SUMMARY);
		}

		public void OnBackToCustom()
		{
			ActivateStep(ECreationStep.STEP_CREATE_CUSTOM);
		}

		public void OnPartyCreationDone()
		{
			LegacyLogic.Instance.WorldManager.PartyCreator.FinalizePartyCreation();
			LegacyLogic.Instance.WorldManager.PartyCreator.GiveStartSetup();
			LegacyLogic.Instance.WorldManager.CheckPassableOnMovement = false;
			Cleanup();
			ContextManager.ChangeContext(EContext.Intro);
		}

		public void OnBackToMain()
		{
			Cleanup();
			LegacyLogic.Instance.WorldManager.ClearAndDestroy();
			ContextManager.ChangeContext(EContext.Mainmenu);
		}

		public void OnBackToDiff()
		{
			ActivateStep(ECreationStep.STEP_DIFFICULTY);
		}

		public void OnBackToMode()
		{
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("CONFRIM_BACK_TO_MAIN_FROM_PARTYCREATION"), new PopupRequest.RequestCallback(BackToMode));
		}

		private void BackToMode(PopupRequest.EResultType p_result, String p_text)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				m_partyCreationCustom.Cleanup();
				ActivateStep(ECreationStep.STEP_SELECT_MODE);
			}
		}

		private enum ECreationStep
		{
			STEP_DIFFICULTY,
			STEP_SELECT_MODE,
			STEP_CREATE_CUSTOM,
			STEP_SUMMARY
		}
	}
}
