using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/Movie subtitle controller")]
	public class MovieSubtitleController : MonoBehaviour
	{
		private Subtitle[] m_subtitleData;

		private Boolean m_showSubtitle;

		private Int32 m_currentSubtitleIndex = -1;

		private Boolean m_LabelSwitch;

		[SerializeField]
		private String m_subtitleFile;

		[SerializeField]
		private UILabel m_labelA;

		[SerializeField]
		private UILabel m_labelB;

		[SerializeField]
		private Single m_labelWidthFactor = 1f;

		[SerializeField]
		private Single m_labelYOffsetFactor = 1f;

		[SerializeField]
		private Single m_blendDuration = 0.3f;

		public Boolean ShowSubtitle
		{
			get => m_showSubtitle;
		    set
			{
				if (m_showSubtitle != value)
				{
					m_showSubtitle = value;
					DisplaySubtitle(m_currentSubtitleIndex);
				}
			}
		}

		public String SubtitleFilePath
		{
			get => m_subtitleFile;
		    set => m_subtitleFile = value;
		}

		public void UpdateSubtitle(Single currentTime)
		{
			if (m_showSubtitle && m_subtitleData != null)
			{
				for (Int32 i = 0; i < m_subtitleData.Length; i++)
				{
					if (currentTime >= m_subtitleData[i].Time)
					{
						if (i + 1 >= m_subtitleData.Length || m_subtitleData[i + 1].Time > currentTime)
						{
							DisplaySubtitle(i);
							return;
						}
					}
				}
			}
			DisplaySubtitle(-1);
		}

		public void ResetSubtitle()
		{
			m_currentSubtitleIndex = -1;
			if (m_labelA != null)
			{
				m_labelA.text = null;
			}
			if (m_labelB != null)
			{
				m_labelB.text = null;
			}
		}

		public void LoadSubtitle()
		{
			String text = Path.Combine(Application.streamingAssetsPath, m_subtitleFile);
			if (!File.Exists(text))
			{
				Debug.LogWarning("Subtitle file not found! " + text, this);
				m_subtitleData = null;
				return;
			}
			ResetSubtitle();
			DisplaySubtitle(-1);
			using (StreamReader streamReader = File.OpenText(text))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Subtitle[]));
				m_subtitleData = (Subtitle[])xmlSerializer.Deserialize(streamReader);
				if (m_subtitleData != null)
				{
					Array.Sort<Subtitle>(m_subtitleData);
				}
			}
		}

		private void DisplaySubtitle(Int32 index)
		{
			if (m_currentSubtitleIndex != index)
			{
				m_currentSubtitleIndex = index;
				if (m_showSubtitle && m_subtitleData != null && index >= 0 && index < m_subtitleData.Length)
				{
					DisplayText(m_subtitleData[index].LocaKey);
				}
				else
				{
					DisplayText(null);
				}
			}
		}

		private void DisplayText(String text)
		{
			UILabel uilabel;
			UILabel uilabel2;
			if (m_LabelSwitch)
			{
				uilabel = m_labelA;
				uilabel2 = m_labelB;
			}
			else
			{
				uilabel = m_labelB;
				uilabel2 = m_labelA;
			}
			m_LabelSwitch = !m_LabelSwitch;
			TweenAlpha.Begin(uilabel.gameObject, m_blendDuration, 1f);
			TweenAlpha.Begin(uilabel2.gameObject, m_blendDuration, 0f);
			uilabel.text = ((!String.IsNullOrEmpty(text)) ? LocaManager.GetText(text) : null);
		}

		private void Awake()
		{
			if (m_labelA == null)
			{
				throw new MissingComponentException("m_labelA");
			}
			if (m_labelB == null)
			{
				throw new MissingComponentException("m_labelB");
			}
			if (!String.IsNullOrEmpty(m_subtitleFile))
			{
				LoadSubtitle();
			}
		}

		private void Update()
		{
			Int32 lineWidth = (Int32)(Screen.width * m_labelWidthFactor);
			Vector3 localPosition = new Vector3(0f, Screen.height * (1f - m_labelYOffsetFactor), 0f);
			m_labelA.lineWidth = lineWidth;
			m_labelA.transform.localPosition = localPosition;
			m_labelB.lineWidth = lineWidth;
			m_labelB.transform.localPosition = localPosition;
		}
	}
}
