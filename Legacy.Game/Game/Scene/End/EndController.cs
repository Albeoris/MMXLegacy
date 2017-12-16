using System;
using System.Collections;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Quests;
using Legacy.Core.StaticData;
using Legacy.Core.Utilities.StateManagement;
using Legacy.Game.Context;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Scene.End
{
	public class EndController : MonoBehaviour
	{
		[SerializeField]
		private UITexture m_texture;

		[SerializeField]
		private UILabel m_text;

		[SerializeField]
		private Single m_startDelayTime = 2f;

		[SerializeField]
		private Single m_fadeinBackgroundTime = 1.5f;

		[SerializeField]
		private Single m_fadeinTextTime = 1.5f;

		[SerializeField]
		private Single m_fadeoutDelay = 3f;

		[SerializeField]
		private Single m_fadeoutTime = 1.5f;

		private TimeStateMachine<EState> m_state;

		private IEnumerator m_iterator;

		private AudioRequest m_currentAudioRequest;

		private AudioObject m_currentAudio;

		private EndingSlidesStaticData m_currentData;

		private void Start()
		{
			m_texture.alpha = 0f;
			m_text.alpha = 0f;
			m_state = new TimeStateMachine<EState>();
			m_state.AddState(new TimeState<EState>(EState.START_DELAY, m_startDelayTime, new State<EState, Transition<EState>>.StateUpdateMethod(StateStartDelay)));
			m_state.AddState(new TimeState<EState>(EState.FADEIN_BACKGROUND, m_fadeinBackgroundTime, new State<EState, Transition<EState>>.StateUpdateMethod(StateFadeinBackground)));
			m_state.AddState(new TimeState<EState>(EState.FADEIN_TEXT, m_fadeinTextTime, new State<EState, Transition<EState>>.StateUpdateMethod(StateFadeinText)));
			m_state.AddState(new TimeState<EState>(EState.ACTIVE, 0f, new State<EState, Transition<EState>>.StateUpdateMethod(StateActive)));
			m_state.AddState(new TimeState<EState>(EState.FADEOUT_DELAY, m_fadeoutDelay, new State<EState, Transition<EState>>.StateUpdateMethod(StateFadeoutDelay)));
			m_state.AddState(new TimeState<EState>(EState.FADEOUT, m_fadeoutTime, new State<EState, Transition<EState>>.StateUpdateMethod(StateFadeout)));
			m_state.ChangeState(EState.START_DELAY);
			m_iterator = StaticDataHandler.GetIterator<EndingSlidesStaticData>(EDataType.ENDING_SLIDES).GetEnumerator();
			if (LegacyLogic.Instance.WorldManager.IsShowingDLCEndingSequences)
			{
				MoveToDLCSlides();
			}
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
		}

		private void Update()
		{
			m_state.Update(Time.deltaTime);
		}

		private void OnCloseKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_state.IsState(EState.ACTIVE))
			{
				if (m_currentAudioRequest != null)
				{
					m_currentAudioRequest.AbortLoad();
				}
				if (m_currentAudio != null)
				{
					m_currentAudio.Stop();
				}
				m_state.ChangeState(EState.FADEOUT);
			}
		}

		private void StateStartDelay()
		{
			if (m_state.IsStateTimeout)
			{
				SetNextSlide();
			}
		}

		private void StateFadeinBackground()
		{
			m_texture.alpha = m_state.CurrentStateTimePer;
			if (m_state.IsStateTimeout)
			{
				m_state.ChangeState(EState.FADEIN_TEXT);
			}
		}

		private void StateFadeinText()
		{
			m_text.alpha = m_state.CurrentStateTimePer;
			if (m_state.IsStateTimeout)
			{
				if (!AudioManager.Instance.IsValidAudioID(m_currentData.VoiceFile))
				{
					Debug.LogError("EndController: Unknow voice audioID '" + m_currentData.VoiceFile + "'!");
					m_state.ChangeState(EState.FADEOUT_DELAY);
					return;
				}
				m_currentAudio = null;
				m_state.ChangeState(EState.ACTIVE);
				m_currentAudioRequest = AudioManager.Instance.RequestByAudioID(m_currentData.VoiceFile, 100, delegate(AudioRequest aReq)
				{
					if (aReq.IsDone && aReq.Controller != null)
					{
						m_currentAudio = AudioController.Play(m_currentData.VoiceFile);
						if (m_currentAudio == null)
						{
							m_state.ChangeState(EState.FADEOUT_DELAY);
						}
					}
					else
					{
						m_state.ChangeState(EState.FADEOUT_DELAY);
					}
				});
			}
		}

		private void StateActive()
		{
			if (m_currentAudio != null && !m_currentAudio.IsPlaying())
			{
				m_state.ChangeState(EState.FADEOUT_DELAY);
			}
		}

		private void StateFadeoutDelay()
		{
			if (m_state.IsStateTimeout)
			{
				m_state.ChangeState(EState.FADEOUT);
			}
		}

		private void StateFadeout()
		{
			m_texture.alpha = 1f - m_state.CurrentStateTimePer;
			if (m_state.IsStateTimeout)
			{
				m_text.alpha = 0f;
				SetNextSlide();
			}
		}

		private void SetNextSlide()
		{
			if (m_iterator.MoveNext())
			{
				m_currentData = (EndingSlidesStaticData)m_iterator.Current;
				if (LegacyLogic.Instance.WorldManager.IsShowingEndingSequences && m_currentData.DLC)
				{
					ContextManager.ChangeContext(EContext.CreditsScreen);
					return;
				}
				if (CheckCondition(m_currentData))
				{
					m_text.text = LocaManager.GetText(m_currentData.TextKey);
					Texture texture = Helper.ResourcesLoad<Texture>("EndingSlides/" + m_currentData.Image);
					if (m_texture.mainTexture != texture)
					{
						Texture mainTexture = m_texture.mainTexture;
						m_texture.mainTexture = texture;
						if (mainTexture != null)
						{
							mainTexture.UnloadAsset();
						}
					}
					m_state.ChangeState(EState.FADEIN_BACKGROUND);
				}
				else
				{
					SetNextSlide();
				}
			}
			else
			{
				ContextManager.ChangeContext(EContext.Diploma);
			}
		}

		private Boolean CheckCondition(EndingSlidesStaticData p_data)
		{
			Boolean flag = true;
			if (p_data.Quest > 0)
			{
				flag &= (LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(p_data.Quest).QuestState == EQuestState.SOLVED);
			}
			if (p_data.Tokens.Length > 0)
			{
				foreach (Int32 p_id in p_data.Tokens)
				{
					flag &= (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(p_id) > 0);
				}
			}
			return flag == p_data.Result;
		}

		private void MoveToDLCSlides()
		{
			try
			{
				Int32 num = 0;
				m_iterator.Reset();
				while (m_iterator.MoveNext())
				{
					EndingSlidesStaticData endingSlidesStaticData = (EndingSlidesStaticData)m_iterator.Current;
					if (!endingSlidesStaticData.DLC)
					{
						num++;
					}
				}
				num--;
				m_iterator.Reset();
				for (Int32 i = 0; i < num; i++)
				{
					m_iterator.MoveNext();
				}
			}
			catch (UnityException)
			{
			}
		}

		private enum EState
		{
			START_DELAY,
			FADEIN_BACKGROUND,
			FADEIN_TEXT,
			ACTIVE,
			FADEOUT_DELAY,
			FADEOUT
		}
	}
}
