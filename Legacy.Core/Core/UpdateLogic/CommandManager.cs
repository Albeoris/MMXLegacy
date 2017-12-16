using System;
using System.Collections.Generic;

namespace Legacy.Core.UpdateLogic
{
	public class CommandManager
	{
		private const Int32 CommandBufferSize = 1;

		private Boolean m_allowContinuousCommands;

		private Queue<Command> m_commandBuffer;

		private List<Command> m_continuesCommands;

		internal CommandManager()
		{
			m_commandBuffer = new Queue<Command>(1);
			m_continuesCommands = new List<Command>();
			m_allowContinuousCommands = true;
		}

		public Int32 CommandCount => m_commandBuffer.Count;

	    public Int32 ContiniousCommandCount => m_continuesCommands.Count;

	    public Boolean AllowContinuousCommands
		{
			get => m_allowContinuousCommands;
	        set => m_allowContinuousCommands = value;
	    }

		public Boolean AddCommand(Command p_command)
		{
			if (m_commandBuffer.Count < 1)
			{
				m_commandBuffer.Enqueue(p_command);
				if (m_allowContinuousCommands)
				{
					RegisterContinuesCommand(p_command);
				}
				return true;
			}
			return false;
		}

		public void EndCommand(Command p_command)
		{
			for (Int32 i = m_continuesCommands.Count - 1; i >= 0; i--)
			{
				if (m_continuesCommands[i] == p_command)
				{
					m_continuesCommands.RemoveAt(i);
				}
			}
		}

		private void RegisterContinuesCommand(Command p_command)
		{
			if (p_command.Type == Command.ECommandTypes.MOVE || p_command.Type == Command.ECommandTypes.ROTATE)
			{
				m_continuesCommands.Add(p_command);
			}
		}

		public Command PullCommand()
		{
			if (m_commandBuffer.Count > 0)
			{
				return m_commandBuffer.Dequeue();
			}
			if (m_continuesCommands.Count > 0)
			{
				return m_continuesCommands[0];
			}
			return null;
		}

		public void ClearQueue()
		{
			foreach (Command command in m_commandBuffer)
			{
				command.CancelCommand();
			}
			m_commandBuffer.Clear();
			ClearContiniousQueue();
		}

		public void ClearContiniousQueue()
		{
			m_continuesCommands.Clear();
		}
	}
}
