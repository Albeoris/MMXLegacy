using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Quests;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Indoor NPC View")]
	internal class IndoorNPCView : BaseView
	{
		[SerializeField]
		private GameObject m_npcModel;

		[SerializeField]
		private Int32 m_npcStaticID = -1;

		protected override void Awake()
		{
			base.Awake();
			if (m_npcModel == null)
			{
				enabled = false;
				return;
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
			m_npcModel.SetActive(!LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(m_npcStaticID));
			if (m_npcStaticID == 50 && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(10) > 0)
			{
				m_npcModel.SetActive(false);
			}
			if (m_npcStaticID == 217)
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(38);
				if (step != null)
				{
					if (step.QuestState == EQuestState.INACTIVE)
					{
						m_npcModel.SetActive(false);
					}
					if (step.QuestState == EQuestState.ACTIVE)
					{
						QuestObjective objective = step.GetObjective(82);
						if (objective != null && objective.QuestState == EQuestState.ACTIVE)
						{
							m_npcModel.SetActive(false);
						}
					}
				}
			}
		}

		private void OnHirelingUpdated(Object sender, EventArgs args)
		{
			if (m_npcModel != null)
			{
				m_npcModel.SetActive(!LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(m_npcStaticID));
			}
		}

		protected override void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
			base.OnDestroy();
		}
	}
}
