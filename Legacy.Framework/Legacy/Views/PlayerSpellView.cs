using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Player Spell View")]
	[RequireComponent(typeof(PlayerEntityView))]
	public class PlayerSpellView : BaseView
	{
		private PlayerEntityView m_PartyView;

		protected override void Awake()
		{
			base.Awake();
			m_PartyView = this.GetComponent< PlayerEntityView>(true);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
			}
		}

		private void OnCharacterCastSpell(Object p_sender, EventArgs p_args)
		{
			Character character = (Character)p_sender;
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			if (character != null)
			{
				SpellEffectEntryEventArgs p_args2 = new SpellEffectEntryEventArgs(p_sender, (SpellEventArgs)p_args);
				character.FightHandler.FeedDelayedActionLog(p_args2);
			}
			if (spellEventArgs.SpellTargets.Count == 0)
			{
				OnFXCancel(p_sender, p_args);
				return;
			}
			FXQueue fxqueue = Helper.ResourcesLoad<FXQueue>(spellEventArgs.Spell.EffectKey, false);
			if (fxqueue == null)
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"PlayerSpellView: OnCharacterCastSpell: Character FXQueue not found! at '",
					spellEventArgs.Spell.EffectKey,
					"' ",
					spellEventArgs
				}));
				OnFXCancel(p_sender, p_args);
				return;
			}
			Party party = (Party)MyController;
			List<GameObject> list = new List<GameObject>(spellEventArgs.SpellTargets.Count);
			foreach (SpellTarget spellTarget in spellEventArgs.SpellTargets)
			{
				if (spellTarget.Target is Character)
				{
					Character character2 = (Character)spellTarget.Target;
					GameObject characterGO = FXHelper.GetCharacterGO(character2.Index);
					if (!list.Contains(characterGO))
					{
						list.Add(characterGO);
					}
				}
				else
				{
					GameObject gameObject = ViewManager.Instance.FindView((BaseObject)spellTarget.Target);
					if (gameObject != null && !list.Contains(gameObject))
					{
						list.Add(gameObject);
					}
				}
				if (spellTarget.Target is Party)
				{
					break;
				}
			}
			if (list.Count == 0)
			{
				Debug.LogError("PlayerSpellView: OnCharacterCastSpell: No views found! " + spellEventArgs);
				OnFXCancel(p_sender, p_args);
				return;
			}
			Vector3 p_slotOriginPosition;
			Vector3 p_slotForward;
			Vector3 p_slotLeft;
			Vector3 p_slotTargetPosition;
			if (spellEventArgs.SpellTargets[0].Target is Monster)
			{
				ViewManager.GetSlotDatas(party.Position, ((Monster)spellEventArgs.SpellTargets[0].Target).Position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
			}
			else
			{
				Position position = party.Position + party.Direction;
				if (LegacyLogic.Instance.MapLoader.Grid.GetSlot(position) != null)
				{
					ViewManager.GetSlotDatas(party.Position, position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
				}
				else
				{
					ViewManager.GetSlotDatas(party.Position, party.Position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
				}
			}
			GameObject memberGameObject = m_PartyView.GetMemberGameObject(character.Index);
			ETargetType targetType = spellEventArgs.Spell.TargetType;
			if ((targetType & ETargetType.LINE_OF_SIGHT) == ETargetType.LINE_OF_SIGHT)
			{
				MovingEntity movingEntity = (MovingEntity)spellEventArgs.SpellTargets[0].Target;
				GameObject slotOrigin = ViewManager.Instance.GetSlotOrigin(movingEntity.Position);
				FXArgs args = new FXArgs(memberGameObject, slotOrigin, memberGameObject, slotOrigin, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list);
				FXQueue fxqueue2 = Helper.Instantiate<FXQueue>(fxqueue);
				fxqueue2.Finished += delegate(Object sender, EventArgs e)
				{
					OnFXQueueFinish(p_sender, p_args);
				};
				fxqueue2.Execute(args);
			}
			else if ((targetType & ETargetType.MULTY) == ETargetType.MULTY)
			{
				GameObject slotOrigin2;
				if (spellEventArgs.SpellTargets[0].Target is MovingEntity)
				{
					MovingEntity movingEntity2 = (MovingEntity)spellEventArgs.SpellTargets[0].Target;
					slotOrigin2 = ViewManager.Instance.GetSlotOrigin(movingEntity2.Position);
				}
				else
				{
					slotOrigin2 = ViewManager.Instance.GetSlotOrigin(party.Position);
				}
				FXArgs args2 = new FXArgs(memberGameObject, slotOrigin2, memberGameObject, slotOrigin2, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list);
				FXQueue fxqueue3 = Helper.Instantiate<FXQueue>(fxqueue);
				fxqueue3.Finished += delegate(Object sender, EventArgs e)
				{
					OnFXQueueFinish(p_sender, p_args);
				};
				fxqueue3.Execute(args2);
			}
			else if ((targetType & ETargetType.SINGLE) == ETargetType.SINGLE)
			{
				Boolean flag = true;
				foreach (GameObject gameObject2 in list)
				{
					GameObject p_endPoint = gameObject2;
					FXTags component = gameObject2.GetComponent<FXTags>();
					if (component != null)
					{
						p_endPoint = component.FindOne("HitSpot");
					}
					else
					{
						Debug.LogError("FXTags not found!!\nTarget=" + component, component);
					}
					FXArgs args3 = new FXArgs(memberGameObject, gameObject2, memberGameObject, p_endPoint, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list);
					FXQueue fxqueue4 = Helper.Instantiate<FXQueue>(fxqueue);
					if (flag)
					{
						flag = false;
						fxqueue4.Finished += delegate(Object sender, EventArgs e)
						{
							OnFXQueueFinish(p_sender, p_args);
						};
					}
					fxqueue4.Execute(args3);
				}
			}
			else if ((targetType & ETargetType.ADJACENT) == ETargetType.ADJACENT)
			{
				GameObject slotOrigin3 = ViewManager.Instance.GetSlotOrigin(party.Position);
				FXArgs args4 = new FXArgs(memberGameObject, slotOrigin3, memberGameObject, slotOrigin3, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list);
				FXQueue fxqueue5 = Helper.Instantiate<FXQueue>(fxqueue);
				fxqueue5.Finished += delegate(Object sender, EventArgs e)
				{
					OnFXQueueFinish(p_sender, p_args);
				};
				fxqueue5.Execute(args4);
			}
			else
			{
				Debug.Log("error !! spellType: " + targetType);
				FXArgs args5 = new FXArgs(memberGameObject, memberGameObject, memberGameObject, memberGameObject, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition, list);
				FXQueue fxqueue6 = Helper.Instantiate<FXQueue>(fxqueue);
				fxqueue6.Finished += delegate(Object sender, EventArgs e)
				{
					OnFXQueueFinish(p_sender, p_args);
				};
				fxqueue6.Execute(args5);
			}
		}

		private void OnFXQueueFinish(Object p_sender, EventArgs p_args)
		{
			if (MyController != null)
			{
				((Character)p_sender).FightHandler.FlushDelayedActionLog();
			}
			DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_CAST_SPELL, p_sender, p_args);
			ETargetType targetType = ((SpellEventArgs)p_args).Spell.TargetType;
			if ((targetType & ETargetType.PARTY) == ETargetType.PARTY)
			{
				DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_CAST_SPELL, p_sender, p_args);
			}
		}

		private void OnFXCancel(Object p_sender, EventArgs p_args)
		{
			if (MyController != null)
			{
				((Character)p_sender).FightHandler.FlushDelayedActionLog();
			}
			DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_CAST_SPELL, p_sender, p_args);
		}
	}
}
