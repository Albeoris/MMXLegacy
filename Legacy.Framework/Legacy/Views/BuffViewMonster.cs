using System;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;

namespace Legacy.Views
{
	public class BuffViewMonster : BuffViewBase
	{
		protected override EEventType[] ApplySpellEventTypes
		{
			get
			{
				EEventType[] applySpellEventTypes = base.ApplySpellEventTypes;
				Array.Resize<EEventType>(ref applySpellEventTypes, applySpellEventTypes.Length + 1);
				applySpellEventTypes[applySpellEventTypes.Length - 1] = EEventType.ITEM_SUFFIX_APPLY_BUFF;
				return applySpellEventTypes;
			}
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_CHANGED, new EventHandler(OnBuffEventFiltered));
				CancelAllFX();
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_BUFF_CHANGED, new EventHandler(OnBuffEventFiltered));
			}
		}

		protected override EEventType BuffAddEventType => EEventType.MONSTER_BUFF_ADDED;

	    protected override EEventType BuffRemoveEventType => EEventType.MONSTER_BUFF_REMOVED;

	    protected override Boolean IsBuffAddEventForMe(Object p_sender, EventArgs p_args)
		{
			MonsterBuff monsterBuff = null;
			if (p_sender is MonsterBuff)
			{
				monsterBuff = (MonsterBuff)p_sender;
			}
			Monster monster = ((MonsterBuffUpdateEventArgs)p_args).Monster;
			return monster == MyController && (monsterBuff == null || monster.BuffHandler.HasBuff(monsterBuff));
		}

		protected override Boolean IsApplySpellEventForMe(Object p_sender, EventArgs p_args, out String out_buffNameKey, out String out_spellNameKey)
		{
			out_buffNameKey = null;
			out_spellNameKey = null;
			if (p_args is SpellEventArgs)
			{
				SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
				foreach (MonsterBuffTarget monsterBuffTarget in spellEventArgs.SpellTargetsOfType<MonsterBuffTarget>())
				{
					if (monsterBuffTarget.Target == MyController && monsterBuffTarget.Buff != EMonsterBuffType.NONE && ((Monster)monsterBuffTarget.Target).BuffHandler.HasBuff(monsterBuffTarget.Buff))
					{
						out_buffNameKey = monsterBuffTarget.Buff.ToString();
						out_spellNameKey = GetSpellNameKey(spellEventArgs.Spell);
						return true;
					}
				}
				return false;
			}
			ItemSuffixApplyBuffEventArgs itemSuffixApplyBuffEventArgs = (ItemSuffixApplyBuffEventArgs)p_args;
			if (itemSuffixApplyBuffEventArgs.TargetMonster == MyController && itemSuffixApplyBuffEventArgs.Buff != EMonsterBuffType.NONE)
			{
				out_buffNameKey = itemSuffixApplyBuffEventArgs.Buff.ToString();
				out_spellNameKey = itemSuffixApplyBuffEventArgs.SuffixKey;
				return true;
			}
			return false;
		}

		protected override void GetBuffBaseFXPath(Object p_sender, EventArgs p_args, out String out_buffBaseFXPath, out String out_buffNameKey)
		{
			MonsterBuff monsterBuff = (MonsterBuff)p_sender;
			out_buffBaseFXPath = monsterBuff.Gfx;
			out_buffNameKey = monsterBuff.Type.ToString();
		}

		protected override String GetBuffDebugName(Object p_sender, EventArgs p_args)
		{
			return ((MonsterBuff)p_sender).NameKey;
		}

		protected void OnBuffEventFiltered(Object p_sender, EventArgs p_args)
		{
			if (p_sender != null && p_sender is MonsterBuff && p_args is MonsterBuffUpdateEventArgs)
			{
				OnBuffEvent(p_sender, p_args);
			}
		}

		private void OnMonsterDied(Object p_sender, EventArgs p_arg)
		{
			if (p_sender == MyController)
			{
				CancelAllFX();
			}
		}
	}
}
