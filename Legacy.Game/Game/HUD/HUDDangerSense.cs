using System;
using System.Collections;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.Loading;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDDangerSense")]
	public class HUDDangerSense : MonoBehaviour
	{
		private const Single SPRITE_FADE_DURATION = 0.4f;

		private const Single PARTICLE_SYSTEM_CLEAR_DELAY = 0.25f;

		private static readonly WaitForSeconds WAIT = new WaitForSeconds(0.25f);

		private static readonly WaitForEndOfFrame WAIT_FRAME = new WaitForEndOfFrame();

		[SerializeField]
		private UISprite m_CloseSprite;

		[SerializeField]
		private UISpriteAnimation m_CloseAnim;

		[SerializeField]
		private String m_CloseAnimName;

		[SerializeField]
		private String m_OpenAnimName;

		[SerializeField]
		private GameObject m_YellowGO;

		[SerializeField]
		private GameObject m_RedGO;

		[SerializeField]
		private ParticleSystem m_DefaultLight;

		private String m_closeSpriteInitialName = String.Empty;

		private GameObject m_enabledGO;

		private Int32 m_lastUpdateFrame = -1;

		private void Start()
		{
			m_closeSpriteInitialName = m_CloseSprite.spriteName;
			DisableFX(m_YellowGO, true, true);
			DisableFX(m_RedGO, true, true);
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.UPDATE_HUD_DANGER_SENSE, new EventHandler(OnUpdateNeeded));
		}

		private void OnDestroy()
		{
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.UPDATE_HUD_DANGER_SENSE, new EventHandler(OnUpdateNeeded));
		}

		private void OnEnable()
		{
			DisableAllFX();
			OnUpdateNeeded(this, EventArgs.Empty);
		}

		private void DisableAllFX()
		{
			m_CloseSprite.spriteName = m_closeSpriteInitialName;
			m_CloseSprite.MakePixelPerfect();
			m_CloseAnim.namePrefix = m_CloseAnimName;
			PlayAnim(m_CloseAnimName);
			DisableFX(m_YellowGO, true, true);
			DisableFX(m_RedGO, true, true);
		}

		private void OnUpdateNeeded(Object p_sender, EventArgs p_args)
		{
			if (m_lastUpdateFrame == Time.frameCount)
			{
				return;
			}
			m_lastUpdateFrame = Time.frameCount;
			if (enabled && gameObject.activeInHierarchy)
			{
				StopCoroutine("UpdateWhenStable");
				StartCoroutine("UpdateWhenStable");
			}
		}

		private IEnumerator UpdateWhenStable()
		{
			yield return WAIT_FRAME;
			yield return WAIT_FRAME;
			if (IsLoadingScreen())
			{
				DisableAllFX();
			}
			while (IsLoadingScreen())
			{
				yield return WAIT_FRAME;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.HasAggro())
			{
				ShowRed();
			}
			else if (party.IsSensingDanger())
			{
				ShowYellow();
			}
			else
			{
				Close();
			}
			yield break;
		}

		private Boolean IsLoadingScreen()
		{
			return LegacyLogic.Instance.MapLoader.IsLoading || !SceneLoader.Instance.IsDone;
		}

		private void Close()
		{
			PlayAnim(m_CloseAnimName);
			DisableFX(m_YellowGO, false, true);
			DisableFX(m_RedGO, false, true);
			m_DefaultLight.Stop();
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(ClearParticlesDelayed(m_DefaultLight));
			}
		}

		private void ShowYellow()
		{
			PlayAnim(m_OpenAnimName);
			Boolean p_wasClosed = m_enabledGO == null;
			DisableFX(m_RedGO, false, false);
			if (!m_DefaultLight.isPlaying)
			{
				LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.TARGET_SPOTTET);
				m_DefaultLight.Play();
			}
			EnableFX(m_YellowGO, p_wasClosed);
		}

		private void ShowRed()
		{
			PlayAnim(m_OpenAnimName);
			Boolean p_wasClosed = m_enabledGO == null;
			DisableFX(m_YellowGO, false, false);
			if (!m_DefaultLight.isPlaying)
			{
				m_DefaultLight.Play();
			}
			EnableFX(m_RedGO, p_wasClosed);
		}

		private void EnableFX(GameObject p_FXRoot, Boolean p_wasClosed)
		{
			if (m_enabledGO != p_FXRoot)
			{
				p_FXRoot.SetActive(true);
				ParticleSystem[] componentsInChildren = p_FXRoot.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Play();
				}
				UISprite[] componentsInChildren2 = p_FXRoot.GetComponentsInChildren<UISprite>();
				foreach (UISprite uisprite in componentsInChildren2)
				{
					TweenColor.Begin(uisprite.gameObject, 0.4f, Color.white);
				}
				Collider[] componentsInChildren3 = p_FXRoot.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren3)
				{
					collider.enabled = true;
				}
				p_FXRoot.SendMessage("OnEnableFXFrom" + ((!p_wasClosed) ? "Opened" : "Closed"));
				m_enabledGO = p_FXRoot;
			}
		}

		private void DisableFX(GameObject p_FXRoot, Boolean p_IsForce, Boolean p_IsClosing)
		{
			if (m_enabledGO == p_FXRoot || p_IsForce)
			{
				ParticleSystem[] componentsInChildren = p_FXRoot.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
					if (p_IsClosing)
					{
						StartCoroutine(ClearParticlesDelayed(particleSystem));
					}
				}
				UISprite[] componentsInChildren2 = p_FXRoot.GetComponentsInChildren<UISprite>();
				foreach (UISprite uisprite in componentsInChildren2)
				{
					TweenColor.Begin(uisprite.gameObject, 0.4f, Color.clear);
				}
				Collider[] componentsInChildren3 = p_FXRoot.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren3)
				{
					collider.enabled = false;
				}
				if (m_enabledGO == p_FXRoot)
				{
					m_enabledGO = null;
				}
			}
		}

		private IEnumerator ClearParticlesDelayed(ParticleSystem p_particleSystem)
		{
			yield return WAIT;
			p_particleSystem.Clear();
			yield break;
		}

		private void PlayAnim(String p_animName)
		{
			Collider[] componentsInChildren = m_CloseAnim.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				collider.enabled = (p_animName == m_CloseAnimName);
			}
			if (m_CloseAnim.namePrefix != p_animName)
			{
				m_CloseAnim.namePrefix = p_animName;
				m_CloseAnim.Reset();
			}
		}
	}
}
