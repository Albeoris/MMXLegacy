using System;
using System.Xml.Serialization;

namespace Legacy.Core.Map
{
	public class GridTransition
	{
		private EGridTransitionType m_type;

		private Boolean m_dynamic;

		private EGridTransitionType m_nextState;

		private GridSlot m_slot;

		public GridTransition()
		{
			m_type = EGridTransitionType.OPEN;
			m_nextState = m_type;
		}

		[XmlAttribute("Type")]
		public EGridTransitionType TransitionType
		{
			get => m_type;
		    set
			{
				m_type = value;
				m_nextState = value;
			}
		}

		public EGridTransitionType NextState
		{
			get => m_nextState;
		    set => m_nextState = value;
		}

		[XmlAttribute("IsDynamic")]
		public Boolean IsDynamic
		{
			get => m_dynamic;
		    set => m_dynamic = value;
		}

		public GridSlot Slot
		{
			get => m_slot;
		    set => m_slot = value;
		}

		public void ToggleState()
		{
			if (m_type == EGridTransitionType.CLOSED)
			{
				m_type = EGridTransitionType.OPEN;
			}
			else
			{
				m_type = EGridTransitionType.CLOSED;
			}
			m_slot.UpdatedDynamicConnections();
		}

		public EGridTransitionType CurrentToggleState()
		{
			return m_type;
		}

		public void Close()
		{
			m_type = EGridTransitionType.CLOSED;
			m_slot.UpdatedDynamicConnections();
		}

		public void Open()
		{
			m_type = EGridTransitionType.OPEN;
			m_slot.UpdatedDynamicConnections();
		}
	}
}
