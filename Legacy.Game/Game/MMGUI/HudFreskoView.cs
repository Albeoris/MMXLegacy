using System;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	internal class HudFreskoView : MonoBehaviour
	{
		[SerializeField]
		private UITexture m_fresko;

		[LocalResourcePath]
		[SerializeField]
		private String m_freskoTexture_43;

		[SerializeField]
		[LocalResourcePath]
		private String m_freskoTexture_169;

		[SerializeField]
		[LocalResourcePath]
		private String m_freskoTexture_1610;

		private void Start()
		{
			OnResolutionChange();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(UpdateFreskoCallback));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(UpdateFreskoCallback));
		}

		private void UpdateFreskoCallback(Object p_sender, EventArgs p_args)
		{
			NGUITools.SetActive(m_fresko.gameObject, ConfigManager.Instance.Options.ShowViewport);
			OnResolutionChange();
		}

		private void OnResolutionChange()
		{
			if (!m_fresko.gameObject.activeSelf)
			{
				return;
			}
			Texture2D texture2D;
			switch (GraphicsConfigManager.GetAspectRatio())
			{
			case EAspectRatio._16_9:
				texture2D = LocalResourcePathAttribute.LoadAsset<Texture2D>(m_freskoTexture_169);
				goto IL_6F;
			case EAspectRatio._16_10:
				texture2D = LocalResourcePathAttribute.LoadAsset<Texture2D>(m_freskoTexture_1610);
				goto IL_6F;
			}
			texture2D = LocalResourcePathAttribute.LoadAsset<Texture2D>(m_freskoTexture_43);
			IL_6F:
			if (m_fresko.mainTexture != texture2D)
			{
				Texture mainTexture = m_fresko.mainTexture;
				m_fresko.mainTexture = texture2D;
				if (mainTexture != null)
				{
					mainTexture.UnloadAsset();
				}
				m_fresko.MakePixelPerfect();
			}
		}
	}
}
