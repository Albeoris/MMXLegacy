using System;
using Legacy.Core.Api;
using Legacy.Core.UpdateLogic;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/CombatActionBarView")]
	public class CombatActionBarView : MonoBehaviour
	{
		private void OnClickedMeleeAttack()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(MeleeAttackCommand.Instance);
		}

		private void OnClickedRangedAttack()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(RangeAttackCommand.Instance);
		}

		private void OnClickedDefend()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(DefendCommand.Instance);
		}
	}
}
