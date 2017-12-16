using System;
using System.Text;
using Legacy.Audio;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/MMGUI/CharacterHudStatManager")]
	public class CharacterHudStatManager : MonoBehaviour
	{
		[SerializeField]
		private CharacterHudStatIcon m_conditionIcon;

		[SerializeField]
		private CharacterHudStatIcon m_brokenItemIcon;

		[SerializeField]
		private CharacterHudStatIcon m_defendedIcon;

		[SerializeField]
		private CharacterHudStatIcon m_starvedIcon;

		private Character m_owner;

		public void Init(Character p_owner)
		{
			UnregisterEvents();
			m_owner = p_owner;
			m_conditionIcon.Init();
			m_brokenItemIcon.Init();
			m_defendedIcon.Init();
			m_starvedIcon.Init();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_DEFEND, new EventHandler(OnCharacterDefend));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESET_FIGHT_ROUND, new EventHandler(OnPartyRoundFinished));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_STARVED, new EventHandler(OnCharacterIsStarving));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemRepairStatusChanged));
		}

		public void ChangeCharacter(Character p_owner)
		{
			m_owner = p_owner;
		}

		public void Cleanup()
		{
			UnregisterEvents();
			m_conditionIcon = null;
			m_brokenItemIcon = null;
			m_defendedIcon = null;
			m_starvedIcon = null;
			m_owner = null;
		}

		private void UnregisterEvents()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_DEFEND, new EventHandler(OnCharacterDefend));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESET_FIGHT_ROUND, new EventHandler(OnPartyRoundFinished));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_STARVED, new EventHandler(OnCharacterIsStarving));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemRepairStatusChanged));
		}

		public void ForceUpdate(Character p_character)
		{
			UpdateData(p_character);
		}

		private void OnConditionChanged(Object p_sender, EventArgs p_args)
		{
			Character p_character = p_sender as Character;
			StatusChangedEventArgs statusChangedEventArgs = p_args as StatusChangedEventArgs;
			if (statusChangedEventArgs.Type != StatusChangedEventArgs.EChangeType.CONDITIONS)
			{
				return;
			}
			UpdateData(p_character);
		}

		private void UpdateData(Character p_character)
		{
			if (p_character != null && p_character == m_owner)
			{
				if (m_owner.ConditionHandler.HUDStarved == 0)
				{
					m_starvedIcon.SetEnabled(false);
				}
				else if (m_owner.ConditionHandler.HUDStarved == 1)
				{
					OnCharacterIsStarving(m_owner, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STARVED_WARNING));
				}
				else if (m_owner.ConditionHandler.HUDStarved == 2)
				{
					OnCharacterIsStarving(m_owner, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STARVED_ALERT));
				}
				ECondition visibleCondition = p_character.ConditionHandler.GetVisibleCondition();
				if (visibleCondition == ECondition.NONE)
				{
					m_conditionIcon.SetEnabled(false);
				}
				else if (visibleCondition == ECondition.DEAD || visibleCondition == ECondition.UNCONSCIOUS)
				{
					m_starvedIcon.SetEnabled(false);
					m_conditionIcon.SetTooltip(GetTooltipForCondition(visibleCondition, p_character));
					m_conditionIcon.SetEnabled(true);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ECondition p_condition in EnumUtil<ECondition>.Values)
					{
						if (p_character.ConditionHandler.HasCondition(p_condition))
						{
							String tooltipForCondition = GetTooltipForCondition(p_condition, p_character);
							stringBuilder.Append(tooltipForCondition);
							if (tooltipForCondition.Length > 1)
							{
								stringBuilder.Append("\n\n");
							}
						}
					}
					String tooltip = stringBuilder.ToString().Trim();
					m_conditionIcon.SetTooltip(tooltip);
					m_conditionIcon.SetEnabled(true);
				}
				if (p_character.Equipment.Equipment.HasBrokenItems())
				{
					m_brokenItemIcon.SetTooltip(LocaManager.GetText("ITEM_IS_BROKEN") + ":");
					foreach (Equipment equipment in p_character.Equipment.Equipment.GetBrokenItems())
					{
						m_brokenItemIcon.AddToTooltip("- " + equipment.Name);
					}
					m_brokenItemIcon.SetEnabled(true);
				}
				else
				{
					m_brokenItemIcon.SetEnabled(false);
				}
			}
		}

		private String GetTooltipForCondition(ECondition p_condition, Character p_character)
		{
			String str = "_M";
			if (p_character.Gender == EGender.FEMALE)
			{
				str = "_F";
			}
			String result;
			switch (p_condition)
			{
			case ECondition.DEAD:
				result = LocaManager.GetText("CHARACTER_CONDITION_DEAD_TT" + str, p_character.Name);
				break;
			case ECondition.UNCONSCIOUS:
				result = LocaManager.GetText("CHARACTER_CONDITION_UNCONSCIOUS_TT" + str, p_character.Name);
				break;
			default:
				if (p_condition != ECondition.SLEEPING)
				{
					if (p_condition != ECondition.POISONED)
					{
						if (p_condition != ECondition.CONFUSED)
						{
							if (p_condition != ECondition.WEAK)
							{
								if (p_condition != ECondition.CURSED)
								{
									result = String.Empty;
								}
								else
								{
									result = LocaManager.GetText("CHARACTER_CONDITION_CURSED_TT" + str, p_character.Name, Mathf.RoundToInt(ConfigManager.Instance.Game.WeakAttribDecrease * 100f), Mathf.RoundToInt(ConfigManager.Instance.Game.CursedAttackDecrease * 100f));
								}
							}
							else
							{
								result = LocaManager.GetText("CHARACTER_CONDITION_WEAK_TT" + str, p_character.Name, Mathf.RoundToInt(ConfigManager.Instance.Game.WeakAttribDecrease * 100f));
							}
						}
						else
						{
							result = LocaManager.GetText("CHARACTER_CONDITION_CONFUSED_TT" + str, p_character.Name);
						}
					}
					else
					{
						result = LocaManager.GetText("CHARACTER_CONDITION_POISONED_TT" + str, p_character.Name, p_character.ConditionHandler.PoisonEvadeDecrease, Mathf.RoundToInt(m_owner.MaximumHealthPoints * ConfigManager.Instance.Game.PoisonHealthDamage));
					}
				}
				else
				{
					result = LocaManager.GetText("CHARACTER_CONDITION_SLEEPING_TT" + str, p_character.Name);
				}
				break;
			case ECondition.PARALYZED:
				result = LocaManager.GetText("CHARACTER_CONDITION_PARALYZED_TT" + str, p_character.Name);
				break;
			case ECondition.STUNNED:
				result = LocaManager.GetText("CHARACTER_CONDITION_KNOCKED_OUT_TT" + str, p_character.Name, ConfigManager.Instance.Game.KnockedOutTurnCount);
				break;
			}
			return result;
		}

		private void OnCharacterDefend(Object p_sender, EventArgs p_args)
		{
			Character character = p_sender as Character;
			if (character != null && character == m_owner && LegacyLogic.Instance.WorldManager.Party.HasAggro())
			{
				m_defendedIcon.SetTooltip(LocaManager.GetText("CHARACTER_HUD_STAT_DEFENDED", character.Name));
				m_defendedIcon.SetEnabled(true);
			}
		}

		private void OnCharacterIsStarving(Object p_sender, EventArgs p_args)
		{
			Character character = p_sender as Character;
			if (character != m_owner)
			{
				return;
			}
			if (character.ConditionHandler.HasCondition(ECondition.DEAD) || character.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS))
			{
				return;
			}
			String id;
			if (character.Gender == EGender.FEMALE)
			{
				id = "CHARACTER_CONDITION_EXHAUSTED_TT_F";
			}
			else
			{
				id = "CHARACTER_CONDITION_EXHAUSTED_TT_M";
			}
			m_starvedIcon.SetEnabled(true);
			StatusChangedEventArgs statusChangedEventArgs = p_args as StatusChangedEventArgs;
			if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.STARVED_WARNING)
			{
				m_starvedIcon.SetColor(Color.yellow);
				m_starvedIcon.SetTooltip(LocaManager.GetText(id, character.Name, GetTimeSinceLastRestAsHours(character.LastRestTime)));
			}
			else if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.STARVED_ALERT)
			{
				m_starvedIcon.SetColor(Color.red);
				m_starvedIcon.SetTooltip(LocaManager.GetText(id, character.Name, GetTimeSinceLastRestAsHours(character.LastRestTime)));
			}
		}

		private Int32 GetTimeSinceLastRestAsHours(MMTime p_lastRestTime)
		{
			MMTime mmtime = LegacyLogic.Instance.GameTime.Time - p_lastRestTime;
			return mmtime.Days * 24 + mmtime.Hours;
		}

		private void OnPartyRested(Object p_sender, EventArgs p_args)
		{
			m_starvedIcon.SetEnabled(false);
			foreach (Character p_sender2 in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				OnConditionChanged(p_sender2, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.CONDITIONS));
			}
		}

		private void OnPartyRoundFinished(Object p_sender, EventArgs p_args)
		{
			if (m_defendedIcon != null)
			{
				m_defendedIcon.SetEnabled(false);
			}
		}

		private void OnItemRepairStatusChanged(Object p_sender, EventArgs p_args)
		{
			if (p_args is InventoryItemEventArgs && ((InventoryItemEventArgs)p_args).Type != InventoryItemEventArgs.ERepairType.NONE)
			{
				AudioManager.Instance.RequestPlayAudioID("Repair", 0);
			}
			else if (p_args is ItemStatusEventArgs && (Equipment)((ItemStatusEventArgs)p_args).AffectedItem != null && ((Equipment)((ItemStatusEventArgs)p_args).AffectedItem).Broken)
			{
				AudioManager.Instance.RequestPlayAudioID("BreakItem", 0);
			}
			Inventory equipment = m_owner.Equipment.Equipment;
			if (equipment.HasBrokenItems())
			{
				m_brokenItemIcon.SetTooltip(LocaManager.GetText("ITEM_IS_BROKEN") + ":");
				foreach (Equipment equipment2 in equipment.GetBrokenItems())
				{
					m_brokenItemIcon.AddToTooltip("- " + equipment2.Name);
				}
				m_brokenItemIcon.SetEnabled(true);
			}
			else
			{
				m_brokenItemIcon.SetEnabled(false);
			}
		}

		public void MoveInFront()
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.z = -10f;
			transform.localPosition = localPosition;
		}

		public void MoveBack()
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.z = 0f;
			transform.localPosition = localPosition;
		}
	}
}
