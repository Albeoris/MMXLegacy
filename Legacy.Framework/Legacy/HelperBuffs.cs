using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.Buffs;
using Legacy.Core.PartyManagement;
using Legacy.HUD;
using UnityEngine;

namespace Legacy
{
	public static class HelperBuffs
	{
		private const Single WAIT_TIME_SHOW_ICON = 0.7f;

		public const Single BUFF_VIEW_PADDING = 72f;

		public static void UpdateMonsterBuffs(GameObject p_parent, Boolean p_barIsVisible, List<MonsterBuff> p_buffs, List<MonsterBuffView> p_buffViews, GameObject p_buffViewPrefab, EventHandler p_onTooltip, Int32 p_offset)
		{
			if (p_offset < 0)
			{
				p_offset = 0;
			}
			Int32 num = p_offset;
			Boolean flag = false;
			foreach (MonsterBuffView monsterBuffView in p_buffViews)
			{
				monsterBuffView.UpdateDurationLabel();
			}
			foreach (MonsterBuff monsterBuff in p_buffs)
			{
				if (!monsterBuff.IsExpired || !monsterBuff.IsDebuff)
				{
					Boolean flag2 = false;
					foreach (MonsterBuffView monsterBuffView2 in p_buffViews)
					{
						if (monsterBuffView2.MonsterBuff != null && monsterBuffView2.MonsterBuff.StaticID == monsterBuff.StaticID && monsterBuffView2.gameObject.activeSelf)
						{
							flag2 = true;
							monsterBuffView2.UpdateDurationLabel();
							break;
						}
					}
					if (flag2)
					{
						num++;
					}
					else
					{
						if (p_buffViews.Count <= num)
						{
							flag = true;
							GameObject gameObject = NGUITools.AddChild(p_parent, p_buffViewPrefab);
							gameObject.transform.localPosition = new Vector3(num * 72f, 130f, 0f);
							MonsterBuffView component = gameObject.GetComponent<MonsterBuffView>();
							component.OnTooltipEvent += p_onTooltip;
							p_buffViews.Add(component);
							p_buffViews[num].Init(monsterBuff, p_barIsVisible);
							p_buffViews[num].UpdateBuff(monsterBuff);
							NGUITools.SetActive(p_buffViews[num].gameObject, false);
						}
						p_buffViews[num].UpdateBuff(monsterBuff);
						if (!p_buffViews[num].gameObject.activeSelf)
						{
							if (p_buffViews[num].IsWaitTimeDone())
							{
								NGUITools.SetActive(p_buffViews[num].gameObject, true);
							}
							else
							{
								NGUITools.SetActive(p_buffViews[num].gameObject, false);
							}
						}
						num++;
					}
				}
			}
			Boolean flag3 = false;
			List<MonsterBuffView> list = new List<MonsterBuffView>();
			if (p_buffs.Count < p_buffViews.Count - p_offset)
			{
				flag3 = true;
				for (Int32 i = p_buffViews.Count - 1; i >= p_offset; i--)
				{
					if (!p_buffs.Contains(p_buffViews[i].MonsterBuff))
					{
						if (!p_buffViews[i].SetDestroyTime)
						{
							p_buffViews[i].SetDisabled(p_barIsVisible);
						}
						else
						{
							list.Add(p_buffViews[i]);
							p_buffViews.RemoveAt(i);
						}
					}
				}
			}
			for (Int32 j = list.Count - 1; j >= 0; j--)
			{
				UnityEngine.Object.Destroy(list[j].gameObject, list[j].TimeTillDestroy);
			}
			list.Clear();
			if ((p_buffs != null && p_buffs.Count > 0) || flag3)
			{
				Vector3 pos = new Vector3((-(Single)p_buffViews.Count * 72f + 80f) / 2f, -132f, 0f);
				if (flag || flag3)
				{
					if (flag3)
					{
						for (Int32 k = p_offset; k < p_buffViews.Count; k++)
						{
							p_buffViews[k].transform.localPosition = new Vector3(k * 72f, 130f, 0f);
						}
					}
					TweenPosition tweenPosition = TweenPosition.Begin(p_parent, 0.3f, pos);
					tweenPosition.method = UITweener.Method.EaseOut;
					tweenPosition.steeperCurves = true;
				}
			}
		}

		public static void DestroyAllBuffViews(List<MonsterBuffView> p_views, EventHandler p_onToolip)
		{
			for (Int32 i = p_views.Count - 1; i >= 0; i--)
			{
				if (p_views[i].IsAbility)
				{
					p_views[i].DisableTooltip();
				}
				p_views[i].OnTooltipEvent -= p_onToolip;
				NGUITools.SetActive(p_views[i].gameObject, false);
			}
		}

