using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Animations;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Spells;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class AbilityViewMonster : BaseView
	{
		[SerializeField]
		protected AnimHandler m_Animation;

		[SerializeField]
		protected AnimEventHandler m_AnimationEvents;

		[SerializeField]
		protected Animator m_animator;

		[SerializeField]
		protected AnimatorControl m_animatorControl;

		[SerializeField]
		protected BaseEventHandler m_eventHandler;

		protected Boolean m_old;

		protected List<FXQueue> m_Queues = new List<FXQueue>();

		private Single m_explosiveAbilityDelay = 0.3f;

		public void ChangeExplosiveAbilityDelay(Single p_explosiveAbilityDelay)
		{
			m_explosiveAbilityDelay = p_explosiveAbilityDelay;
		}

		protected override void Awake()
		{
			if (GetComponentInChildren<AnimEventHandler>() != null)
			{
				base.Awake();
				if (m_Animation == null)
				{
					m_Animation = this.GetComponentInChildren<AnimHandler>(true);
				}
				if (m_AnimationEvents == null)
				{
					m_AnimationEvents = this.GetComponentInChildren< AnimEventHandler>(true);
				}
				m_old = true;
			}
			else
			{
				base.Awake();
				if (m_animator == null)
				{
					m_animator = this.GetComponentInChildren< Animator>(true);
				}
				if (m_animatorControl == null)
				{
					m_animatorControl = this.GetComponentInChildren< AnimatorControl>(true);
				}
				if (m_eventHandler == null)
				{
					m_eventHandler = this.GetComponentInChildren< BaseEventHandler>(true);
				}
				m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ENTITY_ABILITY_ADDED, new EventHandler(OnEntityAbilityAdded));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ENTITY_ABILITY_ADDED, new EventHandler(OnEntityAbilityAdded));
			}
		}

		private void OnMonsterDied(Object p_sender, EventArgs p_arg)
		{
			if (p_sender == MyController)
			{
			}
		}

		protected virtual void OnEntityAbilityAdded(Object p_sender, EventArgs p_args)
		{
			AbilityEventArgs abilityEventArgs = (AbilityEventArgs)p_args;
			if (abilityEventArgs.Monster == MyController)
			{
				String gfx = abilityEventArgs.Ability.StaticData.Gfx;
				String animation = abilityEventArgs.Ability.StaticData.Animation;
				if (!String.IsNullOrEmpty(animation))
				{
					Int32 animationID = Int32.Parse(animation);
					if (m_old)
					{
						m_Animation.Play(animation, -1f, 1f);
					}
					else if (m_animatorControl.DieState == 0)
					{
						m_animatorControl.AttackMagic(animationID);
					}
				}
				if (!String.IsNullOrEmpty(gfx))
				{
					if (gfx == "SKIP_FX")
					{
						return;
					}
					BuffFX buffFX = Helper.ResourcesLoad<BuffFX>(gfx, false);
					if (buffFX != null)
					{
						buffFX = Helper.Instantiate<BuffFX>(buffFX);
						FXQueue fxqueue = new GameObject(name + " " + buffFX.name + " FXQueue").AddComponent<FXQueue>();
						fxqueue.SetData(new FXQueue.Entry[]
						{
							new FXQueue.Entry(buffFX, 0f, 0f)
						}, 0);
						if (abilityEventArgs.Ability.StaticData.TargetType == ETargetType.PARTY)
						{
							FXArgs args = new FXArgs(gameObject, FXHelper.GetPlayerEntity().gameObject, gameObject, FXHelper.GetPlayerEntity().gameObject, transform.position, transform.forward, -transform.right, FXHelper.GetPlayerEntity().transform.position, new List<GameObject>
							{
								FXHelper.GetPlayerEntity().gameObject
							});
							fxqueue.Execute(args);
						}
						else
						{
							FXArgs args2 = new FXArgs(gameObject, gameObject, gameObject, gameObject, transform.position, transform.forward, -transform.right, transform.position, new List<GameObject>
							{
								gameObject
							});
							fxqueue.Execute(args2);
						}
						m_Queues.Add(fxqueue);
					}
					else
					{
						Debug.LogError("OnAbilityEvent: Ability's  given GFX does not exist! " + gfx);
					}
				}
				else
				{
					Debug.LogWarning("OnAbilityEvent: Ability GFX is missing!");
				}
				if (abilityEventArgs.Ability.StaticData.NameKey == "MONSTER_ABILITY_EXPLOSIVE")
				{
					DelayedEventManagerWorker delayedEventManagerWorker = new GameObject("ExplosiveHelper").AddComponent<DelayedEventManagerWorker>();
					DontDestroyOnLoad(delayedEventManagerWorker);
					delayedEventManagerWorker.StartCoroutine(PlayLateShakeFX(delayedEventManagerWorker.gameObject));
					Destroy(gameObject, m_explosiveAbilityDelay);
				}
			}
		}

		private void OnEntityAbilityRemoved()
		{
		}

		private IEnumerator PlayLateShakeFX(GameObject worker)
		{
			yield return new WaitForSeconds(0.3f);
			Vector3 vec = new Vector3(0f, 0.1f, 0f);
			FXMainCamera.Instance.PlayShakeFX(0.1f, vec);
			if (m_explosiveAbilityDelay > 0.5f)
			{
				yield return new WaitForSeconds(m_explosiveAbilityDelay - 0.3f);
				FXMainCamera.Instance.PlayShakeFX(0.1f, vec);
			}
			Destroy(worker);
			yield break;
		}
	}
}
