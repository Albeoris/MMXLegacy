using System;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/CharacterStatBox")]
	public class CharacterStatBox : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_leftColumn;

		[SerializeField]
		private UILabel m_rightColumn;

		private Party m_party;

		private String m_currentItemName;

		public void Init(Party p_party)
		{
			m_party = p_party;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(UpdateAttributes));
		}

		public void CleanUp()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(UpdateAttributes));
		}

		public void OnSelectionChange(String p_selectedItemName)
		{
			if (m_party == null)
			{
				return;
			}
			m_currentItemName = p_selectedItemName;
			UpdateStats();
		}

		public void UpdateStats()
		{
			if (m_currentItemName == "GUI_CHARACTER_STATS_ATTRIBUTES")
			{
				FillAttributes();
			}
			else if (m_currentItemName == "GUI_CHARACTER_STATS_ATTACK")
			{
				FillAttackValues();
			}
			else if (m_currentItemName == "GUI_CHARACTER_STATS_DAMAGE")
			{
				FillDamage();
			}
			else if (m_currentItemName == "GUI_CHARACTER_STATS_DEFENSE")
			{
				FillDefense();
			}
			else if (m_currentItemName == "GUI_CHARACTER_STATS_RESISTANCES")
			{
				FillResistance();
			}
		}

		private void UpdateAttributes(Object p_sender, EventArgs p_args)
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			if (member == p_sender)
			{
				UpdateStats();
			}
		}

		private void FillAttributes()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			m_leftColumn.text = String.Concat(new String[]
			{
				"Health:\nMana:\n",
				LocaManager.GetText("CHARACTER_ATTRIBUTE_MIGHT"),
				"\n",
				LocaManager.GetText("CHARACTER_ATTRIBUTE_MAGIC"),
				"\n",
				LocaManager.GetText("CHARACTER_ATTRIBUTE_PERCEPTION"),
				"\n",
				LocaManager.GetText("CHARACTER_ATTRIBUTE_DESTINY"),
				"\n",
				LocaManager.GetText("CHARACTER_ATTRIBUTE_VITALITY"),
				"\n",
				LocaManager.GetText("CHARACTER_ATTRIBUTE_SPIRIT"),
				"\n"
			});
			m_rightColumn.text = String.Concat(new Object[]
			{
				LocaManager.GetText("GUI_STATS_X_OF_Y", member.HealthPoints, member.CurrentAttributes.HealthPoints),
				"\n",
				LocaManager.GetText("GUI_STATS_X_OF_Y", member.ManaPoints, member.CurrentAttributes.ManaPoints),
				"\n",
				member.CurrentAttributes.Might,
				"\n",
				member.CurrentAttributes.Magic,
				"\n",
				member.CurrentAttributes.Perception,
				"\n",
				member.CurrentAttributes.Destiny,
				"\n",
				member.CurrentAttributes.Vitality,
				"\n",
				member.CurrentAttributes.Spirit,
				"\n"
			});
		}

		private void FillAttackValues()
		{
			Boolean flag = false;
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			BaseItem itemAt = member.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			if (itemAt is MeleeWeapon && (itemAt as MeleeWeapon).GetSubType() == EEquipmentType.TWOHANDED)
			{
				flag = true;
			}
			m_leftColumn.text = "Melee:\n" + LocaManager.GetText("CHARACTER_ATTACK_RANGED") + "\nMainh Crit.:\nOffhand Crit.:\nRange Crit.:\nMagic Crit.:\n";
			if (!flag)
			{
				m_rightColumn.text = String.Concat(new Object[]
				{
					member.FightValues.MainHandAttackValue,
					"/",
					member.FightValues.OffHandAttackValue,
					"\n"
				});
			}
			else
			{
				m_rightColumn.text = member.FightValues.MainHandAttackValue + "\n";
			}
			UILabel rightColumn = m_rightColumn;
			String text = rightColumn.text;
			rightColumn.text = String.Concat(new Object[]
			{
				text,
				member.FightValues.RangedAttackValue,
				"\n",
				member.FightValues.CriticalMainHandHitChance * 100f,
				"%\n",
				member.FightValues.CriticalOffHandHitChance * 100f,
				"%\n",
				Mathf.RoundToInt(member.FightValues.CriticalRangeHitChance * 100f),
				"%\n",
				Mathf.RoundToInt(member.FightValues.CriticalMagicHitChance * 100f),
				"%\n"
			});
		}

		private void FillDamage()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			BaseItem itemAt = member.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			Boolean flag = false;
			if (itemAt is MeleeWeapon && (itemAt as MeleeWeapon).GetSubType() == EEquipmentType.TWOHANDED)
			{
				flag = true;
			}
			if (!flag)
			{
				m_leftColumn.text = "Mainhand:\nOffhand:\n";
				m_rightColumn.text = String.Concat(new Object[]
				{
					member.FightValues.MainHandDamage[EDamageType.PHYSICAL].Minimum,
					"-",
					member.FightValues.MainHandDamage[EDamageType.PHYSICAL].Maximum,
					"\n",
					member.FightValues.OffHandDamage[EDamageType.PHYSICAL].Minimum,
					"-",
					member.FightValues.OffHandDamage[EDamageType.PHYSICAL].Maximum,
					"\n"
				});
			}
			else
			{
				m_leftColumn.text = "Twohand:\n";
				m_rightColumn.text = String.Concat(new Object[]
				{
					member.FightValues.MainHandDamage[EDamageType.PHYSICAL].Minimum,
					"-",
					member.FightValues.MainHandDamage[EDamageType.PHYSICAL].Maximum,
					"\n"
				});
			}
			UILabel leftColumn = m_leftColumn;
			leftColumn.text += "Range:\nMainhand Crit.:\nOffhand Crit.:\nRange Crit.:\nMagic Crit.:\n";
			UILabel rightColumn = m_rightColumn;
			String text = rightColumn.text;
			rightColumn.text = String.Concat(new Object[]
			{
				text,
				member.FightValues.RangeDamage[EDamageType.PHYSICAL].Minimum,
				"-",
				member.FightValues.RangeDamage[EDamageType.PHYSICAL].Maximum,
				"\n",
				Mathf.RoundToInt(member.FightValues.MainHandCriticalDamageMod * 100f),
				"%\n",
				Mathf.RoundToInt(member.FightValues.OffHandCriticalDamageMod * 100f),
				"%\n",
				Mathf.RoundToInt(member.FightValues.RangeCriticalDamageMod * 100f),
				"%\n",
				Mathf.RoundToInt((member.FightValues.MagicalCriticalDamageMod - 1f) * 100f),
				"%\n"
			});
		}

		private void FillDefense()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			m_leftColumn.text = LocaManager.GetText("CHARACTER_DEFENSE_AC") + "\n" + LocaManager.GetText("CHARACTER_DEFENSE_EVADE_VALUE") + "\nMelee Blocks:\nGeneral Blocks:\nBlock Chance:\n";
			m_rightColumn.text = String.Concat(new Object[]
			{
				member.FightValues.ArmorValue,
				"\n",
				member.FightValues.EvadeValue,
				"\n",
				member.FightHandler.CurrentMeleeBlockAttempts,
				"\n",
				member.FightHandler.CurrentGeneralBlockAttempts,
				"\n",
				Mathf.RoundToInt(member.FightValues.GeneralBlockChance * 100f),
				"%\n"
			});
		}

		private void FillResistance()
		{
			Character member = m_party.GetMember(m_party.CurrentCharacter);
			m_leftColumn.text = String.Concat(new String[]
			{
				LocaManager.GetText("CHARACTER_RESISTANCE_FIRE"),
				"\n",
				LocaManager.GetText("CHARACTER_RESISTANCE_WATER"),
				"\n",
				LocaManager.GetText("CHARACTER_RESISTANCE_AIR"),
				"\n",
				LocaManager.GetText("CHARACTER_RESISTANCE_EARTH"),
				"\n",
				LocaManager.GetText("CHARACTER_RESISTANCE_LIGHT"),
				"\n",
				LocaManager.GetText("CHARACTER_RESISTANCE_DARK"),
				"\n",
				LocaManager.GetText("CHARACTER_RESISTANCE_PRIMORDIAL"),
				"\n"
			});
			m_rightColumn.text = String.Concat(new Object[]
			{
				member.FightValues.Resistance[EDamageType.FIRE].Value,
				"\n",
				member.FightValues.Resistance[EDamageType.WATER].Value,
				"\n",
				member.FightValues.Resistance[EDamageType.AIR].Value,
				"\n",
				member.FightValues.Resistance[EDamageType.EARTH].Value,
				"\n",
				member.FightValues.Resistance[EDamageType.LIGHT].Value,
				"\n",
				member.FightValues.Resistance[EDamageType.DARK].Value,
				"\n",
				member.FightValues.Resistance[EDamageType.PRIMORDIAL].Value,
				"\n"
			});
		}
	}
}
