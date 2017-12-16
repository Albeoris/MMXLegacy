using System;
using System.Collections;
using Legacy.Animations;
using Legacy.Audio;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class MamushiAbilityView : AbilityViewMonster
	{
		private Boolean m_isPlaying;

		private Boolean m_OnCommand;

		private Boolean m_OnCommandStart;

		private Boolean m_OnVictory;

		private Boolean m_OnVictoryStart;

		protected override void OnEntityAbilityAdded(Object p_sender, EventArgs p_args)
		{
			base.OnEntityAbilityAdded(p_sender, p_args);
			AbilityEventArgs abilityEventArgs = (AbilityEventArgs)p_args;
			if (abilityEventArgs.Ability.StaticData.NameKey == "MONSTER_ABILITY_PUSH")
			{
				StartCoroutine(PlayLateShakeFX());
				m_isPlaying = true;
				m_OnCommand = false;
				m_OnCommandStart = false;
				m_OnVictory = false;
				m_OnVictoryStart = false;
			}
		}

		private void Update()
		{
			if (m_isPlaying)
			{
				if (!m_OnCommand)
				{
					if (!m_OnCommandStart)
					{
						CommandStart();
					}
					if ((!m_Animation.IsPlaying(EAnimType.ATTACK) || !m_Animation.IsPlaying(EAnimType.ATTACK_CRITICAL)) && !m_Animation.animation.IsPlaying("Command"))
					{
						m_OnCommand = true;
					}
				}
				else if (!m_OnVictory)
				{
					if (!m_OnVictoryStart)
					{
						VictoryStart();
					}
					if ((!m_Animation.IsPlaying(EAnimType.ATTACK) || !m_Animation.IsPlaying(EAnimType.ATTACK_CRITICAL)) && !m_Animation.animation.IsPlaying("Victory"))
					{
						m_OnVictory = true;
						m_isPlaying = false;
					}
				}
			}
		}

		private void CommandStart()
		{
			m_Animation.Play("Command", 2.5f, 1f);
			m_OnCommandStart = true;
		}

		private void VictoryStart()
		{
			m_Animation.Play("Victory", 2.8f, 1f);
			m_OnVictoryStart = true;
		}

		private IEnumerator PlayLateShakeFX()
		{
			yield return new WaitForSeconds(0.3f);
			if (!AudioController.IsPlaying("attack_bark"))
			{
				AudioManager.Instance.RequestPlayAudioID("mamushicommand_bark", 0, -1f, transform, 1f, 0f, 0f, null);
			}
			Vector3 vec = new Vector3(0f, 0.1f, 0f);
			FXMainCamera.Instance.PlayShakeFX(0.1f, vec);
			yield break;
		}
	}
}
