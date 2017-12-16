using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using UnityEngine;

namespace Legacy.Game.MMGUI.Minimap
{
	public class EntranceSymbolView : SimpleSymbolView
	{
		public override String GetLocalizedTooltipText()
		{
			Position myControllerGridPosition = MyControllerGridPosition;
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(myControllerGridPosition);
			if (slot.VisitedByParty)
			{
				Entrance entrance = (Entrance)MyController;
				String text = null;
				foreach (SpawnCommand spawnCommand in entrance.Commands)
				{
					if (spawnCommand.Type == EInteraction.USE_ENTRANCE)
					{
						String[] array = spawnCommand.Extra.Split(new Char[]
						{
							','
						});
						text = array[0].Replace(".xml", String.Empty);
						break;
					}
				}
				if (text != null)
				{
					GridInfo gridInfo = LegacyLogic.Instance.MapLoader.FindGridInfo(text);
					if (gridInfo != null)
					{
						text = LocaManager.GetText(gridInfo.LocationLocaName);
						if (LegacyLogic.Instance.MapLoader.Grid.Type == EMapType.DUNGEON)
						{
							text = text.Replace("@", ", ");
						}
						else if (text.LastIndexOf('@') != -1)
						{
							text = text.Remove(text.LastIndexOf('@'));
						}
						return LocaManager.GetText(m_TooltipLocaKey) + " - " + text;
					}
					Debug.LogError("Grid Info not found " + text);
				}
				return LocaManager.GetText(m_TooltipLocaKey);
			}
			return null;
		}
	}
}
