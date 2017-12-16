using System;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	public class SimpleTooltipView : MonoBehaviour
	{
		public String LocaKey;

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnTooltip(Boolean show)
		{
			if (show)
			{
				if (!String.IsNullOrEmpty(LocaKey) && enabled)
				{
					TooltipManager.Instance.Show(this, LocaManager.GetText(LocaKey), transform.position, new Vector3(24f, 24f, 0f));
				}
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
