using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	public class HirelingHUD : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UISprite m_portrait;

		private Npc m_npc;

		private Int32 m_idx;

		public Npc Npc => m_npc;

	    public void Init(Int32 p_idx)
		{
			m_idx = p_idx;
			Cleanup();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingsUpdated));
		}

		private void OnHirelingsUpdated(Object p_sender, EventArgs p_args)
		{
			if (p_args is HirelingEventArgs && ((HirelingEventArgs)p_args).Index == m_idx)
			{
				HirelingEventArgs hirelingEventArgs = (HirelingEventArgs)p_args;
				ETargetCondition condition = hirelingEventArgs.Condition;
				if (condition != ETargetCondition.HIRE)
				{
					if (condition == ETargetCondition.FIRE)
					{
						SetView(null);
					}
				}
				else
				{
					SetView(hirelingEventArgs.Npc);
				}
			}
		}

		public void SetView(Npc p_npc)
		{
			m_npc = p_npc;
			if (p_npc == null)
			{
				NGUITools.SetActiveSelf(m_background.gameObject, false);
				NGUITools.SetActiveSelf(m_portrait.gameObject, false);
			}
			else
			{
				NGUITools.SetActiveSelf(m_background.gameObject, true);
				m_portrait.spriteName = p_npc.StaticData.PortraitKey;
				NGUITools.SetActiveSelf(m_portrait.gameObject, true);
			}
		}

		public void OnPortraitClicked(GameObject p_sender)
		{
			if (m_npc != null)
			{
				if (LegacyLogic.Instance.WorldManager.Party.HasAggro() || LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					return;
				}
				LegacyLogic.Instance.ConversationManager.OpenNpcDialog(m_npc, m_npc.ConversationData.RootDialog.ID);
			}
		}

		public void Cleanup()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingsUpdated));
		}

		private void OnTooltip(Boolean p_show)
		{
			if (m_npc != null && m_npc.StaticData.HirelingProfession != String.Empty)
			{
				if (p_show)
				{
					Single num = 0f;
					Single num2 = 0f;
					if (m_npc.StaticData.NpcEffects.Length > 0)
					{
						num = m_npc.StaticData.NpcEffects[0].EffectValue;
					}
					if (m_npc.GetNpcEffect(ETargetCondition.HIRE_CURE).TargetEffect != ETargetCondition.NONE)
					{
						num = ConfigManager.Instance.Game.CostCure;
						if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
						{
							num = (Int32)Math.Ceiling(num * ConfigManager.Instance.Game.NpcCureFactor);
						}
					}
					NpcEffect npcEffect = m_npc.GetNpcEffect(ETargetCondition.HIRE_MMEXPLODISTANCE);
					if (npcEffect.TargetEffect != ETargetCondition.NONE)
					{
						num2 = npcEffect.EffectValue;
					}
					NpcEffect npcEffect2 = m_npc.GetNpcEffect(ETargetCondition.HIRE_BONUSGF);
					if (npcEffect2.TargetEffect != ETargetCondition.NONE)
					{
						num2 = npcEffect2.EffectValue * 100f;
					}
					if (m_npc.GetNpcEffect(ETargetCondition.HIRE_BLOCK).TargetEffect != ETargetCondition.NONE)
					{
						num2 = ConfigManager.Instance.Game.HirelingBlockChance * 100f;
					}
					Boolean flag = m_npc.StaticData.HirelingProfession == "HIRELING_TREASURE_HUNTER_F";
					flag |= (m_npc.StaticData.HirelingProfession == "HIRELING_TREASURE_HUNTER_M");
					flag |= (m_npc.StaticData.HirelingProfession == "HIRELING_TRAP_DISARMER_M");
					flag |= (m_npc.StaticData.HirelingProfession == "HIRELING_NEGOTIATOR_M");
					flag |= (m_npc.StaticData.HirelingProfession == "HIRELING_SCHOLAR_M");
					flag |= (m_npc.StaticData.HirelingProfession == "HIRELING_HOUND_M_ANIMAL");
					if (flag)
					{
						num *= 100f;
					}
					String p_captionText = LocaManager.GetText(m_npc.StaticData.NameKey) + " - " + LocaManager.GetText(m_npc.StaticData.HirelingProfession);
					String text = LocaManager.GetText(m_npc.StaticData.HirelingProfession + "_DESC", num, num2);
					if (LegacyLogic.Instance.WorldManager.Party.HasAggro() || LegacyLogic.Instance.WorldManager.Party.InCombat)
					{
						text = text + "\n\n[FF0000]" + LocaManager.GetText("NOT_AVAILABLE_IN_COMBAT") + "[-]";
					}
					TooltipManager.Instance.Show(this, p_captionText, text, TextTooltip.ESize.BIG, transform.position, m_background.transform.localScale * 0.5f);
				}
				else
				{
					TooltipManager.Instance.Hide(this);
				}
			}
		}
	}
}
