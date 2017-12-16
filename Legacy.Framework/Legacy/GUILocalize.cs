using System;
using UnityEngine;

namespace Legacy
{
	[RequireComponent(typeof(UIWidget))]
	[AddComponentMenu("MM Legacy/GUI Misc/GUILocalize")]
	public class GUILocalize : MonoBehaviour
	{
		public String loca_key;

		private SystemLanguage mLanguage = SystemLanguage.Unknown;

		private Boolean mStarted;

		private void OnLocalize()
		{
			if (mLanguage != LocaManager.Language)
			{
				Localize();
			}
		}

		private void OnEnable()
		{
			if (mStarted && mLanguage != LocaManager.Language)
			{
				Localize();
			}
		}

		private void Start()
		{
			mStarted = true;
			Localize();
		}

		public void Localize()
		{
			UIWidget component = GetComponent<UIWidget>();
			UILabel uilabel = component as UILabel;
			UISprite uisprite = component as UISprite;
			if (mLanguage == SystemLanguage.Unknown && String.IsNullOrEmpty(loca_key) && uilabel != null)
			{
				loca_key = uilabel.text;
			}
			String text = (!String.IsNullOrEmpty(loca_key)) ? LocaManager.GetText(loca_key) : LocaManager.GetText(component.name);
			if (uilabel != null)
			{
				uilabel.text = text;
			}
			else if (uisprite != null)
			{
				uisprite.spriteName = text;
				uisprite.MakePixelPerfect();
			}
			mLanguage = LocaManager.Language;
		}
	}
}
