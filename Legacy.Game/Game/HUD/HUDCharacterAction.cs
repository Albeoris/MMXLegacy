using System;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDCharacterAction")]
	public class HUDCharacterAction : MonoBehaviour
	{
		private const String PARTY_TURN = "YOUR TURN.";

		private const String FOES_TURN = "FOES' TURN!";

		[SerializeField]
		private UILabel m_label;

		private void Start()
		{
			m_label.text = "YOUR TURN.";
			m_label.color = Color.white;
		}

		public void DisplayMonsterTurn()
		{
			m_label.text = "FOES' TURN!";
			m_label.color = Color.red;
		}

		public void DisplayPartyTurn()
		{
			m_label.text = "YOUR TURN.";
			m_label.color = Color.white;
		}
	}
}
