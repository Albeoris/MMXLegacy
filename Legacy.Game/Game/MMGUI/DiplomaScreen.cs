using System;
using System.IO;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;
using Legacy.Game.Context;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/DiplomaScreen")]
	public class DiplomaScreen : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_PartyLabel;

		[SerializeField]
		private UILabel m_TimeLabel;

		[SerializeField]
		private UILabel m_ScoreLabel;

		private void Awake()
		{
			String text = String.Empty;
			Int32 num = 0;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(i);
				String text2 = text;
				text = String.Concat(new String[]
				{
					text2,
					member.Name,
					", ",
					LocaManager.GetText("CHARACTER_LEVEL", member.Level),
					" "
				});
				String text3 = member.Class.NameKey;
				if (member.Gender == EGender.MALE)
				{
					text3 += "_M";
				}
				else
				{
					text3 += "_F";
				}
				text = text + LocaManager.GetText(text3) + "\n";
				num += member.Exp;
			}
			m_PartyLabel.text = text;
			m_TimeLabel.text = LocaManager.GetText("DIPLOMA_TIME", 0, 1, 2);
			m_ScoreLabel.text = LocaManager.GetText("DIPLOMA_SCORE", num);
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
		}

		public void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
		}

		private void OnCloseKeyPressed(Object sender, HotkeyEventArgs e)
		{
			String text = GamePaths.UserGamePath + "/ScreenShots";
			Directory.CreateDirectory(text);
			DateTime utcNow = DateTime.UtcNow;
			Application.CaptureScreenshot(String.Format("{0}/{1:D04}{2:D02}{3:D02}_{4:D02}{5:D02}{6:D02}_{7}_Diploma.png", new Object[]
			{
				text,
				utcNow.Year,
				utcNow.Month,
				utcNow.Day,
				utcNow.Hour,
				utcNow.Minute,
				utcNow.Second,
				utcNow.Millisecond
			}));
			ContextManager.ChangeContext(EContext.Game);
		}
	}
}
