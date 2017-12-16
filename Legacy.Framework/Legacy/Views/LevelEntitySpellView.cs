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
	public class LevelEntitySpellView : BaseView
	{
		[SerializeField]
		private Animator m_animator;

		[SerializeField]
		private AnimatorControl m_animatorControl;

		[SerializeField]
		private BaseEventHandler m_eventHandler;

		private LevelEntityView m_MainView;

		public new MovingEntity MyController => (MovingEntity)base.MyController;

	    protected override void Awake()
		{
			base.Awake();
			if (m_animator == null)
			{
				m_animator = this.GetComponent<Animator>(true);
			}
			if (m_animatorControl == null)
			{
				m_animatorControl = this.GetComponent<AnimatorControl>(true);
			}
			if (m_eventHandler == null)
			{
				m_eventHandler = this.GetComponentInChildren< BaseEventHandler>(true);
			}
			m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			m_MainView = this.GetComponent< LevelEntityView>(true);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && MyController == null)
			{
				throw new NotSupportedException("Only MovingEntity objects\n" + MyController.GetType().FullName);
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnEntityCastSpell));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnEntityCastSpell));
			}
		}

		protected virtual void OnEntityCastSpell(Object p_sender, EventArgs p_args)
		{
			if (p_sender != MyController)
			{
				return;
			}
			SpellEventArgs args = (SpellEventArgs)p_args;
			MonsterSpell spell = (MonsterSpell)args.Spell;
			EventHandler eventHandler = delegate(Object sender, EventArgs e)
			{
				DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, p_sender, args);
				MyController.AttackingDone.Trigger();
				if (spell.SpellType == EMonsterSpell.FLICKER)
				{
					m_MainView.SetEntityPosition();
				}
				if (MyController is Monster)
				{
					((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_CAST_SPELL);
					((Monster)MyController).BuffHandler.RemoveFlaggedBuffs();
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
					((Monster)MyController).BuffHandler.FlushActionLog(MonsterBuffHandler.ELogEntryPhase.ON_END_TURN);
				}
			};
			FXDescription fxdescription = Helper.ResourcesLoad<FXDescription>(spell.EffectKey, false);
			if (fxdescription == null)
			{
				Debug.LogError("FXDescription not found! at " + spell.EffectKey, this);
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
			Int32 animationID;
			if (Int32.TryParse(spell.EffectAnimationClip, out animationID))
			{
				m_animatorControl.AttackMagic(animationID);
			}
			else
			{
				Debug.LogError("Error parse animation id for attack magic '" + spell.EffectAnimationClip + "'");
				m_animatorControl.AttackMagic();
			}
			GameObject gameObject = this.gameObject;
			ETargetType targetType = spell.TargetType;
			if (args.SpellTargets.Count == 0)
			{
				Debug.LogError("Error, missing targets for effects\n" + DebugUtil.DumpObjectText(spell));
				FXArgs p_args2 = new FXArgs(gameObject, gameObject, gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, new List<GameObject>
				{
					gameObject
				});
				fxdescription.Configurate(m_eventHandler, p_args2);
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
				fxdescription.Configurate(m_eventHandler, p_args3);
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
				fxdescription.Configurate(m_eventHandler, p_args4);
			}
			else if ((targetType & ETargetType.SINGLE) == ETargetType.SINGLE)
			{
				Boolean flag = true;
				foreach (AttackedTarget attackedTarget in args.SpellTargetsOfType<AttackedTarget>())
				{
					Character character = (Character)attackedTarget.Target;
					GameObject characterGO = FXHelper.GetCharacterGO(character.Index);
					if (!flag)
					{
						fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
					}
					FXArgs p_args5 = new FXArgs(gameObject, characterGO, gameObject, characterGO, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, new List<GameObject>
					{
						characterGO
					});
					fxdescription.Configurate(m_eventHandler, p_args5);
					flag = false;
				}
				foreach (MonsterBuffTarget monsterBuffTarget in args.SpellTargetsOfType<MonsterBuffTarget>())
				{
					Character character2 = (Character)monsterBuffTarget.Target;
					GameObject characterGO2 = FXHelper.GetCharacterGO(character2.Index);
					if (!flag)
					{
						fxdescription = Helper.Instantiate<FXDescription>(fxdescription);
					}
					FXArgs p_args6 = new FXArgs(gameObject, characterGO2, gameObject, characterGO2, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, new List<GameObject>
					{
						characterGO2
					});
					fxdescription.Configurate(m_eventHandler, p_args6);
					flag = false;
				}
			}
			else if ((targetType & ETargetType.ADJACENT) == ETargetType.ADJACENT)
			{
				FXArgs p_args7 = new FXArgs(gameObject, gameObject, gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
				fxdescription.Configurate(m_eventHandler, p_args7);
			}
			else
			{
				FXArgs p_args8 = new FXArgs(gameObject, gameObject, gameObject, gameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
				fxdescription.Configurate(m_eventHandler, p_args8);
			}
		}
	}
}
