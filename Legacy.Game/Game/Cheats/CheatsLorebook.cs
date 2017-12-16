using System;
using Legacy.Core.Api;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsLorebook")]
	public class CheatsLorebook : MonoBehaviour
	{
		public void OnAddAllButtonClick(GameObject p_sender)
		{
			foreach (LoreBookStaticData loreBookStaticData in StaticDataHandler.GetIterator<LoreBookStaticData>(EDataType.LOREBOOK))
			{
				LegacyLogic.Instance.WorldManager.LoreBookHandler.AddLoreBook(loreBookStaticData.StaticID);
			}
		}
	}
}
