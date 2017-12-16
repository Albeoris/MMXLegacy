using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using UnityEngine;

namespace Legacy.Game.HUD
{
	public abstract class HUDSideInfoHandlerBase : MonoBehaviour
	{
		[SerializeField]
		protected Int32 MAX_ENTRY_COUNT = 6;

		[SerializeField]
		protected Int32 ENTRY_HEIGHT = 80;

		[SerializeField]
		protected Int32 ENTRY_HEIGHT_OFFSET = 15;

		[SerializeField]
		protected Single SLIDE_DOWN_SPEED = 50f;

		[SerializeField]
		protected Single POSITION_DELTA_CONVERSATION = 100f;

		[SerializeField]
		protected Transform ENTRIES_ROOT;

		protected List<HUDSideInfoBase> m_entries = new List<HUDSideInfoBase>();

		protected Single m_highestEntryPosition = -1f;

		protected Boolean m_isSlidingAnimationPlaying;

		protected Boolean m_conversationWasOpen;

		public Boolean IsFreeSlot => m_entries.Count < MAX_ENTRY_COUNT && m_highestEntryPosition + ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET - 0.1f <= (MAX_ENTRY_COUNT - 1) * (ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET) && !m_isSlidingAnimationPlaying;

	    public void AddEntry(HUDSideInfoBase p_entry)
		{
			ENTRIES_ROOT.AddChildAlignOrigin(p_entry.transform);
			p_entry.transform.localScale = Vector3.one;
			SetEntryLocalPosition(p_entry, m_highestEntryPosition + ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET);
			m_entries.Add(p_entry);
		}

		protected virtual void Awake()
		{
			if (ENTRIES_ROOT == null)
			{
				Debug.LogError("HUDSideInfoHandlerBase: ENTRIES_ROOT is not set!");
			}
			m_highestEntryPosition = -(Single)(ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET);
		}

		protected virtual void Update()
		{
			for (Int32 i = m_entries.Count - 1; i >= 0; i--)
			{
				HUDSideInfoBase hudsideInfoBase = m_entries[i];
				if (hudsideInfoBase == null || !hudsideInfoBase.IsUsed())
				{
					Destroy(hudsideInfoBase.gameObject);
					m_entries.RemoveAt(i);
				}
			}
			m_isSlidingAnimationPlaying = false;
			Single num = -(Single)(ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET);
			for (Int32 j = 0; j < m_entries.Count; j++)
			{
				HUDSideInfoBase hudsideInfoBase2 = m_entries[j];
				if (hudsideInfoBase2.IsAlignedToBottom)
				{
					Single entryHeight = GetEntryHeight(j);
					if (entryHeight != hudsideInfoBase2.transform.localPosition.y && num + (ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET) < hudsideInfoBase2.transform.localPosition.y)
					{
						SetEntryLocalPosition(hudsideInfoBase2, hudsideInfoBase2.transform.localPosition.y - Time.deltaTime * SLIDE_DOWN_SPEED);
						if (entryHeight > hudsideInfoBase2.transform.localPosition.y)
						{
							SetEntryLocalPosition(hudsideInfoBase2, entryHeight);
						}
						else
						{
							m_isSlidingAnimationPlaying = true;
						}
					}
				}
				num = hudsideInfoBase2.transform.localPosition.y;
			}
			m_highestEntryPosition = num;
			if (m_conversationWasOpen != LegacyLogic.Instance.ConversationManager.IsOpen)
			{
				MoveOnConversation();
			}
			foreach (HUDSideInfoBase hudsideInfoBase3 in m_entries)
			{
				if (hudsideInfoBase3.IsUsed())
				{
					hudsideInfoBase3.UpdateEntry();
				}
			}
		}

		protected Single GetEntryHeight(Int32 p_index)
		{
			return p_index * (ENTRY_HEIGHT + ENTRY_HEIGHT_OFFSET);
		}

		protected void MoveOnConversation()
		{
			Vector3 localPosition = ENTRIES_ROOT.localPosition;
			if (m_conversationWasOpen)
			{
				localPosition.y -= POSITION_DELTA_CONVERSATION;
			}
			else
			{
				localPosition.y += POSITION_DELTA_CONVERSATION;
			}
			ENTRIES_ROOT.localPosition = localPosition;
			m_conversationWasOpen = !m_conversationWasOpen;
		}

		private void SetEntryLocalPosition(HUDSideInfoBase p_entry, Single p_locHeight)
		{
			Vector3 localPosition = p_entry.transform.localPosition;
			localPosition.y = p_locHeight;
			p_entry.transform.localPosition = localPosition;
		}
	}
}
