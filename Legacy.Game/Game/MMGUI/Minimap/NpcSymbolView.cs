using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	public class NpcSymbolView : SimpleSymbolView
	{
		[SerializeField]
		private String m_SpriteNormal;

		[SerializeField]
		private Boolean m_SpriteNormalAsymmetric;

		[SerializeField]
		private String m_SpriteHouse;

		[SerializeField]
		private Boolean m_SpriteHouseAsymmetric;

		[SerializeField]
		private String m_SpriteInn;

		[SerializeField]
		private Boolean m_SpriteInnAsymmetric;

		[SerializeField]
		private String m_SpriteSmith;

		[SerializeField]
		private Boolean m_SpriteSmithAsymmetric;

		[SerializeField]
		private String m_SpriteShrine;

		[SerializeField]
		private Boolean m_SpriteShrineAsymmetric;

		public override String GetLocalizedTooltipText()
		{
			m_TooltipLocaKey = null;
			NpcContainer npcContainer = MyController as NpcContainer;
			if (npcContainer != null)
			{
				m_TooltipLocaKey = npcContainer.MinimapTooltipLocaKey;
				if (String.IsNullOrEmpty(m_TooltipLocaKey) && npcContainer.Npcs.Count > 0 && npcContainer.Npcs[0].IsEnabled)
				{
					m_TooltipLocaKey = npcContainer.Npcs[0].StaticData.NameKey;
				}
			}
			return base.GetLocalizedTooltipText();
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			UpdateVisibility();
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnObjectEnabledChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnObjectEnabledChanged));
			}
		}

		private void OnHirelingUpdated(Object sender, EventArgs args)
		{
			HirelingEventArgs hirelingEventArgs = (HirelingEventArgs)args;
			NpcContainer npcContainer = MyController as NpcContainer;
			if (npcContainer != null && npcContainer.Contains(hirelingEventArgs.Npc))
			{
				UpdateVisibility();
			}
		}

		private void OnObjectEnabledChanged(Object sender, EventArgs args)
		{
			BaseObject baseObject = sender as BaseObject;
			if (baseObject == MyController)
			{
				UpdateVisibility();
			}
		}

		private void UpdateVisibility()
		{
			NpcContainer npcContainer = MyController as NpcContainer;
			if (npcContainer != null)
			{
				UISprite uisprite = (UISprite)MyUIWidget;
				NGUITools.SetActiveSelf(uisprite.gameObject, npcContainer.IsEnabled && npcContainer.Enabled);
				if (uisprite.enabled)
				{
					switch (npcContainer.MinimapSymbol)
					{
					case ENpcMinimapSymbol.HOUSE:
						uisprite.spriteName = m_SpriteHouse;
						m_asymmetricSymbol = m_SpriteHouseAsymmetric;
						goto IL_104;
					case ENpcMinimapSymbol.INN:
						uisprite.spriteName = m_SpriteInn;
						m_asymmetricSymbol = m_SpriteInnAsymmetric;
						goto IL_104;
					case ENpcMinimapSymbol.SMITH:
						uisprite.spriteName = m_SpriteSmith;
						m_asymmetricSymbol = m_SpriteSmithAsymmetric;
						goto IL_104;
					case ENpcMinimapSymbol.SHRINE:
						uisprite.spriteName = m_SpriteShrine;
						m_asymmetricSymbol = m_SpriteShrineAsymmetric;
						goto IL_104;
					}
					uisprite.spriteName = m_SpriteNormal;
					m_asymmetricSymbol = m_SpriteNormalAsymmetric;
					IL_104:
					uisprite.MakePixelPerfect();
					NGUITools.AddWidgetCollider(gameObject);
				}
			}
		}
	}
}
