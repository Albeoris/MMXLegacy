using System;
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
	public abstract class DiscoverHighlightViewBase : BaseView
	{
		protected TrapHighlightFX m_HighlightFX;

		protected Int32 m_LatestHighlightFXFrame = -1;

		protected Boolean m_IsHighlighted;

		protected Boolean m_IsTwoSided;

		protected abstract ETargetCondition NeededEnhancedHirelingCondition { get; }

		protected abstract Single FadeOutAfterTime { get; }

		protected abstract void CheckVisibility(Object sender, EventArgs p_args);

		protected abstract void InstantiateHighlight();

		protected virtual void Start()
		{
			CheckVisibility(null, EventArgs.Empty);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnPartyMove));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnNPCHirelingUpdated));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(CheckVisibility));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(CheckVisibility));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(CheckVisibility));
			}
			if (MyController != null && !(MyController is InteractiveObject))
			{
				Debug.LogError("DiscoverHighlightViewBase: OnChangeMyController: works only for InteractiveObjects! Was given '" + MyController.GetType().FullName + "'");
				return;
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnPartyMove));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnNPCHirelingUpdated));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(CheckVisibility));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_ADDED, new EventHandler(CheckVisibility));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_BUFF_REMOVED, new EventHandler(CheckVisibility));
			}
		}

		protected void Highlight()
		{
			m_LatestHighlightFXFrame = Time.frameCount;
			if (!m_IsHighlighted)
			{
				m_IsHighlighted = true;
				if (m_HighlightFX == null)
				{
					InstantiateHighlight();
				}
				Single fadeOutAfterTime = FadeOutAfterTime;
				m_HighlightFX.ResetToInvisible();
				if (fadeOutAfterTime == -1f)
				{
					m_HighlightFX.FadeIn();
				}
				else
				{
					m_HighlightFX.FadeIn(fadeOutAfterTime);
				}
			}
		}

		protected void FadeOutHighlight()
		{
			m_IsHighlighted = false;
			if (m_HighlightFX != null)
			{
				m_HighlightFX.FadeOut();
			}
		}

		protected void DestroyHighlight()
		{
			m_IsHighlighted = false;
			if (m_HighlightFX != null)
			{
				Destroy(m_HighlightFX.gameObject);
				m_HighlightFX = null;
			}
		}

		protected void OnDiscoverHighlight(Object sender, EventArgs e)
		{
			if (sender == MyController)
			{
				Highlight();
			}
			else if (m_IsTwoSided)
			{
				InteractiveObject interactiveObject = (InteractiveObject)MyController;
				BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)e;
				if (baseObjectEventArgs.Position == interactiveObject.Position + interactiveObject.Location)
				{
					Highlight();
				}
			}
		}

		private void OnNPCHirelingUpdated(Object sender, EventArgs p_args)
		{
			if (p_args is HirelingEventArgs)
			{
				HirelingEventArgs hirelingEventArgs = (HirelingEventArgs)p_args;
				if (hirelingEventArgs.Npc != null)
				{
					foreach (NpcEffect npcEffect in hirelingEventArgs.Npc.StaticData.NpcEffects)
					{
						if (npcEffect.TargetEffect == NeededEnhancedHirelingCondition)
						{
							CheckVisibility(null, EventArgs.Empty);
							if (hirelingEventArgs.Condition == ETargetCondition.FIRE)
							{
								FadeOutHighlight();
								return;
							}
							if (hirelingEventArgs.Condition == ETargetCondition.HIRE && m_IsHighlighted)
							{
								m_IsHighlighted = false;
								Highlight();
								return;
							}
						}
					}
				}
			}
		}

		private void OnPartyMove(Object sender, EventArgs e)
		{
			if (sender == LegacyLogic.Instance.WorldManager.Party && m_LatestHighlightFXFrame < Time.frameCount)
			{
				FadeOutHighlight();
			}
		}
	}
}
