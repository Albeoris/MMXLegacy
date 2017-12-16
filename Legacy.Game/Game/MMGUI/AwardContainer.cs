using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/AwardContainer")]
	public class AwardContainer : MonoBehaviour
	{
		private Party m_party;

		[SerializeField]
		private AwardView m_blessingSpotSecrets;

		[SerializeField]
		private AwardView m_blessingDangerSense;

		[SerializeField]
		private AwardView m_blessingClairvoyance;

		[SerializeField]
		private AwardView m_blessingEnterWater;

		[SerializeField]
		private AwardView m_blessingEnterRough;

		[SerializeField]
		private AwardView m_blessingEnterForest;

		[SerializeField]
		private AwardView m_act1;

		[SerializeField]
		private AwardView m_act2;

		[SerializeField]
		private AwardView m_act3;

		[SerializeField]
		private AwardView m_act4;

		[SerializeField]
		private AwardView m_advancedClass1;

		[SerializeField]
		private AwardView m_advancedClass2;

		[SerializeField]
		private AwardView m_advancedClass3;

		[SerializeField]
		private AwardView m_advancedClass4;

		public void Init(Party p_party)
		{
			m_party = p_party;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_ORDER_CHANGED, new EventHandler(OnPartyOrderChanged));
			UpdateAwards();
		}

		private void OnTokenAdded(Object p_sender, EventArgs p_args)
		{
			UpdateAwards();
		}

		private void OnPartyOrderChanged(Object p_sender, EventArgs p_args)
		{
			UpdateAwards();
		}

		public void CleanUp()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_ORDER_CHANGED, new EventHandler(OnPartyOrderChanged));
		}

		private void UpdateAwards()
		{
			if (!LegacyLogic.Instance.WorldManager.IsSaveGame || LegacyLogic.Instance.WorldManager.LoadedFromStartMenu)
			{
				m_blessingSpotSecrets.Init(m_party, 1);
				m_blessingDangerSense.Init(m_party, 3);
				m_blessingClairvoyance.Init(m_party, 2);
				m_blessingEnterWater.Init(m_party, 4);
				m_blessingEnterRough.Init(m_party, 5);
				m_blessingEnterForest.Init(m_party, 6);
				m_act1.Init(m_party, 7);
				m_act2.Init(m_party, 8);
				m_act3.Init(m_party, 9);
				m_act4.Init(m_party, 10);
				m_advancedClass1.Init(m_party, (Int32)m_party.TokenHandler.AdvancedClassTokenIdForClass(m_party.GetMemberByOrder(0).Class.Class));
				m_advancedClass2.Init(m_party, (Int32)m_party.TokenHandler.AdvancedClassTokenIdForClass(m_party.GetMemberByOrder(1).Class.Class));
				m_advancedClass3.Init(m_party, (Int32)m_party.TokenHandler.AdvancedClassTokenIdForClass(m_party.GetMemberByOrder(2).Class.Class));
				m_advancedClass4.Init(m_party, (Int32)m_party.TokenHandler.AdvancedClassTokenIdForClass(m_party.GetMemberByOrder(3).Class.Class));
			}
		}
	}
}
