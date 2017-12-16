using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/TooltipManager")]
	public class TooltipManager : MonoBehaviour
	{
		[SerializeField]
		private TextTooltip m_textTooltip;

		[SerializeField]
		private ItemTooltip m_itemTooltip;

		[SerializeField]
		private ItemTooltip m_compareTooltip;

		[SerializeField]
		private ItemTooltip m_compareSecondaryTooltip;

		[SerializeField]
		private SkillTooltip m_skillTooltip;

		[SerializeField]
		private SpellTooltip m_spellTooltip;

		[SerializeField]
		private AttributeTooltip m_AttributeTooltip;

		[SerializeField]
		private MapTooltip m_MapTooltip;

		[SerializeField]
		private Single m_tooltipOffset = 5f;

		[SerializeField]
		private Single m_borderTreshhold = 50f;

		[SerializeField]
		private Single m_compareDelay = 0.5f;

		[SerializeField]
		private Single m_compareGap = 5f;

		private Single m_zPosition;

		private Camera m_uiCamera;

		private UIRoot m_uiRoot;

		private Vector3 m_position;

		private Transform m_transform;

		private Vector3 m_size;

		private MonoBehaviour m_tooltipCaller;

		private Single m_showTime;

		private Boolean m_showCompare;

		private Boolean m_showCompareSecondary;

		private EHorizontalAlignmentType m_horizontalAlignment;

		private static TooltipManager s_instance;

		public static TooltipManager Instance => s_instance;

	    private void Awake()
		{
			Init();
		}

		private void Start()
		{
			m_transform = transform;
			m_position = m_transform.localPosition;
			m_size = m_transform.localScale;
			if (m_uiCamera == null)
			{
				m_uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
			}
			if (m_uiRoot == null)
			{
				m_uiRoot = NGUITools.FindInParents<UIRoot>(gameObject);
			}
			ForceHide();
		}

		public void Init()
		{
			if (s_instance == null)
			{
				s_instance = this;
				m_zPosition = gameObject.transform.position.z;
			}
		}

		public void Show(MonoBehaviour p_caller, String p_mapObjectNote, String p_mapUserNote, Vector3 p_position, Vector3 p_offset)
		{
			m_tooltipCaller = p_caller;
			m_MapTooltip.Fill(p_mapObjectNote, p_mapUserNote);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_MapTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_MapTooltip.Scale.x + p_offset.x, m_MapTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_MapTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, String p_tooltipText, Vector3 p_position, Vector3 p_offset)
		{
			Show(p_caller, String.Empty, p_tooltipText, TextTooltip.ESize.BIG, p_position, p_offset);
		}

		public void Show(MonoBehaviour p_caller, String p_captionText, String p_tooltipText, TextTooltip.ESize p_size, Vector3 p_position, Vector3 p_offset)
		{
			if (!String.IsNullOrEmpty(p_tooltipText) || !String.IsNullOrEmpty(p_captionText))
			{
				m_tooltipCaller = p_caller;
				m_textTooltip.Fill(p_captionText, p_tooltipText, p_size);
				m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
				Single p_xOffset = m_tooltipOffset + 0.5f * m_textTooltip.Scale.x + p_offset.x;
				Single p_yOffset = m_tooltipOffset + p_offset.y;
				AdjustAlignment(p_xOffset, p_yOffset, m_textTooltip.Scale.x + p_offset.x, m_textTooltip.Scale.y, EVerticalAlignmentType.CORNER);
				AdjustPosition();
				m_textTooltip.Show();
			}
		}

		public void Show(MonoBehaviour p_caller, BaseItem p_item, IInventory p_inventory, Vector3 p_position, Vector3 p_offset)
		{
			m_tooltipCaller = p_caller;
			m_itemTooltip.Fill(p_item, null, null, p_inventory);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_itemTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_itemTooltip.Scale.x + p_offset.x, m_itemTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_itemTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, Equipment p_item, Equipment p_compareItem, Equipment p_compareSecondaryItem, IInventory p_inventory, Vector3 p_position, Vector3 p_offset)
		{
			m_showTime = Time.time;
			m_showCompare = true;
			m_tooltipCaller = p_caller;
			Boolean flag = p_item is MeleeWeapon && ((MeleeWeapon)p_item).GetSubType() == EEquipmentType.TWOHANDED;
			flag |= (p_item is MagicFocus && ((MagicFocus)p_item).GetMagicfocusType() == EEquipmentType.MAGIC_FOCUS_TWOHANDED);
			if (p_compareSecondaryItem != null && !flag)
			{
				m_itemTooltip.Fill(p_item, null, null, p_inventory);
				m_compareTooltip.Fill(p_compareItem, p_item, null, p_inventory);
				m_compareSecondaryTooltip.Fill(p_compareSecondaryItem, p_item, null, p_inventory);
				m_showCompareSecondary = true;
			}
			else
			{
				m_itemTooltip.Fill(p_item, p_compareItem, p_compareSecondaryItem, p_inventory);
				m_compareTooltip.Fill(p_compareItem, null, null, p_inventory);
				if (p_compareSecondaryItem != null)
				{
					m_compareSecondaryTooltip.Fill(p_compareSecondaryItem, null, null, p_inventory);
					m_showCompareSecondary = true;
				}
			}
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_itemTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			Single num = p_offset.x + m_itemTooltip.Scale.x + m_compareTooltip.Scale.x + m_compareGap;
			Single num2 = Math.Max(m_itemTooltip.Scale.y, m_compareTooltip.Scale.y);
			if (p_compareSecondaryItem != null)
			{
				num += m_compareSecondaryTooltip.Scale.x + m_compareGap;
				num2 = Math.Max(num2, m_compareSecondaryTooltip.Scale.y);
			}
			AdjustAlignment(p_xOffset, p_yOffset, num, num2, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			Vector3 localPosition = m_compareTooltip.gameObject.transform.localPosition;
			Vector3 localPosition2 = m_compareSecondaryTooltip.gameObject.transform.localPosition;
			if (m_horizontalAlignment == EHorizontalAlignmentType.RIGHT)
			{
				localPosition.x = m_itemTooltip.Scale.x + m_compareGap;
				localPosition2.x = m_itemTooltip.Scale.x + m_compareTooltip.Scale.x + m_compareGap * 2f;
			}
			else
			{
				localPosition.x = -m_itemTooltip.Scale.x - m_compareGap;
				localPosition2.x = -m_itemTooltip.Scale.x - m_compareTooltip.Scale.x - m_compareGap * 2f;
			}
			localPosition.y = m_compareTooltip.EquippedGroup.Size.y;
			localPosition2.y = m_compareSecondaryTooltip.EquippedGroup.Size.y;
			m_compareTooltip.gameObject.transform.localPosition = localPosition;
			m_compareSecondaryTooltip.gameObject.transform.localPosition = localPosition2;
			m_itemTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, Skill p_skill, SkillTooltip.TooltipType p_type, Boolean p_isDefault, Vector3 p_position, Vector3 p_offset)
		{
			m_tooltipCaller = p_caller;
			m_skillTooltip.Fill(p_skill, p_type, p_isDefault);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_skillTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_skillTooltip.Scale.x + p_offset.x, m_skillTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_skillTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, EQuickActionType p_action, Vector3 p_position, Vector3 p_offset)
		{
			m_tooltipCaller = p_caller;
			m_spellTooltip.Fill(p_action);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_spellTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_spellTooltip.Scale.x + p_offset.x, m_spellTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_spellTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, BaseAbilityStaticData p_ability, Vector3 p_position, Vector3 p_offset)
		{
			m_tooltipCaller = p_caller;
			m_spellTooltip.Fill(p_ability);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_spellTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_spellTooltip.Scale.x + p_offset.x, m_spellTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_spellTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, CharacterSpell p_spell, Vector3 p_position, Vector3 p_offset)
		{
			m_tooltipCaller = p_caller;
			m_spellTooltip.Fill(p_spell);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_spellTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_spellTooltip.Scale.x + p_offset.x, m_spellTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_spellTooltip.Show();
		}

		public void Show(MonoBehaviour p_caller, AttributeTooltip.TooltipType p_type, EPotionTarget p_attribute, Int32 p_currentIncrease, Vector3 p_position, Vector3 p_offset, Character p_character, DummyCharacter p_dummy)
		{
			m_tooltipCaller = p_caller;
			m_AttributeTooltip.Fill(p_type, p_attribute, p_currentIncrease, p_character, p_dummy);
			m_position = UICamera.currentCamera.WorldToScreenPoint(p_position);
			Single p_xOffset = m_tooltipOffset + 0.5f * m_AttributeTooltip.Scale.x + p_offset.x;
			Single p_yOffset = m_tooltipOffset + p_offset.y;
			AdjustAlignment(p_xOffset, p_yOffset, m_AttributeTooltip.Scale.x + p_offset.x, m_AttributeTooltip.Scale.y, EVerticalAlignmentType.MIDDLE);
			AdjustPosition();
			m_AttributeTooltip.Show();
		}

		public void Hide(MonoBehaviour p_caller)
		{
			if (p_caller != m_tooltipCaller)
			{
				return;
			}
			if (m_textTooltip != null)
			{
				m_textTooltip.Hide();
			}
			if (m_itemTooltip != null)
			{
				m_itemTooltip.Hide();
			}
			if (m_skillTooltip != null)
			{
				m_skillTooltip.Hide();
			}
			if (m_spellTooltip != null)
			{
				m_spellTooltip.Hide();
			}
			if (m_AttributeTooltip != null)
			{
				m_AttributeTooltip.Hide();
			}
			if (m_compareTooltip != null)
			{
				m_compareTooltip.Hide();
			}
			if (m_compareSecondaryTooltip != null)
			{
				m_compareSecondaryTooltip.Hide();
			}
			if (m_MapTooltip != null)
			{
				m_MapTooltip.Hide();
			}
			m_showCompare = false;
			m_showCompareSecondary = false;
			m_tooltipCaller = null;
		}

		public void ForceHide()
		{
			Hide(m_tooltipCaller);
		}

		private void AdjustAlignment(Single p_xOffset, Single p_yOffset, Single p_backgroundWidth, Single p_backgroundHeight, EVerticalAlignmentType p_verticalAlignmentType)
		{
			Single pixelSizeAdjustment = m_uiRoot.pixelSizeAdjustment;
			p_xOffset /= pixelSizeAdjustment;
			p_yOffset /= pixelSizeAdjustment;
			p_backgroundHeight /= pixelSizeAdjustment;
			p_backgroundWidth /= pixelSizeAdjustment;
			Boolean flag = m_position.x <= Screen.width - p_backgroundWidth - m_borderTreshhold;
			Boolean flag2 = m_position.x > p_backgroundWidth + m_borderTreshhold;
			if (flag || !flag2)
			{
				if (!flag)
				{
					m_position.x = Screen.width - p_backgroundWidth - m_borderTreshhold;
				}
				m_position.x = m_position.x + p_xOffset;
				m_horizontalAlignment = EHorizontalAlignmentType.RIGHT;
			}
			else
			{
				m_position.x = m_position.x - p_xOffset;
				m_horizontalAlignment = EHorizontalAlignmentType.LEFT;
			}
			if (p_verticalAlignmentType == EVerticalAlignmentType.CORNER)
			{
				if (m_position.y <= Screen.height - p_backgroundHeight - p_yOffset - m_borderTreshhold)
				{
					m_position.y = m_position.y + (p_yOffset + p_backgroundHeight);
				}
				else
				{
					m_position.y = m_position.y - p_yOffset;
				}
			}
			else if (m_position.y > Screen.height - m_borderTreshhold - p_backgroundHeight / 2f)
			{
				m_position.y = Screen.height - m_borderTreshhold;
			}
			else if (m_position.y < m_borderTreshhold + p_backgroundHeight / 2f)
			{
				m_position.y = m_borderTreshhold + p_backgroundHeight;
			}
			else
			{
				m_position.y = m_position.y + p_backgroundHeight / 2f;
			}
		}

		private void AdjustPosition()
		{
			if (m_uiCamera != null)
			{
				m_position.x = Mathf.Clamp01(m_position.x / Screen.width);
				m_position.y = Mathf.Clamp01(m_position.y / Screen.height);
				Single num = m_uiCamera.orthographicSize / m_transform.parent.lossyScale.y;
				Single num2 = Screen.height * 0.5f / num;
				Vector2 vector = new Vector2(num2 * m_size.x / Screen.width, num2 * m_size.y / Screen.height);
				m_position.x = Mathf.Min(m_position.x, 1f - vector.x);
				m_position.y = Mathf.Max(m_position.y, vector.y);
				m_position.z = m_zPosition;
				m_transform.position = m_uiCamera.ViewportToWorldPoint(m_position);
				m_position = m_transform.localPosition;
				m_position.x = Mathf.Round(m_position.x);
				m_position.y = Mathf.Round(m_position.y);
				m_transform.localPosition = m_position;
			}
			else
			{
				if (m_position.x + m_size.x > Screen.width)
				{
					m_position.x = Screen.width - m_size.x;
				}
				if (m_position.y - m_size.y < 0f)
				{
					m_position.y = m_size.y;
				}
				m_position.x = m_position.x - Screen.width * 0.5f;
				m_position.y = m_position.y - Screen.height * 0.5f;
			}
		}

		private void Update()
		{
			if (m_showCompare && Time.time > m_showTime + m_compareDelay)
			{
				m_showCompare = false;
				m_compareTooltip.Show();
				m_itemTooltip.ShowComparison();
				m_compareTooltip.ShowComparison();
				if (m_showCompareSecondary)
				{
					m_compareSecondaryTooltip.Show();
					m_compareSecondaryTooltip.ShowComparison();
				}
			}
		}

		private enum EVerticalAlignmentType
		{
			CORNER,
			MIDDLE
		}

		private enum EHorizontalAlignmentType
		{
			LEFT,
			RIGHT
		}
	}
}