		public static void AddMonsterAbilities(GameObject p_parent, List<MonsterAbilityBase> p_abilities, List<MonsterBuffView> p_buffViews, GameObject p_buffViewPrefab, EventHandler p_onTooltip)
		{
			if (p_buffViews.Count > 0)
			{
				return;
			}
			if (p_abilities.Count == 0)
			{
				return;
			}
			Int32 num = 0;
			foreach (MonsterAbilityBase p_ability in p_abilities)
			{
				GameObject gameObject = NGUITools.AddChild(p_parent, p_buffViewPrefab);
				gameObject.transform.localPosition = new Vector3(num * 72f, 130f, 0f);
				MonsterBuffView component = gameObject.GetComponent<MonsterBuffView>();
				component.IsAbility = true;
				component.OnTooltipEvent += p_onTooltip;
				p_buffViews.Add(component);
				p_buffViews[num].Init(null, false);
				p_buffViews[num].UpdateAbility(p_ability);
				NGUITools.SetActive(p_buffViews[num].gameObject, true);
				num++;
			}
			Vector3 pos = new Vector3((-(Single)num * 72f + 80f) / 2f, -132f, 0f);
			TweenPosition tweenPosition = TweenPosition.Begin(p_parent, 0.3f, pos);
			tweenPosition.method = UITweener.Method.EaseOut;
			tweenPosition.steeperCurves = true;
		}

		public static void UpdatePartyBuffs(GameObject p_parent, List<PartyBuff> p_buffs, List<PartyBuffView> p_buffViews, GameObject p_buffViewPrefab, EventHandler p_onTooltip)
		{
			Int32 num = 0;
			Boolean flag = false;
			foreach (PartyBuffView partyBuffView in p_buffViews)
			{
				partyBuffView.UpdateDurationLabel();
			}
			foreach (PartyBuff partyBuff in p_buffs)
			{
				if (!partyBuff.IsExpired())
				{
					Boolean flag2 = false;
					foreach (PartyBuffView partyBuffView2 in p_buffViews)
					{
						if (partyBuffView2.PartyBuff != null && partyBuffView2.PartyBuff.StaticData.StaticID == partyBuff.StaticData.StaticID && partyBuffView2.gameObject.activeSelf)
						{
							flag2 = true;
							partyBuffView2.UpdateDurationLabel();
							break;
						}
					}
					if (flag2)
					{
						num++;
					}
					else
					{
						if (p_buffViews.Count <= num)
						{
							flag = true;
							GameObject gameObject = NGUITools.AddChild(p_parent, p_buffViewPrefab);
							gameObject.transform.localPosition = new Vector3(num * 72f, 400f, 0f);
							PartyBuffView component = gameObject.GetComponent<PartyBuffView>();
							component.OnTooltipEvent += p_onTooltip;
							p_buffViews.Add(component);
							p_buffViews[num].Init();
							p_buffViews[num].UpdateBuff(partyBuff);
							NGUITools.SetActive(p_buffViews[num].gameObject, false);
						}
						p_buffViews[num].UpdateBuff(partyBuff);
						if (!p_buffViews[num].gameObject.activeSelf)
						{
							if (p_buffViews[num].IsWaitTimeDone())
							{
								NGUITools.SetActive(p_buffViews[num].gameObject, true);
							}
							else
							{
								NGUITools.SetActive(p_buffViews[num].gameObject, false);
							}
						}
						num++;
					}
				}
			}
			Boolean flag3 = false;
			List<PartyBuffView> list = new List<PartyBuffView>();
			if (p_buffs.Count < p_buffViews.Count)
			{
				flag3 = true;
				for (Int32 i = p_buffViews.Count - 1; i >= 0; i--)
				{
					if (!p_buffs.Contains(p_buffViews[i].PartyBuff))
					{
						if (!p_buffViews[i].SetDestroyTime)
						{
							p_buffViews[i].OnTooltipEvent -= p_onTooltip;
							p_buffViews[i].SetDisabled();
						}
						else
						{
							list.Add(p_buffViews[i]);
							p_buffViews.RemoveAt(i);
						}
					}
				}
			}
			for (Int32 j = list.Count - 1; j >= 0; j--)
			{
				if (list[j].SetDestroyTime)
				{
					PartyBuffView partyBuffView3 = list[j];
					list.RemoveAt(j);
					NGUITools.SetActive(partyBuffView3.gameObject, false);
					Helper.DestroyGO<PartyBuffView>(partyBuffView3);
				}
			}
			list.Clear();
			if ((p_buffs != null && p_buffs.Count > 0) || flag3)
			{
				Vector3 pos = new Vector3((-(Single)p_buffViews.Count * 72f + 80f) / 2f, p_parent.transform.localPosition.y, 0f);
				if (flag || flag3)
				{
					if (flag3)
					{
						for (Int32 k = 0; k < p_buffViews.Count; k++)
						{
							p_buffViews[k].transform.localPosition = new Vector3(k * 72f, 400f, 0f);
						}
					}
					TweenPosition tweenPosition = TweenPosition.Begin(p_parent, 0.3f, pos);
					tweenPosition.method = UITweener.Method.EaseOut;
					tweenPosition.steeperCurves = true;
				}
			}
		}
	}
}
