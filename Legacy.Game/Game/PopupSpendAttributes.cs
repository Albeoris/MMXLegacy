using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PopupSpendAttributes")]
	public class PopupSpendAttributes : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private UILabel m_ptsLeft;

		[SerializeField]
		private AttributeChanger m_mightAC;

		[SerializeField]
		private AttributeChanger m_magicAC;

		[SerializeField]
		private AttributeChanger m_perceptionAC;

		[SerializeField]
		private AttributeChanger m_destinyAC;

		[SerializeField]
		private AttributeChanger m_vitalityAC;

		[SerializeField]
		private AttributeChanger m_spiritAC;

		[SerializeField]
		private UIButton m_btnOK;

		private Boolean m_active;

		public event EventHandler OnAttributesConfirmed;

		private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCancelKeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnConfirmKeyPressed));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCancelKeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnConfirmKeyPressed));
		}

		public void Activate()
		{
			m_active = true;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character member = party.GetMember(party.CurrentCharacter);
			member.TemporaryAttributePoints = member.AttributePoints;
			m_mightAC.OnAttributeRaised += OnAttributeRaised;
			m_magicAC.OnAttributeRaised += OnAttributeRaised;
			m_perceptionAC.OnAttributeRaised += OnAttributeRaised;
			m_destinyAC.OnAttributeRaised += OnAttributeRaised;
			m_vitalityAC.OnAttributeRaised += OnAttributeRaised;
			m_spiritAC.OnAttributeRaised += OnAttributeRaised;
			m_mightAC.OnAttributeLowered += OnAttributeLowered;
			m_magicAC.OnAttributeLowered += OnAttributeLowered;
			m_perceptionAC.OnAttributeLowered += OnAttributeLowered;
			m_destinyAC.OnAttributeLowered += OnAttributeLowered;
			m_vitalityAC.OnAttributeLowered += OnAttributeLowered;
			m_spiritAC.OnAttributeLowered += OnAttributeLowered;
			NGUITools.SetActive(gameObject, true);
			m_btnOK.enabled = true;
			UpdateAttributes();
		}

		public void Deactivate()
		{
			m_active = false;
			m_mightAC.OnAttributeRaised -= OnAttributeRaised;
			m_magicAC.OnAttributeRaised -= OnAttributeRaised;
			m_perceptionAC.OnAttributeRaised -= OnAttributeRaised;
			m_destinyAC.OnAttributeRaised -= OnAttributeRaised;
			m_vitalityAC.OnAttributeRaised -= OnAttributeRaised;
			m_spiritAC.OnAttributeRaised -= OnAttributeRaised;
			m_mightAC.OnAttributeLowered -= OnAttributeLowered;
			m_magicAC.OnAttributeLowered -= OnAttributeLowered;
			m_perceptionAC.OnAttributeLowered -= OnAttributeLowered;
			m_destinyAC.OnAttributeLowered -= OnAttributeLowered;
			m_vitalityAC.OnAttributeLowered -= OnAttributeLowered;
			m_spiritAC.OnAttributeLowered -= OnAttributeLowered;
			NGUITools.SetActive(gameObject, false);
		}

		private void OnAttributeRaised(Object p_sender, EventArgs p_args)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character member = party.GetMember(party.CurrentCharacter);
			EPotionTarget attribute = (p_sender as AttributeChanger).Attribute;
			if (attribute == EPotionTarget.MIGHT)
			{
				member.AttributeChanger.IncreaseMight();
			}
			else if (attribute == EPotionTarget.MAGIC)
			{
				member.AttributeChanger.IncreaseMagic();
			}
			else if (attribute == EPotionTarget.PERCEPTION)
			{
				member.AttributeChanger.IncreasePerception();
			}
			else if (attribute == EPotionTarget.DESTINY)
			{
				member.AttributeChanger.IncreaseDestiny();
			}
			else if (attribute == EPotionTarget.VITALITY)
			{
				member.AttributeChanger.IncreaseVitality();
			}
			else if (attribute == EPotionTarget.SPIRIT)
			{
				member.AttributeChanger.IncreaseSpirit();
			}
			UpdateAttributes();
		}

		public void OnAttributeLowered(Object p_sender, EventArgs p_args)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character member = party.GetMember(party.CurrentCharacter);
			EPotionTarget attribute = (p_sender as AttributeChanger).Attribute;
			if (attribute == EPotionTarget.MIGHT)
			{
				member.AttributeChanger.DecreaseMight();
			}
			else if (attribute == EPotionTarget.MAGIC)
			{
				member.AttributeChanger.DecreaseMagic();
			}
			else if (attribute == EPotionTarget.PERCEPTION)
			{
				member.AttributeChanger.DecreasePerception();
			}
			else if (attribute == EPotionTarget.DESTINY)
			{
				member.AttributeChanger.DecreaseDestiny();
			}
			else if (attribute == EPotionTarget.VITALITY)
			{
				member.AttributeChanger.DecreaseVitality();
			}
			else if (attribute == EPotionTarget.SPIRIT)
			{
				member.AttributeChanger.DecreaseSpirit();
			}
			UpdateAttributes();
		}

		private void UpdateAttributes()
		{
			GameConfig game = ConfigManager.Instance.Game;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character member = party.GetMember(party.CurrentCharacter);
			String arg = "[ffffff]";
			if (member.TemporaryAttributePoints > 0)
			{
				arg = "[008000]";
			}
			m_ptsLeft.text = LocaManager.GetText("GUI_POINTS_LEFT", arg, member.TemporaryAttributePoints);
			m_mightAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT"), LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT_TT", game.HealthPerMight), EPotionTarget.MIGHT, member.CurrentAttributes.Might, member.AttributeChanger.TemporaryPoints.Might, null, member);
			m_magicAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC"), LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC_TT", game.ManaPerMagic), EPotionTarget.MAGIC, member.CurrentAttributes.Magic, member.AttributeChanger.TemporaryPoints.Magic, null, member);
			m_perceptionAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION"), LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION_TT"), EPotionTarget.PERCEPTION, member.CurrentAttributes.Perception, member.AttributeChanger.TemporaryPoints.Perception, null, member);
			m_destinyAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY"), LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY_TT"), EPotionTarget.DESTINY, member.CurrentAttributes.Destiny, member.AttributeChanger.TemporaryPoints.Destiny, null, member);
			m_vitalityAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY"), LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY_TT", member.Class.GetHPPerVitality()), EPotionTarget.VITALITY, member.CurrentAttributes.Vitality, member.AttributeChanger.TemporaryPoints.Vitality, null, member);
			m_spiritAC.Init(LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT"), LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT_TT", game.ManaPerSpirit), EPotionTarget.SPIRIT, member.CurrentAttributes.Spirit, member.AttributeChanger.TemporaryPoints.Spirit, null, member);
		}

		public void Cancel()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character member = party.GetMember(party.CurrentCharacter);
			member.AttributeChanger.Reset();
			IngameController.Instance.ChangeIngameContextToPrevious();
		}

		public void Confirm()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character member = party.GetMember(party.CurrentCharacter);
			member.AttributeChanger.FinalizeTemporarySpendPoints();
			if (OnAttributesConfirmed != null)
			{
				OnAttributesConfirmed(this, EventArgs.Empty);
			}
			IngameController.Instance.ChangeIngameContextToPrevious();
		}

		private void OnCancelKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_active && p_args.KeyDown)
			{
				Cancel();
			}
		}

		private void OnConfirmKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_active && p_args.KeyDown)
			{
				Confirm();
			}
		}
	}
}
