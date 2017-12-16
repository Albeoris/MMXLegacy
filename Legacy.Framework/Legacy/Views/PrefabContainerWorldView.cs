using System;
using Legacy.Core.Api;
using Legacy.Core.Quests;
using UnityEngine;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Prefab Container World View")]
	public class PrefabContainerWorldView : MonoBehaviour
	{
		public GameObject ActiveObject;

		public GameObject BaseObject;

		public Int32 ID;

		public CheckMode Mode;

		public EQuestState QuestState = EQuestState.SOLVED;

		public Int32 TokenCount = 1;

		private void Start()
		{
			Boolean flag = false;
			if (Mode == CheckMode.QUESTID)
			{
				if (LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(ID).QuestState == QuestState)
				{
					flag = true;
				}
			}
			else if (Mode == CheckMode.TOKEN && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(ID) >= TokenCount)
			{
				flag = true;
			}
			SetActiveRecursively(ActiveObject, flag);
			SetActiveRecursively(BaseObject, !flag);
			enabled = false;
		}

		private void SetActiveRecursively(GameObject obj, Boolean state)
		{
			if (obj != null)
			{
				obj.SetActive(state);
			}
		}

		public enum CheckMode
		{
			QUESTID,
			TOKEN
		}
	}
}
