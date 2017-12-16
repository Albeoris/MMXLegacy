using System;
using System.Collections;
using System.Threading;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Game.IngameManagement;
using Legacy.MMGUI;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/DialogEntryView")]
	public class DialogEntryView : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_text;

		[SerializeField]
		private GUIMultiColorSpriteButton m_button;

		[SerializeField]
		private Int32 m_index;

		private Boolean m_enabled;

		private Boolean m_isBackButton;

		private Boolean m_isFeatureButton;

		private Boolean m_isNextPageButton;

		private Int32 m_backToId;

		private DialogEntry m_entry;

		private DialogFunction m_featureFunction;

		private AudioRequest m_currentAudioRequest;

		private AudioObject m_currentAudioObject;

		public event EventHandler ClickedBackButton;

		public event EventHandler ClickedNextPageButton;

		public Boolean IsVisible => (m_entry != null && m_entry.State != EDialogState.HIDDEN) || m_isBackButton || m_isFeatureButton || m_isNextPageButton;

	    public Boolean IsBackButton => m_isBackButton;

	    public Boolean IsNextPageButton => m_isNextPageButton;

	    public Int32 BackToDialogId => m_backToId;

	    public void SetEntry(DialogEntry p_entry, GameObject p_dialogView)
		{
			m_entry = p_entry;
			if (p_entry.Text != null)
			{
				m_text.text = m_index + ". " + LocaManager.GetText(p_entry.Text.LocaKey);
			}
			else
			{
				m_text.text = "no text found";
			}
			if (p_entry.State == EDialogState.DISABLED)
			{
				m_enabled = false;
				m_button.IsEnabled = false;
			}
		}

		public void SetBackFunction(Int32 p_backToId)
		{
			m_backToId = p_backToId;
			m_isBackButton = true;
			m_text.text = m_index + ". " + LocaManager.GetText("DIALOG_OPTION_BACK");
		}

		public void SetNextPageFunction()
		{
			m_isNextPageButton = true;
			m_text.text = m_index + ". " + LocaManager.GetText("DIALOG_OPTION_NEXT_PAGE");
		}

		public void SetFeature(String p_text, Boolean p_enabled, DialogFunction p_function)
		{
			m_isFeatureButton = true;
			m_featureFunction = p_function;
			m_text.text = m_index + ". " + p_text;
			if (!p_enabled)
			{
				m_enabled = false;
				m_button.IsEnabled = false;
			}
		}

		public void Reset()
		{
			m_entry = null;
			m_text.text = null;
			m_isBackButton = false;
			m_backToId = 0;
			m_isFeatureButton = false;
			m_isNextPageButton = false;
			m_featureFunction = null;
			m_enabled = true;
			m_button.IsEnabled = true;
		}

		public void OnClickedEntry(GameObject p_sender)
		{
			if (m_enabled && IngameController.Instance.Overlay.BackAlpha == 0f)
			{
				if (m_isBackButton)
				{
					if (ClickedBackButton != null)
					{
						ClickedBackButton(this, EventArgs.Empty);
					}
				}
				else if (m_isNextPageButton)
				{
					if (ClickedNextPageButton != null)
					{
						ClickedNextPageButton(this, EventArgs.Empty);
					}
				}
				else if (m_isFeatureButton)
				{
					m_featureFunction.Trigger(LegacyLogic.Instance.ConversationManager);
				}
				else if (m_entry != null)
				{
					if (m_entry.Text != null && !String.IsNullOrEmpty(m_entry.Text.VoiceID))
					{
						PlayVoiceOver(m_entry.Text.VoiceID);
					}
					m_entry.ExecuteFunction(LegacyLogic.Instance.ConversationManager);
				}
			}
		}

		private void PlayVoiceOver(String p_voiceAudioID)
		{
			if (m_currentAudioRequest != null && m_currentAudioRequest.IsLoading)
			{
				m_currentAudioRequest.AbortLoad();
			}
			if (m_currentAudioObject != null)
			{
				if (m_currentAudioObject.IsPlaying())
				{
					m_currentAudioObject.Stop();
				}
				AudioManager.Instance.UnloadByAudioID(m_currentAudioObject.audioID);
				m_currentAudioObject = null;
			}
			if (!AudioManager.Instance.IsValidAudioID(p_voiceAudioID))
			{
				Debug.LogError("Dialog: Unknow voice audioID '" + p_voiceAudioID + "'!");
				return;
			}
			m_currentAudioRequest = AudioManager.Instance.RequestByAudioID(p_voiceAudioID, 100, delegate(AudioRequest a)
			{
				if (a.IsDone && a.Controller != null)
				{
					m_currentAudioObject = AudioController.Play(p_voiceAudioID);
					StopAllCoroutines();
					StartCoroutine(UnloadVoiceOver());
				}
			});
		}

		private IEnumerator UnloadVoiceOver()
		{
			while (m_currentAudioObject != null)
			{
				if (!m_currentAudioObject.IsPlaying())
				{
					AudioManager.Instance.UnloadByAudioID(m_currentAudioObject.audioID);
					m_currentAudioObject = null;
					break;
				}
				yield return null;
			}
			yield break;
		}
	}
}
