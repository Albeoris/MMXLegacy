using System;
using System.Threading;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreate/ClassSelectButton")]
	public class ClassSelectButton : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_classIcon;

		[SerializeField]
		private UILabel m_className;

		[SerializeField]
		private Color m_hoverColor;

		private Boolean m_selected = true;

		private EClass m_class;

		private Color m_pBackgroundNormalColor;

		private Boolean m_isButton;

		public event EventHandler OnClassSelected;

		public EClass SelectedClass => m_class;

	    private void Awake()
		{
			m_pBackgroundNormalColor = new Color(0.375f, 0.375f, 0.375f, 1f);
		}

		public void Init(EClass p_class, CharacterClassStaticData p_staticData)
		{
			m_class = p_class;
			if (p_class != EClass.NONE)
			{
				m_className.text = LocaManager.GetText(p_staticData.NameKey + "_M");
				m_classIcon.spriteName = p_staticData.Icon;
				m_isButton = true;
			}
			else
			{
				m_className.text = LocaManager.GetText("CLASS_LOCKED");
				m_classIcon.spriteName = "ICO_award_promotion_unknown";
				m_isButton = false;
			}
		}

		private void OnClick()
		{
			if (OnClassSelected != null && m_isButton)
			{
				OnClassSelected(this, EventArgs.Empty);
			}
		}

		private void OnHover(Boolean p_isOver)
		{
			if (!m_isButton)
			{
				return;
			}
			if (!m_selected)
			{
				TweenColor.Begin(m_classIcon.gameObject, 0.1f, (!p_isOver) ? m_pBackgroundNormalColor : m_hoverColor);
			}
		}

		public void SetSelected(EClass p_class)
		{
			m_selected = (m_class == p_class);
			if (m_selected)
			{
				m_classIcon.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				m_classIcon.color = new Color(0.375f, 0.375f, 0.375f, 1f);
			}
		}
	}
}
