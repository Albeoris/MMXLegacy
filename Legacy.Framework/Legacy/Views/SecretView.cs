using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/SecretView")]
	public class SecretView : DiscoverHighlightViewBase
	{
		private Boolean m_IsCoroutineRunning;

		[SerializeField]
		private Boolean m_hideIfNoSpotSecret;

		public Boolean HideIfNoSpotSecret
		{
			get => m_hideIfNoSpotSecret;
		    set => m_hideIfNoSpotSecret = value;
		}

		protected override ETargetCondition NeededEnhancedHirelingCondition => ETargetCondition.HIRE_ENHSPOTSECRETS;

	    protected override Single FadeOutAfterTime => (!LegacyLogic.Instance.WorldManager.Party.HasEnchSpotSecrets()) ? 3f : -1f;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && !((InteractiveObject)MyController).IsSecret)
			{
				Destroy(this);
				return;
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.HIGHLIGHT_SECRET, new EventHandler(OnDiscoverHighlight));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.DOOR_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.LEVER_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.HIGHLIGHT_SECRET, new EventHandler(OnDiscoverHighlight));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.DOOR_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.LEVER_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnHighlightRelatedStateChangeEvent));
			}
		}

		protected override void CheckVisibility(Object sender, EventArgs p_args)
		{
			if (m_hideIfNoSpotSecret && MyController is InteractiveObject && ((InteractiveObject)MyController).IsSecret)
			{
				Boolean active = LegacyLogic.Instance.WorldManager.Party.HasSpotSecrets();
				gameObject.SetActive(active);
			}
		}

		private void OnHighlightRelatedStateChangeEvent(Object sender, EventArgs e)
		{
			BaseObject baseObject = null;
			if (e is BaseObjectEventArgs)
			{
				baseObject = ((BaseObjectEventArgs)e).Object;
			}
			if (baseObject == MyController)
			{
				Boolean flag = m_IsHighlighted || m_IsCoroutineRunning;
				DestroyHighlight();
				StopAllCoroutines();
				m_IsCoroutineRunning = false;
				if (((InteractiveObject)MyController).IsExecutable(LegacyLogic.Instance.MapLoader.Grid) && flag)
				{
					Single num = -1f;
					Animation componentInChildren = GetComponentInChildren<Animation>();
					if (componentInChildren != null)
					{
						IEnumerator enumerator = componentInChildren.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								Object obj = enumerator.Current;
								AnimationState animationState = (AnimationState)obj;
								if (animationState.enabled)
								{
									num = Mathf.Max(num, animationState.length);
								}
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
					}
					if (num == -1f)
					{
						num = 1f;
					}
					StartCoroutine(DelayedHighlight(num));
				}
			}
		}

		protected override void InstantiateHighlight()
		{
			m_HighlightFX = Helper.Instantiate<GameObject>(Helper.ResourcesLoad<GameObject>("FX/Traps/SecretFXGenericObject")).GetComponent<TrapHighlightFX>();
			transform.AddChildAlignOrigin(m_HighlightFX.transform);
		}

		private IEnumerator DelayedHighlight(Single p_Delay)
		{
			m_IsCoroutineRunning = true;
			yield return new WaitForSeconds(p_Delay);
			Highlight();
			m_IsCoroutineRunning = false;
			yield break;
		}
	}
}
