using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	public class GUIDialogController
	{
		public void OnShowDialog(GUIDialogParameters p_dialogParams, UIAnchor p_anchor)
		{
			GUIDialogParameters.EDialogType type = p_dialogParams.Type;
			GameObject prefab;
			if (type != GUIDialogParameters.EDialogType.DIALOG_OK)
			{
				if (type != GUIDialogParameters.EDialogType.DIALOG_YES_NO)
				{
					Debug.LogError("OnShowDialog: missing instructions for " + p_dialogParams.Type);
					return;
				}
				prefab = Helper.ResourcesLoad<GameObject>("GuiPrefabs/Dialog_YES_NO");
			}
			else
			{
				prefab = Helper.ResourcesLoad<GameObject>("GuiPrefabs/Dialog_OK");
			}
			GameObject gameObject = NGUITools.AddChild(p_anchor.gameObject, prefab);
			GUIDialog component = gameObject.GetComponent<GUIDialog>();
			component.SetTitle(p_dialogParams.Title);
			component.SetText(p_dialogParams.Text);
			component.SetCallback(p_dialogParams.CallbackFunc);
			if (p_dialogParams.IsPopup)
			{
				GameObject gameObject2 = NGUITools.AddChild(gameObject, Helper.ResourcesLoad<GameObject>("GuiPrefabs/GUIBlocker"));
				gameObject2.GetComponent<UIButtonScale>().tweenTarget = gameObject.transform;
			}
		}
	}
}
