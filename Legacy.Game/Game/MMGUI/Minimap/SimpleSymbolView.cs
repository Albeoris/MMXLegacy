using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/Simple Symbol View")]
	public class SimpleSymbolView : SymbolView
	{
		[SerializeField]
		protected String m_TooltipLocaKey;

		[SerializeField]
		protected Boolean m_asymmetricSymbol;

		private void OnDisable()
		{
			OnTooltip(false);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnSetEntityPosition));
			}
			if (MyController != null)
			{
				OnSetEntityPosition(MyController, null);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnSetEntityPosition));
			}
		}

		protected virtual void OnSetEntityPosition(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				transform.localPosition = MyControllerPosition;
			}
		}

		protected virtual void OnTooltip(Boolean show)
		{
			if (show && !ConfigManager.Instance.Options.HideMinimapTooltips)
			{
				String localizedTooltipText = GetLocalizedTooltipText();
				if (localizedTooltipText != null)
				{
					TooltipManager.Instance.Show(IngameController.Instance.MapController, localizedTooltipText, GUIMainCamera.Instance.camera.ScreenToWorldPoint(Input.mousePosition), Vector3.one);
				}
			}
			else
			{
				TooltipManager.Instance.Hide(IngameController.Instance.MapController);
			}
		}

		protected virtual void OnClick()
		{
			UserMapNoteController.ProcessMouseClick();
		}

		public virtual String GetLocalizedTooltipText()
		{
			if (!String.IsNullOrEmpty(m_TooltipLocaKey))
			{
				Position myControllerGridPosition = MyControllerGridPosition;
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(myControllerGridPosition);
				if (slot != null && slot.VisitedByParty)
				{
					return LocaManager.GetText(m_TooltipLocaKey);
				}
			}
			return null;
		}

		public void HideTooltip()
		{
			OnTooltip(false);
		}

		protected virtual void Update()
		{
			if (m_asymmetricSymbol)
			{
				transform.localRotation = MyControllerRotation * s_InverseRotation;
			}
		}
	}
}
