using System;

namespace Legacy.Core.UpdateLogic
{
	public class RotateCommand : Command
	{
		private ERotateDirection m_rotation;

		private static RotateCommand m_instanceLeft;

		private static RotateCommand m_instanceRight;

		private static RotateCommand m_instanceTurnAround;

		private RotateCommand(ERotateDirection p_rotation) : base(ECommandTypes.ROTATE)
		{
			m_rotation = p_rotation;
		}

		public ERotateDirection Rotation => m_rotation;

	    public static RotateCommand Right
		{
			get
			{
				if (m_instanceRight == null)
				{
					m_instanceRight = new RotateCommand(ERotateDirection.RIGHT);
				}
				return m_instanceRight;
			}
		}

		public static RotateCommand Left
		{
			get
			{
				if (m_instanceLeft == null)
				{
					m_instanceLeft = new RotateCommand(ERotateDirection.LEFT);
				}
				return m_instanceLeft;
			}
		}

		public static RotateCommand TurnAround
		{
			get
			{
				if (m_instanceTurnAround == null)
				{
					m_instanceTurnAround = new RotateCommand(ERotateDirection.TURNAROUND);
				}
				return m_instanceTurnAround;
			}
		}

		public virtual Boolean IsEqual(Command p_other)
		{
			RotateCommand rotateCommand = (RotateCommand)p_other;
			return rotateCommand != null && rotateCommand.Rotation == Rotation;
		}

		public enum ERotateDirection
		{
			LEFT = -1,
			RIGHT = 1,
			TURNAROUND
		}
	}
}
