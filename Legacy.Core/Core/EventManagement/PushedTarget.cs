using System;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	public class PushedTarget : SpellTarget
	{
		public readonly Position Position;

		public readonly Boolean Success;

		public PushedTarget(Object p_Target, Position p_position, Boolean p_success) : base(p_Target)
		{
			Position = p_position;
			Success = p_success;
		}

		public override String ToString()
		{
			return String.Concat(new Object[]
			{
				"[PushedTarget Target=",
				Target,
				" Position=",
				Position,
				"]"
			});
		}
	}
}
