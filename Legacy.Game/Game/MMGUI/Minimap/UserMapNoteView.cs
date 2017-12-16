using System;
using System.Threading;
using Legacy.Core.Configuration;
using Legacy.Core.Map.Notes;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMGUI;
using UnityEngine;

namespace Legacy.Game.MMGUI.Minimap
{
	internal class UserMapNoteView : MonoBehaviour
	{
		public event EventHandler MouseClick;

		public MapNote MapNote { get; set; }

		private void OnClick()
		{
			if (MouseClick != null)
			{
				MouseClick(this, null);
			}
		}

		private void OnTooltip(Boolean show)
		{
			if (show && MapNote != null)
			{
				TooltipManager.Instance.Show(IngameController.Instance.MapController, (!ConfigManager.Instance.Options.HideMinimapTooltips) ? MinimapView.GetLocalizedSymbolTooltipText(MapNote.Position) : null, MapNote.Note, GUIMainCamera.Instance.camera.ScreenToWorldPoint(Input.mousePosition), new Vector3(5f, 0f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(IngameController.Instance.MapController);
			}
		}
	}
}
