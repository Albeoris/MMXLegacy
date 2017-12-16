using System;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneSceenText : MonoBehaviour
	{
		[SerializeField]
		private String m_LocaKey;

		[SerializeField]
		private GUIStyle m_Style;

		[SerializeField]
		private Single m_BlendSpeed = 2f;

		private String m_Text;

		private Single m_alphaTarget;

		private Single m_alpha;

		public void Show()
		{
			m_alphaTarget = 1f;
			enabled = true;
		}

		public void Hide()
		{
			m_alphaTarget = 0f;
			enabled = true;
		}

		private void Start()
		{
			m_Text = LocaManager.GetText(m_LocaKey);
		}

		private void Update()
		{
			m_alpha = Mathf.Lerp(m_alpha, m_alphaTarget, Time.deltaTime * m_BlendSpeed);
			if (m_alpha == 0f)
			{
				enabled = false;
			}
		}

		private void OnGUI()
		{
			if (Event.current.type == EventType.Repaint)
			{
				GUI.color = new Color(1f, 1f, 1f, m_alpha);
				GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), m_Text, m_Style);
			}
		}
	}
}
