using System;

namespace Legacy.Core.UpdateLogic
{
	public class MoveCommand : Command
	{
		private static MoveCommand m_instanceForward;

		private static MoveCommand m_instanceBackward;

		private static MoveCommand m_instanceLeft;

		private static MoveCommand m_instanceRight;

		private EMoveDirection m_direction;

		private MoveCommand(EMoveDirection p_direction) : base(ECommandTypes.MOVE)
		{
			m_direction = p_direction;
		}

		public static MoveCommand Forward
		{
			get
			{
				if (m_instanceForward == null)
				{
					m_instanceForward = new MoveCommand(EMoveDirection.FORWARD);
				}
				return m_instanceForward;
			}
		}

		public static MoveCommand Backward
		{
			get
			{
				if (m_instanceBackward == null)
				{
					m_instanceBackward = new MoveCommand(EMoveDirection.BACKWARD);
				}
				return m_instanceBackward;
			}
		}

		public static MoveCommand Left
		{
			get
			{
				if (m_instanceLeft == null)
				{
					m_instanceLeft = new MoveCommand(EMoveDirection.LEFT);
				}
				return m_instanceLeft;
			}
		}

		public static MoveCommand Right
		{
			get
			{
				if (m_instanceRight == null)
				{
					m_instanceRight = new MoveCommand(EMoveDirection.RIGHT);
				}
				return m_instanceRight;
			}
		}

		public EMoveDirection Direction => m_direction;

	    public enum EMoveDirection
		{
			FORWARD,
			RIGHT,
			BACKWARD,
			LEFT
		}
	}
}
