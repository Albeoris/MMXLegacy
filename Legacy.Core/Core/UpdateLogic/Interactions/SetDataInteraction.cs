using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class SetDataInteraction : BaseInteraction
	{
		private InteractiveObject m_parent;

		private EInteractiveObjectData m_key;

		private String m_value;

		public SetDataInteraction(InteractiveObject p_parent, SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = ((p_parent != null) ? p_parent : Grid.FindInteractiveObject(m_parentID));
		}

		public InteractiveObject Parent => m_parent;

	    public EInteractiveObjectData Key => m_key;

	    public String Value => m_value;

	    protected override void DoExecute()
		{
			if (m_parent == null)
			{
				throw new NullReferenceException("Data could not be set because the interactive object is null! (" + m_parentID + ")");
			}
			m_parent.SetData(m_key, m_value);
			FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (Enum.IsDefined(typeof(EInteractiveObjectData), array[0]))
			{
				m_key = (EInteractiveObjectData)Enum.Parse(typeof(EInteractiveObjectData), array[0]);
				m_value = String.Empty;
				for (Int32 i = 1; i < array.Length; i++)
				{
					if (i > 1)
					{
						m_value += ",";
					}
					m_value += array[i];
				}
				return;
			}
			throw new FormatException("First parameter was not an InteractiveObjectData key!");
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}
	}
}
