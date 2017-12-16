using System;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/PartyCreationSummary")]
	public class PartyCreationSummary : MonoBehaviour
	{
		[SerializeField]
		private SummaryCharacter m_char1;

		[SerializeField]
		private SummaryCharacter m_char2;

		[SerializeField]
		private SummaryCharacter m_char3;

		[SerializeField]
		private SummaryCharacter m_char4;

		[SerializeField]
		private UIButton m_btnReroll;

		private PartyCreator m_partyCreator;

		public void Init(Boolean p_fromRandom, PartyCreator p_partyCreator)
		{
			m_partyCreator = p_partyCreator;
			m_char1.Init(m_partyCreator.GetDummyCharacter(0));
			m_char2.Init(m_partyCreator.GetDummyCharacter(1));
			m_char3.Init(m_partyCreator.GetDummyCharacter(2));
			m_char4.Init(m_partyCreator.GetDummyCharacter(3));
			NGUITools.SetActive(m_btnReroll.gameObject, p_fromRandom);
		}

		public void Cleanup()
		{
		}
	}
}
