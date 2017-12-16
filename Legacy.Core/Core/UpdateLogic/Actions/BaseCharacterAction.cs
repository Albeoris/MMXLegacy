using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Actions
{
	public abstract class BaseCharacterAction : BaseAction
	{
		private Int32 m_characterIndex;

		public BaseCharacterAction(Int32 characterIndex)
		{
			m_characterIndex = characterIndex;
		}

		protected Character Character => Party.GetMember(m_characterIndex);
	}
}
