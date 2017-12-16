using System;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using UnityEngine;

namespace Legacy.MMGUI
{
	public class GUIHUDText
	{
		public static void Print(HUDDamageText pDamageText, AttackResult pAttackResult, Boolean pIsMagical)
		{
			if (!ConfigManager.Instance.Options.ShowFloatingDamageMonsters)
			{
				return;
			}
			if (pAttackResult.Result == EResultType.BLOCK)
			{
				pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_SHIELD"), false, Color.white, 0f);
			}
			else if (pAttackResult.Result == EResultType.EVADE)
			{
				if (pIsMagical)
				{
					pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_RESISTED"), false, Color.white, 0f);
				}
				else
				{
					pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_MISS"), false, Color.white, 0f);
				}
			}
			else if (pAttackResult.Result == EResultType.IMMUNE)
			{
				pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_IMMUNE"), false, Color.white, 0f);
			}
			else if (pAttackResult.Result == EResultType.CRITICAL_HIT)
			{
				pDamageText.Add(pAttackResult, true, new Color32(245, 240, 135, Byte.MaxValue), 0f);
			}
			else if (pAttackResult.Result == EResultType.HEAL)
			{
				pDamageText.Add(pAttackResult, false, Color.green, 0f);
			}
			else
			{
				pDamageText.Add(pAttackResult, false, Color.white, 0f);
			}
		}

		public static void PrintPortrait(HUDDamageText pDamageText, AttackResult pAttackResult, Boolean pIsMagical)
		{
			if (!ConfigManager.Instance.Options.ShowFloatingDamageCharacters)
			{
				return;
			}
			if (pAttackResult.Result == EResultType.BLOCK)
			{
				pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_SHIELD"), false, Color.white, 0f);
			}
			else if (pAttackResult.Result == EResultType.EVADE)
			{
				if (pIsMagical)
				{
					pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_RESISTED"), false, Color.white, 0f);
				}
				else
				{
					pDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_MISS"), false, Color.white, 0f);
				}
			}
			else if (pAttackResult.Result == EResultType.CRITICAL_HIT)
			{
				pDamageText.Add(pAttackResult, true, new Color32(245, 240, 135, Byte.MaxValue), 0f);
			}
			else if (pAttackResult.Result == EResultType.HEAL)
			{
				pDamageText.Add(pAttackResult, false, Color.green, 0f);
			}
			else
			{
				pDamageText.Add(pAttackResult, false, Color.white, 0f);
			}
		}
	}
}
