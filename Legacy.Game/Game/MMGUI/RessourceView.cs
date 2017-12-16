using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/RessourceView")]
	public class RessourceView : MonoBehaviour
	{
		private const Single BLEND_TIME = 0.1f;

		private const Single PREVIEW_TIME = 5f;

		private const Single STARVED_ANIM_DURATION_WARNING = 2f;

		private const Single STARVED_ANIM_DURATION_ALERT = 1f;

		private Color STARVED_COLOR_WARNING = new Color(0.75f, 0.75f, 0.75f);

		private Color STARVED_COLOR_ALERT = Color.white;

		[SerializeField]
		private Transform m_iconGold;

		[SerializeField]
		private UILabel m_textGold;

		[SerializeField]
		private UILabel m_textTempGold;

		[SerializeField]
		private GameObject m_goldFX;

		[SerializeField]
		private UILabel m_textFood;

		[SerializeField]
		private UISprite m_restButtonIcon;

		[SerializeField]
		private UISprite m_restButtonBackground;

		[SerializeField]
		private UIButton m_restButton;

		[SerializeField]
		private UILabel m_restText;

		private Int32 m_partyRestedUpdateCounter;

		private Int32 m_oldGold;

		private Single m_goldTimer;

		private Color m_restBtnBgOriginalColor = Color.white;

		private Boolean m_isStarvedWarning;

		private Boolean m_isStarvedAlert;

		private TweenColor m_cachedRestBtnTweener;

		private AudioObject m_audioObject;

		private Int32 m_lastLootFrame = -1;

		public void OnResourceChanged(Object p_sender, EventArgs p_args)
		{
			Party party = (Party)p_sender;
			if (party != null)
			{
				if (m_textGold != null && party.Gold != m_oldGold)
				{
					Int32 num = party.Gold - m_oldGold;
					m_textTempGold.text = ((num <= 0) ? num.ToString("#,0") : ("+" + num.ToString("#,0")));
					m_textTempGold.color = ((num <= 0) ? Color.red : Color.green);
					TweenAlpha.Begin(m_textGold.gameObject, 0.1f, 0f);
					TweenAlpha.Begin(m_textTempGold.gameObject, 0.1f, 1f);
					m_goldTimer = 5f;
					if (enabled && gameObject.activeInHierarchy && num > 0)
					{
						StartCoroutine(ShowLateGoldFX());
					}
				}
				if (m_textFood != null)
				{
					m_textFood.text = party.Supplies.ToString();
					m_textFood.color = ((party.Supplies <= 0) ? Color.red : Color.white);
				}
				UpdateRestButtonState();
			}
		}

		public void OnPartyResting(Object p_sender, EventArgs p_args)
		{
			SetRestButtonState(false);
			m_isStarvedWarning = false;
			m_isStarvedAlert = false;
		}

		public void OnPartyRested(Object p_sender, EventArgs p_args)
		{
			m_partyRestedUpdateCounter = 3;
			UpdateStarvedAnim();
		}

		private void OnSaveGameLoaded(Object p_sender, EventArgs p_args)
		{
			OnEnable();
		}

		private void OnCharacterStarved(Object p_sender, EventArgs p_args)
		{
			StatusChangedEventArgs statusChangedEventArgs = (StatusChangedEventArgs)p_args;
			if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.STARVED_WARNING)
			{
				m_isStarvedWarning = true;
			}
			else if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.STARVED_ALERT)
			{
				m_isStarvedAlert = true;
			}
			else
			{
				UpdateStarvedAnim();
			}
		}

		private void OnEnable()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			m_textTempGold.alpha = 0f;
			if (m_textFood != null)
			{
				m_textFood.text = party.Supplies.ToString();
				m_textFood.color = ((party.Supplies <= 0) ? Color.red : Color.white);
			}
			m_textGold.text = party.Gold.ToString("#,0");
			m_oldGold = party.Gold;
			m_goldTimer = 0f;
			TweenAlpha.Begin(m_textGold.gameObject, 0f, 1f);
			TweenAlpha.Begin(m_textTempGold.gameObject, 0f, 0f);
			UpdateRestButtonState();
			UpdateStarvedAnim();
		}

		private void UpdateRestButtonState()
		{
			if (m_restButton != null && LegacyLogic.Instance.UpdateManager != null)
			{
				Boolean restButtonState = LegacyLogic.Instance.UpdateManager.PartyTurnActor.CanDoCommand(RestCommand.Instance, LegacyLogic.Instance.WorldManager.Party.CurrentCharacter);
				SetRestButtonState(restButtonState);
			}
		}

		private void SetRestButtonState(Boolean p_nextState)
		{
			m_restButton.isEnabled = p_nextState;
			if (m_restText != null)
			{
				m_restText.color = ((!m_restButton.isEnabled) ? Color.gray : Color.white);
			}
			if (m_restButtonIcon != null)
			{
				m_restButtonIcon.color = ((!m_restButton.isEnabled) ? Color.gray : Color.white);
			}
		}

		private void UpdateStarvedAnim()
		{
			m_isStarvedWarning = false;
			m_isStarvedAlert = false;
			if (!LegacyLogic.Instance.WorldManager.Party.Buffs.HasBuff(EPartyBuffs.WELL_RESTED))
			{
				foreach (Character character in LegacyLogic.Instance.WorldManager.Party.GetCharactersAlive())
				{
					if (character.ConditionHandler.HUDStarved == 2)
					{
						m_isStarvedAlert = true;
						break;
					}
					if (character.ConditionHandler.HUDStarved == 1)
					{
						m_isStarvedWarning = true;
					}
				}
			}
		}

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_GET_LOOT, new EventHandler(OnGetLoot));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTING, new EventHandler(OnPartyResting));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_LOADED, new EventHandler(OnSaveGameLoaded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_STARVED, new EventHandler(OnCharacterStarved));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnResourceChanged));
			if (m_restButtonBackground != null)
			{
				m_restBtnBgOriginalColor = m_restButtonBackground.color;
			}
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_GET_LOOT, new EventHandler(OnGetLoot));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTING, new EventHandler(OnPartyResting));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_LOADED, new EventHandler(OnSaveGameLoaded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_STARVED, new EventHandler(OnCharacterStarved));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnResourceChanged));
		}

		private void Update()
		{
			if (m_goldTimer > 0f)
			{
				m_goldTimer -= Time.deltaTime;
				if (m_goldTimer <= 0f)
				{
					Party party = LegacyLogic.Instance.WorldManager.Party;
					m_oldGold = party.Gold;
					m_textGold.text = party.Gold.ToString("#,0");
					TweenAlpha.Begin(m_textGold.gameObject, 0.1f, 1f);
					TweenAlpha.Begin(m_textTempGold.gameObject, 0.1f, 0f);
				}
			}
			if (m_partyRestedUpdateCounter > 0)
			{
				m_partyRestedUpdateCounter--;
			}
			else
			{
				UpdateRestButtonState();
				m_partyRestedUpdateCounter = 5;
			}
			if (m_cachedRestBtnTweener == null)
			{
				m_cachedRestBtnTweener = m_restButtonBackground.GetComponent<TweenColor>();
			}
			if (m_restButton.isEnabled && m_restButtonBackground != null && (m_cachedRestBtnTweener == null || !m_cachedRestBtnTweener.enabled))
			{
				Single num = -1f;
				Color color = Color.white;
				if (m_isStarvedAlert)
				{
					num = 1f;
					color = STARVED_COLOR_ALERT;
				}
				else if (m_isStarvedWarning)
				{
					num = 2f;
					color = STARVED_COLOR_WARNING;
				}
				else if (m_restButtonBackground.color == STARVED_COLOR_ALERT || m_restButtonBackground.color == STARVED_COLOR_WARNING)
				{
					num = 0.2f;
					color = m_restBtnBgOriginalColor;
				}
				if (m_restButtonBackground.color == color)
				{
					color = m_restBtnBgOriginalColor;
				}
				if (num != -1f)
				{
					TweenColor.Begin(m_restButtonBackground.gameObject, num, color);
				}
			}
		}

		public void OnRestButtonClicked()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(RestCommand.Instance);
		}

		private void OnGetLoot(Object sender, EventArgs e)
		{
			m_lastLootFrame = Time.frameCount;
		}

		private IEnumerator ShowLateGoldFX()
		{
			if (m_lastLootFrame == Time.frameCount)
			{
				yield return new WaitForSeconds(DelayedEventManager.GetFixedDelay(EEventType.PARTY_GET_LOOT));
			}
			if (m_audioObject == null || !m_audioObject.IsPlaying())
			{
				m_audioObject = AudioController.Play("Gold_loot");
			}
			if (m_iconGold != null)
			{
				if (m_goldFX != null)
				{
					GameObject fx = (GameObject)Instantiate(m_goldFX, m_iconGold.position, Quaternion.identity);
					if (fx != null)
					{
						fx.transform.parent = m_iconGold;
						fx.transform.localScale = Vector3.one;
					}
				}
				else
				{
					Debug.LogError("RessourceView: gold highlight fx is not set!");
				}
			}
			else
			{
				Debug.LogError("RessourceView: gold icon transform is not set!");
			}
			yield break;
		}
	}
}
