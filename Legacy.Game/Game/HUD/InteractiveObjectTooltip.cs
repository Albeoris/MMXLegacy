using System;
using System.Collections.Generic;
using System.IO;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic.Interactions;
using Legacy.Core.UpdateLogic.Preconditions;
using Legacy.EffectEngine;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/InteractiveObjectTooltip")]
	public class InteractiveObjectTooltip : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private Single m_alphaTweenDuration = 0.5f;

		[SerializeField]
		private Color m_actionColor = new Color(0f, 0.75f, 1f);

		[SerializeField]
		private GameMessageController m_gameMessageController;

		private String m_actionColorHex;

		private Boolean m_active;

		private void Awake()
		{
			m_actionColorHex = "[" + NGUITools.EncodeColor(m_actionColor) + "]";
			m_label.alpha = 0f;
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_SELECTED, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.DOOR_STATE_CHANGED, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.BARREL_STATE_CHANGE, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_DONE_LOOTING, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_ALL_SELECTIONS_REMOVED, new EventHandler(OnInteractiveObjectAllSelectionsRemoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnInteractiveObjectAllSelectionsRemoved));
			m_gameMessageController.OccupacionChangeEvent += OnGameMessageOccupationChanged;
		}

		private void OnDestroy()
		{
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_SELECTED, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.DOOR_STATE_CHANGED, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.BARREL_STATE_CHANGE, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CONTAINER_DONE_LOOTING, new EventHandler(OnInteractiveObjectSelected));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.INTERACTIVE_OBJECT_ALL_SELECTIONS_REMOVED, new EventHandler(OnInteractiveObjectAllSelectionsRemoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnInteractiveObjectAllSelectionsRemoved));
			m_gameMessageController.OccupacionChangeEvent -= OnGameMessageOccupationChanged;
		}

		private void OnInteractiveObjectSelected(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			InteractiveObject interactiveObject = baseObjectEventArgs.Object as InteractiveObject;
			if (interactiveObject != null)
			{
				if (interactiveObject != LegacyLogic.Instance.WorldManager.Party.SelectedInteractiveObject)
				{
					return;
				}
				UpdateName(interactiveObject);
				Show(interactiveObject);
			}
			else
			{
				Hide();
			}
		}

		private void OnInteractiveObjectAllSelectionsRemoved(Object p_sender, EventArgs p_args)
		{
			Hide();
		}

		private void Show(InteractiveObject p_interactiveObject)
		{
			m_active = true;
			if (!m_gameMessageController.Occupied || !m_gameMessageController.HasSubMessage())
			{
				TweenAlpha.Begin(m_label.gameObject, m_alphaTweenDuration, 1f);
			}
		}

		private void Hide()
		{
			m_active = false;
			TweenAlpha.Begin(m_label.gameObject, m_alphaTweenDuration, 0f);
		}

		private void OnGameMessageOccupationChanged(Object p_sender, EventArgs p_args)
		{
			if (m_gameMessageController.Occupied)
			{
				if (m_gameMessageController.HasSubMessage())
				{
					TweenAlpha.Begin(m_label.gameObject, m_alphaTweenDuration, 0f);
				}
			}
			else if (m_active)
			{
				TweenAlpha.Begin(m_label.gameObject, m_alphaTweenDuration, 1f);
			}
		}

		private void UpdateName(InteractiveObject p_interactiveObject)
		{
			Boolean flag = false;
			IEnumerable<InteractiveObjectTooltipStaticData> iterator = StaticDataHandler.GetIterator<InteractiveObjectTooltipStaticData>(EDataType.INTERACTIVE_OBJECT_TOOLTIPS);
			KeyCode keyCode = KeyConfigManager.KeyBindings[EHotkeyType.INTERACT].Key1;
			if (keyCode == KeyCode.None)
			{
				keyCode = KeyConfigManager.KeyBindings[EHotkeyType.INTERACT].AltKey1;
			}
			String text = m_actionColorHex + "[" + LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + keyCode.ToString().ToUpper()) + "][-]";
			if (!String.IsNullOrEmpty(p_interactiveObject.Prefab))
			{
				String directoryName = Path.GetDirectoryName(p_interactiveObject.Prefab);
				foreach (InteractiveObjectTooltipStaticData interactiveObjectTooltipStaticData in iterator)
				{
					if (interactiveObjectTooltipStaticData.PrefabFolder == directoryName || interactiveObjectTooltipStaticData.PrefabFolder == p_interactiveObject.Prefab)
					{
						m_label.text = LocaManager.GetText(interactiveObjectTooltipStaticData.LocaKey, text);
						flag = true;
						break;
					}
				}
			}
			if (p_interactiveObject is Button)
			{
				if (!flag)
				{
					m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_BUTTON", text);
				}
				return;
			}
			if (p_interactiveObject is Lever)
			{
				if (!flag)
				{
					m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_LEVER", text);
				}
				return;
			}
			if (!(p_interactiveObject is Barrel))
			{
				if (p_interactiveObject is Container)
				{
					Container container = p_interactiveObject as Container;
					if (!container.IsEmpty())
					{
						if (!flag)
						{
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_CONTAINER", text);
						}
					}
					else
					{
						foreach (SpawnCommand spawnCommand in container.Commands)
						{
							if (spawnCommand.Type == EInteraction.CHANGE_ATTRIBUTE)
							{
								m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_CONTAINER", text);
								return;
							}
						}
						m_label.text = String.Empty;
					}
					return;
				}
				if (p_interactiveObject is Door)
				{
					if (LegacyLogic.Instance.WorldManager.Party.SelectedInteractiveObject == null || p_interactiveObject.IsSecret)
					{
						m_label.text = String.Empty;
						return;
					}
					if (((Door)p_interactiveObject).State == EInteractiveObjectState.DOOR_OPEN)
					{
						m_label.text = String.Empty;
					}
					else
					{
						m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_DOOR", text);
					}
					return;
				}
				else
				{
					if (p_interactiveObject is Sign)
					{
						if (!flag)
						{
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_SIGN", text);
						}
						return;
					}
					if (p_interactiveObject is Entrance)
					{
						if (flag)
						{
							return;
						}
						Entrance entrance = (Entrance)p_interactiveObject;
						String text2 = entrance.MinimapTooltipLocaKey;
						if (!String.IsNullOrEmpty(text2))
						{
							text2 = LocaManager.GetText(entrance.MinimapTooltipLocaKey);
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_ENTRANCE", text, text2);
							return;
						}
						text2 = null;
						foreach (SpawnCommand spawnCommand2 in entrance.Commands)
						{
							if (spawnCommand2.Type == EInteraction.USE_ENTRANCE)
							{
								String[] array = spawnCommand2.Extra.Split(new Char[]
								{
									','
								});
								text2 = array[0].Replace(".xml", String.Empty);
								break;
							}
						}
						if (text2 != null)
						{
							GridInfo gridInfo = LegacyLogic.Instance.MapLoader.FindGridInfo(text2);
							if (gridInfo != null)
							{
								text2 = LocaManager.GetText(gridInfo.LocationLocaName);
								if (LegacyLogic.Instance.MapLoader.Grid.Type == EMapType.DUNGEON)
								{
									text2 = text2.Replace("@", ", ");
								}
								else if (text2.LastIndexOf('@') != -1)
								{
									text2 = text2.Remove(text2.LastIndexOf('@'));
								}
								m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_ENTRANCE", text, text2);
								return;
							}
							Debug.LogError("Grid Info not found " + text2);
						}
					}
					else if (p_interactiveObject is NpcContainer)
					{
						NpcContainer npcContainer = (NpcContainer)p_interactiveObject;
						if (!npcContainer.IsEnabled)
						{
							m_label.text = String.Empty;
							return;
						}
						String minimapTooltipLocaKey = npcContainer.MinimapTooltipLocaKey;
						if (!String.IsNullOrEmpty(npcContainer.IndoorScene))
						{
							if (!String.IsNullOrEmpty(minimapTooltipLocaKey))
							{
								String text3 = LocaManager.GetText(minimapTooltipLocaKey);
								m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_BUILDING_DEFAULT", text, text3);
								return;
							}
							foreach (InteractiveObjectTooltipStaticData interactiveObjectTooltipStaticData2 in iterator)
							{
								if (interactiveObjectTooltipStaticData2.PrefabFolder == npcContainer.IndoorScene)
								{
									m_label.text = LocaManager.GetText(interactiveObjectTooltipStaticData2.LocaKey, text);
									return;
								}
							}
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_INDOOR_CONTEXT", text);
							return;
						}
						else
						{
							if (!npcContainer.Npcs[0].IsEnabled)
							{
								m_label.text = String.Empty;
								return;
							}
							foreach (SpawnCommand spawnCommand3 in npcContainer.Commands)
							{
								if (spawnCommand3.Type == EInteraction.START_DEFINED_DIALOG && !String.IsNullOrEmpty(spawnCommand3.Precondition) && spawnCommand3.Precondition.StartsWith("PARTY_CHECK"))
								{
									BasePrecondition basePrecondition = BaseInteraction.ParsePrecondition(spawnCommand3.Precondition);
									if (basePrecondition is PartyCheckPrecondition && !((PartyCheckPrecondition)basePrecondition).Evaluate())
									{
										m_label.text = String.Empty;
										return;
									}
								}
							}
							if (flag)
							{
								return;
							}
							if (!String.IsNullOrEmpty(minimapTooltipLocaKey))
							{
								m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_NPC", text, minimapTooltipLocaKey);
								return;
							}
							if (npcContainer.Npcs[0].StaticID != 224 && npcContainer.Npcs[0].StaticID != 225)
							{
								String text4 = LocaManager.GetText(npcContainer.Npcs[0].StaticData.NameKey);
								m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_NPC", text, text4);
								return;
							}
						}
					}
					else if (p_interactiveObject is RechargingObject)
					{
						RechargingObject rechargingObject = (RechargingObject)p_interactiveObject;
						if (rechargingObject.IsFountain())
						{
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_FOUNTAIN", text);
						}
						else if (rechargingObject.IsCrystal())
						{
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_CRYSTAL", text);
						}
						else
						{
							m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_STATUE", text);
						}
						return;
					}
					m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_FALLBACK", text);
				}
				return;
			}
			Barrel barrel = p_interactiveObject as Barrel;
			if (barrel.State == EInteractiveObjectState.BARREL_EMPTY)
			{
				m_label.text = String.Empty;
				return;
			}
			if (barrel.State == EInteractiveObjectState.BARREL_OPEN)
			{
				m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_FALLBACK", text);
				return;
			}
			m_label.text = LocaManager.GetText("INTERACT_TOOLTIP_BARREL", text);
		}
	}
}
