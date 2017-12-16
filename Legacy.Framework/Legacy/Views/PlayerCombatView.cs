using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Player Combat View")]
	[RequireComponent(typeof(PlayerEntityView))]
	public class PlayerCombatView : BaseView
	{
		private PlayerEntityView m_PartyView;

		[SerializeField]
		private FXQueue m_EffectAttackRangedLongbow;

		[SerializeField]
		private FXQueue m_EffectAttackRangedCrossbow;

		[SerializeField]
		private FXQueue m_EffectAttackRangedHarpoonSnatch;

		[SerializeField]
		private FXQueue m_EffectAttackRangedLongbowBlocked;

		[SerializeField]
		private FXQueue m_EffectAttackRangedCrossbowBlocked;

		[SerializeField]
		private FXQueue m_EffectAttackRangedHarpoonSnatchBlocked;

		[SerializeField]
		private FXQueue m_EffectAttackRangedLongbowEvaded;

		[SerializeField]
		private FXQueue m_EffectAttackRangedCrossbowEvaded;

		[SerializeField]
		private FXQueue m_EffectAttackRangedHarpoonSnatchEvaded;

		protected override void Awake()
		{
			base.Awake();
			m_PartyView = this.GetComponent< PlayerEntityView>(true);
			enabled = false;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttacks));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttacksRanged));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterFXDone));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterFXDone));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterFXDone));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterAttacks));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterAttacksRanged));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_ATTACKS, new EventHandler(OnCharacterFXDone));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_ATTACKS_RANGED, new EventHandler(OnCharacterFXDone));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_FINISH, EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterFXDone));
			}
		}

		private void OnCharacterFXDone(Object p_sender, EventArgs p_args)
		{
			if (MyController != null)
			{
				Boolean flag = false;
				AttacksEventArgs attacksEventArgs = p_args as AttacksEventArgs;
				if (attacksEventArgs != null)
				{
					flag = attacksEventArgs.Counterattack;
				}
				if (!flag)
				{
					((Party)MyController).AttackingDone.Trigger();
					((Party)MyController).MonsterHitAnimationDone.Trigger();
					if (p_sender is Character)
					{
						((Character)p_sender).FlushActionLog();
						((Character)p_sender).FinishCastSpellEvent();
					}
				}
			}
		}

		private void OnCharacterAttacks(Object p_sender, EventArgs p_args)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			if (attacksEventArgs.Attacks.Count == 0)
			{
				OnCharacterFXDone(p_sender, p_args);
				return;
			}
			Character character = (Character)p_sender;
			character.FightHandler.FlushActionLog();
			character.EnchantmentHandler.FlushActionLog();
			DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS, p_sender, p_args);
			GameObject currentCamera = FXMainCamera.Instance.CurrentCamera;
			if (currentCamera != null)
			{
				Int32 num = Random.Range(1, 4);
				GameObject gameObject = Helper.Instantiate<GameObject>(Helper.ResourcesLoad<GameObject>("FX/Hit/HitFx" + num));
				Vector3 a = Vector3.zero;
				foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
				{
					if (attackedTarget.AttackTarget != null && attackedTarget.AttackTarget is Monster)
					{
						Monster p_controller = (Monster)attackedTarget.AttackTarget;
						GameObject gameObject2 = ViewManager.Instance.FindView(p_controller).gameObject;
						MonsterHPBarView component = gameObject2.GetComponent<MonsterHPBarView>();
						a = gameObject2.transform.position + new Vector3(0f, component.HPBarAnchor.position.y / 1.8f, 0f);
					}
				}
				gameObject.transform.position = a - currentCamera.transform.forward * 1.9f;
				gameObject.transform.rotation = currentCamera.transform.rotation;
				Destroy(gameObject.gameObject, 0.5f);
			}
		}

		private void OnCharacterAttacksRanged(Object p_sender, EventArgs p_args)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			if (attacksEventArgs.Attacks.Count == 0)
			{
				OnCharacterFXDone(p_sender, p_args);
				return;
			}
			Character character = (Character)p_sender;
			Boolean flag = true;
			foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
			{
				Monster monster = (Monster)attackedTarget.AttackTarget;
				EResultType result = attackedTarget.AttackResult.Result;
				FXQueue rangedFX = GetRangedFX(attacksEventArgs.IsTryingToPushToParty, character, result);
				if (rangedFX != null)
				{
					Vector3 p_slotOriginPosition;
					Vector3 p_slotForward;
					Vector3 p_slotLeft;
					Vector3 p_slotTargetPosition;
					ViewManager.GetSlotDatas(((Party)MyController).Position, monster.Position, out p_slotOriginPosition, out p_slotForward, out p_slotLeft, out p_slotTargetPosition);
					GameObject memberGameObject = m_PartyView.GetMemberGameObject(character.Index);
					GameObject gameObject = null;
					GameObject gameObject2 = null;
					if (result == EResultType.EVADE)
					{
						gameObject = ViewManager.Instance.FindView(monster);
						gameObject2 = new GameObject("Miss hit point on the ground");
						Destroy(gameObject2, 30f);
						gameObject2.transform.position = gameObject.transform.position + (transform.position - gameObject.transform.position).normalized + transform.right * 2f * (UnityEngine.Random.value - 0.5f);
					}
					else
					{
						FXTags fxtags = ViewManager.Instance.FindViewAndGetComponent<FXTags>(monster);
						if (fxtags != null)
						{
							gameObject = fxtags.gameObject;
							gameObject2 = fxtags.FindOne((result != EResultType.BLOCK) ? "HitSpot" : "BlockHitSpot");
						}
					}
					if (memberGameObject == null || gameObject == null || gameObject2 == null)
					{
						if (flag)
						{
							OnRangedFXHit(p_sender, p_args);
						}
						OnCharacterFXDone(p_sender, p_args);
						Debug.LogError(String.Concat(new Object[]
						{
							"Attacker/Target not found!\nAttacker=",
							memberGameObject,
							"\ntargetGO=",
							gameObject,
							"\nfxEndpointGO=",
							gameObject2
						}));
						break;
					}
					FXArgs args = new FXArgs(memberGameObject, gameObject, memberGameObject, gameObject2, p_slotOriginPosition, p_slotForward, p_slotLeft, p_slotTargetPosition);
					FXQueue fxqueue = Helper.Instantiate<FXQueue>(rangedFX);
					fxqueue.Execute(args);
					if (flag)
					{
						flag = false;
						fxqueue.Finished += delegate(Object o, EventArgs e)
						{
							OnRangedFXHit(p_sender, p_args);
						};
					}
				}
				else
				{
					if (flag)
					{
						flag = false;
						OnRangedFXHit(p_sender, p_args);
					}
					OnCharacterFXDone(p_sender, p_args);
					Debug.LogError("Ranged FX(" + result + ") missing!");
				}
			}
		}

		private FXQueue GetRangedFX(Boolean p_isTryingToPushToParty, Character p_attacker, EResultType p_attackResult)
		{
			Int32 num = 0;
			BaseItem itemAt = p_attacker.Equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
			if (p_isTryingToPushToParty)
			{
				num += 2;
			}
			else if (itemAt != null && itemAt is RangedWeapon && (itemAt as RangedWeapon).GetWeaponType() == EEquipmentType.CROSSBOW)
			{
				num++;
			}
			if (p_attackResult == EResultType.BLOCK)
			{
				num += 10;
			}
			else if (p_attackResult == EResultType.EVADE)
			{
				num += 20;
			}
			Int32 num2 = num;
			FXQueue result;
			switch (num2)
			{
			case 10:
				result = m_EffectAttackRangedLongbowBlocked;
				break;
			case 11:
				result = m_EffectAttackRangedCrossbowBlocked;
				break;
			case 12:
				result = m_EffectAttackRangedHarpoonSnatchBlocked;
				break;
			default:
				switch (num2)
				{
				case 20:
					result = m_EffectAttackRangedLongbowEvaded;
					break;
				case 21:
					result = m_EffectAttackRangedCrossbowEvaded;
					break;
				case 22:
					result = m_EffectAttackRangedHarpoonSnatchEvaded;
					break;
				default:
					if (num2 != 1)
					{
						if (num2 != 2)
						{
							result = m_EffectAttackRangedLongbow;
						}
						else
						{
							result = m_EffectAttackRangedHarpoonSnatch;
						}
					}
					else
					{
						result = m_EffectAttackRangedCrossbow;
					}
					break;
				}
				break;
			}
			return result;
		}

		private void OnRangedFXHit(Object p_sender, EventArgs p_args)
		{
			Character character = (Character)p_sender;
			character.FightHandler.FlushActionLog();
			character.FightHandler.FlushDelayedActionLog();
			character.EnchantmentHandler.FlushActionLog();
			DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, EEventType.CHARACTER_ATTACKS_RANGED, p_sender, p_args);
		}
	}
}
