using System;
using System.Threading;
using Legacy.Core.WorldMap;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI.WorldMap
{
	[AddComponentMenu("MM Legacy/MMGUI/WorldMap icon")]
	public class WorldMapIcon : MonoBehaviour
	{
		private WorldMapController m_controller;

		private WorldMapPoint m_data;

		[SerializeField]
		private UISprite m_sprite;

		public event EventHandler MouseClick;

		public event EventHandler<XEventArgs<Boolean>> MouseTooltip;

		public WorldMapPoint Data => m_data;

	    public void Initialize(WorldMapController p_controller, WorldMapPoint p_data)
		{
			m_controller = p_controller;
			m_data = p_data;
			UpdateIcon();
		}

		public void UpdateIcon()
		{
			if (m_data != null)
			{
				Boolean flag = m_data.CurrentState == EWorldMapPointState.VISIBLE;
				if (gameObject.activeSelf != flag)
				{
					gameObject.SetActive(flag);
				}
				if (flag)
				{
					m_sprite.spriteName = m_data.StaticData.Icon;
					m_sprite.MakePixelPerfect();
					NGUITools.AddWidgetCollider(gameObject);
				}
			}
		}

		private void OnClick()
		{
			if (MouseClick != null)
			{
				MouseClick(this, null);
			}
		}

		private void OnDestroy()
		{
			m_controller = null;
			m_data = null;
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnTooltip(Boolean show)
		{
			if (m_data != null && gameObject.activeSelf && MouseTooltip != null)
			{
				MouseTooltip(this, new XEventArgs<Boolean>(show));
			}
		}
	}
}
