using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Mods;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ModMenu")]
	public class ModMenu : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_modEntryList;

		[SerializeField]
		private GameObject m_modEntryPrefab;

		[SerializeField]
		private UILabel m_modName;

		[SerializeField]
		private Texture m_defaultTexture;

		[SerializeField]
		private UITexture m_modTitleScreen;

		[SerializeField]
		private Int32 m_entriesPerPage = 12;

		[SerializeField]
		private UILabel m_pageLabel;

		[SerializeField]
		private GameObject m_nextPageButton;

		[SerializeField]
		private GameObject m_previousPageButton;

		[SerializeField]
		private UILabel m_authorLabel;

		[SerializeField]
		private UILabel m_versionLabel;

		[SerializeField]
		private MultiTextControl m_descriptionLabel;

		[SerializeField]
		private GameObject m_loadButton;

		private List<ModEntry> m_entries = new List<ModEntry>();

		private ModEntry m_selectedEntry;

		private Boolean m_isVisible;

		private Int32 m_page;

		private Int32 m_maxPage;

		private Texture2D m_createdModPreview;

		public event EventHandler OnClose;

		public event EventHandler OnLoadMod;

		public Boolean IsVisible => m_isVisible;

	    public void ToggleVisiblity()
		{
			m_isVisible = !m_isVisible;
			NGUITools.SetActive(gameObject, m_isVisible);
			if (m_isVisible)
			{
				Init();
			}
		}

		public void Init()
		{
			SetupList();
		}

		public void ClickedEntry(ModEntry p_modEntry)
		{
			if (m_selectedEntry != null)
			{
				m_selectedEntry.Unselect();
			}
			m_selectedEntry = p_modEntry;
			if (m_selectedEntry != null)
			{
				m_selectedEntry.Select();
				ModController.ModInfo modInfo = m_selectedEntry.ModInfo;
				m_modName.text = modInfo.Name;
				m_authorLabel.text = modInfo.Creators;
				m_versionLabel.text = modInfo.Version;
				m_descriptionLabel.SetInternalText(modInfo.Description);
				m_descriptionLabel.Show();
				Texture texture = m_defaultTexture;
				if (!String.IsNullOrEmpty(modInfo.TitleImage))
				{
					String path = Path.Combine(modInfo.ImageFolder, modInfo.TitleImage);
					if (File.Exists(path))
					{
						if (m_createdModPreview == null)
						{
							m_createdModPreview = new Texture2D(1, 1, TextureFormat.RGBA32, false);
							m_createdModPreview.hideFlags = HideFlags.DontSave;
						}
						Byte[] data = File.ReadAllBytes(path);
						if (m_createdModPreview.LoadImage(data))
						{
							texture = m_createdModPreview;
						}
					}
				}
				if (m_modTitleScreen.mainTexture != texture)
				{
					Texture mainTexture = m_modTitleScreen.mainTexture;
					m_modTitleScreen.mainTexture = texture;
					if (mainTexture != null && mainTexture != m_defaultTexture)
					{
						mainTexture.UnloadAsset();
					}
				}
			}
			else
			{
				m_modName.text = "-";
				m_authorLabel.text = "-";
				m_versionLabel.text = "-";
				m_descriptionLabel.SetInternalText(" ");
				m_descriptionLabel.Show();
			}
		}

		private void OnDestroy()
		{
			Helper.DestroyImmediate<Texture2D>(ref m_createdModPreview);
		}

		private void SetupList()
		{
			ClearEntries();
			List<ModController.ModInfo> modList = LegacyLogic.Instance.ModController.GetModList();
			m_maxPage = (Int32)(modList.Count / (Single)m_entriesPerPage + 1f);
			if (modList.Count % m_entriesPerPage == 0)
			{
				m_maxPage--;
			}
			m_page = Math.Min(m_page, m_maxPage);
			if (modList.Count == 0)
			{
				NGUITools.SetActive(m_loadButton, false);
				m_pageLabel.text = String.Empty;
			}
			else
			{
				m_pageLabel.text = m_page + 1 + "/" + m_maxPage;
				NGUITools.SetActive(m_loadButton, true);
			}
			NGUITools.SetActive(m_nextPageButton, m_page < m_maxPage - 1);
			NGUITools.SetActive(m_previousPageButton, m_page > 0);
			Int32 num = 0;
			for (Int32 i = 0; i < modList.Count; i++)
			{
				ModController.ModInfo p_modInfo = modList[i];
				if (num >= m_page * m_entriesPerPage)
				{
					GameObject gameObject = NGUITools.AddChild(m_modEntryList, m_modEntryPrefab);
					ModEntry component = gameObject.GetComponent<ModEntry>();
					component.Init(this, p_modInfo);
					gameObject.transform.localPosition = new Vector3(0f, m_entries.Count * -42, -10f);
					m_entries.Add(component);
				}
				num++;
				if (num >= m_page * m_entriesPerPage + m_entriesPerPage)
				{
					break;
				}
			}
			if (m_entries.Count > 0)
			{
				ClickedEntry(m_entries[0]);
			}
			else
			{
				ClickedEntry(null);
			}
		}

		private void ClearEntries()
		{
			for (Int32 i = 0; i < m_entries.Count; i++)
			{
				m_entries[i].CleanUp();
				Destroy(m_entries[i].gameObject);
			}
			m_entries.Clear();
		}

		private void OnClickedClose(GameObject p_sender)
		{
			if (OnClose != null)
			{
				OnClose(this, EventArgs.Empty);
			}
		}

		private void OnClickedLoad(GameObject p_sender)
		{
			LegacyLogic.Instance.ModController.LoadMod(m_selectedEntry.ModInfo);
			if (OnClose != null)
			{
				OnClose(this, EventArgs.Empty);
			}
			if (OnLoadMod != null)
			{
				OnLoadMod(this, EventArgs.Empty);
			}
		}
	}
}
