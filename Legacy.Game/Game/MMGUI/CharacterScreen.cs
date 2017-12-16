using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/CharacterScreen")]
	public class CharacterScreen : BaseScreen
	{
		public const Int32 DEFAULT_TAB = 0;

		public const Int32 SKILL_TAB = 1;

		public const Single BAR_WIDTH = 768f;

		private const String XP_BAR_NAME = "BAR_line";

		[SerializeField]
		private UILabel m_nameLabel;

		[SerializeField]
		private UILabel m_LevelLabel;

		[SerializeField]
		private UILabel m_RaceClassLabel;

		[SerializeField]
		private XPBar m_XPBar;

		[SerializeField]
		private UIFilledSprite m_XPBarSprite;

		[SerializeField]
		private UILabel m_XPLabel;

		[SerializeField]
		private CharacterStatCollection m_statsCollection;

		[SerializeField]
		private UITexture m_characterPosing;

		[SerializeField]
		private EquipmentItemContainer m_itemContainer;

		[SerializeField]
		private PopupSpendAttributes m_popup;

		private UIAtlas m_atlas;

		private Party m_party;

		public void Init(Party p_party)
		{
			ChangeParty(p_party);
			m_atlas = m_XPBarSprite.atlas;
			CreateXPBarSegments();
			m_popup.OnAttributesConfirmed += OnAttributesConfirmed;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(UpdateAttributes));
		}

		public void ChangeParty(Party p_party)
		{
			m_party = p_party;
			m_statsCollection.Init(p_party);
			m_itemContainer.CleanUp();
			m_itemContainer.Init(p_party.GetMember(m_party.CurrentCharacter).Equipment);
		}

		private void CreateXPBarSegments()
		{
			GameObject gameObject = m_XPBarSprite.transform.parent.gameObject;
			Vector3 localPosition = m_XPBarSprite.gameObject.transform.localPosition;
			for (Int32 i = 1; i < 8; i++)
			{
				UISprite uisprite = NGUITools.AddSprite(gameObject, m_atlas, "BAR_line");
				Single num = 96f;
				uisprite.layer = 0;
				uisprite.transform.localPosition = new Vector3(localPosition.x + num * i - 1f, localPosition.y, 0f);
				uisprite.transform.localScale = new Vector3(6f, 16f, 1f);
				uisprite.depth = 10;
			}
		}

		public void CleanUp()
		{
			m_popup.OnAttributesConfirmed -= OnAttributesConfirmed;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(UpdateAttributes));
		}

		private void UpdateAttributes(Object sender, EventArgs e)
		{
			if (gameObject.activeInHierarchy)
			{
				m_statsCollection.UpdateContent();
			}
		}

		private void OnEnable()
		{
			m_statsCollection.UpdateContent();
		}

		protected override void SetElementsActiveState(Boolean p_enabled)
		{
			base.SetElementsActiveState(p_enabled);
			SetCharacter(m_party.GetMember(m_party.CurrentCharacter));
		}

		public void OnCharacterSelected(Object p_sender, EventArgs p_args)
		{
			Character character = (Character)p_sender;
			if (character != null)
			{
				SetCharacter(character);
			}
		}

		private void SetCharacter(Character p_character)
		{
			m_itemContainer.ChangeInventory(p_character.Equipment);
			m_itemContainer.InitStartItems();
			m_statsCollection.UpdateContent();
			SetBaseInfo(p_character);
			SetPosing(p_character);
		}

		private void SetBaseInfo(Character p_character)
		{
			if (m_nameLabel != null)
			{
				m_nameLabel.text = p_character.Name;
			}
			if (m_LevelLabel != null)
			{
				m_LevelLabel.text = LocaManager.GetText("CHARACTER_LEVEL", p_character.Level);
			}
			if (m_RaceClassLabel != null)
			{
				String text = p_character.Class.NameKey;
				if (p_character.Gender == EGender.MALE)
				{
					text += "_M";
				}
				else
				{
					text += "_F";
				}
				m_RaceClassLabel.text = LocaManager.GetText(p_character.Class.RaceKey) + " " + LocaManager.GetText(text);
			}
			SetLevelLabePosition();
			Single percentExpForNextLevel = p_character.GetPercentExpForNextLevel();
			m_XPBarSprite.fillAmount = percentExpForNextLevel;
			if (p_character.MaxLevelReached)
			{
				m_XPLabel.text = LocaManager.GetText("CHARACTER_MAX_LEVEL_REACHED");
			}
			else
			{
				m_XPLabel.text = LocaManager.GetText("CHARACTER_XP", p_character.Exp, p_character.TotalExpNeededForNextLevel);
			}
			m_XPLabel.transform.localPosition = new Vector3(m_XPLabel.transform.localPosition.x, m_XPLabel.transform.localPosition.y, -5f);
			m_XPBar.SetNextXPAndLevel(p_character);
		}

		private void SetLevelLabePosition()
		{
			Single num = m_nameLabel.transform.localPosition.x + m_nameLabel.relativeSize.x * m_nameLabel.transform.localScale.x;
			Single num2 = m_RaceClassLabel.transform.localPosition.x - m_RaceClassLabel.relativeSize.x * m_RaceClassLabel.transform.localScale.x;
			Single num3 = m_LevelLabel.relativeSize.x * m_LevelLabel.transform.localScale.x;
			Single x = num + (num2 - num - num3) / 2f + num3 / 2f;
			m_LevelLabel.transform.localPosition = new Vector3(x, m_LevelLabel.transform.localPosition.y, m_LevelLabel.transform.localPosition.z);
		}

		private void SetPosing(Character p_character)
		{
			String p_path = "CharacterPosings/" + p_character.GetPosingTextName();
			Texture mainTexture = m_characterPosing.mainTexture;
			Texture texture = Helper.ResourcesLoad<Texture2D>(p_path, true);
			if (mainTexture != texture)
			{
				m_characterPosing.mainTexture = texture;
				mainTexture.UnloadAsset();
			}
		}

		public void OnPointsButtonClick()
		{
			IngameController.Instance.ChangeIngameContext(m_popup);
		}

		public void OnHudPointsButtonClick()
		{
			if (m_popup.gameObject.activeSelf)
			{
				return;
			}
			OnPointsButtonClick();
		}

		public void OnAttributesConfirmed(Object p_sender, EventArgs p_args)
		{
			m_statsCollection.UpdateContent();
		}
	}
}
