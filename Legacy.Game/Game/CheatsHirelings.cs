using System;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsHirelings")]
	public class CheatsHirelings : MonoBehaviour
	{
		[SerializeField]
		private UIPopupList m_hirelingssList;

		private static readonly Int32[] NPC_HIRELING_IDS = new Int32[]
		{
			7,
			8,
			9,
			217,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			20,
			21,
			22,
			24,
			25,
			50,
			107,
			139,
			140,
			141,
			167,
			210,
			275
		};

		private void Awake()
		{
			UpdateHirelings();
		}

		private void UpdateHirelings()
		{
			if (m_hirelingssList == null)
			{
				return;
			}
			m_hirelingssList.items.Clear();
			for (Int32 i = 0; i < NPC_HIRELING_IDS.Length; i++)
			{
				NpcStaticData staticData = StaticDataHandler.GetStaticData<NpcStaticData>(EDataType.NPC, NPC_HIRELING_IDS[i]);
				String id = (staticData != null) ? staticData.NameKey : "MISS STATIC DATA";
				m_hirelingssList.items.Add(LocaManager.GetText(id));
			}
			SelectDefaultNewQuestEntry();
		}

		private void SelectDefaultNewQuestEntry()
		{
			if (m_hirelingssList.items.Count > 0)
			{
				m_hirelingssList.selection = m_hirelingssList.items[0];
			}
			else
			{
				m_hirelingssList.selection = String.Empty;
			}
		}

		public void OnAddHirelingButtonClicked()
		{
			Int32 num = m_hirelingssList.items.IndexOf(m_hirelingssList.selection);
			Npc p_npc = LegacyLogic.Instance.WorldManager.NpcFactory.Get(NPC_HIRELING_IDS[num]);
			LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Hire(p_npc);
		}

		public void OnFireHirelingButtonClicked()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.HirelingHandler.Hirelings[0] != null)
			{
				party.HirelingHandler.Fire(party.HirelingHandler.Hirelings[0]);
			}
			if (party.HirelingHandler.Hirelings[1] != null)
			{
				party.HirelingHandler.Fire(party.HirelingHandler.Hirelings[1]);
			}
		}
	}
}
