using System;
using Legacy.Core.Quests;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/QuestTabInfo")]
	public class QuestTabInfo : MonoBehaviour
	{
		[SerializeField]
		private EQuestType m_questType = EQuestType.ALL;

		public EQuestType QuestType => m_questType;
	}
}
