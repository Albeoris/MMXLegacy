using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/InteractiveObjectHighlight")]
	public class InteractiveObjectHighlight : BaseView
	{
		private const Single BLUR_FACTOR = 2f;

		private const Single HOVER_ALPHA_FACTOR = 0.5f;

		[SerializeField]
		private Boolean m_IsTwoSided;

		[SerializeField]
		private Boolean m_ignoreSpawnerPositioningOffsets;

		private List<InteractiveObject> m_refererInteractObjs = new List<InteractiveObject>();

		[SerializeField]
		private OutlineFXBase m_OutlineFX;

		private GridSlot m_GridSlot;

		private GridSlot m_SecondGridSlot;

		private Boolean m_IsSecondSecondGridSlotInitialized;

		private Boolean m_IsMouseOver;

		private Boolean m_IsHighlighted;

		private Single m_currentlyUsedOutlineSetting = 1f;

		public Boolean IsTwoSided
		{
			get => m_IsTwoSided;
		    set => m_IsTwoSided = value;
		}

		public Boolean IsClickable => m_IsMouseOver && (m_IsHighlighted || LegacyLogic.Instance.WorldManager.Party.SelectedInteractiveObject == MyController);

	    public new InteractiveObject MyController => (InteractiveObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject p_OldController)
		{
			base.OnChangeMyController(p_OldController);
			if (MyController != null && !(MyController is InteractiveObject))
			{
				Debug.LogError("InteractiveObjectHighlight: OnChangeMyController: works only for InteractiveObjects! Was given '" + MyController.GetType().FullName + "'");
				return;
			}
			m_currentlyUsedOutlineSetting = ConfigManager.Instance.Options.ObjectOutlineOpacity;
			if (MyController != null && !MyController.TrapActive && GetComponentsInChildren<Renderer>().Length == 0 && !(MyController is Barrel))
			{
				Debug.LogError("InteractiveObjectHighlight: works only with renderers, but there are no renderers attached to this game object or its children! " + name);
			}
			if (p_OldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_SELECTED, new EventHandler(OnInteractiveObjectSelected));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_ALL_SELECTIONS_REMOVED, new EventHandler(OnInteractiveObjectAllSelectionsRemoved));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnPartyMoveOrRotate));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.ROTATE_ENTITY, new EventHandler(OnPartyMoveOrRotate));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.SET_ENTITY_POSITION, new EventHandler(OnPartyMoveOrRotate));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.TRAP_DISARMED, new EventHandler(OnTrapDisarmed));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.TOKEN_ADDED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.PARTY_BUFF_ADDED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.DOOR_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.LEVER_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_SCREEN_CLOSED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				m_refererInteractObjs.Clear();
				m_GridSlot = null;
				m_SecondGridSlot = null;
				m_IsSecondSecondGridSlotInitialized = false;
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_SELECTED, new EventHandler(OnInteractiveObjectSelected));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_ALL_SELECTIONS_REMOVED, new EventHandler(OnInteractiveObjectAllSelectionsRemoved));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnPartyMoveOrRotate));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.ROTATE_ENTITY, new EventHandler(OnPartyMoveOrRotate));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.SET_ENTITY_POSITION, new EventHandler(OnPartyMoveOrRotate));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.TRAP_DISARMED, new EventHandler(OnTrapDisarmed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.TOKEN_ADDED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.PARTY_BUFF_ADDED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.PARTY_BUFF_REMOVED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.DOOR_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.LEVER_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_SCREEN_CLOSED, new EventHandler(OnHighlightRelatedEvent));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnHighlightRelatedEvent));
				InteractiveObject myController = MyController;
				m_GridSlot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(myController.Position);
				m_IsSecondSecondGridSlotInitialized = false;
				if (m_ignoreSpawnerPositioningOffsets)
				{
					RevertSpawnerOffsets();
					m_ignoreSpawnerPositioningOffsets = false;
				}
				enabled = true;
				OnPartyMoveOrRotate(LegacyLogic.Instance.WorldManager.Party, EventArgs.Empty);
			}
		}

		private void OnInteractiveObjectSelected(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (MyController == baseObjectEventArgs.Object || m_refererInteractObjs.Contains((InteractiveObject)baseObjectEventArgs.Object))
			{
				if (baseObjectEventArgs.Object is NpcContainer)
				{
					NpcContainer npcContainer = (NpcContainer)baseObjectEventArgs.Object;
					if (!npcContainer.IsEnabled)
					{
						HideOutline();
						return;
					}
				}
				if (baseObjectEventArgs.Object is Door)
				{
					Door door = (Door)baseObjectEventArgs.Object;
					if (door.State == EInteractiveObjectState.DOOR_OPEN)
					{
						HideOutline();
						return;
					}
				}
				ShowOutline(false);
			}
			else
			{
				HideOutline();
			}
		}

		private void OnInteractiveObjectAllSelectionsRemoved(Object p_sender, EventArgs p_args)
		{
			HideOutline();
		}

		private void OnPartyMoveOrRotate(Object p_Sender, EventArgs pArgs)
		{
			if (LegacyLogic.Instance.WorldManager.Party == p_Sender)
			{
				UpdateIsClickableViewState();
			}
		}

		private void OnTrapDisarmed(Object sender, EventArgs e)
		{
			if (sender == MyController)
			{
				UpdateIsClickableViewState();
			}
		}

		private void OnHighlightRelatedEvent(Object sender, EventArgs p_args)
		{
			UpdateIsClickableViewState();
		}

		private void UpdateIsClickableViewState()
		{
			InteractiveObject myController = MyController;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Boolean p_hasSpotSecrets = party.HasSpotSecrets();
			Boolean p_hasClairvoyance = party.HasClairvoyance();
			UpdateIsClickableViewState(m_GridSlot, party, myController, p_hasSpotSecrets, p_hasClairvoyance);
			if (m_IsTwoSided)
			{
				if (!m_IsSecondSecondGridSlotInitialized)
				{
					Position p_pos = myController.Position + myController.Location;
					m_SecondGridSlot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_pos);
					if (m_SecondGridSlot != null)
					{
						List<InteractiveObject> list = new List<InteractiveObject>();
						for (Int32 i = -1; i < 4; i++)
						{
							list.AddRange(m_SecondGridSlot.GetPassiveInteractiveObjects((EDirection)i, true, true, true));
						}
						foreach (InteractiveObject interactiveObject in list)
						{
							foreach (SpawnCommand spawnCommand in interactiveObject.Commands)
							{
								if (spawnCommand.TargetSpawnID == myController.SpawnerID)
								{
									m_refererInteractObjs.Add(interactiveObject);
									break;
								}
							}
						}
					}
					m_IsSecondSecondGridSlotInitialized = true;
				}
				if (m_SecondGridSlot != null)
				{
					foreach (InteractiveObject p_interactiveObj in m_refererInteractObjs)
					{
						UpdateIsClickableViewState(m_SecondGridSlot, party, p_interactiveObj, p_hasSpotSecrets, p_hasClairvoyance);
					}
				}
			}
		}

		private void UpdateIsClickableViewState(GridSlot p_slot, Party p_party, InteractiveObject p_interactiveObj, Boolean p_hasSpotSecrets, Boolean p_hasClairvoyance)
		{
			Boolean isClickableViewState = p_interactiveObj.IsClickableViewState;
			p_interactiveObj.IsClickableViewState = (p_interactiveObj.Position == p_party.Position && (p_interactiveObj.IsInteractable(p_party.Direction, p_hasSpotSecrets, p_hasClairvoyance, true) || p_interactiveObj.IsInteractable(EDirection.CENTER, p_hasSpotSecrets, p_hasClairvoyance, true)));
			if (isClickableViewState != p_interactiveObj.IsClickableViewState && p_interactiveObj == p_party.SelectedInteractiveObject)
			{
				p_party.AutoSelectInteractiveObject();
			}
		}

		protected virtual void ShowOutline(Boolean p_MouseOverStateChanged)
		{
			m_IsHighlighted = true;
			InteractiveObject myController = MyController;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Boolean flag = IsHighlightIgnoringSettings(party, myController);
			Color color = Color.white;
			if (myController.TrapActive && party.HasClairvoyance())
			{
				color = Color.red;
			}
			if (myController.IsSecret && party.HasSpotSecrets())
			{
				color = Color.yellow;
			}
			if (!m_IsMouseOver)
			{
				color.a -= 0.5f;
			}
			if (!flag)
			{
				color.a *= Mathf.Sqrt(m_currentlyUsedOutlineSetting);
			}
			if (color.a > 0.001f || flag)
			{
				if (m_OutlineFX == null || m_OutlineFX.IsDestroyed)
				{
					Transform transform = this.transform.Find("HighlightFXGeometry");
					if (transform != null)
					{
						m_OutlineFX = transform.gameObject.AddComponent<OutlineGlowFX>();
					}
					else
					{
						m_OutlineFX = gameObject.AddComponent<OutlineGlowFX>();
					}
					if (flag)
					{
						m_OutlineFX.SetGlobalIntensity(2f);
					}
					else
					{
						m_OutlineFX.SetGlobalIntensity(2f * (m_currentlyUsedOutlineSetting * m_currentlyUsedOutlineSetting));
					}
				}
				if (!m_OutlineFX.IsOutlineShown || color != m_OutlineFX.OutlineColor)
				{
					m_OutlineFX.ShowOutline(!p_MouseOverStateChanged, color);
				}
			}
			else if (m_OutlineFX != null)
			{
				m_OutlineFX.HideOutline();
			}
		}

		private void HideOutline()
		{
			m_IsHighlighted = false;
			if (m_OutlineFX != null)
			{
				m_OutlineFX.HideOutline();
			}
		}

		private void OnMouseEnter()
		{
			InteractiveObject myController = MyController;
			if (myController == null || LegacyLogic.Instance.ConversationManager.IsOpen)
			{
				return;
			}
			if (myController.IsClickableViewState)
			{
				LegacyLogic.Instance.WorldManager.Party.SelectedInteractiveObject = myController;
			}
			m_IsMouseOver = true;
			if (m_OutlineFX != null && m_OutlineFX.IsOutlineShown)
			{
				ShowOutline(true);
			}
		}

		private void OnMouseExit()
		{
			m_IsMouseOver = false;
			if (m_OutlineFX != null && m_OutlineFX.IsOutlineShown)
			{
				ShowOutline(true);
			}
		}

		private void Update()
		{
			if (MyController != null && m_currentlyUsedOutlineSetting != ConfigManager.Instance.Options.ObjectOutlineOpacity)
			{
				m_currentlyUsedOutlineSetting = ConfigManager.Instance.Options.ObjectOutlineOpacity;
				if (m_OutlineFX != null && m_OutlineFX.IsOutlineShown && !IsHighlightIgnoringSettings(LegacyLogic.Instance.WorldManager.Party, MyController))
				{
					m_OutlineFX.SetGlobalIntensity(2f * (m_currentlyUsedOutlineSetting * m_currentlyUsedOutlineSetting));
					ShowOutline(true);
				}
				else if (LegacyLogic.Instance.WorldManager.Party.SelectedInteractiveObject == MyController)
				{
					ShowOutline(false);
				}
			}
		}

		private void RevertSpawnerOffsets()
		{
			transform.localRotation = Quaternion.Inverse(Quaternion.Euler(new Vector3(MyController.ObjectRotation.X, MyController.ObjectRotation.Y, MyController.ObjectRotation.Z))) * transform.localRotation;
			transform.position = transform.position - new Vector3(MyController.OffsetPosition.X, MyController.OffsetPosition.Y, MyController.OffsetPosition.Z);
		}

		private Boolean IsHighlightIgnoringSettings(Party p_party, InteractiveObject p_myObject)
		{
			return (p_myObject.TrapActive && p_party.HasClairvoyance()) || (p_myObject.IsSecret && p_party.HasSpotSecrets()) || (m_OutlineFX != null && m_OutlineFX is OutlineParticleSystemFX);
		}
	}
}
