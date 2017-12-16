using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellView")]
	public class SpellView : MonoBehaviour
	{
		private const String BEST_HEALTH_POTION_DEFAULT = "ITM_consumable_potion_health_1";

		private const String BEST_MANA_POTION_DEFAULT = "ITM_consumable_potion_mana_1";

		[SerializeField]
		private UILabel m_labelName;

		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UISprite m_iconItem;

		[SerializeField]
		private UILabel m_labelMana;

		[SerializeField]
		private Color m_notUsableColorText;

		[SerializeField]
		private Color m_unlearnedColor = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_unlearnedColorText = new Color(0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Vector3 m_tooltipOffset;

		[SerializeField]
		private UIAtlas m_spellsAtlas;

		private Color m_normalColor;

		private Color m_iconColor;

		private CharacterSpell m_spell;

		private Character m_character;

		private BaseAbilityStaticData m_passiveAbility;

		private EQuickActionType m_actionType;

		private Boolean m_isForStandardAction;

		private Boolean m_isForPassiveAbility;

		private Boolean m_hasSpell;

		private Boolean m_isAvailable;

		private Boolean m_visible;

		private Boolean m_isUsable = true;

		public CharacterSpell Spell => m_spell;

	    public EQuickActionType ActionType => m_actionType;

	    public UISprite Icon => m_icon;

	    public UISprite IconItem => m_iconItem;

	    private void Awake()
		{
			m_normalColor = m_labelName.color;
			m_iconColor = m_icon.color;
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void SetSpell(CharacterSpell p_spell, Character p_character)
		{
			NGUITools.SetActive(gameObject, true);
			m_isForStandardAction = false;
			m_isForPassiveAbility = false;
			m_visible = true;
			m_spell = p_spell;
			m_character = p_character;
			m_labelName.text = LocaManager.GetText(m_spell.StaticData.NameKey);
			m_labelMana.text = LocaManager.GetText("SPELLBOOK_SPELL_MANA", p_spell.StaticData.ManaCost);
			m_icon.atlas = m_spellsAtlas;
			m_hasSpell = m_character.SpellHandler.HasSpell(p_spell.SpellType);
			m_icon.spriteName = m_spell.StaticData.Icon;
			NGUITools.SetActiveSelf(m_iconItem.gameObject, false);
			UpdateData();
		}

		public void SetAction(EQuickActionType p_type, Character p_character)
		{
			NGUITools.SetActive(gameObject, true);
			m_actionType = p_type;
			m_character = p_character;
			m_isForStandardAction = true;
			m_isForPassiveAbility = false;
			m_visible = true;
			m_labelName.text = LocaManager.GetText("STANDARD_ACTION_" + p_type.ToString());
			SetActionSprite();
			UpdateData();
		}

		public void SetRacialAbility(RacialAbilitiesStaticData p_rsd, Character p_character)
		{
			NGUITools.SetActive(gameObject, true);
			m_isForStandardAction = false;
			m_isForPassiveAbility = true;
			m_visible = true;
			m_passiveAbility = p_rsd;
			m_character = p_character;
			m_labelName.text = LocaManager.GetText(p_rsd.NameKey);
			m_labelMana.text = LocaManager.GetText("SPELLBOOK_PASSIVE_ABILITY");
			m_icon.atlas = m_spellsAtlas;
			m_hasSpell = true;
			m_icon.spriteName = p_rsd.Icon;
			NGUITools.SetActiveSelf(m_iconItem.gameObject, false);
			UpdateData();
		}

		public void SetParagonAbility(ParagonAbilitiesStaticData p_psd, Character p_character)
		{
			NGUITools.SetActive(gameObject, true);
			m_isForStandardAction = false;
			m_isForPassiveAbility = true;
			m_visible = true;
			m_passiveAbility = p_psd;
			m_character = p_character;
			m_labelName.text = LocaManager.GetText(p_psd.NameKey);
			m_labelMana.text = LocaManager.GetText("SPELLBOOK_PASSIVE_ABILITY");
			m_icon.atlas = m_spellsAtlas;
			m_hasSpell = m_character.Class.IsAdvanced;
			m_icon.spriteName = p_psd.Icon;
			NGUITools.SetActiveSelf(m_iconItem.gameObject, false);
			UpdateData();
		}

		public void Hide()
		{
			m_visible = false;
			NGUITools.SetActive(gameObject, false);
		}

		private void SetActionSprite()
		{
			NGUITools.SetActiveSelf(m_iconItem.gameObject, m_actionType == EQuickActionType.USE_BEST_HEALTHPOTION || m_actionType == EQuickActionType.USE_BEST_MANAPOTION);
			switch (m_actionType)
			{
			case EQuickActionType.ATTACK:
				m_icon.spriteName = "SPL_action_melee";
				break;
			case EQuickActionType.ATTACKRANGED:
				m_icon.spriteName = "SPL_action_ranged";
				break;
			case EQuickActionType.USE_BEST_MANAPOTION:
			{
				m_icon.spriteName = "SPL_action_bestpotion";
				Potion bestPotion = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(EPotionType.MANA_POTION, null);
				if (bestPotion != null)
				{
					m_iconItem.spriteName = bestPotion.Icon;
				}
				else
				{
					m_iconItem.spriteName = "ITM_consumable_potion_mana_1";
				}
				break;
			}
			case EQuickActionType.USE_BEST_HEALTHPOTION:
			{
				m_icon.spriteName = "SPL_action_bestpotion";
				Potion bestPotion2 = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(EPotionType.HEALTH_POTION, null);
				if (bestPotion2 != null)
				{
					m_iconItem.spriteName = bestPotion2.Icon;
				}
				else
				{
					m_iconItem.spriteName = "ITM_consumable_potion_health_1";
				}
				break;
			}
			case EQuickActionType.DEFEND:
				m_icon.spriteName = "SPL_action_defend";
				break;
			}
		}

		private void OnClick()
		{
			if (!m_isForStandardAction && !m_isForPassiveAbility && !m_isUsable)
			{
				Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
				selectedCharacter.BarkHandler.TriggerBark(EBarks.LOW_MANA, selectedCharacter);
			}
			if (!m_isAvailable)
			{
				return;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (m_isForStandardAction)
			{
				switch (m_actionType)
				{
				case EQuickActionType.ATTACK:
					LegacyLogic.Instance.CommandManager.AddCommand(MeleeAttackCommand.Instance);
					break;
				case EQuickActionType.ATTACKRANGED:
					LegacyLogic.Instance.CommandManager.AddCommand(RangeAttackCommand.Instance);
					break;
				case EQuickActionType.USE_BEST_MANAPOTION:
				{
					Potion bestPotion = party.GetBestPotion(EPotionType.MANA_POTION, null);
					if (bestPotion != null)
					{
						InventorySlotRef consumableSlot = party.GetConsumableSlot(bestPotion);
						LegacyLogic.Instance.CommandManager.AddCommand(new ConsumeCommand(consumableSlot, m_character.Index));
					}
					break;
				}
				case EQuickActionType.USE_BEST_HEALTHPOTION:
				{
					Potion bestPotion2 = party.GetBestPotion(EPotionType.HEALTH_POTION, null);
					if (bestPotion2 != null)
					{
						InventorySlotRef consumableSlot2 = party.GetConsumableSlot(bestPotion2);
						LegacyLogic.Instance.CommandManager.AddCommand(new ConsumeCommand(consumableSlot2, m_character.Index));
					}
					break;
				}
				case EQuickActionType.DEFEND:
					LegacyLogic.Instance.CommandManager.AddCommand(DefendCommand.Instance);
					break;
				}
			}
			else if (!m_isForPassiveAbility)
			{
				if (m_spell.TargetType == ETargetType.SINGLE_PARTY_MEMBER)
				{
					OpenSpellCharacterSelectionEventArgs p_eventArgs = new OpenSpellCharacterSelectionEventArgs(m_spell);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_CAST_SPELL_WITH_CHARACTER_SELECTION, p_eventArgs);
				}
				else if (m_spell.SpellType == ECharacterSpell.SPELL_PRIME_SPIRIT_BEACON)
				{
					SpiritBeaconEventArgs p_eventArgs2 = new SpiritBeaconEventArgs(null);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_spell, EEventType.CHARACTER_CAST_SPIRIT_BEACON, p_eventArgs2);
				}
				else if (m_spell.SpellType == ECharacterSpell.SPELL_PRIME_IDENTIFY)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(m_spell, EEventType.CHARACTER_CAST_IDENTIFY, EventArgs.Empty);
				}
				else
				{
					LegacyLogic.Instance.CommandManager.AddCommand(new CastSpellCommand(m_spell, null, null));
				}
			}
		}

		public void OnPress(Boolean p_pressed)
		{
			if (!p_pressed)
			{
				EndDrag();
			}
		}

		private void OnDrag(Vector2 p_delta)
		{
			StartDrag();
		}

		public void UpdateData()
		{
			if (!m_visible)
			{
				return;
			}
			if (m_isForStandardAction)
			{
				PartyTurnActor partyTurnActor = LegacyLogic.Instance.UpdateManager.PartyTurnActor;
				m_isAvailable = false;
				m_isUsable = true;
				switch (m_actionType)
				{
				case EQuickActionType.ATTACK:
					m_isAvailable = partyTurnActor.CanDoCommand(MeleeAttackCommand.Instance, m_character.Index);
					m_isUsable = m_character.Equipment.IsMeleeAttackWeaponEquiped();
					break;
				case EQuickActionType.ATTACKRANGED:
					m_isAvailable = partyTurnActor.CanDoCommand(RangeAttackCommand.Instance, m_character.Index);
					m_isUsable = m_character.Equipment.IsRangedAttackWeaponEquiped();
					break;
				case EQuickActionType.USE_BEST_MANAPOTION:
				{
					Potion bestPotion = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(EPotionType.MANA_POTION, null);
					if (bestPotion != null)
					{
						InventorySlotRef consumableSlot = LegacyLogic.Instance.WorldManager.Party.GetConsumableSlot(bestPotion);
						m_isAvailable = partyTurnActor.CanDoCommand(new ConsumeCommand(consumableSlot, m_character.Index), m_character.Index);
					}
					else
					{
						m_isUsable = false;
					}
					break;
				}
				case EQuickActionType.USE_BEST_HEALTHPOTION:
				{
					Potion bestPotion2 = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(EPotionType.HEALTH_POTION, null);
					if (bestPotion2 != null)
					{
						InventorySlotRef consumableSlot2 = LegacyLogic.Instance.WorldManager.Party.GetConsumableSlot(bestPotion2);
						m_isAvailable = partyTurnActor.CanDoCommand(new ConsumeCommand(consumableSlot2, m_character.Index), m_character.Index);
					}
					else
					{
						m_isUsable = false;
					}
					break;
				}
				case EQuickActionType.DEFEND:
					m_isAvailable = partyTurnActor.CanDoCommand(DefendCommand.Instance, m_character.Index);
					break;
				}
				NGUITools.SetActiveSelf(m_labelMana.gameObject, false);
				m_icon.color = m_iconColor;
				m_iconItem.color = m_iconColor;
				if (m_isUsable)
				{
					m_labelName.color = ((!m_isAvailable) ? m_unlearnedColorText : m_normalColor);
				}
				else
				{
					m_labelName.color = m_notUsableColorText;
				}
			}
			else if (m_isForPassiveAbility)
			{
				m_labelName.color = ((!m_hasSpell) ? m_unlearnedColorText : m_normalColor);
				m_labelMana.color = ((!m_hasSpell) ? m_unlearnedColorText : m_normalColor);
				m_icon.color = ((!m_hasSpell) ? m_unlearnedColor : m_iconColor);
				NGUITools.SetActiveSelf(m_labelMana.gameObject, true);
			}
			else
			{
				m_isUsable = true;
				NGUITools.SetActiveSelf(m_labelMana.gameObject, m_hasSpell);
				if (m_hasSpell)
				{
					PartyTurnActor partyTurnActor2 = LegacyLogic.Instance.UpdateManager.PartyTurnActor;
					m_isAvailable = partyTurnActor2.CanDoCommand(new CastSpellCommand(m_spell, null, null), m_character.Index);
					m_icon.color = m_iconColor;
					if (m_isAvailable)
					{
						m_labelName.color = m_normalColor;
						m_labelMana.color = m_normalColor;
					}
					else
					{
						m_isUsable = (m_character.ManaPoints >= m_spell.StaticData.ManaCost);
						if (m_spell.SpellType == ECharacterSpell.SPELL_LIGHT_LAY_ON_HANDS)
						{
							m_isUsable = (m_character.ManaPoints >= 1);
						}
						m_labelName.color = ((!m_isUsable) ? m_notUsableColorText : m_unlearnedColorText);
						m_labelMana.color = ((!m_isUsable) ? m_notUsableColorText : m_unlearnedColorText);
					}
				}
				else
				{
					m_isAvailable = false;
					m_icon.color = m_unlearnedColor;
					m_labelName.color = m_unlearnedColorText;
				}
			}
		}

		public void CleanUp()
		{
		}

		public void DisableButton()
		{
			gameObject.GetComponentInChildren<UIButton>().isEnabled = false;
		}

		public void StartDrag()
		{
			if (m_isForPassiveAbility)
			{
				return;
			}
			if (DragDropManager.Instance.DraggedItem == null && UICamera.currentTouchID == -1)
			{
				if (!m_isForStandardAction)
				{
					if (m_hasSpell)
					{
						DragDropManager.Instance.StartDrag(new SpellDragObject(this, m_spell));
					}
				}
				else
				{
					DragDropManager.Instance.StartDrag(new BasicActionDragObject(this));
				}
			}
		}

		public void EndDrag()
		{
			if (DragDropManager.Instance.DraggedItem is SpellDragObject || DragDropManager.Instance.DraggedItem is BasicActionDragObject)
			{
				DragDropManager.Instance.StopDrag();
			}
		}

		public void OnTooltip(Boolean isOver)
		{
			if (isOver)
			{
				Vector3 position = m_icon.gameObject.transform.position;
				if (m_isForStandardAction)
				{
					TooltipManager.Instance.Show(this, m_actionType, position, m_tooltipOffset);
				}
				else if (m_isForPassiveAbility)
				{
					TooltipManager.Instance.Show(this, m_passiveAbility, position, m_tooltipOffset);
				}
				else
				{
					TooltipManager.Instance.Show(this, m_spell, position, m_tooltipOffset);
				}
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
