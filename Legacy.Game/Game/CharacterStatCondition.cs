using System;
using Legacy.Core.Configuration;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/CharacterStatCondition")]
	public class CharacterStatCondition : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_bar;

		[SerializeField]
		private UISprite m_icon;

		private String m_tt;

		public void SetCondition(ECondition p_condition, Character p_character)
		{
			m_icon.spriteName = GetIcon(p_condition);
			if (p_character.ConditionHandler.HasCondition(p_condition))
			{
				SetEnabled(p_condition, p_character);
			}
			else
			{
				SetDisabled();
			}
		}

		private void SetEnabled(ECondition p_condition, Character p_character)
		{
			m_bar.spriteName = GetSpriteName(p_condition);
			m_tt = GetConditionToolTip(p_condition, p_character);
		}

		private void SetDisabled()
		{
			m_bar.spriteName = "BAR_condition_idle";
			m_tt = String.Empty;
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private String GetIcon(ECondition p_condition)
		{
			switch (p_condition)
			{
			case ECondition.UNCONSCIOUS:
				return "ICO_condition_unconscious";
			default:
				if (p_condition == ECondition.STUNNED)
				{
					return "ICO_condition_knockedout";
				}
				if (p_condition == ECondition.SLEEPING)
				{
					return "ICO_condition_sleeping";
				}
				if (p_condition == ECondition.POISONED)
				{
					return "ICO_condition_poisoned";
				}
				if (p_condition == ECondition.CONFUSED)
				{
					return "ICO_condition_confused";
				}
				if (p_condition == ECondition.WEAK)
				{
					return "ICO_condition_weak";
				}
				if (p_condition != ECondition.CURSED)
				{
					throw new NotImplementedException();
				}
				return "ICO_condition_cursed";
			case ECondition.PARALYZED:
				return "ICO_condition_paralyzed";
			}
		}

		private String GetSpriteName(ECondition p_condition)
		{
			switch (p_condition)
			{
			case ECondition.UNCONSCIOUS:
			case ECondition.PARALYZED:
				break;
			default:
				if (p_condition != ECondition.STUNNED && p_condition != ECondition.SLEEPING)
				{
					if (p_condition == ECondition.POISONED)
					{
						return "BAR_condition_poisoned";
					}
					if (p_condition == ECondition.CONFUSED)
					{
						return "BAR_condition_confused";
					}
					if (p_condition == ECondition.WEAK)
					{
						return "BAR_condition_weak";
					}
					if (p_condition != ECondition.CURSED)
					{
						throw new NotImplementedException();
					}
					return "BAR_condition_cursed";
				}
				break;
			}
			return "BAR_condition_unconscious";
		}

		private String GetConditionName(ECondition p_condition)
		{
			switch (p_condition)
			{
			case ECondition.DEAD:
				return "CHARACTER_CONDITION_DEAD";
			case ECondition.UNCONSCIOUS:
				return "CHARACTER_CONDITION_UNCONSCIOUS";
			default:
				if (p_condition == ECondition.SLEEPING)
				{
					return "CHARACTER_CONDITION_SLEEPING";
				}
				if (p_condition == ECondition.POISONED)
				{
					return "CHARACTER_CONDITION_POISONED";
				}
				if (p_condition == ECondition.CONFUSED)
				{
					return "CHARACTER_CONDITION_CONFUSED";
				}
				if (p_condition == ECondition.WEAK)
				{
					return "CHARACTER_CONDITION_WEAK";
				}
				if (p_condition != ECondition.CURSED)
				{
					return "CHARACTER_CONDITION_EXHAUSTED";
				}
				return "CHARACTER_CONDITION_CURSED";
			case ECondition.PARALYZED:
				return "CHARACTER_CONDITION_PARALYZED";
			case ECondition.STUNNED:
				return "CHARACTER_CONDITION_KNOCKED_OUT";
			}
		}

		private String GetConditionToolTip(ECondition p_condition, Character p_character)
		{
			String str = "_M";
			if (p_character.Gender == EGender.FEMALE)
			{
				str = "_F";
			}
			String text;
			switch (p_condition)
			{
			case ECondition.DEAD:
			{
				String id = "CHARACTER_CONDITION_DEAD_TT" + str;
				text = LocaManager.GetText(id, p_character.Name);
				break;
			}
			case ECondition.UNCONSCIOUS:
			{
				String id = "CHARACTER_CONDITION_UNCONSCIOUS_TT" + str;
				text = LocaManager.GetText(id, p_character.Name);
				break;
			}
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
									String id = "CHARACTER_CONDITION_EXHAUSTED_TT" + str;
									text = LocaManager.GetText(id, p_character.Name, ConfigManager.Instance.Game.HoursDeficiencySyndromesRest);
								}
								else
								{
									String id = "CHARACTER_CONDITION_CURSED_TT" + str;
									text = LocaManager.GetText(id, p_character.Name, Mathf.RoundToInt(ConfigManager.Instance.Game.WeakAttribDecrease * 100f), Mathf.RoundToInt(ConfigManager.Instance.Game.CursedAttackDecrease * 100f));
								}
							}
							else
							{
								String id = "CHARACTER_CONDITION_WEAK_TT" + str;
								text = LocaManager.GetText(id, p_character.Name, Mathf.RoundToInt(ConfigManager.Instance.Game.WeakAttribDecrease * 100f));
							}
						}
						else
						{
							String id = "CHARACTER_CONDITION_CONFUSED_TT" + str;
							text = LocaManager.GetText(id, p_character.Name);
						}
					}
					else
					{
						String id = "CHARACTER_CONDITION_POISONED_TT" + str;
						text = LocaManager.GetText(id, p_character.Name, p_character.ConditionHandler.PoisonEvadeDecrease, (Int32)Math.Round(p_character.MaximumHealthPoints * ConfigManager.Instance.Game.PoisonHealthDamage));
					}
				}
				else
				{
					String id = "CHARACTER_CONDITION_SLEEPING_TT" + str;
					text = LocaManager.GetText(id, p_character.Name);
				}
				break;
			case ECondition.PARALYZED:
			{
				String id = "CHARACTER_CONDITION_PARALYZED_TT" + str;
				text = LocaManager.GetText(id, p_character.Name);
				break;
			}
			case ECondition.STUNNED:
			{
				String id = "CHARACTER_CONDITION_KNOCKED_OUT_TT" + str;
				text = LocaManager.GetText(id, p_character.Name, ConfigManager.Instance.Game.KnockedOutTurnCount);
				break;
			}
			}
			return text;
		}

		public void OnTooltip(Boolean isOver)
		{
			if (isOver && m_tt != String.Empty)
			{
				Vector3 position = m_bar.transform.position;
				Vector3 p_offset = m_bar.transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, m_tt, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
