using System;
using System.Threading;
using Legacy.Core.Map.Notes;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI.WorldMap
{
	internal class WorldMapUserMapNoteView : MonoBehaviour
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
				TooltipManager.Instance.Show(IngameController.Instance.MapController, null, MapNote.Note, transform.position, new Vector3(25f, 0f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(IngameController.Instance.MapController);
			}
		}
	}
}
