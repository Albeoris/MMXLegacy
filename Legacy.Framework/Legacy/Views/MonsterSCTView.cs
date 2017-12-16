using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.HUD;
using Legacy.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Monster SCT View")]
	public class MonsterSCTView : BaseView
	{
		private HUDDamageText m_HUDDamageText;

		[SerializeField]
		private Transform m_TextAnchor;

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_BUFF_PERFORM, new EventHandler(OnBuffPerform));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_BUFF_CANNOT_BE_APPLIED, new EventHandler(OnBuffCannotBeApplied));
			}
			if (MyController != null && MyController is Monster)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_BUFF_PERFORM, new EventHandler(OnBuffPerform));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_BUFF_CANNOT_BE_APPLIED, new EventHandler(OnBuffCannotBeApplied));
			}
		}

		private void Start()
		{
			m_HUDDamageText = HUDTextProvider.CreateHUDDamageText(this, m_TextAnchor);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			HUDTextProvider.DestroyHUDDamageText(this);
		}

		private void OnBuffPerform(Object sender, EventArgs e)
		{
			if (m_HUDDamageText == null || MyController != sender)
			{
				return;
			}
			BuffPerformedEventArgs buffPerformedEventArgs = (BuffPerformedEventArgs)e;
			GUIHUDText.Print(m_HUDDamageText, buffPerformedEventArgs.Result, true);
		}

		private void OnBuffCannotBeApplied(Object p_sender, EventArgs p_args)
		{
			if (m_HUDDamageText == null || MyController != ((MonsterBuffUpdateEventArgs)p_args).Monster)
			{
				return;
			}
			m_HUDDamageText.Add(LocaManager.GetText("SCROLLING_COMBAT_IMMUNE"), false, Color.white, 0f);
		}

		private void OnReceivedAttacks(AttacksUnityEventArgs e)
		{
			if (m_HUDDamageText == null)
			{
				return;
			}
			GUIHUDText.Print(m_HUDDamageText, e.Result, e.IsMagical);
		}
	}
}
