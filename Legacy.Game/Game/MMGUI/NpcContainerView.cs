using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/NpcContainerView")]
	public class NpcContainerView : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_npcViewPrefab;

		[SerializeField]
		private GameObject m_npcViewAnchor;

		[SerializeField]
		private GameObject m_npcViewAnchorBackground;

		private List<NpcView> m_views = new List<NpcView>();

		public void Init()
		{
			if (m_views.Count > 0)
			{
				ClearView();
			}
			List<Npc> npcs = LegacyLogic.Instance.ConversationManager.NPCs;
			m_npcViewAnchorBackground.SetActive(true);
			Int32 num = npcs.Count * 160 - 8;
			Vector3 vector = m_npcViewAnchorBackground.transform.localScale;
			vector.x = num + 50;
			m_npcViewAnchorBackground.transform.localScale = vector;
			vector = m_npcViewAnchorBackground.transform.localPosition;
			vector.y = ((!LegacyLogic.Instance.ConversationManager.ShowNpcs) ? 200f : -85f);
			m_npcViewAnchorBackground.transform.localPosition = vector;
			Int32 num2 = (Int32)(-(Single)num * 0.5f) + 76;
			for (Int32 i = 0; i < npcs.Count; i++)
			{
				Npc npc = npcs[i];
				if (npc != null)
				{
					GameObject gameObject = NGUITools.AddChild(m_npcViewAnchor, m_npcViewPrefab);
					NpcView component = gameObject.GetComponent<NpcView>();
					component.Init(npc);
					component.ClickedNpcView += HandleClickedNpcView;
					component.SetSeperationBarVisible(i < npcs.Count - 1);
					if (LegacyLogic.Instance.ConversationManager.ShowNpcs)
					{
						gameObject.transform.localPosition = new Vector3(num2, -85f, 0f);
					}
					else
					{
						gameObject.transform.localPosition = new Vector3(num2, 200f, 0f);
					}
					m_views.Add(component);
					num2 += 160;
					if (i == 0)
					{
						component.SetSelected(true);
					}
				}
			}
		}

		public void SelectNPCView(Int32 p_index)
		{
			if (p_index < m_views.Count)
			{
				m_views[p_index].OnNpcClicked(gameObject);
			}
		}

		private void OnDestroy()
		{
			ClearView();
		}

		private void HandleClickedNpcView(Object p_sender, EventArgs p_args)
		{
			for (Int32 i = 0; i < m_views.Count; i++)
			{
				m_views[i].SetSelected(false);
			}
		}

		private void ClearView()
		{
			for (Int32 i = 0; i < m_views.Count; i++)
			{
				m_views[i].ClickedNpcView -= HandleClickedNpcView;
				Destroy(m_views[i].gameObject);
			}
			m_views.Clear();
		}

		public void HideForEntrance()
		{
			NGUITools.SetActive(m_npcViewAnchorBackground.gameObject, false);
		}

		public void SetNPCsVisible(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(m_npcViewAnchor.gameObject, p_visible);
		}
	}
}
