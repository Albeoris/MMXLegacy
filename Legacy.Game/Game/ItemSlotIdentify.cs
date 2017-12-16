using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Game.MMGUI;
using UnityEngine;

namespace Legacy.Game
{
	public class ItemSlotIdentify : ItemSlotTrading
	{
		[SerializeField]
		private UISprite m_brokenItemIcon;

		[SerializeField]
		private String m_doneSpriteName = "ICO_checkbox_checked";

		[SerializeField]
		private String m_goldSpriteName = "ICO_ressource_gold";

		[SerializeField]
		protected GameObject m_identifyFX;

		[SerializeField]
		protected GameObject m_repairFX;

		private IdentifyItemContainer m_myParent;

		private void OnEnable()
		{
			m_canDrag = false;
		}

		public void PlayFX(IdentifyItemContainer.EMode p_mode)
		{
			if (m_item == null)
			{
				return;
			}
			if (p_mode == IdentifyItemContainer.EMode.IDENTIFY && ((Equipment)m_item).Identified)
			{
				return;
			}
			if (p_mode == IdentifyItemContainer.EMode.REPAIR && !((Equipment)m_item).Broken)
			{
				return;
			}
			if (p_mode == IdentifyItemContainer.EMode.IDENTIFY)
			{
				if (m_identifyFX != null)
				{
					GameObject gameObject = (GameObject)Instantiate(m_identifyFX, m_itemTexture.transform.position, new Quaternion(0f, 0f, 0f, 0f));
					if (gameObject != null)
					{
						gameObject.transform.parent = transform;
						gameObject.transform.localScale = Vector3.one;
					}
				}
			}
			else if (m_repairFX != null)
			{
				GameObject gameObject2 = (GameObject)Instantiate(m_repairFX, m_itemTexture.transform.position, new Quaternion(0f, 0f, 0f, 0f));
				if (gameObject2 != null)
				{
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.parent = transform;
					Vector3 position = m_itemTexture.transform.position;
					position.z -= 0.2f;
					gameObject2.transform.position = position;
				}
			}
		}

		public override void SetItem(BaseItem p_item, Int32 p_originSlotIndex)
		{
			m_item = p_item;
			Equipment equipment = (Equipment)p_item;
			m_originSlotIndex = p_originSlotIndex;
			if (m_myParent == null)
			{
				m_myParent = (IdentifyItemContainer)m_parent;
			}
			if (m_itemTexture != null)
			{
				if (p_item != null)
				{
					if (m_myParent.Mode == IdentifyItemContainer.EMode.IDENTIFY)
					{
						NGUITools.SetActive(m_brokenItemIcon.gameObject, false);
						m_itemPrice = m_myParent.IdentifyInventory.IdentifyPrice;
						m_itemName.color = m_originColorCost;
						m_itemName.text = p_item.Name;
						if (m_myParent.IdentifyInventory.FromSpell)
						{
							NGUITools.SetActive(m_goldIcon.gameObject, false);
							NGUITools.SetActive(m_itemCost.gameObject, !equipment.Identified);
							m_itemCost.text = LocaManager.GetText("SPELLBOOK_SPELL_MANA", m_itemPrice);
						}
						else if (m_myParent.IdentifyInventory.FromScroll)
						{
							NGUITools.SetActive(m_goldIcon.gameObject, false);
							NGUITools.SetActive(m_itemCost.gameObject, false);
						}
						else
						{
							NGUITools.SetActive(m_goldIcon.gameObject, !equipment.Identified);
							NGUITools.SetActive(m_itemCost.gameObject, !equipment.Identified);
							m_itemCost.text = m_itemPrice.ToString();
						}
						if (equipment.Identified)
						{
							NGUITools.SetActive(m_itemCost.gameObject, false);
							NGUITools.SetActive(m_goldIcon.gameObject, true);
							m_goldIcon.spriteName = m_doneSpriteName;
						}
						else
						{
							m_goldIcon.spriteName = m_goldSpriteName;
						}
					}
					else
					{
						m_itemPrice = m_myParent.RepairInventory.RepairPrice;
						m_itemName.color = m_originColorCost;
						m_itemName.text = p_item.Name;
						NGUITools.SetActive(m_brokenItemIcon.gameObject, equipment.Broken);
						NGUITools.SetActive(m_goldIcon.gameObject, true);
						m_goldIcon.spriteName = ((!equipment.Broken) ? m_doneSpriteName : m_goldSpriteName);
						NGUITools.SetActive(m_itemCost.gameObject, equipment.Broken);
						m_itemCost.text = m_itemPrice.ToString();
					}
					UpdateItemCostColor();
					ItemSlot.UpdateItemBackground(m_itemBackground, m_item);
					if (m_item.Icon != null && m_item.Icon != String.Empty)
					{
						NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
						m_itemTexture.spriteName = m_item.Icon;
					}
					else
					{
						m_itemTexture.spriteName = "ITM_missing";
					}
				}
				else
				{
					NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
					NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
				}
			}
			NGUITools.SetActiveSelf(m_itemCounter.gameObject, false);
		}

		public override void UpdateItemCostColor()
		{
			if (m_myParent.Mode == IdentifyItemContainer.EMode.IDENTIFY)
			{
				if (((Equipment)m_item).Identified)
				{
					m_itemCost.color = m_originColorCost;
					return;
				}
				if (m_myParent.IdentifyInventory.FromSpell)
				{
					if (m_itemPrice <= LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.ManaPoints)
					{
						m_itemCost.color = m_originColorCost;
					}
					else
					{
						m_itemCost.color = m_insufficientGold;
					}
				}
				else if (m_myParent.IdentifyInventory.FromScroll)
				{
					if (m_myParent.IdentifyInventory.HasScrollResourcesForAmount(1))
					{
						m_itemName.color = m_originColorCost;
					}
					else
					{
						m_itemName.color = m_insufficientGold;
					}
				}
				else if (m_itemPrice <= LegacyLogic.Instance.WorldManager.Party.Gold)
				{
					m_itemCost.color = m_originColorCost;
				}
				else
				{
					m_itemCost.color = m_insufficientGold;
				}
			}
			else
			{
				if (((Equipment)m_item).Broken)
				{
					m_itemCost.color = m_originColorCost;
					return;
				}
				if (m_itemPrice <= LegacyLogic.Instance.WorldManager.Party.Gold)
				{
					m_itemCost.color = m_originColorCost;
				}
				else
				{
					m_itemCost.color = m_insufficientGold;
				}
			}
		}
	}
}
