using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/TrapEffectContainerView")]
	public class TrapEffectContainerView : BaseView
	{
		protected override void Awake()
		{
			base.Awake();
			gameObject.SetActive(false);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			}
		}

		private void OnTrapTriggered(Object p_sender, EventArgs p_args)
		{
			TrapEventArgs trapEventArgs = p_args as TrapEventArgs;
			if (trapEventArgs != null && trapEventArgs.Trap == MyController)
			{
				EventHandler eventHandler = delegate(Object p_localSender, EventArgs p_localArgs)
				{
					DelayedEventManager.InvokeEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, p_sender, p_args);
				};
				String gfx = trapEventArgs.TrapEffect.GFX;
				if (!String.IsNullOrEmpty(gfx))
				{
					FXQueue fxqueue = Helper.ResourcesLoad<FXQueue>(gfx, false);
					if (fxqueue != null)
					{
						fxqueue = Helper.Instantiate<FXQueue>(fxqueue);
						FXArgs args = new FXArgs(gameObject, gameObject, gameObject, gameObject, Vector3.zero, transform.forward, -transform.right, Vector3.zero);
						fxqueue.Finished += eventHandler;
						fxqueue.Execute(args);
					}
					else
					{
						eventHandler(this, EventArgs.Empty);
						Debug.LogError("OnTrapTriggered: given GFX does not exist! " + gfx);
					}
				}
				else
				{
					eventHandler(this, EventArgs.Empty);
					Debug.LogWarning("OnTrapTriggered: Trap GFX is missing!");
				}
			}
		}
	}
}
