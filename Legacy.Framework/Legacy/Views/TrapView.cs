using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/TrapView")]
	public class TrapView : DiscoverHighlightViewBase
	{
		protected override ETargetCondition NeededEnhancedHirelingCondition => ETargetCondition.HIRE_ENHCLAIRVOYANCE;

	    protected override Single FadeOutAfterTime => (!LegacyLogic.Instance.WorldManager.Party.HasEnchClairvoyance()) ? 2f : -1f;

	    protected override void Start()
		{
			base.Start();
			InstantiateCollider();
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && !((InteractiveObject)MyController).TrapActive)
			{
				Destroy(this);
				return;
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.HIGHLIGHT_TRAP, new EventHandler(OnDiscoverHighlight));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TRAP_DISARMED, new EventHandler(OnTrapDisarmed));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.HIGHLIGHT_TRAP, new EventHandler(OnDiscoverHighlight));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TRAP_DISARMED, new EventHandler(OnTrapDisarmed));
			}
		}

		protected override void CheckVisibility(Object sender, EventArgs p_args)
		{
			if (MyController is Trap)
			{
				Boolean active = LegacyLogic.Instance.WorldManager.Party.HasClairvoyance();
				gameObject.SetActive(active);
			}
		}

		protected override void InstantiateHighlight()
		{
			Vector3 b = Vector3.zero;
			EDirection location = ((InteractiveObject)MyController).Location;
			String p_path;
			if (MyController is Trap)
			{
				if (location == EDirection.CENTER)
				{
					p_path = "FX/Traps/TrapFXCenter";
				}
				else
				{
					p_path = "FX/Traps/TrapFXTransition";
					b = 5f * (Helper.GridDirectionToQuaternion(location) * Vector3.forward);
					m_IsTwoSided = true;
					InteractiveObjectHighlight component = GetComponent<InteractiveObjectHighlight>();
					if (component != null)
					{
						component.IsTwoSided = true;
					}
					else
					{
						Debug.LogError("TrapView: InstantiateHighlight: could not find InteractiveObjectHighlight script! " + name);
					}
				}
			}
			else
			{
				p_path = "FX/Traps/TrapFXGenericObject";
			}
			m_HighlightFX = Helper.Instantiate<GameObject>(Helper.ResourcesLoad<GameObject>(p_path)).GetComponent<TrapHighlightFX>();
			transform.AddChildAlignOrigin(m_HighlightFX.transform);
			m_HighlightFX.transform.position = transform.position + b;
		}

		private void OnTrapDisarmed(Object sender, EventArgs e)
		{
			if (sender == MyController)
			{
				DestroyHighlight();
			}
		}

		private void InstantiateCollider()
		{
			if (MyController != null && MyController is InteractiveObject)
			{
				EDirection location = ((InteractiveObject)MyController).Location;
				if (MyController is Trap && location != EDirection.CENTER)
				{
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.center = Vector3.forward * 5f + Vector3.up * 1f;
					boxCollider.size = new Vector3(10f, 2f, 1f);
				}
			}
		}
	}
}
