using System;
using System.Collections.Generic;
using Legacy.Animations;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using Legacy.Utilities;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Monster Spell View")]
	public class MonsterSpellView : BaseView
	{
		[SerializeField]
		private AnimHandler m_Animation;

		[SerializeField]
		private AnimEventHandler m_AnimationEvents;

		public new Monster MyController => (Monster)base.MyController;

	    protected override void Awake()
		{
			base.Awake();
			if (m_Animation == null)
			{
				m_Animation = this.GetComponent<AnimHandler>(true);
			}
			if (m_AnimationEvents == null)
			{
				m_AnimationEvents = this.GetComponent< AnimEventHandler>(true);
			}
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
			}
		}

		private void OnMonsterCastSpell(Object p_sender, EventArgs p_args)
		{
			if (p_sender != MyController)
			{
				return;
			}
			SpellEventArgs args = (SpellEventArgs)p_args;
			MonsterSpell monsterSpell = (MonsterSpell)args.Spell;
			EventHandler eventHandler = delegate(Object sender, EventArgs e)
			{
				DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, p_sender, p_args);
				MyController.AttackingDone.Trigger();
				if (MyController is Monster)
				{
					MyController.BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_CAST_SPELL);
					MyController.BuffHandler.RemoveFlaggedBuffs();
				}
				SpellEffectEntryEventArgs p_args9 = new SpellEffectEntryEventArgs(p_sender, args);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args9);
				foreach (SpellTarget spellTarget2 in args.SpellTargets)
				{
					if (spellTarget2.Target is Character)
					{
						((Character)spellTarget2.Target).ConditionHandler.FlushActionLog();
						((Character)spellTarget2.Target).ConditionHandler.FlushDelayedActionLog();
						LegacyLogic.Instance.WorldManager.Party.Buffs.FlushActionLog();
					}
				}
				if (MyController is Monster)
				{
					MyController.BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_END_TURN);
				}
			};
			FXDescription fxdescription = Helper.ResourcesLoad<FXDescription>(monsterSpell.EffectKey, false);
			if (fxdescription == null)
			{
				Debug.LogError("FXDescription not found! at " + monsterSpell.EffectKey, this);
				eventHandler(this, EventArgs.Empty);
				return;
			}
			fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
			fxdescription.Finished += eventHandler;
			Vector3 p_slotOriginPosition;
			Vector3 p_slotForward;
			Vector3 p_slotLeft;
			Vector3 p_slotTargetPosition;
			ViewManager.GetSlotDatas(MyController.Position, LegacyLogic.Instance.WorldManager.Party.Position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
			m_Animation.Play(monsterSpell.EffectAnimationClip, -1f, 1f);
			GameObject gameObject = this.gameObject;
			ETargetType targetType = monsterSpell.TargetType;
			if (args.SpellTargets.Count == 0)
			{
				Debug.LogError("Error, missing targets for effects\n" + DebugUtil.DumpObjectText(monsterSpell));
				FXArgs p_args2 = new FXArgs(gameObject, gameObject, gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
				fxdescription.Configurate(m_AnimationEvents, p_args2);
				return;
			}
			if ((targetType & ETargetType.MONSTER) == ETargetType.MONSTER)
			{
				PlayerEntityView playerEntityView = ViewManager.Instance.FindViewAndGetComponent<PlayerEntityView>(LegacyLogic.Instance.WorldManager.Party);
				MovingEntity movingEntity = null;
				List<GameObject> list = new List<GameObject>();
				foreach (SpellTarget spellTarget in args.SpellTargets)
				{
					GameObject gameObject2 = null;
					if (spellTarget.Target is Character)
					{
						if (movingEntity == null)
						{
							movingEntity = LegacyLogic.Instance.WorldManager.Party;
						}
						gameObject2 = playerEntityView.GetMemberGameObject(((Character)spellTarget.Target).Index);
					}
					else if (spellTarget.Target is MovingEntity)
					{
						if (movingEntity == null)
						{
							movingEntity = (MovingEntity)spellTarget.Target;
						}
						gameObject2 = ViewManager.Instance.FindView((MovingEntity)spellTarget.Target);
					}
					if (gameObject2 != null)
					{
						list.Add(gameObject2);
					}
				}
				ViewManager.GetSlotDatas(MyController.Position, movingEntity.Position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
				FXArgs p_args3 = new FXArgs(gameObject, playerEntityView.gameObject, gameObject, playerEntityView.gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list);
				fxdescription.Configurate(m_AnimationEvents, p_args3);
			}
			else if ((targetType & ETargetType.MULTY) == ETargetType.MULTY)
			{
				PlayerEntityView playerEntityView2 = ViewManager.Instance.FindViewAndGetComponent<PlayerEntityView>(LegacyLogic.Instance.WorldManager.Party);
				List<GameObject> list2 = new List<GameObject>(4);
				for (Int32 i = 0; i < 4; i++)
				{
					list2.Add(playerEntityView2.GetMemberGameObject(i));
				}
				FXArgs p_args4 = new FXArgs(gameObject, playerEntityView2.gameObject, gameObject, playerEntityView2.gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list2);
				fxdescription.Configurate(m_AnimationEvents, p_args4);
			}
			else if ((targetType & ETargetType.SINGLE) == ETargetType.SINGLE)
			{
				PlayerEntityView playerEntityView3 = ViewManager.Instance.FindViewAndGetComponent<PlayerEntityView>(LegacyLogic.Instance.WorldManager.Party);
				Boolean flag = true;
				foreach (AttackedTarget attackedTarget in args.SpellTargetsOfType<AttackedTarget>())
				{
					Character character = (Character)attackedTarget.Target;
					if (character == null)
					{
						return;
					}
					GameObject memberGameObject = playerEntityView3.GetMemberGameObject(character.Index);
					if (!flag)
					{
						fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
					}
					FXArgs p_args5 = new FXArgs(gameObject, memberGameObject, gameObject, memberGameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, new List<GameObject>
					{
						memberGameObject
					});
					fxdescription.Configurate(m_AnimationEvents, p_args5);
					flag = false;
				}
				foreach (MonsterBuffTarget monsterBuffTarget in args.SpellTargetsOfType<MonsterBuffTarget>())
				{
					Character character2 = (Character)monsterBuffTarget.Target;
					if (character2 == null)
					{
						break;
					}
					GameObject memberGameObject2 = playerEntityView3.GetMemberGameObject(character2.Index);
					if (!flag)
					{
						fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
					}
					FXArgs p_args6 = new FXArgs(gameObject, memberGameObject2, gameObject, memberGameObject2, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, new List<GameObject>
					{
						memberGameObject2
					});
					fxdescription.Configurate(m_AnimationEvents, p_args6);
					flag = false;
				}
			}
			else if ((targetType & ETargetType.ADJACENT) == ETargetType.ADJACENT)
			{
				FXArgs p_args7 = new FXArgs(gameObject, gameObject, gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
				fxdescription.Configurate(m_AnimationEvents, p_args7);
			}
			else
			{
				FXArgs p_args8 = new FXArgs(gameObject, gameObject, gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
				fxdescription.Configurate(m_AnimationEvents, p_args8);
			}
		}
	}
}
